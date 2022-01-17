using GradingSytemApi.Common.Constant;
using GradingSytemApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Entities
{
    public class AccountCourseMap : AuditableEntity<Guid>
    {
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }
        public string AccountId { get; set; }
        public virtual Account Account { get; set; }
        public ClassRole Role { get; set; }
        public float TotalPoint { get; set; }
    }
}
