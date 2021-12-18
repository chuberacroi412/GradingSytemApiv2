using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Entities
{
    public class Role : IdentityRole
    {
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Createdby { get; set; }
        public virtual ICollection<RoleModuleMap> RoleModuleMaps { get; set; }
        public virtual ICollection<AccountRoleMap> AccountRoleMaps { get; set; }

        public Role() : base()
        {

        }

        public Role(string roleName) : base(roleName)
        {

        }
    }
}
