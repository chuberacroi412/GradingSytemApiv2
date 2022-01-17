using GradingSytemApi.Common.Constant;
using GradingSytemApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.DTOs
{
    public class ScoreModel : BaseDTO
    {
        public Guid Id { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public float Percent { get; set; }
        public ScoreType Type { get; set; }
        public Guid CourseId { get; set; }

        public ScoreModel(Score score) : base(score)
        {
            this.Id = score.Id;
            this.Active = score.Active;
            this.Name = score.Name;
            this.Percent = score.Percent;
            this.Type = score.Type;
            this.CourseId = score.CourseId;
        }
    }

    

    public class ScoreLookupModel
    {
        public Guid Id { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public float Percent { get; set; }
        public ScoreType Type { get; set; }
        public float Point { get; set; }
        public ScoreLookupModel(Score score, float point)
        {
            this.Id = score.Id;
            this.Active = score.Active;
            this.Percent = score.Percent;
            this.Type = score.Type;
            this.Point = point;
        }

    }
    public class CreateUpdateScoreBaseModel
    {
        [Required( ErrorMessage = "Score component name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Score component percent is required")]
        public float Percent { get; set; }
        [Required(ErrorMessage = "Score component type is required")]
        public ScoreType Type { get; set; }
    }
    public class CreateScoreModel : CreateUpdateScoreBaseModel
    {
        public Guid? CourseId { get; set; }
        public bool Active { get; set; }

        public Score ParseToEntity(Guid courseId)
        {
            Score score = new Score()
            {
                Active = this.Active,
                Name = this.Name,
                Percent = this.Percent,
                Type = this.Type,
                CourseId = courseId
            };

            return score;
        }
    }

    public class UpdateScoreComponentModel : CreateUpdateScoreBaseModel
    {
        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public void UpdateEntity(Score score)
        {
            score.Name = this.Name;
            score.Percent = score.Percent;
        }

        public CreateScoreModel ParseToCreateModel(Guid coruseId)
        {
            CreateScoreModel model = new CreateScoreModel()
            {
                Name = this.Name,
                Percent = this.Percent,
                Type = this.Type,
                Active = true,
                CourseId = coruseId
            };

            return model;
        }
    }

    public class UpdateScoreModel
    {
        public Guid ScoreId { get; set; }
        public string AccountId { get; set; }
        public float Point { get; set; }

        public void UpdateEntity(AccountScoreMap score)
        {
            score.Point = this.Point;
        }
    }

    public class ImportStudentScoreModel
    {
        public string Code { get; set; }
        public float Point { get; set; }
    }

    public class ScoreComponentModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ScoreType Type { get; set; }
        public float Point { get; set; }

        public ScoreComponentModel(Score score, float point)
        {
            this.Id = score.Id;
            this.Name = score.Name;
            this.Type = score.Type;
            this.Point = point;
        }
    }
    public class AllScoreModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string FistName { get; set; }
        public string LastName { get; set; }
        public List<ScoreComponentModel> Scores { get; set; }
        public float TotalPoint { get; set; }
    }
}
