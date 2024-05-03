using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.V1.Response.Auth
{
    public class AuthResponse:BaseResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public long ExpireTimestamp { get; set; }
        public long MyId { get; set; }


        public AuthResponse() { 

        }
        public AuthResponse(string errorMessage)
        {
            Errors = new List<string>()
            {
                errorMessage
            };
        }

        
    }
}
