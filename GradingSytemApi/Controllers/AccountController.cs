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
    public class AccountController : BaseController
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService accountService)
        {
            _service = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountModel model)
        {
            ErrorModel errors = new ErrorModel();
            if(!ModelState.IsValid)
            {
                AddErrorsFromModelState(ref errors);
                return BadRequest(errors);
            }
            else
            {
                var result = await _service.CreateAccount(model);

                if(result.First().Key.IsEmpty)
                {
                    return Ok(result.First().Value);
                }
                else
                {
                    var check = result.First().Key;
                    return BadRequest(check);
                }
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(string Id)
        {
            ErrorModel errors = new ErrorModel();

            var result = await _service.GetById(Id);

            if(result.First().Key.IsEmpty)
            {
                return Ok(result.First().Value);
            }
            else
            {
                return BadRequest(result.First().Key);
            }    
        }

        //[HttpPut("Id")]
        //public async Task<IActionResult> Update(string Id, [FromBody]UpdateAccountModel model)
        //{
        //    ErrorModel errors = new ErrorModel();

        //    var result = await _service.UpdateAccount(Id);

        //    if (result.First().Key.IsEmpty)
        //    {
        //        return Ok(result.First().Value);
        //    }
        //    else
        //    {
        //        return BadRequest(result.First().Key);
        //    }
        //}

        [HttpPut]
        public async Task<IActionResult> UpdateByToken(UpdateProfileModel model)
        {
            ErrorModel errors = new ErrorModel();

            var result = await _service.UpdateByToken(model);

            if (result.First().Key.IsEmpty)
            {
                return Ok(result.First().Value);
            }
            else
            {
                return BadRequest(result.First().Key);
            }
        }

        [HttpDelete("Id")]
        public async Task<IActionResult> Delete(string Id)
        {
            ErrorModel errors = new ErrorModel();

            var result = await _service.DeleteAccount(Id);

            if (result.First().Key.IsEmpty)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.First().Key);
            }
        }
    }
}
