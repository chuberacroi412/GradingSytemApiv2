using GradingSytemApi.DTOs;
using GradingSytemApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CourseController : BaseController
    {
        private readonly ICourseService _service;

        public CourseController(ICourseService courseService)
        {
            _service = courseService;
        }

        /// <summary>
        /// Create course
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create([FromBody] CreateCourseModel model)
        {
            ErrorModel errors = new ErrorModel();
            if (!ModelState.IsValid)
            {
                AddErrorsFromModelState(ref errors);
                return BadRequest(errors);
            }
            else
            {
                var result = _service.CreateCourse(model, ref errors);

                if (errors.IsEmpty)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(errors);
                }
            }
        }

        /// <summary>
        /// Get all course
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public PaginationModel<CourseShortModel> GetAll([FromQuery] PaginationCourseRequest req)
        {
            req.Format();

            return _service.GetCourses(req);
        }

        /// <summary>
        /// Get a course by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        public IActionResult GetById(Guid Id, [FromBody] string userId)
        {
            ErrorModel errors = new ErrorModel();

            var result = _service.GetById(Id, userId, ref errors);

            if (errors.IsEmpty)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(errors);
            }
        }

        /// <summary>
        /// Join a course
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("Join/{Id:Guid}")]
        public IActionResult Join(Guid Id, [FromBody] string userId)
        {
            ErrorModel errors = new ErrorModel();

             _service.Join(Id, userId, ref errors);

            if (errors.IsEmpty)
            {
                return Ok();
            }
            else
            {
                return BadRequest(errors);
            }
        }

        /// <summary>
        /// Update score component
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{Id:Guid}")]
        public IActionResult Join(Guid Id, [FromBody] List<UpdateScoreComponentModel> model)
        {
            ErrorModel errors = new ErrorModel();

            var result = _service.UpdateScoreComponent(Id, model, ref errors);

            if (errors.IsEmpty)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(errors);
            }
        }

        [HttpPost("ExcelTemplate/{Id:Guid}")]
        public IActionResult ImportFromExcelFile(Guid Id, IFormFile file)
        {
            ErrorModel errors = new ErrorModel();

            _service.ImportStudentBoard(Id, file, ref errors);

            if (errors.IsEmpty)
            {
                return Ok();
            }
            else
            {
                return BadRequest(errors);
            }
                
        }

        [HttpGet("ExportTemplate/{Id:Guid}")]
        [AllowAnonymous]
        public IActionResult ExportStudentBoard(Guid Id)
        {
            ErrorModel errors = new ErrorModel();
            byte[] data = _service.ExportStudentBoard(Id, ref errors);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "StudentBoard.xlsx";
            return File(content, contentType, fileName);
        }
    }
}
