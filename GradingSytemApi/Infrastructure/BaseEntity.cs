using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Infrastructure
{
    public abstract class BaseEntity
    {
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
