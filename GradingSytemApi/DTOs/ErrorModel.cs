using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.DTOs
{
    public class ErrorModel
    {
        public List<string> Errors { get; set; }
        public bool IsEmpty
        {
            get
            {
                return !this.Errors.Any();
            }
        }

        public void Add(string error)
        {
            Errors.Add(error);
        }

        public ErrorModel()
        {
            this.Errors = new List<string>();
        }
    }
}
