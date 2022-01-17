using GradingSytemApi.Common.Constant;
using GradingSytemApi.Common.Helpers;
using GradingSytemApi.DTOs;
using GradingSytemApi.Entities;
using GradingSytemApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Services.Implements
{
    public class ModuleService : IModuleService
    {
        private readonly ApiDbContext _dbContext;

        public ModuleService(ApiDbContext apiDbContext)
        {
            _dbContext = apiDbContext;
        }

        private bool FilterById(int Id, out Module module, ref ErrorModel errors)
        {
            module = _dbContext.Modules.FirstOrDefault(x => !x.Deleted && x.Id == Id);

            if(module == null)
            {
                errors.Add("Module not found");
            }

            return errors.IsEmpty;
        }

        private IQueryable<Module> Filter(PaginationModuleRequest req)
        {
            return _dbContext.Modules.Where(x => !x.Deleted
                                                   &&
                                                   (string.IsNullOrEmpty(req.ModuleName) || x.Name.ToLower().Contains(req.ModuleName.ToLower()))
                                                   &&
                                                   (!req.Active.HasValue || x.Active == req.Active.Value)
                                                   &&
                                                   (!req.IsParent.HasValue || x.IsParent == req.IsParent.Value));
                                                    
        }

        private IQueryable<Module> Sorts(PaginationModuleRequest req, IQueryable<Module> modules)
        {
            switch(req.Sort.ToLower())
            {
                case Sort.GeneralSort.NAME:
                    modules = modules.OrderBy(x => x.Name);
                    break;
                case Sort.GeneralSort.NAME_DESC:
                    modules = modules.OrderByDescending(x => x.Name);
                    break;
                case Sort.GeneralSort.ACTIVE:
                    modules = modules.OrderBy(x => x.Active);
                    break;
                case Sort.GeneralSort.ACTIVE_DESC:
                    modules = modules.OrderByDescending(x => x.Active);
                    break;
            }

            return modules;
        }

        private bool FilterRoleModuleMapById(Guid Id, out RoleModuleMap roleModuleMap, ref ErrorModel errors)
        {
            roleModuleMap = _dbContext.RoleModuleMaps.FirstOrDefault(x => !x.Deleted && x.Id == Id);

            if(roleModuleMap == null)
            {
                errors.Add("Module not found");
            }

            return errors.IsEmpty;
        }

        private bool ValidateUpdate(string roleId, ref ErrorModel errors)
        {
            var role = _dbContext.Roles.FirstOrDefault(x => !x.Deleted && x.Id == roleId);

            var adminRoleName = Settings.DEFAULT_ADMIN_ROLE_NAME;

            if(role == null )
            {
                errors.Add("Role not found");
            }
            else if(role.Name == adminRoleName)
            {
                errors.Add("Can not modify super admin role");
            }

            return errors.IsEmpty;
        }

        private void UpdateChildModule(string roleId, int parentModuleId, bool state)
        {
            var childModules = _dbContext.Modules.Where(x => !x.Deleted && x.ParentModuleId == parentModuleId).Select(x => x.Id);

            var childMap = _dbContext.RoleModuleMaps.Where(x => !x.Deleted && x.RoleId == roleId && childModules.Contains(x.ModuleId)).ToList();

            childMap.ForEach(map => map.Active = state);
        }

        public void ChangeRoleModuleMapState(Guid Id, bool state, ref ErrorModel errors)
        {
            if(FilterRoleModuleMapById(Id, out RoleModuleMap map, ref errors) && ValidateUpdate(map.RoleId, ref errors))
            {
                map.Active = state;

                if(map.Module.IsParent)
                {
                    UpdateChildModule(map.RoleId, map.ModuleId, state);
                }

                _dbContext.SaveChanges();
            }
        }

        public ModuleModel GetById(int id, ref ErrorModel errors)
        {
            if(FilterById(id, out Module module, ref errors))
            {
                return new ModuleModel(module);
            }

            return null;
        }

        public PaginationModel<ModuleModel> GetModules(PaginationModuleRequest req)
        {
            IQueryable<Module> modules = Filter(req);
            modules = Sorts(req, modules);

            PaginationModel<ModuleModel> pagination = new PaginationModel<ModuleModel>(req, modules);

            if(req.PaginationType == PaginationType.Pagination)
            {
                pagination.Data = modules.Skip(req.Page * req.Amount).Take(req.Amount).AsEnumerable().Select(x => new ModuleModel(x)).ToList();
            }
            else
            {
                pagination.Data = modules.AsEnumerable().Select(x => new ModuleModel(x)).ToList();
            }

            return pagination;
        }

        public IEnumerable<RoleModuleMapModel> GetModuleOfRole(Guid roleId, ref ErrorModel errors)
        {
            var modulesMap = _dbContext.RoleModuleMaps.Where(x => !x.Deleted && x.RoleId == roleId.ToString()).Select(x => new { x.Module, x.Active }).ToList();

            var modules = modulesMap.Select(x => new RoleModuleMapModel(x.Module, x.Active)).ToList();

            return modules;
        }
    }
}
