using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсач
{
    public struct ApiMethods
    {
        public const string GETTeachers = "/api/teachers/get_teachers_with_subjects";
        public const string GETClasses = "/api/teachers/get_subjects_by_day";
        public const string GETDays = "/api/teachers/check_day";
        public const string GETSchedule = "/api/teachers/get_schedule";
        public const string GETTeachersId = "/api/teachers/get_teachers";


        public const string POST = "/api/teachers/create_schedule";
        public const string PUT = "/api/put";
        public const string DELETE = "/api/teachers/delete_schedule";
    }
}
