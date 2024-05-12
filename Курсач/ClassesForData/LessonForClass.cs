using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсач
{
    public class LessonsForClass
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }

        private List<string> _lessons;

        public List<string> Lessons
        {
            get { return _lessons; }
            private set { _lessons = value; }
        }

        private int[] _times;

        public int[] Times
        {
            get { return _times; }
            set { _times = value; }
        }

        public LessonsForClass(string _name, List<string> lessons)
        {
            Lessons = lessons;
            Times = new int[Lessons.Count];
            Name = _name;
        }
        public LessonsForClass(string _name, List<string> lessons, int number)
        {
            Lessons = lessons;
            Times = new int[number];
            Name = _name;
        }

        public void AddByTime(int time, int number)
        {
            _times[time] = number;
        }

        public void RemoveByTime(int time)
        {
            _times[time] = 0;
        }

        public static List<LessonsForClass> FormLesForEachClass(string json)
        {
            JArray jsonObject = JArray.Parse(json);
            List<LessonsForClass> list = new List<LessonsForClass>();
            foreach (var item in jsonObject)
            {
                string usernameItem = item["class_name"].ToString();
                list.Add(new LessonsForClass(usernameItem, ExtensionMethods.GetList((JArray)item["subjects"])));
            }
            return list;
        }

    }
}
