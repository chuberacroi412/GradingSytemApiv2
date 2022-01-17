using GradingSytemApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.DTOs
{
    public class BaseDTO
    {
        public string CreatedName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedName { get; set; }
        public DateTime UpdatedDate { get; set; }

        public BaseDTO() { }
        public BaseDTO(IAuditableEntity auditableEntity)
        {
            if (auditableEntity != null)
            {
                CreatedDate = auditableEntity.CreatedDate;
                UpdatedDate = auditableEntity.UpdatedDate;
            }
        }
    }

}
