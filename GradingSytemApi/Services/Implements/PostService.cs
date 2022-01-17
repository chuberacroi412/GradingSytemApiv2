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
    public class PostService : IPostService
    {
        private readonly ApiDbContext _dbContext;

        public PostService(ApiDbContext apiDbContext)
        {
            _dbContext = apiDbContext;
        }

        private bool ValidateModel(CreatePostModel model, ref ErrorModel errors)
        {
            if(!_dbContext.Courses.Any(x => !x.Deleted && x.Id == model.CourseId))
            {
                errors.Add("Course is not found");
            }

            if (!_dbContext.Users.Any(x => !x.Deleted && x.Id == model.CreatedBy))
            {
                errors.Add("User is not found");
            }

            switch (model.Type)
            {
                case PostType.Exercise:
                    if (!model.DueDate.HasValue)
                        errors.Add("Due date is required");
                    else if(!model.ScoreId.HasValue)
                        errors.Add("Score component is requried");
                    break;

                case PostType.Report:
                    if (!model.ScoreId.HasValue)
                        errors.Add("Score component is requried");
                    break;

                default:
                    break;
            }
            return errors.IsEmpty;
        }

        private bool FilterById(Guid Id, out Post post, ref ErrorModel errors)
        {
            post = _dbContext.Posts.FirstOrDefault(x => !x.Deleted && x.Id == Id);

            if(post == null)
            {
                errors.Add("Post is not found");
            }

            return errors.IsEmpty;
        }
        public Guid? CreatePost(CreatePostModel model, ref ErrorModel errors)
        {
            if(ValidateModel(model, ref errors))
            {
                var post = model.ParseToEntity();

                var entity = _dbContext.Posts.Add(post).Entity;
                _dbContext.SaveChanges();

                return entity.Id;
            }

            return null;
        }

        public PostModel GetById(Guid Id, ref ErrorModel errors)
        {
            if(FilterById(Id, out Post post, ref errors))
            {
                var creater = _dbContext.Users.FirstOrDefault(x => x.Id == post.CreatedBy);
                var comments = _dbContext.Comments.Where(x => !x.Deleted && x.PostId == post.Id).ToList();

                var usersId = comments.Select(x => x.CreatedBy).ToList();
                var accounts = _dbContext.Users.Where(x => usersId.Contains(x.CreatedBy)).ToDictionary(x => x.Id, x => x);

                return new PostModel(post, creater, accounts);
            }

            return null;
        }

        public PaginationModel<PostLookupModel> GetPosts(PaginationPostRequest req)
        {
            var posts = _dbContext.Posts.Where(x => !x.Deleted
                                                &&
                                                x.CourseId == req.CourseId
                                                &&
                                                (!req.Type.HasValue || x.Type == req.Type.Value))
                                                .OrderBy(x => x.CreatedDate);
            var usersId = posts.Select(x => x.CreatedBy);

            var accounts = _dbContext.Users.Where(x => usersId.Contains(x.CreatedBy)).ToDictionary(x => x.Id, x => x);
            var pagination = new PaginationModel<PostLookupModel>(req, posts);

            if(req.PaginationType == PaginationType.Pagination)
            {
                pagination.Data = posts.Skip(req.Amount * req.Page).Take(req.Amount).AsEnumerable().Select(x => new PostLookupModel(x, accounts[x.CreatedBy])).ToList();
            }
            else
            {
                pagination.Data = posts.AsEnumerable().Select(x => new PostLookupModel(x, accounts[x.CreatedBy])).ToList();
            }

            return pagination;
        }

        private void ChangeReportState(Guid Id, ReportStatus state ,ref ErrorModel errors)
        {
            if (FilterById(Id, out Post post, ref errors))
            {
                if (post.Active)
                {
                    post.ReportStatus = state;
                    post.Active = false;

                    _dbContext.SaveChanges();
                }
            }
        }
        public void AcceptReport(Guid Id, ref ErrorModel errors)
        {
            ChangeReportState(Id, ReportStatus.Accept, ref errors);
        }

        public void RejectReport(Guid Id, ref ErrorModel errors)
        {
            ChangeReportState(Id, ReportStatus.Reject, ref errors);
        }
    }
}
