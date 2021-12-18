using GradingSytemApi.Common.Constant;
using GradingSytemApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.DTOs
{
    public class AccountModel 
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public bool Deleted { get; set; }
        public bool Active { get; set; }
        public bool IsFirstLogin { get; set; }
        public DateTime Birthday { get; set; }
        public long TotalScore { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public RoleLookupModel Role { get; set; }

        public AccountModel() { }
        public AccountModel(Account account)
        {
            this.Id = account.Id;
            this.Code = account.Code;
            this.FirstName = account.FirstName;
            this.LastName = account.LastName;
            this.Email = account.Email;
            this.Image = account.Image;
            this.Deleted = account.Deleted;
            this.Active = account.Active;
            this.IsFirstLogin = account.IsFirstLogin;
            this.Birthday = account.Birthday;
            this.TotalScore = account.TotalScore;
            this.CreatedDate = account.CreatedDate;
            this.UpdatedDate = account.UpdatedDate;
            this.Role = new RoleLookupModel(account.AccountRoleMaps.FirstOrDefault()?.Role);
        }

        public AccountModel(Account account, Role role)
        {
            this.Id = account.Id;
            this.Code = account.Code;
            this.FirstName = account.FirstName;
            this.LastName = account.LastName;
            this.Email = account.Email;
            this.Image = account.Image;
            this.Deleted = account.Deleted;
            this.Active = account.Active;
            this.IsFirstLogin = account.IsFirstLogin;
            this.Birthday = account.Birthday;
            this.TotalScore = account.TotalScore;
            this.CreatedDate = account.CreatedDate;
            this.UpdatedDate = account.UpdatedDate;
            this.Role = new RoleLookupModel(role);
        }
    }

    public class CreateAccountModel
    {
        [Required(ErrorMessage = "First name is requried")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is requried")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is requried")]
        public string Email { get; set; }
        public string Code { get; set; }
        [Required(ErrorMessage = "Account type is requried")]
        public CreateUserType Type { get; set; }
        public DateTime BirthDay { get; set; }

        public Account ParseToEntity()
        {
            var entity = new Account()
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
                Code = this.Code,
                Birthday = this.BirthDay
            };

            return entity;
        }
    }

    public class UpdateAccountModel
    {
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime Birthday { get; set; }
    }

    public class UpdateProfileModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

    public class AccountLookupModel
    {
        public string Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }

        public AccountLookupModel() { }

        public AccountLookupModel(Account account)
        {
            this.Id = account.Id;
            this.LastName = account.LastName;
            this.FirstName = account.FirstName;
            this.Email = account.Email;
            this.Avatar = account.Image;
        }

    }
}
