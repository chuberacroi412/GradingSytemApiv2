using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GradingSytemApi.Infrastructure;

namespace GradingSytemApi.Entities
{
    public class Module : Entity<int>
    {
        public int? ParentModuleId { get; set; }
        public virtual Module ParentModule { get; set; }
        public string Name { get; set; }
        public bool IsParent { get; set; }
        public virtual ICollection<RoleModuleMap> RoleModuleMaps { get; set; }
    }
}
