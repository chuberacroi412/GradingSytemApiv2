using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GradingSytemApi.Common.Helpers
{
    public class UserResolverService
    {
        private readonly IHttpContextAccessor _httpContext;

        public UserResolverService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public string GetUser()
        {
            try
            {
                var claims = _httpContext.HttpContext?.User?.Claims;

                if(claims.Count() > 0)
                {
                    var nameId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                    if(nameId != null)
                    {
                        return nameId.Value;
                    }
                }
            }
            catch(Exception e)
            {
                
            }

            return string.Empty;
        }
    }
}
