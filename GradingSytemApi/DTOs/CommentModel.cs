using GradingSytemApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.DTOs
{
    public class CommentModel : BaseDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public Guid PostId { get; set; }
        public bool Active { get; set; }
        public AccountLookupModel Account { get; set; }

        public CommentModel(Comment comment, Account account) : base(comment)
        {
            this.Id = comment.Id;
            this.Content = comment.Content;
            this.Image = comment.Image;
            this.Active = comment.Active;
            this.PostId = comment.PostId;
            this.Account = new AccountLookupModel(account);
        }
    }

    public class CreateCommentModel
    {
        [Required(ErrorMessage = "Comment content is required")]
        public string Content { get; set; }
        public string Image { get; set; }
        [Required(ErrorMessage = "Post Id is required")]
        public Guid PostId { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }

        public Comment ParseToEntity()
        {
            Comment comment = new Comment()
            {
                Content = this.Content,
                Image = this.Image,
                PostId = this.PostId,
                Active = this.Active,
                CreatedBy = this.CreatedBy
            };

            return comment;
        }
    }
}
