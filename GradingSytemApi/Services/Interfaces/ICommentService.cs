using GradingSytemApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Services.Interfaces
{
    public interface ICommentService
    {
        CommentModel GetById(Guid Id, ref ErrorModel errors);
        Guid? CreateComment(CreateCommentModel model, ref ErrorModel errors);
        PaginationModel<CommentModel> GetComments(PaginationCommentRequest req);
    }
}
