using GradingSytemApi.DTOs;
using GradingSytemApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ModuleController : BaseController
    {
        private readonly IModuleService _service;

        public ModuleController(IModuleService moduleService)
        {
            _service = moduleService;
        }

        /// <summary>
        /// Get all modules
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public PaginationModel<ModuleModel> GetAll([FromQuery] PaginationModuleRequest req)
        {
            req.Format();

            return _service.GetModules(req);
        }

        [HttpGet("{Id:int}")]
        public IActionResult GetById(int Id)
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
        /// Update role module map state
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPut("{Id:Guid}/{state}")]
        public IActionResult ChangeState(Guid Id, bool state)
        {
            ErrorModel errors = new ErrorModel();
            _service.ChangeRoleModuleMapState(Id, state, ref errors);

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

