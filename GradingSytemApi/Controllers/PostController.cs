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
    public class PostController : BaseController
    {
        private readonly IPostService _service;

        public PostController(IPostService accountService)
        {
            _service = accountService;
        }

        /// <summary>
        /// Create comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create([FromBody]CreatePostModel model)
        {
            ErrorModel errors = new ErrorModel();
            if (!ModelState.IsValid)
            {
                AddErrorsFromModelState(ref errors);
                return BadRequest(errors);
            }
            else
            {
                var result = _service.CreatePost(model, ref errors);

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
        public PaginationModel<PostLookupModel> GetAll([FromQuery]PaginationPostRequest req)
        {
            req.Format();

            return _service.GetPosts(req);
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

            var result = _service.GetById(Id, ref errors);

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
        /// Accept report request
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("{Id:Guid}/Accept")]
        public IActionResult AcceptReport(Guid Id)
        {
            ErrorModel errors = new ErrorModel();

             _service.AcceptReport(Id, ref errors);

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
        /// Reject report request
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("{Id:Guid}/Reject")]
        public IActionResult RejectReport(Guid Id)
        {
            ErrorModel errors = new ErrorModel();

            _service.RejectReport(Id, ref errors);

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
}
