using GradingSytemApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Services.Interfaces
{
    public interface IModuleService
    {
        ModuleModel GetById(int id, ref ErrorModel errors);
        PaginationModel<ModuleModel> GetModules(PaginationModuleRequest req);

        IEnumerable<RoleModuleMapModel> GetModuleOfRole(Guid roleId, ref ErrorModel errors);
        void ChangeRoleModuleMapState(Guid Id, bool state, ref ErrorModel errors);
    }
}
