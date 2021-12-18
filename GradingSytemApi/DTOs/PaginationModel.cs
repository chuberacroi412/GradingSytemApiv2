using GradingSytemApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.DTOs
{
    public class PaginationModel<T>
    {
        public List<T> Data { get; set; }
        public int Page { get; set; }
        public int Amount { get; set; }
        public long TotalCount { get; set; }
        public int TotalPage { get; set; }
        public string Sort { get; set; }

        public PaginationModel()
        {
            this.Data = new List<T>();
        }

        public PaginationModel(PaginationRequest request, IQueryable<BaseEntity> list)
        {
            this.Data = new List<T>();
            this.Amount = request.Amount == 0 ? 1 : request.Amount;
            this.Sort = request.Sort;
            this.Page = request.Page;
            this.TotalCount = list.Count();
            this.TotalPage = (int)Math.Ceiling((decimal)this.TotalCount / Amount);
        }
    }
}
