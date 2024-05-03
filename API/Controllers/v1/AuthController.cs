using Microsoft.AspNetCore.Mvc;
using Contracts.V1;
using Contracts.V1.Response.Auth;
using Contracts.V1.Requests.Auth;
using API.Services.Contracts;


namespace API.Controllers.v1
{
    public class AuthController(ILogger<AuthController> logger,IIdentityService identity,IUserNotifications userNotification) : Controller
    {
        [HttpPost(ApiRoutes.Auth.Login)]
        public async Task<IActionResult> Login([FromBody] AuthRequest authRequest)
        {

            if (authRequest == null)
            {
                return BadRequest("Invalid/wrong request");
            }

            try
            {
                var authResult = await identity.LoginAsync(authRequest);
                if (authResult.IsSuccess)
                {
                    //user notifications after login.
                    await userNotification.NotificationAboutUserOnline(authResult.MyId);
                }
                return Ok(authResult);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Auth Login request");
                return Ok(new AuthResponse("Internal Server Error.Please try later"));
            }
            //todo: later would be better to check error's type, and if error related to DB connection, re-try mechanism implement.
        }
    }
}
