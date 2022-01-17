using GradingSytemApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Entities
{
    public class AccountScoreMap : AuditableEntity<Guid>
    {
        public Guid ScoreId { get; set; }
        public virtual Score Score { get; set; }
        public string AccountId { get; set; }
        public virtual Account Account { get; set; }
        public float Point { get; set; }
    }
}
