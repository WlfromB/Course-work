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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Курсач
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {

        public Page1()
        {
            
            InitializeComponent();
            btn_Form.Effect = new DropShadowEffect
            {
                ShadowDepth = 0,
                Color = Colors.Black,
                Opacity = 0.5,
                BlurRadius = 5
            };
            btn_View.Effect = new DropShadowEffect
            {
                ShadowDepth = 0,
                Color = Colors.Black,
                Opacity = 0.5,
                BlurRadius = 5
            };
        }
        private void btn_Form_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.FormSelected();
            NavigationService.Navigate(new Page2());            
        }
        
        private void btn_Preview_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PreviewSelected();
            NavigationService.Navigate(new Page2());
        }
    }
}
