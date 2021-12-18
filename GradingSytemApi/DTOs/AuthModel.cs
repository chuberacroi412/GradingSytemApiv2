using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.DTOs
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class TokenModel
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public double expires_in { get; set; }
        public bool is_confirmed { get; set; } = false;

        public TokenModel() { }
        public TokenModel(string token, JwtSecurityToken properties)
        {
            this.access_token = token;
            this.token_type = "bearer";
            this.expires_in = Math.Floor(properties.ValidTo.Subtract(properties.ValidFrom).TotalMinutes);
        }
    }
}
