using GradingSytemApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.DTOs
{
    public class ModuleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsParent { get; set; }
        public int? ParentId { get; set; }

        public ModuleModel(Module module)
        {
            this.Id = module.Id;
            this.Name = module.Name;
            this.IsParent = module.IsParent;
            this.ParentId = module.ParentModuleId;
        }
    }

    public class RoleModuleMapModel : ModuleModel
    {
        public bool Active { get; set; }

        public RoleModuleMapModel(Module module, bool active) : base(module)
        {
            this.Active = active;
        }
    }
}
