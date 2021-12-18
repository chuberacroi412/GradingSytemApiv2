using GradingSytemApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Entities
{
    public class Comment : AuditableEntity<Guid>
    {
        public string Content { get; set; }
        public string Image { get; set; }
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
