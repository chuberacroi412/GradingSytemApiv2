using GradingSytemApi.Common.Constant;
using GradingSytemApi.Common.Helpers;
using GradingSytemApi.DTOs;
using GradingSytemApi.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly ApiDbContext _dbContext;
        private readonly UserResolverService _userResolverService;

        public AuthController(UserManager<Account> userManager, SignInManager<Account> signInManager, ApiDbContext apiDbContext, UserResolverService userResolverService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = apiDbContext;
            _userResolverService = userResolverService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            ErrorModel errors = new ErrorModel();
            if(!ModelState.IsValid)
            {
                AddErrorsFromModelState(ref errors);
                return BadRequest(errors);
            }
            else
            {
                try
                {
                    // Check sign in
                    var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

                    if (!result.Succeeded)
                    {
                        errors.Add("Username or password is incorrect");
                        return BadRequest(errors);
                    }
                    else
                    {
                        // Find user
                        var user = await _userManager.FindByNameAsync(model.Username);

                        // Check active
                        if (!user.Active)
                        {
                            errors.Add("Account has been locked");
                            return BadRequest(errors);
                        }
                        else
                        {
                            var token_expire_time = GlobalConstant.LOGIN_EXPIRE_TIME;

                            // Create token expire time
                            var authProperties = new AuthenticationProperties
                            {
                                AllowRefresh = true,
                                IssuedUtc = DateTime.UtcNow,
                                ExpiresUtc = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(token_expire_time), DateTimeKind.Utc)
                            };

                            // Sign in
                            await _signInManager.SignInAsync(user, authProperties, JwtBearerDefaults.AuthenticationScheme);

                            return Ok(new
                            {
                                Token = JWTHelper.GenerateJwtToken(user.UserName, user.Id, "Admin"),
                                UserId = user.Id
                            });

                        }
                    }
                }
                catch(Exception e)
                {
                    errors.Add("Unexpected errors in server occur, try again later");
                    return BadRequest(errors);
                }
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
          
            foreach(var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return NoContent();
        }
    }
}
