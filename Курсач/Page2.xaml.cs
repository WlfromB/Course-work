using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Курсач
{
    

    public class Page2Commands
    { 
        static Page2Commands()
        {
            SelectDay = new RoutedCommand("Select",typeof(Page2));
            DeleteDay = new RoutedCommand("Delete",typeof(Page2));
        }

        public static RoutedCommand SelectDay { get; set; }
              
        public static RoutedCommand DeleteDay { get; set;}
    }


    /// <summary>
    /// Логика взаимодействия для Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        private delegate void AskApiHandler(UniformGrid un);

        private event AskApiHandler AskApi;
        // обработать нужные кнопки
        
        
        [Flags]
        public enum FormedDays
        {
            None = 0b_000000,
            Monday = 0b_000001,
            Tuesday = 0b_000010,
            Wednesday = 0b_000100,
            Thursday = 0b_001000,
            Friday = 0b_010000,
            Saturday = 0b_100000,            
        }

        
        private FormedDays days = (FormedDays)1;

        public FormedDays Days { get => days; private set =>days = value; }
        //если была нажата кнопка сформировать, то нужно задизейблить кнопки по тем дням которые составлены, вместо них 
        //поставить кнопки(или изменить поведение) стереть расписание на этот день, потом разблокировать
        //если нажата кнопка просмотреть то задизейблить несоставленные
        public Page2()
        {
            InitializeComponent();
            CommandBinding commandbinding = new CommandBinding() ;
            commandbinding.Command = Page2Commands.SelectDay;            
            commandbinding.Executed += Select_Executed;
            FormButtons(uniformGridDays, commandbinding);
            AskApi += MainWindow.FormOrPreview == false? IgnoreButtons: IgnoreUnformed;
            Days = GetInfoSchedule();//реальное заполнение из бд
            AskApi?.Invoke(uniformGridDays);
        }
        private static Binding FontSizeBinding(double parameter)
        {
            Binding binding = new Binding("ActualWidth");
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Page), 1);
            WidthToFontSizeConverter converter = new WidthToFontSizeConverter();
            binding.Converter = converter;
            binding.ConverterParameter = parameter;
            return binding;
        }
        private static Button FormBtn(string name, CommandBinding command)
        {
            Button button1 = new Button();
            TextBlock tb = new TextBlock()
            {
                Text = name,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,

            };
            BindingOperations.SetBinding(tb, TextBlock.FontSizeProperty, FontSizeBinding(25));
            button1.Content = tb;
            button1.CommandBindings.Add(command);
            button1.Name = name;
            button1.Command = command.Command;
            button1.Effect = new DropShadowEffect
            {
                ShadowDepth = 0,
                Color = Colors.Black,
                Opacity = 0.5,
                BlurRadius = 5
            };

            return button1;
        }
       
        private static void FormButtons(UniformGrid nameUnifGrid, CommandBinding command)
        {
            nameUnifGrid.Children.Add(FormBtn("Понедельник",command));
            nameUnifGrid.Children.Add(FormBtn("Вторник",command));
            nameUnifGrid.Children.Add(FormBtn("Среда",command));
            nameUnifGrid.Children.Add(FormBtn("Четверг",command));
            nameUnifGrid.Children.Add(FormBtn("Пятница",command));
            nameUnifGrid.Children.Add(FormBtn("Суббота",command));
        }
        private void Select_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Button tmp = sender as Button;
            if (MainWindow.FormOrPreview == false)
            {
                NavigationService.Navigate(new Page3(tmp.Name));
            }
            else
            {
                NavigationService.Navigate(new PagePreview(tmp.Name));
            }
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if(sender is Button btn) 
            {
                //метод отправляющий запрос на удаление этого дня по имени
                RequestDeleteDay(btn.Name.GetEngDay());
                btn.Command = Page2Commands.SelectDay;
                TextBlock tb = new TextBlock()
                {
                    Text = btn.Name,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                btn.Content = tb;
                BindingOperations.SetBinding(tb, TextBlock.FontSizeProperty, FontSizeBinding(25));
            }
        }

        private void RequestDeleteDay(string day)
        {
            string addQuery = $"{ApiMethods.DELETE}?day={day}";
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                using(HttpClient client = new HttpClient())
                {
                    HttpRequestMessage message = new HttpRequestMessage(new HttpMethod("DELETE"), 
                        new Uri(ExtensionMethods.pathConnection, addQuery));
                    HttpResponseMessage response = client.Send(message);                       
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch
            {
                throw new Exception("Exception with database!");
            }
        }

        private void EcsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.FormOrPreview = null;
            NavigationService.Navigate(new Page1());
        }
        private void IgnoreButtons(UniformGrid uniformGrid)
        {
            CommandBinding binding = new CommandBinding();
            binding.Command = Page2Commands.DeleteDay;
            binding.Executed += Delete_Executed;
            for (int i = 0; i < 6; i++)
            {
                if ((Days & (FormedDays)(1 << i)) != 0)
                {
                    ChangeButtonFormed(i, binding);
                }
            }
        }
        private void ChangeButtonFormed(int i, CommandBinding command)
        {
            if (uniformGridDays.Children[i] is Button btn)
            {
                btn.CommandBindings.Add(command);
                btn.Command = command.Command;
                TextBlock tb = new TextBlock()
                {
                    Text = $"{btn.Name}\n составлен",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.IndianRed)
                };
                BindingOperations.SetBinding(tb, TextBlock.FontSizeProperty, FontSizeBinding(23));
                btn.Content = tb;
                btn.BorderBrush = new SolidColorBrush(Colors.LightGray);
            }
        }
        private void IgnoreUnformed(UniformGrid uniformGrid)
        {

            for (int i = 0; i < 6; i++)
            {
                if ((Days & (FormedDays)(1 << i)) == 0)
                {
                    ChangeButtonUnformed(i);
                }
            }
        }

        private void ChangeButtonUnformed(int i)
        {
            if (uniformGridDays.Children[i] is Button btn)
            {
                btn.Command = null;
                TextBlock tb = new TextBlock()
                {
                    Text = $"{btn.Name}\nне составлен",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.IndianRed)
                };
                BindingOperations.SetBinding(tb, TextBlock.FontSizeProperty, FontSizeBinding(23));
                btn.Content = tb;
                btn.BorderBrush = new SolidColorBrush(Colors.LightGray);
                btn.IsEnabled = false;
            }
        }
        private FormedDays GetInfoSchedule()
        {            
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage message = new HttpRequestMessage(new HttpMethod("GET"),
                        new Uri(ExtensionMethods.pathConnection,ApiMethods.GETDays));
                    HttpResponseMessage response = client.Send(message);
                    string data = response.Content.ReadAsStringAsync().Result;
                    JObject jsonWithFormedDays = JObject.Parse(data);
                    int.TryParse(jsonWithFormedDays["days"].ToString(), out int days);
                    Mouse.OverrideCursor = Cursors.Arrow;
                    return (FormedDays)days;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exteption with database!");
            }
        }

    }
}
