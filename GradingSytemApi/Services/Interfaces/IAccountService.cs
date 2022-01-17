using GradingSytemApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Dictionary<ErrorModel, AccountModel>> CreateAccount(CreateAccountModel model);
        Task<Dictionary<ErrorModel, AccountModel>> GetById(string Id);
        Task<Dictionary<ErrorModel, AccountModel>> UpdateAccount(string Id);
        Task<Dictionary<ErrorModel, AccountModel>> DeleteAccount(string Id);
        Task<Dictionary<ErrorModel, AccountModel>> ChangeState(string Id);
        Task<Dictionary<ErrorModel, AccountModel>> UpdateByToken(UpdateProfileModel model);
        Task<ErrorModel> ResetPassword(string email);
    }
}
