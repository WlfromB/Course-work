
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.IO;

namespace Курсач
{
    public static class ExtensionMethods
    {
        public static Uri pathConnection;

        static ExtensionMethods()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config\\config.txt");
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("connection"))
                        {
                            string[] parts = line.Split(new char[] { ',', ' ' });
                            if (parts.Length >= 2)
                                for (int i = 1; i < parts.Length; i++)
                                    if (parts[i - 1] == "connection")
                                    {
                                        pathConnection = new Uri(parts[i], UriKind.RelativeOrAbsolute);
                                        break;
                                    }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string ReadDataAsync(string method, string _day)
        {
            string data = "";
            string addQuery = $"{method}?day={_day}";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzdHJpbmciLCJzY29wZXMiOiJyZWFkIiwicmVmZXIiOiJ1c2VyIiwiaWF0IjoxNzA4MTkxNDU0LCJleHAiOjE3MDkyMDIzNTR9.RNqjTXpO5j5kAhZq_XfD_-4o4wtAMCxQnAKauVtmiuw");
                    HttpRequestMessage message = new HttpRequestMessage(new HttpMethod("GET"), new Uri(pathConnection,addQuery));
                    HttpResponseMessage response =  client.Send(message);
                    response.EnsureSuccessStatusCode();                    
                    data = response.Content.ReadAsStringAsync().Result;
                }
                catch (Exception e) { }
            }
            return data;
        }

        public static List<string> GetList(JArray jArray)
        {
            List<string> list = new List<string>();
            foreach (var item in jArray)
                list.Add(item.ToString());
            return list;
        }


        public static void FormList(this List<Tuple<string, string>> tempForForm, List<string> subjects, List<string> classes)
        {
            if (subjects.Count == classes.Count)
                for (int i = 0; i < subjects.Count; i++)
                {
                    Tuple<string, string> tupleTmp = new Tuple<string, string>(subjects[i], classes[i]);
                    tempForForm.Add(tupleTmp);
                }
            else
            {
                throw new Exception("Counts not equal!");
            }
        }

        public static string Get2Numeric(int number)
        {
            string result = number.ToString();
            if (result.Length == 2)
                return result;
            return $"0{result}";
        }

        public static int CorrectionListClasses(this List<LessonsForClass> list)
        {
            int max = 0;
            foreach (var item in list)
                max = Math.Max(max, item.Times.Length);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Times.Length < max)
                {
                    LessonsForClass tmp = new LessonsForClass(list[i].Name, list[i].Lessons, max);
                    list.Insert(i, tmp);
                    list.RemoveAt(i + 1);
                }
            }
            return max;
        }

        public static void CorrectionListTeachers(this List<LessonsForTeacher> list, int max)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].CheckerTime.Length < max)
                {
                    LessonsForTeacher tmp = new LessonsForTeacher(list[i].Id,max, list[i].Name, list[i].LessonAndClass);// добавить id
                    list.Insert(i, tmp);
                    list.RemoveAt(i + 1);
                }
            }
        }

        public static LessonsForTeacher FindTeacher(this List<LessonsForTeacher> forTeachers, string lesson, string _class)
        {
            LessonsForTeacher teacher = null;
            foreach (var item in forTeachers)
            {
                bool t = false;
                foreach (var tmpItem in item.LessonAndClass)
                {
                    if (tmpItem.Item2 == lesson && tmpItem.Item1 == _class)
                        t = true;
                    if (t)
                        break;

                }
                if (t)
                {
                    teacher = item;
                    break;
                }
            }
            return teacher;
        }

        public static int GetNumberClass(string _string, int number) => (number * int.Parse(_string[3..5]) + int.Parse(_string[5..7]));

        public static int GetNumberTime(string _string) => int.Parse(_string[7..]);

        public static string GetNameBtn(int rowUnif, int colUnif, int row) => $"Btn{Get2Numeric(rowUnif)}{Get2Numeric(colUnif)}{Get2Numeric(row)}";


        public static LessonsForClass FindByRowCol(this List<LessonsForClass> lessonsForClass, int row, int col, int magicNumber) => lessonsForClass[magicNumber * row + col];

        public static int GetRow(int index) => index / 16;

        public static int GetCol(int index)
        {
            if (index / 16 == 1)
                return index - 16;
            return index;
        }

        private static Dictionary<string, string> daysOfWeek = new Dictionary<string, string>
        {
            {"понедельник", "Monday"},
            {"вторник", "Tuesday"},
            {"среда", "Wednesday"},
            {"четверг", "Thursday"},
            {"пятница", "Friday"},
            {"суббота", "Saturday"},
            {"воскресенье", "Sunday"}
        };


        public static string GetEngDay(this string day)
        {
            if (daysOfWeek.ContainsKey(day.ToLower()))
            {
                return daysOfWeek[day.ToLower()];
            }
            else
            {
                throw new Exception("Ошибка в наименовании дня недели!");
            }
        }

    }

}