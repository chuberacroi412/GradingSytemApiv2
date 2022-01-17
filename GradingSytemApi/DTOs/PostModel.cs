using GradingSytemApi.Common.Constant;
using GradingSytemApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.DTOs
{
    public class PostModel : BaseDTO
    {
        public Guid Id { get; set; }
        public string Image { get; set; }
        public string Tile { get; set; }
        public string Content { get; set; }
        public PostType Type { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid CourseId { get; set; }
        public Guid? ScoreId { get; set; }
        public bool Active { get; set; }
        public ReportStatus ReportStatus { get; set; }
        public List<CommentModel> Comments { get; set; }
        public AccountLookupModel Account { get; set; }

        public PostModel(Post post, Account account, Dictionary<string, Account> users) : base(post)
        {
            this.Id = post.Id;
            this.Image = post.Image;
            this.Tile = post.Tile;
            this.Content = post.Content;
            this.Type = post.Type;
            this.DueDate = post.DueDate;
            this.CourseId = post.CourseId;
            this.ScoreId = post.ScoreId;
            this.Active = post.Active;
            this.ReportStatus = post.ReportStatus;
            this.Comments = post.Comments?.Select(x => new CommentModel(x, users[x.CreatedBy])).ToList();
            this.Account = new AccountLookupModel(account);
        }
    }

    public class CreatePostModel
    {
        public string Image { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Tile { get; set; }
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }
        [Required(ErrorMessage = "Post type is requried")]
        public PostType Type { get; set; }
        public DateTime? DueDate { get; set; }
        [Required(ErrorMessage = "Course Id is requried")]
        public Guid CourseId { get; set; }
        public Guid? ScoreId { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }

        public Post ParseToEntity()
        {
            Post post = new Post()
            {
                Image = this.Image,
                Tile = this.Tile,
                Content = this.Content,
                Type = this.Type,
                DueDate = this.DueDate,
                CourseId = this.CourseId,
                ScoreId = this.ScoreId,
                ReportStatus = ReportStatus.Pending,
                Active = this.Active,
                CreatedBy = this.CreatedBy
            };

            return post;
        }
    }

    public class UpdatePostModel
    {
        public string Image { get; set; }
        public string Tile { get; set; }
        public string Content { get; set; }
        public DateTime? DueDate { get; set; }

        public void UpdateEntity(Post post)
        {
            post.Image = this.Image;
            post.Tile = this.Tile;
            post.Content = this.Content;
            post.DueDate = this.DueDate;
        }
    }

    public class PostLookupModel
    {
        public Guid Id { get; set; }
        public string Tile { get; set; }
        public PostType Type { get; set; }
        public DateTime? DueDate { get; set; }
        public AccountLookupModel Account { get; set; }
        public PostLookupModel(Post post, Account account)
        {
            this.Id = post.Id;
            this.Tile = post.Tile;
            this.Type = post.Type;
            this.DueDate = post.DueDate;
            this.Account = new AccountLookupModel(account);
        }
    }
}
