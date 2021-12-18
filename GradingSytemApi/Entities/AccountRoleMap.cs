using GradingSytemApi.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Entities
{
    public class AccountRoleMap : IdentityUserRole<string>
    {
        public virtual Role Role { get; set; }
        public virtual Account Account { get; set; }
    }
}
