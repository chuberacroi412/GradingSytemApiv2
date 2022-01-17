using GradingSytemApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Services.Interfaces
{
    public interface IPostService
    {
        Guid? CreatePost(CreatePostModel model, ref ErrorModel errors);
        PostModel GetById(Guid Id, ref ErrorModel errors);
        PaginationModel<PostLookupModel> GetPosts(PaginationPostRequest req);
        void AcceptReport(Guid Id, ref ErrorModel errors);
        void RejectReport(Guid Id, ref ErrorModel errors);
    }
}
