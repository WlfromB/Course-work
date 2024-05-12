using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсач
{
    class ScheduleRow
    {
        public int? IdTeacher { get; set; }

        public string? Class { get; set; }

        public string? Lesson { get; set; }

        public int? Time { get; set; }

        public string? Day { get; set; }
        public ScheduleRow(int? id, string? _class, string? lesson, int? time, string? day)
        {
            IdTeacher = id;
            Class = _class;
            Lesson = lesson;
            Time = time;
            Day = day;
        }
#nullable disable annotations
        public Dictionary<string, object> RowToDictionary()
        {
            Dictionary<string,object> result = new Dictionary<string, object>()
            {
                {"day", Day },
                {"lessonNumber", Time},
                {"teacherId", IdTeacher },
                {"subjectName", Lesson },
                {"className", Class}
            };
            return result;
        }

#nullable restore
    }
}
