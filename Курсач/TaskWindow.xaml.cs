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
    /// Логика взаимодействия для TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        public string Lesson { get; set; }

        private LessonsForClass lessonsForClass;

        public LessonsForClass LessonsForClass
        {
            get => lessonsForClass;
            private set => lessonsForClass = value;
        }
        public TaskWindow(LessonsForClass lessonsForClass)
        {
            InitializeComponent();
            LessonsForClass = lessonsForClass;
            gridTaskWindow.Children.Add(FormComboBox());
            
        }

        public ComboBox FormComboBox()
        {
            ComboBox box = new ComboBox()
            {
                Margin = new Thickness(45,45,45,45)
            };
            box.ItemsSource = this.LessonsForClass.Lessons;
           // box.SelectionChanged += null;
            return box;
        }


        /*private StringBuilder BuildStringPlaces()
        {
            StringBuilder sb = new StringBuilder();
            this.Owner 
        }*/

        private void CmbOXSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            
        }
        
        private void btn_Ok(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ComboBox comboBox = (ComboBox)gridTaskWindow.Children[1];
            Lesson = (string)comboBox.SelectedValue;
            LessonsForClass.Lessons.RemoveAt(comboBox.SelectedIndex);
            Close();
        }
    }
}
