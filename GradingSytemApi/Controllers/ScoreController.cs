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
    public class ScoreController : BaseController
    {
        private readonly IScoreService _service;

        public ScoreController(IScoreService scoreService)
        {
            _service = scoreService;
        }

        /// <summary>
        /// Create course
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Create([FromBody] List<UpdateScoreModel> model)
        {
            ErrorModel errors = new ErrorModel();
            if (!ModelState.IsValid)
            {
                AddErrorsFromModelState(ref errors);
                return BadRequest(errors);
            }
            else
            {
                _service.UpdateScore(model, ref errors);

                if (errors.IsEmpty)
                {
                    return Ok();
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
        [HttpPut("ChangeState/{Id:Guid}/{state}")]
        public IActionResult ChangeState(Guid Id, bool state)
        {
            ErrorModel errors = new ErrorModel();
            _service.ChangeState(Id, state, ref errors);

            if (errors.IsEmpty)
            {
                return Ok();
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

            _service.ImportScoreComponent(Id, file, ref errors);

            if (errors.IsEmpty)
            {
                return Ok();
            }
            else
            {
                return BadRequest(errors);
            }

        }

        [HttpGet("ExportScoreBoard/{Id:Guid}")]
        [AllowAnonymous]
        public IActionResult ExportScoreBoard(Guid Id)
        {
            ErrorModel errors = new ErrorModel();
            byte[] data = _service.ExportScoreBoard(Id, ref errors);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "ScoreBoard.xlsx";
            return File(content, contentType, fileName);
        }

        [HttpGet("ExportScoreComponent/{Id:Guid}")]
        [AllowAnonymous]
        public IActionResult ExportScoreComponent(Guid Id)
        {
            ErrorModel errors = new ErrorModel();
            byte[] data = _service.ExportScoreComponent(Id, ref errors);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "Scores.xlsx";
            return File(content, contentType, fileName);
        }
    }
}
