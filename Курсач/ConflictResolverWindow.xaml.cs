using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Курсач
{
    /// <summary>
    /// Логика взаимодействия для ConflictResolverWindow.xaml
    /// </summary>
    public partial class ConflictResolverWindow : Window
    {
        public int Selected { get; set; }
        public LessonsForTeacher? Teacher { get; set; }
        public ConflictResolverWindow(List<LessonsForTeacher> buffer, LessonsForTeacher conflictTeacher, int indexChangedLesson)
        {
            InitializeComponent();
            Selected = -1;// по умолчанию перекидываем урок на другой день
            buffer.Add(new LessonsForTeacher(-1, -1, "Перенести на другой день", null));
            selectorScript.ItemsSource = buffer;
            
            conflictInfoLabel.Content = new StackPanel();
            (conflictInfoLabel.Content as StackPanel).Children.Add(
                new TextBlock()
                {
                    Text = conflictTeacher.Name,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(5), 
                    FontFamily = new FontFamily("Times New Roman"),
                    FontSize = 20
                });
            (conflictInfoLabel.Content as StackPanel).Children.Add(
                new TextBlock() 
                {
                    Text = conflictTeacher.LessonAndClass[indexChangedLesson].ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(5),
                    FontFamily = new FontFamily("Times New Roman"),
                    FontSize = 16
                });
        }

        private void select_Click(object sender, RoutedEventArgs e)
        {            
            switch (selectorScript.SelectedValue) 
            {
                case "Перенести на другой день":
                    Selected = -1;
                    break;
                default:
                    { 
                        Selected = selectorScript.SelectedIndex;
                        Teacher = selectorScript.SelectedItem as LessonsForTeacher;
                    }
                    break;
            }
            // обращение к endpoint 
        }
    }
}
