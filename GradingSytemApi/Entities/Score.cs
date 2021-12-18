using GradingSytemApi.Common.Constant;
using GradingSytemApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Entities
{
    public class Score : AuditableEntity<Guid>
    {
        public string Name { get; set; }
        public float Percent { get; set; }
        public ScoreType Type { get; set; }
        public float score { get; set; }
        public string AccountId { get; set; }
        public virtual Account Account { get; set; }
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }
    }
}
