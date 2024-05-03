using API.Services.Contracts;
using Contracts.V1.Requests.Auth;
using Contracts.V1.Response.Auth;
using DAL.Models;
using DAL.Models.Identity;
using DBLogic.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using Shared.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services.Implementation
{
    public class IdentityService:IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly YSConfiguration _config;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<IdentityService> _logger;
        public IdentityService(UserManager<User> userManager, IOptions<YSConfiguration> config,
            TokenValidationParameters tokenValidationParameters,
            IRefreshTokenRepository refreshTokenRepository, RoleManager<Role> roleManager,
            ILogger<IdentityService> logger)
        {
            _userManager = userManager;
            _config = config.Value;
            _refreshTokenRepository = refreshTokenRepository;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<AuthResponse> LoginAsync(AuthRequest loginRequest)
        {
            var existingUser = await _userManager.FindByEmailAsync(loginRequest.Email);

            if (existingUser == null){
                //even though we know that the error is only in email, we do not report it to avoid
                //matching and verifying the existence of users in our API by email. 
                return new AuthResponse("Email or/and password are incorrect");
               
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(existingUser, loginRequest.Password);

            if (!userHasValidPassword)
            {
                return new AuthResponse("Email or/and password are incorrect");
            }

            return await GenerateAuthenticationResultForUserAsync(existingUser);
        }

        private async Task<AuthResponse> GenerateAuthenticationResultForUserAsync(User user)
        {
            if (_config == null || _config.JwtSettings == null)
            {
                _logger.LogError("JWT configurations are missed or not loaded!!");
                return new AuthResponse("Internal error,please try later");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.JwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.GivenName, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id.ToString())// to have userId in context
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null) continue;
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_config.JwtSettings.TokenLifetime),
                Issuer= _config.JwtSettings.Issuer,
                Audience=_config.JwtSettings.Audience,
                
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString(),
            };
            try
            {
                await _refreshTokenRepository.CreateAndSaveAsync(refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during jwt Token generation");
                return new AuthResponse("Internal error,please try later");
            }

            return new AuthResponse
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
                ExpireTimestamp = (long)tokenDescriptor.Expires.Value.Subtract(new DateTime(1970, 1, 1)).TotalSeconds * 1000,
                MyId = user.Id,
            };
        }
    }
}
