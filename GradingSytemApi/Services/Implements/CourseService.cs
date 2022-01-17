using GradingSytemApi.Common.Constant;
using GradingSytemApi.Common.Helpers;
using GradingSytemApi.DTOs;
using GradingSytemApi.Entities;
using GradingSytemApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Services.Implements
{
    public class CourseService : ICourseService
    {
        private readonly ApiDbContext _dbContext;

        public CourseService(ApiDbContext apiDbContext)
        {
            _dbContext = apiDbContext;
        }

        private bool ValidateModel(Guid? Id, CreateCourseModel model, ref ErrorModel errors)
        {
            if(_dbContext.Courses.Any(x => !x.Deleted && x.Code == model.Code && x.Id != Id))
            {
                errors.Add("Course code is dupplicate");
            }

            if(!model.Scores.Any())
            {
                errors.Add("Score component is required");
            }
            else if(!model.Scores.Any(x => x.Type == ScoreType.MidtermScore))
            {
                errors.Add("Midterm score is missing");
            }
            else if(model.Scores.Count(x => x.Type == ScoreType.MidtermScore) > 1)
            {
                errors.Add("Midterm score is dupplicate");
            }
            else if(!model.Scores.Any(x => x.Type == ScoreType.FinalScore))
            {
                errors.Add("Final score is missing");
            }
            else if (model.Scores.Count(x => x.Type == ScoreType.FinalScore) > 1)
            {
                errors.Add("Final score is dupplicate");
            }
            else if(model.Scores.Sum(x => x.Percent) != 100)
            {
                errors.Add("Sum of all component must be equal 100 percent");
            }

            return errors.IsEmpty;
        }

        private List<Score> GenerateScoreForStudent(List<Score> scoreComponent, Guid courseId)
        {
            var studentsId = _dbContext.AccountCourseMaps.Where(x => !x.Deleted && x.CourseId == courseId && x.Role == ClassRole.Student)
                                    .Select(x => x.AccountId).ToList();

            foreach(var score in scoreComponent)
            {
                foreach (var studentId in studentsId)
                {
                    score.AccountScoreMaps.Add(new AccountScoreMap()
                    {
                        AccountId = studentId,
                        Point = 0
                    });
                }
            }

            return scoreComponent;
        }

        private List<Score> GenerateScoreForStudent(List<Score> scoreComponent, Guid courseId, string studentId)
        {
            foreach (var score in scoreComponent)
            {
                score.AccountScoreMaps.Add(new AccountScoreMap()
                {
                    AccountId = studentId,
                    Point = 0
                });
            }

            return scoreComponent;
        }

        public Guid? CreateCourse(CreateCourseModel model, ref ErrorModel errors)
        {
            if(ValidateModel(null, model, ref errors))
            {
                var course = model.ParseToEntity();

                // Add parent score
                var entity = _dbContext.Add(course).Entity;
                _dbContext.SaveChanges();


                // Generate score for student
                var scoreComponent = model.Scores.Select(x => x.ParseToEntity(entity.Id)).ToList();
                scoreComponent = GenerateScoreForStudent(scoreComponent, entity.Id);

                entity.Scores = scoreComponent;
                _dbContext.SaveChanges();

                return entity.Id;
            }

            return null;
        }


        private bool FilterById(Guid Id, out Course course, ref ErrorModel errors)
        {
            course = _dbContext.Courses.FirstOrDefault(x => !x.Deleted && x.Id == Id);

            if(course == null)
            {
                errors.Add("Course is not found");
            }

            return errors.IsEmpty;
        }

        public CourseModel GetById(Guid Id, string userId ,ref ErrorModel errors)
        {
            if(FilterById(Id, out Course course, ref errors))
            {
                // Get owner and other student
                var owner = _dbContext.Users.FirstOrDefault(x => x.Id == course.CreatedBy);
                var studentsId = _dbContext.AccountCourseMaps.Where(x => !x.Deleted && x.CourseId == course.Id && x.AccountId != owner.Id && x.AccountId != userId)
                                                            .Select(x => x.AccountId).ToList();

                var students = _dbContext.Users.Where(x => !x.Deleted && studentsId.Contains(x.Id)).ToList();

                // Get current user score information
                var classScoresId = course.Scores.Select(x => x.Id).ToList();
                var userScore = _dbContext.AccountScoreMaps.Where(x => !x.Deleted && x.AccountId == userId && classScoresId.Contains(x.ScoreId))
                                                            .Select(x => new {x.Score, x.Point }).ToList();

                // Build model
                var courseModel = new CourseModel(course, owner, students);
                courseModel.Scrores = userScore.Select(x => new ScoreLookupModel(x.Score, x.Point)).ToList();

                return courseModel;
            }

            return null;
        }

        private IQueryable<Course> Filter(PaginationCourseRequest req)
        {
            return _dbContext.Courses.Where(x => !x.Deleted
                                            &&
                                            (string.IsNullOrEmpty(req.SearchText) || x.Name.ToLower().Contains(req.SearchText.ToLower()) || x.Code.ToLower().Contains(req.SearchText.ToLower())));
        }

        private IQueryable<Course> Sorts(PaginationCourseRequest req, IQueryable<Course> courses)
        {
            switch(req.Sort)
            {
                case Sort.CourseSort.NAME:
                    courses = courses.OrderBy(x => x.Name);
                    break;
                case Sort.CourseSort.NAME_DESC:
                    courses = courses.OrderByDescending(x => x.Name);
                    break;
                case Sort.CourseSort.CODE:
                    courses = courses.OrderBy(x => x.Code);
                    break;
                case Sort.CourseSort.CODE_DESC:
                    courses = courses.OrderByDescending(x => x.Code);
                    break;
                case Sort.CourseSort.SIZE:
                    courses = courses.OrderBy(x => x.Size);
                    break;
                case Sort.CourseSort.SIZE_DESC:
                    courses = courses.OrderByDescending(x => x.Size);
                    break;
                case Sort.CourseSort.SCHOOL_YEAR:
                    courses = courses.OrderBy(x => x.YearStart);
                    break;
                case Sort.CourseSort.SCHOOL_YEAR_DESC:
                    courses = courses.OrderByDescending(x => x.YearStart);
                    break;
            }

            return courses;
        }

        public PaginationModel<CourseShortModel> GetCourses(PaginationCourseRequest req)
        {
            var courses = Filter(req);
            courses = Sorts(req, courses);
            var usersId = courses.Select(x => x.CreatedBy);

            var accounts = _dbContext.Users.Where(x => usersId.Contains(x.CreatedBy)).ToDictionary(x => x.Id, x => x);
            var pagination = new PaginationModel<CourseShortModel>(req, courses);

            if(req.PaginationType == PaginationType.Pagination)
            {
                pagination.Data = courses.Skip(req.Amount * req.Page).Take(req.Amount).AsEnumerable().Select(x => new CourseShortModel(x, accounts[x.CreatedBy])).ToList();
            }
            else
            {
                pagination.Data = courses.AsEnumerable().Select(x => new CourseShortModel(x, accounts[x.CreatedBy])).ToList();
            }

            return pagination;
        }

        public void Join(Guid Id, string userId, ref ErrorModel errors)
        {
            if(FilterById(Id, out Course course, ref errors))
            {
                var scores = _dbContext.Scores.Where(x => !x.Deleted && x.CourseId == course.Id).ToList();
                if(scores != null)
                {
                    scores = GenerateScoreForStudent(scores, course.Id, userId);
                    course.Scores = scores;

                    _dbContext.SaveChanges();
                }

            }
        }

        private bool ValidateUpdateScoreModel(List<UpdateScoreComponentModel> model, ref ErrorModel errors)
        {
            if (!model.Any())
            {
                errors.Add("Score component is required");
            }
            else if (!model.Any(x => !x.Deleted && x.Type == ScoreType.MidtermScore))
            {
                errors.Add("Midterm score is missing");
            }
            else if (model.Count(x => !x.Deleted && x.Type == ScoreType.MidtermScore) > 1)
            {
                errors.Add("Midterm score is dupplicate");
            }
            else if (!model.Any(x => !x.Deleted && x.Type == ScoreType.FinalScore))
            {
                errors.Add("Final score is missing");
            }
            else if (model.Count(x => !x.Deleted && x.Type == ScoreType.FinalScore) > 1)
            {
                errors.Add("Final score is dupplicate");
            }
            else if (model.Where(x => !x.Deleted).Sum(x => x.Percent) != 100)
            {
                errors.Add("Sum of all component must be equal 100 percent");
            }

            return errors.IsEmpty;
        }

        private bool ValidateDeletedScoreComponent(List<Guid> scoresId, out List<Score> scoresToDeleted, out List<AccountScoreMap> studentScoresToDelete, ref ErrorModel errors)
        {
            scoresToDeleted = new List<Score>();
            studentScoresToDelete = new List<AccountScoreMap>();

            if (_dbContext.AccountScoreMaps.Any(x => !x.Deleted && scoresId.Contains(x.ScoreId) && x.Point != 0))
            {
                errors.Add("Can not delete score have values");
            }
            else
            {
                scoresToDeleted = _dbContext.Scores.Where(x => !x.Deleted && scoresId.Contains(x.Id)).ToList();
                studentScoresToDelete = _dbContext.AccountScoreMaps.Where(x => !x.Deleted && scoresId.Contains(x.ScoreId)).ToList();
            }

            return errors.IsEmpty;
        }

        public Guid? UpdateScoreComponent(Guid Id, List<UpdateScoreComponentModel> model, ref ErrorModel errors)
        {
            // Validate request data
            if(FilterById(Id, out Course course, ref errors) && ValidateUpdateScoreModel(model, ref errors))
            {
                var deletedScoresId = model.Where(x => x.Deleted).Select(x => x.Id).ToList();

                // Validate logic
                if(ValidateDeletedScoreComponent(deletedScoresId, out List<Score> scoresToDeleted, out List<AccountScoreMap> studentScoresToDelete, ref errors))
                {
                    // Build model
                    var createModels = model.Where(x => x.Id == null).Select(x => x.ParseToCreateModel(course.Id)).ToList();
                    var scoresToCreate = createModels.Select(x => x.ParseToEntity(course.Id)).ToList();

                    // Generate map for new scores
                    GenerateScoreForStudent(scoresToCreate, course.Id).ForEach(x => course.Scores.Add(x));

                    // Remove scores
                    _dbContext.AccountScoreMaps.RemoveRange(studentScoresToDelete);
                    _dbContext.Scores.RemoveRange(scoresToDeleted);

                    _dbContext.SaveChanges();

                    return course.Id;
                }
            }

            return null;
        }

        public byte[] ExportStudentBoard(Guid Id, ref ErrorModel errors)
        {
            if(FilterById(Id, out Course course, ref errors))
            {
                var studentsId = _dbContext.AccountCourseMaps.Where(x => !x.Deleted && x.CourseId == course.Id).Select(x => x.AccountId);
                var students = _dbContext.Users.Where(x => !x.Deleted && studentsId.Contains(x.Id)).Select(x => new { x.Id, x.FirstName, x.LastName, x.Code }).ToList();

                ExcelPackage package = new ExcelPackage();
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ImportScoreComponent");
                int countFormat = students.Count;
                var cell = worksheet.Cells;
                cell.Style.WrapText = true;
                var FirstTableRange = cell[1, 1, 1 + countFormat, 1 + students.Count()];
                FirstTableRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                FirstTableRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                FirstTableRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                FirstTableRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                FirstTableRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                cell[1, 1, 1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1, 1, 2].AutoFitColumns(15, 20);

                cell[1, 1].Value = "Student Id";
                cell[1, 2].Value = "Full name";

                for(int i = 0; i < students.Count(); i++)
                {
                    cell[i + 1, 1].Value = students[i].Code;
                    cell[i + 1, 2].Value = $"{students[i].FirstName} {students[i].LastName}";
                }

                return package.GetAsByteArray();
            }

            return null;
        }
        private List<ImportStudentModel> ReadImportStudentFromExcelFile(IFormFile file, ref ErrorModel errors)
        {
            List<ImportStudentModel> result = new List<ImportStudentModel>();
            using (MemoryStream stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = 0;
                    if (worksheet.Dimension != null)
                    {
                        rowCount = worksheet.Dimension.Rows;
                    }

                    int row = 2;
                    while(row < rowCount)
                    {
                        string code = worksheet.Cells[row, 1].Value?.ToString().Trim();
                        string fullName = worksheet.Cells[row, 2].Value?.ToString().Trim();

                        if(string.IsNullOrEmpty(code) && string.IsNullOrEmpty(fullName))
                        {
                            break;
                        }
                        else if(!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(fullName))
                        {
                            result.Add(new ImportStudentModel()
                            {
                                Code = code,
                                Fullname = fullName
                            });
                        }
                        else
                        {
                            errors.Add($"Data invalid at line {row}");
                        }
                    }
                }
            }

            return result;
        }
        public void ImportStudentBoard(Guid Id, IFormFile file, ref ErrorModel errors)
        {
            UtilService.CheckImportExcelFilleType(file, ref errors);
            if (!errors.IsEmpty)
            {
                return;
            }

            if(FilterById(Id, out Course course, ref errors))
            {
                var studentsId = _dbContext.AccountCourseMaps.Where(x => !x.Deleted && x.CourseId == course.Id).Select(x => x.AccountId);
                var codeExisted = _dbContext.Users.Where(x => !x.Deleted && !string.IsNullOrEmpty(x.Code)).Select(x => new { x.Code, x.Id}).ToList();

                var students = _dbContext.Users.Where(x => !x.Deleted && studentsId.Contains(x.Id)).ToList();

                var importData = ReadImportStudentFromExcelFile(file, ref errors);

                foreach(var item in importData)
                {
                    var student = students.FirstOrDefault(x => $"{x.FirstName}  {x.LastName}" == item.Fullname);
                    var checkCodeExisted = codeExisted.Where(x => x.Code == item.Code);

                    if(student != null)
                    {
                        if(!string.IsNullOrEmpty(student.Code) && student.Code != item.Code)
                        {
                            errors.Add($"student id {item.Code} is invalid");
                        }
                        else if(checkCodeExisted.Count() > 1 || checkCodeExisted.FirstOrDefault().Id != student.Id)
                        {
                            errors.Add($"student id {item.Code} is using by other person");
                        }
                        else
                        {
                            student.Code = item.Code;
                        }
                    }
                    else
                    {
                        errors.Add($"student id {item.Code} is not found");
                    }
                }

                if (errors.IsEmpty)
                    _dbContext.SaveChanges();
            }
        }

        public void UpdateTotalScore(Guid Id, ref ErrorModel errors)
        {
            if(FilterById(Id, out Course course, ref errors))
            {
                var students = _dbContext.AccountCourseMaps.Where(x => !x.Deleted && x.CourseId == course.Id).ToList();
                var scores = _dbContext.Scores.Where(x => !x.Deleted && x.CourseId == course.Id).ToList();

                var studentScores = _dbContext.AccountScoreMaps.Where(x => !x.Deleted && scores.Select(y => y.Id).ToList().Contains(x.ScoreId)).ToList();

                foreach(var student in students)
                {
                    var components = studentScores.Where(x => x.AccountId == student.AccountId).Select(x => x.Point / 10 * x.Score.Percent / 10);
                    var total = components.Sum();

                    student.TotalPoint = total;
                }
            }
        }

        public List<AllScoreModel> GetFullBoard(Guid Id, ref ErrorModel errors)
        {
            if(FilterById(Id, out Course course, ref errors))
            {
                var board = new List<AllScoreModel>();

                var students = _dbContext.AccountCourseMaps.Where(x => !x.Deleted && x.CourseId == course.Id).ToList();
                var scores = _dbContext.Scores.Where(x => !x.Deleted && x.CourseId == course.Id).ToList();

                var studentInfo = _dbContext.Users.Where(x => !x.Deleted && students.Select(y => y.AccountId).ToList().Contains(x.Id)).ToDictionary(x => x.Id, x => x);
                var studentScores = _dbContext.AccountScoreMaps.Where(x => !x.Deleted && scores.Select(y => y.Id).ToList().Contains(x.ScoreId)).ToList();

                foreach(var student in students)
                {
                    var studentScore = studentScores.Where(x => x.AccountId == student.AccountId).ToList();
                    board.Add(new AllScoreModel()
                    {
                        Id = student.AccountId,
                        Code = studentInfo[student.AccountId].Code,
                        FistName = studentInfo[student.AccountId].FirstName,
                        LastName = studentInfo[student.AccountId].LastName,
                        TotalPoint = student.TotalPoint,
                        Scores = studentScore.Select(x => new ScoreComponentModel(scores.FirstOrDefault(y => y.Id == x.ScoreId), x.Point)).ToList()
                    });
                }

                return board;
            }

            return null;
        }
    }
}
