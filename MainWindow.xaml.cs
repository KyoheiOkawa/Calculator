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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var str = "button";

            for(int i = 0; i < 9; i++)
            {
                var newStr = str + i.ToString();

                if(button.Name == newStr)
                {
                    if(!(i == 0 && calcText.Text == ""))
                        calcText.Text += i.ToString();
                }
            }

            //calcText.Text = string.Format("{0:#,0}", calcText.Text);
        }
    }
}
