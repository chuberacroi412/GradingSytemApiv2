using GradingSytemApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.DTOs
{
    public class CourseModel : CourseShortModel
    {
        public List<ScoreLookupModel> Scrores { get; set; }
        public List<PostLookupModel> Posts { get; set; }
        public List<AccountLookupModel> Accounts { get; set; }

        public CourseModel(Course course, Account owner, List<Account> accounts) : base(course, owner)
        {
            this.Scrores = new List<ScoreLookupModel>();
            this.Accounts = accounts.Select(x => new AccountLookupModel(x)).ToList();
            this.Owner = new AccountLookupModel(owner);
        }
    }

    public class CreateCourseModel
    {
        public bool Active { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }
        public string Image { get; set; }
        [Required(ErrorMessage = "Course start time is required")]
        public DateTime YearStart { get; set; }
        [Required(ErrorMessage = "Course end time is required")]
        public DateTime YearEnd { get; set; }
        [Range(50, 250)]
        public int Size { get; set; }
        public string CreatedBy { get; set; }
        public List<CreateScoreModel> Scores { get; set; }

        public Course ParseToEntity()
        {
            Course course = new Course()
            {
                Name = this.Name,
                Active = this.Active,
                Image = this.Image,
                YearStart = this.YearStart,
                YearEnd = this.YearEnd,
                Size = this.Size,
                CreatedBy = this.CreatedBy
            };

            return course;
        }
    }

    public class CourseShortModel : BaseDTO
    {
        public Guid Id { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Image { get; set; }
        public DateTime YearStart { get; set; }
        public DateTime YearEnd { get; set; }
        public int Size { get; set; }
        public AccountLookupModel Owner { get; set; }
        public CourseShortModel(Course course, Account owner) : base(course)
        {
            this.Id = course.Id;
            this.Name = course.Name;
            this.Code = course.Code;
            this.Image = course.Image;
            this.YearStart = course.YearStart;
            this.YearEnd = course.YearEnd;
            this.Size = course.Size;
            this.Active = course.Active;
            this.Owner = new AccountLookupModel(owner);
        }
    }

    public class ImportStudentModel
    {
        public string Code { get; set; }
        public string Fullname { get; set; }
    }

}
