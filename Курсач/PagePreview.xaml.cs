using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Курсач.ClassesForData;
namespace Курсач
{
    /// <summary>
    /// Логика взаимодействия для PagePreview.xaml
    /// </summary>
    public partial class PagePreview : Page
    {
        private string day;

        private int maxTime;

        private Schedule schedule;

        private int countClasses;

        private int rows;

        private List<Teacher> Teachers { get;set; } 
        private record class Teacher(int Id , string Name);

        private SortedSet<string> namesClasses = new SortedSet<string>();
        public PagePreview(string day)
        {
            
            InitializeComponent();
            Teachers = new List<Teacher>();
            TextForLabelNameDay.Text = day;
            this.day = day;
            schedule = GetSchedule(day, out maxTime, out countClasses);
            GetTeachers();
            rows = countClasses / 15;
            foreach(ScheduleRow row in schedule)
            {
                namesClasses.Add(row.Class);
            }
            ScheduleToGrid();
        }

        private Schedule GetSchedule(string day, out int time, out int countClasses)
        {
            Schedule schedule = new Schedule();
            string query = $"{ApiMethods.GETSchedule}?day={day.GetEngDay()}";
            using(HttpClient client = new HttpClient())
            {
                try
                {
                    HttpRequestMessage message = new HttpRequestMessage(
                        new HttpMethod("GET"), new Uri(ExtensionMethods.pathConnection, query));
                    HttpResponseMessage response = client.Send(message);
                    schedule = Schedule.JsonToSchedule(response.Content.ReadAsStringAsync().Result, out time, out countClasses);
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception with database!");
                }
            }
            return schedule;
        }


        private void GetTeachers()
        {
            using(HttpClient client = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage(
                    new HttpMethod("GET"), new Uri(ExtensionMethods.pathConnection, ApiMethods.GETTeachersId));
                HttpResponseMessage response = client.Send(message);
                JsonToTeacher(response.Content.ReadAsStringAsync().Result);
            }
        }

        private void JsonToTeacher(string json)
        {
            JArray jsonForSchedule = JArray.Parse(json);
            foreach(JObject item in jsonForSchedule)
            {
                int id = int.Parse(item["id"].ToString());
                string secondName = item["second_name"].ToString();
                string firstName = item["first_name"].ToString();
                string lastName = item["last_name"].ToString();
                string Name = $"{secondName} {firstName} {lastName}";
                Teachers.Add(new Teacher(id, Name));
            }            
        }

        private string FindNameTeacherById(int id)
        {
            return Teachers.Find(p=>p.Id == id).Name;
        }
        private void ScheduleToGrid()
        {
            if (schedule.Rows != null)
            {
                for(int i = 0; i < rows;i++)
                gridForClasses.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                int bound = Math.Min(countClasses, 16);
                for (int i = 0; i < bound; i++)
                {
                    gridForClasses.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });
                }
                FormGrid();
            }
        }
        private void FormGrid()
        {
            for (int i = 0; i < countClasses; i++)
            {
                Grid grid = new Grid();
                for (int j = 0; j <= maxTime; j++)
                { 
                    grid.RowDefinitions.Add(new RowDefinition() 
                    { 
                        Height = new GridLength(1, GridUnitType.Star) 
                    }
                    );
                    
                }
                Grid.SetColumn(grid, i % 16);
                int indexRow = i > 15 ? 1 : 0;                
                Grid.SetRow(grid, indexRow);
                gridForClasses.Children.Add(grid);
            }
            int k = 0;
            foreach (string item in namesClasses)
            {
                if (gridForClasses.Children[k] is Grid grid)
                {
                    Label label = FormLabel(item, 18, true);
                    Grid.SetRow(label, 0);
                    grid.Children.Add(label);
                    k++;
                }
            }

            foreach (ScheduleRow row in schedule)
            {
                Label label = FormLabel($"{row.Lesson}\n{FindNameTeacherById(row.IdTeacher ?? -1) ?? ""}", 15, false);
                Grid.SetRow(label, (int)row.Time);
                int index = GetIndexByName(row.Class);
                if (gridForClasses.Children[index] is Grid grid)
                {
                    grid.Children.Add(label);
                }
            }
        }

        private int GetIndexByName(string name)
        {
            for (int i = 0; i < gridForClasses.Children.Count; i++)
            {
                if (gridForClasses.Children[i] is Grid grid)
                {
                    if (grid.Children[0] is Label label && (label.Content as TextBlock).Text == name)
                    return i;
                }
            }
            return -1;
        }
        private Label FormLabel(string text, double fontSize,bool isClass)
        {
            Label label = new Label() 
            {
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("Arial"),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
            };
            if (isClass)
            {
                label.Name = text[1..^1];
            }
            label.Content = LabelMethods.FormateTextBlock(text, fontSize);
            return label;
        }
        private void EcsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.FormOrPreview = null;
            NavigationService.Navigate(new Page2());
        }
    }
}
