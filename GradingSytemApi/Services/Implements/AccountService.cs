using GradingSytemApi.Common.Helpers;
using GradingSytemApi.DTOs;
using GradingSytemApi.Entities;
using GradingSytemApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserResolverService _userResolverService;
        private readonly ApiDbContext _dbContext;
        private readonly EmailService _emailService;

        public AccountService(UserManager<Account> userManager, RoleManager<Role> roleManager, UserResolverService userResolverService, 
            ApiDbContext dbContext, EmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userResolverService = userResolverService;
            _dbContext = dbContext;
            _emailService = emailService;
        }

        private async Task<Dictionary<ErrorModel, Account>> FilterById(string userId, bool isUpdateByToken = false)
        {
            ErrorModel errors = new ErrorModel();
            var actionResult = new Dictionary<ErrorModel, Account>();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                errors.Add("Account not found");
                actionResult.Add(errors, null);
            }
            else if (isUpdateByToken && !user.Active)
            {
                errors.Add("Account has been locked");
                actionResult.Add(errors, null);
            }
            else
            {
                actionResult.Add(errors, user);
            }

            return actionResult;
        }
        public Task<Dictionary<ErrorModel, AccountModel>> ChangeState(string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<Dictionary<ErrorModel, AccountModel>> CreateAccount(CreateAccountModel model)
        {
            var actionResult = new Dictionary<ErrorModel, AccountModel>();
            var errors = new ErrorModel();

            // Validate model
            if(_userManager.Users.Any(x => !x.Deleted && x.Email == model.Email))
            {
                errors.Add("Email is existed");
            }

            if(!string.IsNullOrEmpty(model.Code) && _userManager.Users.Any(x => !x.Deleted && x.Code.ToLower() == model.Code.ToLower()))
            {
                if(model.Type == Common.Constant.CreateUserType.Student)
                {
                    errors.Add("Student Id existed");
                }
                else
                {
                    errors.Add("Teacher Id existed");
                }
            }

            if(!errors.IsEmpty)
            {
                actionResult.Add(errors, new AccountModel());
                return actionResult;
            }

            // Parse to account entity
            var account = model.ParseToEntity();
            account.CreatedBy = _userResolverService.GetUser();
            account.UpdatedBy = _userResolverService.GetUser();
            account.CreatedDate = DateTime.Now;
            account.UpdatedDate = DateTime.Now;

            var password = "123456Aa@";
            //var password = JWTHelper.GenerateRandomPassword();

            var roleName = model.Type == Common.Constant.CreateUserType.Student ? Settings.DEFAULT_STUDENT_ROLE_NAME : Settings.DEFAULT_TEACHER_ROLE_NAME;

            var checkRole = await _roleManager.FindByNameAsync(roleName);
            if(checkRole != null)
            {
                // Full init account
                account.UserName = model.Email;
                account.Active = true;

                // Add to database
                var result = await _userManager.CreateAsync(account, password);

                if(!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        errors.Add(error.Description);
                    }

                    actionResult.Add(errors, new AccountModel());
                    return actionResult;
                }

                // Get user from database and add role
                var user = await _userManager.FindByNameAsync(model.Email);
                user.AccountRoleMaps.Add(new AccountRoleMap()
                {
                    RoleId = checkRole.Id,
                    Role = checkRole,
                    UserId = user.Id
                });

                _dbContext.SaveChanges();
                //var roleResult = await _userManager.AddToRoleAsync(user, checkRole.Name);

                // Send email
                await _emailService.SendAccountInformation(user.Id, password);
                // Return 
                var profile = new AccountModel(user, checkRole);
                actionResult.Add(errors, profile);

                return actionResult;
            }

            return null;
        }

        public async Task<Dictionary<ErrorModel, AccountModel>> DeleteAccount(string Id)
        {
            var actionResult = new Dictionary<ErrorModel, AccountModel>();
            ErrorModel errors = new ErrorModel();

            var checkAccount = await FilterById(Id);

            if(!checkAccount.First().Key.IsEmpty)
            {
                checkAccount.First().Key.Errors.ForEach(e => errors.Add(e));
                actionResult.Add(errors, new AccountModel());
            }
            else
            {
                actionResult.Add(errors, new AccountModel(checkAccount.First().Value));
            }

            return actionResult;
        }

        public async Task<Dictionary<ErrorModel, AccountModel>> GetById(string Id)
        {
            var actionResult = new Dictionary<ErrorModel, AccountModel>();
            ErrorModel errors = new ErrorModel();

            var checkAccount = await FilterById(Id);

            if (!checkAccount.First().Key.IsEmpty)
            {
                checkAccount.First().Key.Errors.ForEach(e => errors.Add(e));
                actionResult.Add(errors, new AccountModel());
            }
            else
            {
                actionResult.Add(errors, new AccountModel(checkAccount.First().Value));
            }

            return actionResult;
        }

        public Task<Dictionary<ErrorModel, AccountModel>> UpdateAccount(string Id)
        {
            throw new NotImplementedException();
        }

        private async Task<ErrorModel> ValidateUpdateAccountByToken(UpdateProfileModel model, Account user)
        {
            ErrorModel errors = new ErrorModel();

            if(string.IsNullOrEmpty(model.CurrentPassword))
            {
                errors.Add("Current password is required");
            }
            else 
            {
                var passwordCorrect = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);

                if(!passwordCorrect)
                {
                    errors.Add("Current password is incorrect");
                }
                else if (string.IsNullOrEmpty(model.NewPassword))
                {
                    errors.Add("New password is required");
                }
                else if (!string.IsNullOrEmpty(model.NewPassword) && model.NewPassword != model.ConfirmNewPassword)
                {
                    errors.Add("Confirm password not match");
                }
            }

            return errors;
        }

        private void UpdateProfile(UpdateProfileModel model, Account account)
        {
            account.FirstName = string.IsNullOrEmpty(model.FirstName) ? account.FirstName : model.FirstName;
            account.LastName = string.IsNullOrEmpty(model.FirstName) ? account.LastName : model.LastName;
            account.UpdatedDate = DateTime.Now;
            account.UpdatedBy = _userResolverService.GetUser();
        }

        public async Task<Dictionary<ErrorModel, AccountLookupModel>> UpdateByToken(UpdateProfileModel model)
        {
            // Init values
            var actionResult = new Dictionary<ErrorModel, AccountLookupModel>();
            ErrorModel errors = new ErrorModel();

            // Find user by token
            var userId = _userResolverService.GetUser();
            var result = await FilterById(userId, true);

            // Check user existed
            if(!result.First().Key.IsEmpty)
            {
                result.First().Key.Errors.ForEach(error => errors.Add(error));
                actionResult.Add(errors, new AccountLookupModel());

                return actionResult;
            }

            // Validate update model
            var modelErrors = await ValidateUpdateAccountByToken(model, result.First().Value);

            if(!modelErrors.IsEmpty)
            {
                modelErrors.Errors.ForEach(error => errors.Add(error));
                actionResult.Add(errors, new AccountLookupModel());

                return actionResult;
            }

            // Update informations (exclude password)
            UpdateProfile(model, result.First().Value);

            // Update password
            var changePasswordResult = await _userManager.ChangePasswordAsync(result.First().Value, model.CurrentPassword ,model.NewPassword);

            if(!changePasswordResult.Succeeded)
            {
                foreach(var error in changePasswordResult.Errors)
                {
                    errors.Add(error.Description);
                }
            }
            else // Update other informations
            {
                result.First().Value.IsFirstLogin = true;
                await _userManager.UpdateAsync(result.First().Value);

                // Build return model
                actionResult.Add(errors, new AccountLookupModel(result.First().Value));
            }

            return actionResult;
        }
    }
}
