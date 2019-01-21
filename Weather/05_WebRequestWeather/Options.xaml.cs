using System;
using System.Collections.Generic;
using System.IO;
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

namespace _05_WebRequestWeather
{
    /// <summary>
    /// Логика взаимодействия для Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private MainWindow mainWindow;

        public Options(MainWindow window)
        {
            InitializeComponent();

            mainWindow = window;
            tb_Delay.Text = mainWindow.updateTimer.ToString();
            tb_City.Text = mainWindow.city;
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllLines("options.dat", new string[]{ tb_Delay.Text, tb_City.Text});

            mainWindow.city = tb_City.Text;
            mainWindow.updateTimer = Convert.ToInt32(tb_Delay.Text);
            mainWindow.UpdateData();

            this.Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Options_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
