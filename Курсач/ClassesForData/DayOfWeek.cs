
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсач
{
    public class DayOfWeek
    {
        private string _day;

        public string Day
        {
            get
            {
                return _day;
            }

            private set
            {
                _day = value;
            }
        }

        private List<LessonsForClass> _lessonsForClasses;

        public List<LessonsForClass> LessonsForClasses
        {
            get
            {
                return _lessonsForClasses;
            }
            private set { _lessonsForClasses = value; }
        }


        private List<LessonsForTeacher> _lessonsForTeachers;

        public List<LessonsForTeacher> LessonsFroEachTeacher
        {
            get
            {
                return _lessonsForTeachers;
            }
            private set
            {
                _lessonsForTeachers = value;
            }
        }

        public DayOfWeek(string _day, List<LessonsForClass> _lessonsForClasses, List<LessonsForTeacher> _lessonsForTeachers)
        {
            Day = _day;
            LessonsForClasses = _lessonsForClasses;
            LessonsFroEachTeacher = _lessonsForTeachers;
        }
    }

}
