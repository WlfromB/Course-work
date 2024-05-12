using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections;



namespace Курсач
{
#nullable disable annotations
    class Schedule:IEnumerable
    {
        public ScheduleRow[]? Rows { get; set; }

        public Schedule(ScheduleRow[] rows = null)
        {
            Rows = rows;
        }
        public void FormShedule(UniformGrid uniformGrid, string day)
        {
            int numberLessons = Page3.countLessonsInDay;

            ScheduleRow[]? rows = new ScheduleRow?[numberLessons];
            int count = 0;
            foreach(Grid grid in uniformGrid.Children)
            {
                Grid gridWhereNameClass = grid.Children[0] as Grid;
                Label nameClass = gridWhereNameClass.Children[0] as Label;
                string scheduleRowNameClass = Page3.GetLabelContent(nameClass);//(string)nameClass.Content;
                for (int i = 1;i < grid.Children.Count;i++)
                {
                    // время i - 1, узнать учителя, достать название предмета
                    Grid gridWhereNameLesson = grid.Children[i] as Grid;
                    Label nameLesson = gridWhereNameLesson.Children[0] as Label;
                    string scheduleRowNameLesson = Page3.GetLabelContent(nameLesson);// (string)nameLesson.Content;
                    if (scheduleRowNameLesson != "")
                    {
                        int idTeacher = Page3.Day.LessonsFroEachTeacher.FindTeacher(scheduleRowNameLesson, scheduleRowNameClass).Id;
                        rows[count] = new ScheduleRow(idTeacher, scheduleRowNameClass, scheduleRowNameLesson, i, day);
                        count++;
                    }
                }
                if(count == numberLessons)
                {
                    break;
                }
            }

            Rows = rows;

        }
        //сформировать JSON
        public JArray FormJSON()
        {
            JArray jsonForSchedule = new JArray();  
            foreach(ScheduleRow? row in this.Rows)
            {
                jsonForSchedule.Add(JObject.FromObject(row.RowToDictionary()));
            }
            return jsonForSchedule;
        }

        public static Schedule JsonToSchedule(string json, out int maxTime, out int numberClasses)
        {
            maxTime = 0;
            Schedule schedule = new Schedule();
            JArray jsonForSchedule = JArray.Parse(json);
            ScheduleRow?[] rows = new ScheduleRow?[jsonForSchedule.Count];
            HashSet<string> classes = new HashSet<string>();
            int count = 0;
            foreach (JObject item in  jsonForSchedule)
            {
                string? day = item["day"].ToString();
                int? time = int.Parse(item["lesson_number"].ToString());
                int? teacherId = int.Parse(item["teacher_id"].ToString());
                string? subjectName = item["subject_name"].ToString();
                string? className = item["class_name"].ToString();
                maxTime = Math.Max(maxTime, (int)time);
                rows[count] = new ScheduleRow(teacherId, className, subjectName, time, day);
                count++;
                classes.Add(className);
            }
            schedule.Rows= rows;
            numberClasses = classes.Count;
            return schedule;
        }

        public IEnumerator GetEnumerator() => Rows.GetEnumerator();
    }
}
