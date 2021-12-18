using GradingSytemApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Entities
{
    public class Course : AuditableEntity<Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Image { get; set; }
        public DateTime YearStart { get; set; }
        public DateTime YearEnd { get; set; }
        public int Size { get; set; }
        public virtual ICollection<Score> Scrores { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
