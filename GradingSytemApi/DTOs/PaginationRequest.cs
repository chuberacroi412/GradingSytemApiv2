using GradingSytemApi.Common.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.DTOs
{
    public class PaginationRequest
    {
        public int Page { get; set; }
        public int Amount { get; set; }
        public string Sort { get; set; }
        public bool? Active { get; set; }
        public DateTime? LastUpdate { get; set; }
        public PaginationType PaginationType { get; set; }

        public void Format()
        {
            this.Sort = string.IsNullOrEmpty(this.Sort) ? "" : this.Sort;
            this.Amount = this.Amount <= 0 ? 50 : this.Amount;
            this.Page = this.Page <= 0 ? 0 : this.Page;
        }
    }

    public class PaginationModuleRequest : PaginationRequest
    {
        public string ModuleName { get; set; }
        public bool? IsParent { get; set; }
    }

    public class PaginationCommentRequest : PaginationRequest
    {
        public Guid PostId { get; set; }
    }

    public class PaginationPostRequest : PaginationRequest
    {
        public Guid CourseId { get; set; }
        public PostType? Type { get; set; }
    }

    public class PaginationCourseRequest : PaginationRequest
    {
        public string SearchText { get; set; }

    }
}
