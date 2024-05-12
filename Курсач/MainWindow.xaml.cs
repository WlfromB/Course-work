using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Курсач
{

    



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public static void FormSelected()
        {
            FormOrPreview = false;
        }

        public static void PreviewSelected()
        {
            FormOrPreview = true;
        }

        public static bool? FormOrPreview { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            frame.Content = new Page1();
            FormOrPreview = null;
        }

        private void Prev_Page_Click(object sender, RoutedEventArgs e)
        { 

        }
        
        private void Home_page_Event(object sender, RoutedEventArgs e)
        { 

        }
    }
}
