using GradingSytemApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Controllers
{
    public class BaseController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public void AddErrorsFromModelState(ref ErrorModel errors)
        {
            foreach(var modelState in ModelState.Values)
            {
                var errorState = modelState.Errors.FirstOrDefault();

                if(errorState != null)
                {
                    string errorMessage = errorState.ErrorMessage;

                    if(!string.IsNullOrEmpty(errorMessage))
                    {
                        errors.Add(errorMessage);
                    }
                    else
                    {
                        errors.Add(modelState.Errors.FirstOrDefault().Exception.Message);
                    }
                }
            }
        }
    }
}
