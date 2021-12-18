using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Common.Constant
{
    public enum CreateUserType
    {
        Student = 0,
        Teacher = 1
    }

    public enum PaginationType
    {
        Not_Pagination = 0,
        Pagination = 1,
    }

    public enum ScoreType
    {
        Component = 0,
        MidtermScore = 1,
        FinalScore = 2
    }

    public enum PostType
    {
        Notification = 0,
        Exercise = 1,
        Report = 2,
        Lesson = 3
    }
}
