using GradingSytemApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Entities
{
    public class RoleModuleMap : AuditableEntity<Guid>
    {
        public string RoleId { get; set; }
        public virtual Role Role { get; set; }
        public int ModuleId { get; set; }
        public virtual Module Module { get; set; }
    }
}
