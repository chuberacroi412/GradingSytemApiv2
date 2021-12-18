using GradingSytemApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.DTOs
{
    public class RoleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public List<ModuleModel> Modules { get; set; }

        public RoleModel(Role role)
        {
            this.Id = role.Id;
            this.Name = role.Name;
            this.Active = role.Active;
            this.Deleted = role.Deleted;

            if(role.RoleModuleMaps.Any())
            {
                this.Modules = new List<ModuleModel>();               
            }
        }
    }

    public class CreateRoleModel
    {
        //[Required(ErrorMessageResourceName = "NameRequried", ErrorMessageResourceType = typeof(ErrorResource))]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public bool Active { get; set; }

        public Role ParseToEntity(string userId)
        {
            var now = DateTime.Now;
            var role = new Role()
            {
                Name = this.Name,
                Active = true,
                Deleted = false,
                CreatedDate = now,
                UpdatedDate = now,
                Createdby = userId,
                AccountRoleMaps = new List<AccountRoleMap>(),
                RoleModuleMaps = new List<RoleModuleMap>()
            };

            return role;
        }
    }

    public class RoleLookupModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }

        public RoleLookupModel(Role role)
        {
            this.Id = role.Id;
            this.Name = role.Name;
            this.Active = role.Active;
        }
    }
}
