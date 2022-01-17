using GradingSytemApi.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Services.Interfaces
{
    public interface ICourseService
    {
        Guid? CreateCourse(CreateCourseModel model, ref ErrorModel errors);
        Guid? UpdateScoreComponent(Guid Id, List<UpdateScoreComponentModel> model, ref ErrorModel errors);
        CourseModel GetById(Guid Id, string userId, ref ErrorModel errors);
        PaginationModel<CourseShortModel> GetCourses(PaginationCourseRequest req);
        void Join(Guid Id, string userId, ref ErrorModel errors);
        byte[] ExportStudentBoard(Guid Id, ref ErrorModel errors);
        void ImportStudentBoard(Guid Id, IFormFile file, ref ErrorModel errors);
        void UpdateTotalScore(Guid Id, ref ErrorModel errors);
        List<AllScoreModel> GetFullBoard(Guid Id, ref ErrorModel errors);
    }
}
