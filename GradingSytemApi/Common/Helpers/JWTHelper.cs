using GradingSytemApi.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GradingSytemApi.Common.Helpers
{
    public static class JWTHelper
    {
        public static IConfiguration Configuration { get; set; }

        public static TokenModel GenerateJwtToken(string username, string accountId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, accountId),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.JWT_KEY));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Settings.JWT_EXPIRE_DAY);
            var issuer = Settings.JWT_ISSUER;

            var tokenProperties = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires,
                signingCredentials: credential
                );

            return new TokenModel(new JwtSecurityTokenHandler().WriteToken(tokenProperties), tokenProperties);
        }

        public static string GenerateRandomPassword(bool useLowercase = true, bool useUppercase = true, bool useNumbers = true, bool useSpecial = true, int passwordSize = 8)
        {
            const string LOWER_CASE = "abcdefghijklmnopqursuvwxyz";
            const string UPPER_CASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string NUMBERS = "0123456789";
            const string SPECIALS = @"!@£$%^&*()#€";

            char[] _passwords = new char[8];
            string charSet = "";
            System.Random _random = new Random();
            int counter = 0;

            if (useLowercase)
            {
                _passwords[counter] = LOWER_CASE[_random.Next(LOWER_CASE.Length - 1)];
                charSet += LOWER_CASE;
                counter++;
            }
            if (useUppercase)
            {
                _passwords[counter] = UPPER_CASE[_random.Next(UPPER_CASE.Length - 1)];
                charSet += UPPER_CASE;
                counter++;
            }
            if (useNumbers)
            {
                _passwords[counter] = NUMBERS[_random.Next(NUMBERS.Length - 1)];
                charSet += NUMBERS;
                counter++;
            }
            if (useSpecial)
            {
                _passwords[counter] = SPECIALS[_random.Next(SPECIALS.Length - 1)];
                charSet += SPECIALS;
                counter++;
            }

            while (counter < passwordSize)
            {
                _passwords[counter] = charSet[_random.Next(charSet.Length - 1)];
                counter++;
            }

            Random rng = new Random();
            int n = _passwords.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = _passwords[k];
                _passwords[k] = _passwords[n];
                _passwords[n] = value;
            }

            return String.Join(null, _passwords);
        }
    }
}
