using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Entities
{
    public class Account : IdentityUser
    {
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override string Email { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        [DefaultValue(false)]
        public bool Deleted { get; set; }
        public bool Active { get; set; }
        public bool IsFirstLogin { get; set; }
        public DateTime Birthday { get; set; }
        public long TotalScore { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public virtual ICollection<AccountRoleMap> AccountRoleMaps { get; set; }

        public Account()
        {
            this.AccountRoleMaps = new List<AccountRoleMap>();
        }
    }
}
