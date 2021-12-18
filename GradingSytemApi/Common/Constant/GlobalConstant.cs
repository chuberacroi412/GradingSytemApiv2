using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Common.Constant
{
    public static class INIT_SQL_PATH
    {
        public static readonly string INIT_MODULES = "/Resources/sql_init_modules.sql";
    }

    public static class GlobalConstant
    {
        public static readonly int LOGIN_EXPIRE_TIME = 4; // 4 day
    }
    
}
