using GradingSytemApi.Common.Constant;
using GradingSytemApi.DTOs;
using GradingSytemApi.Entities;
using GradingSytemApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Services.Implements
{
    public class CommentService : ICommentService
    {
        private readonly ApiDbContext _dbContext;
        public CommentService(ApiDbContext apiDbContext)
        {
            _dbContext = apiDbContext;
        }

        private bool ValidateModel(CreateCommentModel model, ref ErrorModel errors)
        {
            if(!_dbContext.Posts.Any(x => !x.Deleted && x.Id == model.PostId))
            {
                errors.Add("Post is not found");
            }
            else if(_dbContext.Posts.Any(x => !x.Deleted && x.Id == model.PostId && !x.Active))
            {
                errors.Add("This post has been close");
            }

            if(!_dbContext.Users.Any(x => !x.Deleted && x.Id == model.CreatedBy))
            {
                errors.Add("User is not found");
            }



            return errors.IsEmpty;
        }
        public Guid? CreateComment(CreateCommentModel model, ref ErrorModel errors)
        {
            var comment = model.ParseToEntity();

            var entity = _dbContext.Comments.Add(comment).Entity;
            _dbContext.SaveChanges();

            return entity.Id;
        }

        private bool FilterById(Guid Id, out Comment comment, ref ErrorModel errors)
        {
            comment = _dbContext.Comments.FirstOrDefault(x => !x.Deleted && x.Id == Id);

            if(comment == null)
            {
                errors.Add("Comment is not found");
            }

            return errors.IsEmpty;
        }

        public CommentModel GetById(Guid Id, ref ErrorModel errors)
        {
            if(FilterById(Id, out Comment comment, ref errors))
            {
                var creater = _dbContext.Users.FirstOrDefault(x => x.Id == comment.CreatedBy);
                return new CommentModel(comment, creater);
            }

            return null;
        }

        public PaginationModel<CommentModel> GetComments(PaginationCommentRequest req)
        {
            var comments = _dbContext.Comments.Where(x => !x.Deleted && x.PostId == req.PostId).OrderBy(x => x.CreatedDate);
            var usersId = comments.Select(x => x.CreatedBy);

            var accounts = _dbContext.Users.Where(x => usersId.Contains(x.CreatedBy)).ToDictionary(x => x.Id, x => x);

            var pagination = new PaginationModel<CommentModel>(req, comments);

            if(req.PaginationType == PaginationType.Pagination)
            {
                pagination.Data = comments.Skip(req.Amount * req.Page).Take(req.Amount).AsEnumerable().Select(x => new CommentModel(x, accounts[x.CreatedBy])).ToList();
            }
            else
            {
                pagination.Data = comments.AsEnumerable().Select(x => new CommentModel(x, accounts[x.CreatedBy])).ToList();
            }

            return pagination;
        }
    }
}
