using GradingSytemApi.Common.Constant;
using GradingSytemApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Entities
{
    public class Post : AuditableEntity<Guid>
    {
        public string Image { get; set; }
        public string Tile { get; set; }
        public string Content { get; set; }
        public PostType Type { get; set; }
        public ReportStatus ReportStatus { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }
        public Guid? ScoreId { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

    }
}
