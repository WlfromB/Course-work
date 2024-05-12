using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсач
{
    public class LessonsForTeacher
    {
        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            private set
            {
                _name = value;
            }
        }
        // добавить id
        private int _id;
        public int Id
        {
            get => _id;
            private set=> _id = value;
        }
        private List<Tuple<string, string>> _lessonAndClass;
        /// <summary>
        /// ( предмет, класс )
        /// </summary>
        public List<Tuple<string, string>> LessonAndClass 
        {
            get
            {
                return _lessonAndClass;
            }
            private set
            {
                _lessonAndClass = value;
            }
        }

        private int[] _checkerTime;
        public int[] CheckerTime
        {
            get
            {
                return _checkerTime;
            }
            set
            {
                _checkerTime = value;
            }
        }

        public bool IsCorrectAddition(int _time)
        {
            if (CheckerTime[_time] == 2)
                return false;
            return true;
        }



        public LessonsForTeacher FindLessonAdd(string lesson, string _class, int _time)
        {
            foreach (var tmp in LessonAndClass)
            {
                if (tmp.Item2 == lesson && tmp.Item1 == _class)
                {
                    CheckerTime[_time]++;
                    return this;
                }
            }
            return null;
        }

        public LessonsForTeacher FindLessonRemove(string lesson, string _class, int _time)
        {
            foreach (var tmp in _lessonAndClass)
            {
                if (tmp.Item2 == lesson && tmp.Item1 == _class)
                {
                    CheckerTime[_time]--;
                    return this;
                }
            }
            return null;
        }



        public static List<LessonsForTeacher> ToListLessonsForTeacher(Dictionary<string, List<Tuple<string, string>>> tempForForm, List<int>ids) // добавить параметр ID
        {
            List<LessonsForTeacher> lessonsForTeachers = new List<LessonsForTeacher>();
            int idCount = 0;
            foreach (var item in tempForForm.Keys)
            {
                string name = item;
                int count = tempForForm[item].Count;
                List<Tuple<string, string>> tmp = new List<Tuple<string, string>>();
                foreach (var tmp1 in tempForForm[item])
                    tmp.Add(new Tuple<string, string>(tmp1.Item1, tmp1.Item2));
                lessonsForTeachers.Add(new LessonsForTeacher(ids[idCount],count, name, tmp));
                idCount++;
            }
            return lessonsForTeachers;
        }



        public static List<LessonsForTeacher> FormLesForEachTeacher(string json) 
        {
            JArray jsonObject = JArray.Parse(json);
            Dictionary<string, List<Tuple<string, string>>> tempForForm = new Dictionary<string, List<Tuple<string, string>>>();
            List<int> ids = new List<int>();
            foreach (var item in jsonObject)
            {
                string usernameItem = item["fullname"].ToString();
                ids.Add(int.Parse(item["id"].ToString()));
                List<string> tmp1 = ExtensionMethods.GetList((JArray)item["subjects"]);
                List<string> tmp2 = ExtensionMethods.GetList((JArray)item["classes"]);
                tempForForm.Add(usernameItem, new List<Tuple<string, string>>()); /// создаю чтобы строчкой ниже его заполнить
                tempForForm[usernameItem].FormList(tmp2, tmp1);     // заполняю по ключу          
            }
            return ToListLessonsForTeacher(tempForForm,ids); // добавить id
        }



        public LessonsForTeacher(int id,int count = 0, string name = null, List<Tuple<string, string>> lessonsAndClass = null) // добавить id
        {
            Id = id;
            Name = name;
            LessonAndClass = lessonsAndClass;
            CheckerTime = new int[count];
        }


    }
}
