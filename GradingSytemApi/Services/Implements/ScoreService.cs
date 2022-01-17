using GradingSytemApi.DTOs;
using GradingSytemApi.Entities;
using GradingSytemApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using GradingSytemApi.Common.Helpers;
using System.IO;

namespace GradingSytemApi.Services.Implements
{
    public class ScoreService : IScoreService
    {
        private readonly ApiDbContext _dbContext;
        private readonly ICourseService _courseService;

        public ScoreService(ApiDbContext apiDbContext, ICourseService courseService)
        {
            _dbContext = apiDbContext;
            _courseService = courseService;
        }

        private bool FilterById(Guid Id, out Score score, ref ErrorModel errors)
        {
            score = _dbContext.Scores.FirstOrDefault(x => !x.Deleted && x.Id == Id);

            if (score == null)
            {
                errors.Add("Score is not found");
            }

            return errors.IsEmpty;
        }

        private bool FilterCourseById(Guid Id, out Course course, ref ErrorModel errors)
        {
            course = _dbContext.Courses.FirstOrDefault(x => !x.Deleted && x.Id == Id);

            if (course == null)
            {
                errors.Add("Course is not found");
            }

            return errors.IsEmpty;
        }
        public byte[] ExportScoreBoard(Guid Id, ref ErrorModel errors)
        {
            if(FilterCourseById(Id, out Course course, ref errors))
            {
                return ExportScore(course.Scores.ToList());
            }

            return null;
        }

        private byte[] ExportScore(List<Score> scores)
        {
            var studentScores = _dbContext.AccountScoreMaps.Where(x => !x.Deleted && scores.Select(x => x.Id).Contains(x.ScoreId)).ToList();
            var studentsId = studentScores.Select(x => x.AccountId).Distinct();
            var studentName = _dbContext.Users.Where(x => !x.Deleted && studentsId.Contains(x.Id)).ToDictionary(x => x.Id, x => $"{x.FirstName}  {x.LastName}");

            int countFormat = studentScores.Count;
            ExcelPackage package = new ExcelPackage();
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ImportScoreComponent");

            var cell = worksheet.Cells;
            cell.Style.WrapText = true;
            var FirstTableRange = cell[1, 1, 1 + countFormat, 1 + scores.Count()];
            FirstTableRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            FirstTableRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            FirstTableRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            FirstTableRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            FirstTableRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cell[1, 1, 1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, 1, 1, 2].AutoFitColumns(15, 20);

            for(int i = 0; i < scores.Count(); i++)
            {
                cell[1, 1].Value = $"{scores[i].Name}";
            }

            cell[1, 1, 1, scores.Count()].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            cell[1, 1, 1, scores.Count()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            for (int i = 0; i < studentName.Values.Count(); i++)
            {
                cell[i + 1, 1].Value = studentName[studentScores[i].AccountId];

                for(int j = 0; j < scores.Count(); j++)
                {
                    var score = studentScores.FirstOrDefault(x => x.AccountId == studentScores[i].AccountId && x.ScoreId == scores[j].Id);
                    cell[i + 1, 2 + j].Value = score.Point;
                }
                
            }

            return package.GetAsByteArray();
        }

        public byte[] ExportScoreComponent(Guid Id, ref ErrorModel errors)
        {
            if(FilterById(Id, out Score score, ref errors))
            {
                return ExportScore(new List<Score>() { score });
            }

            return null;
        }

        public void ImportScoreBoard(Guid Id, IFormFile file, ref ErrorModel errors)
        {
            throw new NotImplementedException();
        }

        private List<ImportStudentScoreModel> ReadImportScoreFromExcelFile(IFormFile file, ref ErrorModel errors)
        {
            List<ImportStudentScoreModel> result = new List<ImportStudentScoreModel>();
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

                    while (row < rowCount)
                    {
                        string code = worksheet.Cells[row, 1].Value?.ToString().Trim();
                        float.TryParse(worksheet.Cells[row, 2].Value?.ToString().Trim(), out float point);

                        if(string.IsNullOrEmpty(code))
                        {
                            errors.Add($"student id in row {row} is invalid");
                        }
                        else
                        {
                            result.Add(new ImportStudentScoreModel()
                            {
                                Code = code,
                                Point = point
                            });
                        }
                    }
                }
            }

                    return result;
        }
        public void ImportScoreComponent(Guid Id, IFormFile file, ref ErrorModel errors)
        {
            UtilService.CheckImportExcelFilleType(file, ref errors);
            if (!errors.IsEmpty)
            {
                return;
            }

            if (FilterById(Id, out Score score, ref errors))
            {
                var studentScores = _dbContext.AccountScoreMaps.Where(x => !x.Deleted && x.ScoreId == score.Id).ToList();
                var studentsId = studentScores.Select(x => x.AccountId);
                var students = _dbContext.Users.Where(x => !x.Deleted && !string.IsNullOrEmpty(x.Code) && studentsId.Contains(x.Id)).ToDictionary(x => x.Code, x => x.Id);

                var importData = ReadImportScoreFromExcelFile(file, ref errors);

                foreach(var item in importData)
                {
                    if(students.ContainsKey(item.Code))
                    {
                        var studentScore = studentScores.FirstOrDefault(x => x.AccountId == students[item.Code]);
                        if(studentScore != null)
                        {
                            studentScore.Point = item.Point;
                        }

                    }
                    else
                    {
                        errors.Add($"student id {item.Code} is not exsited");
                    }
                }


                _courseService.UpdateTotalScore(studentScores.FirstOrDefault().Score.CourseId, ref errors);
                if(errors.IsEmpty)
                {
                    _dbContext.SaveChanges();
                }
            }
        }

        public void UpdateScore(List<UpdateScoreModel> model, ref ErrorModel errors)
        {
            var scoresId = model.Select(x => x.ScoreId).Distinct().ToList();

            var students = _dbContext.AccountScoreMaps.Where(x => !x.Deleted && scoresId.Contains(x.ScoreId)).ToList();

            foreach(var score in model)
            {
                var student = students.FirstOrDefault(x => !x.Deleted && x.ScoreId == score.ScoreId && x.AccountId == score.AccountId);

                if(student != null)
                {
                    student.Point = score.Point;
                }
                else
                {
                    errors.Add($"Student id {score.AccountId} not found");
                    break;
                }
            }

            _courseService.UpdateTotalScore(students.FirstOrDefault().Score.CourseId, ref errors);
            if (errors.IsEmpty)
            {              
                _dbContext.SaveChanges();
            }
        }

        public void ChangeState(Guid Id, bool state, ref ErrorModel errors)
        {
            if(FilterById(Id, out Score score, ref errors))
            {
                var studentScores = _dbContext.AccountScoreMaps.Where(x => !x.Deleted && x.ScoreId == score.Id).ToList();

                score.Active = state;
                studentScores.ForEach(x => x.Active = state);

                _dbContext.SaveChanges();
            }
        }
    }
}
