using GradingSytemApi.DTOs;
using GradingSytemApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CommentController : BaseController
    {
        private readonly ICommentService _service;

        public CommentController(ICommentService accountService)
        {
            _service = accountService;
        }

        /// <summary>
        /// Create comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create([FromBody]CreateCommentModel model)
        {
            ErrorModel errors = new ErrorModel();
            if (!ModelState.IsValid)
            {
                AddErrorsFromModelState(ref errors);
                return BadRequest(errors);
            }
            else
            {
                var result = _service.CreateComment(model, ref errors);

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
        /// Get all comments in a post
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public PaginationModel<CommentModel> GetAll([FromQuery]PaginationCommentRequest req)
        {
            req.Format();

            return _service.GetComments(req);
        }

        /// <summary>
        /// Get a comment by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        public IActionResult GetById(Guid Id)
        {
            ErrorModel errors = new ErrorModel();

            var result =  _service.GetById(Id, ref errors);

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
}
