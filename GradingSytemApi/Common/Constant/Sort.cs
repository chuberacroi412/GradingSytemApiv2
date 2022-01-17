using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Common.Constant
{
    public static class Sort
    {
        public static class GeneralSort
        {
            public const string NAME = "name";
            public const string NAME_DESC = "name_desc";
            public const string CODE = "code";
            public const string CODE_DESC = "code_desc";
            public const string ACTIVE = "active";
            public const string ACTIVE_DESC = "active_desc";
            public const string LAST_UPDATE = "last_update";
            public const string LAST_UPDATE_DESC = "last_update_desc";
        }

        public static class CourseSort
        {
            public const string NAME = "name";
            public const string NAME_DESC = "name_desc";
            public const string CODE = "code";
            public const string CODE_DESC = "code_desc";
            public const string SCHOOL_YEAR = "school_year";
            public const string SCHOOL_YEAR_DESC = "school_year_desc";
            public const string SIZE = "size";
            public const string SIZE_DESC = "size_desc";
        }
    }
    

}
