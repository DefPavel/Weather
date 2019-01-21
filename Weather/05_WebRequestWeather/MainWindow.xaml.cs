using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace _05_WebRequestWeather
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Label> dayLabels = new List<Label>();
        List<Label> tempLabels = new List<Label>();

        private string baseUri = "https://sinoptik.ua/";
        public string city = "%D0%BF%D0%BE%D0%B3%D0%BE%D0%B4%D0%B0-%D0%BB%D1%83%D0%B3%D0%B0%D0%BD%D1%81%D0%BA";
        public int updateTimer = 60;

        public MainWindow()
        {
            InitializeComponent();

            SetupLabels();
            
            LoadOptions();

            new Thread(new ThreadStart(UpdateDataInvoker)) { IsBackground = true}.Start();
        }

        void UpdateDataInvoker()
        {
            while (true)
            {
                Dispatcher.Invoke(UpdateData);
                Thread.Sleep(updateTimer * 1000);
            }
        }

        void LoadOptions()
        {
            try
            {
                string[] options = File.ReadAllLines("options.dat");

                updateTimer = Convert.ToInt32(options[0]);

                city = options[1];
            }
            catch { }
        }

        void SetupLabels()
        {
            for (int i = 0; i < 12; i++)
            {
                Label tempLabel = new Label();
                tempLabel.FontSize = 20;
                tempLabel.HorizontalAlignment = HorizontalAlignment.Center;
                tempLabel.VerticalAlignment = VerticalAlignment.Bottom;
                tempLabel.Foreground = new SolidColorBrush(Color.FromRgb(255,255,255));
                tempLabel.Content = "123";
                tempLabels.Add(tempLabel);

                mainGrid.Children.Add(tempLabel);

                Grid.SetColumn(tempLabel, i);
                Grid.SetRow(tempLabel, 2);
            }

            for (int i = 0; i < 6; i++)
            {
                Label dayLabel = new Label();
                dayLabel.FontSize = 30;
                dayLabel.Foreground = new SolidColorBrush(Color.FromRgb(255,255,255));
                dayLabel.HorizontalAlignment = HorizontalAlignment.Center;
                dayLabel.VerticalAlignment = VerticalAlignment.Center;
                dayLabel.Content = "Воскресенье";

                dayLabels.Add(dayLabel);

                mainGrid.Children.Add(dayLabel);

                Grid.SetColumn(dayLabel, i*2);
                Grid.SetColumnSpan(dayLabel, 2);
                Grid.SetRow(dayLabel, 1);
            }
        }

        public void UpdateData()
        {
            WebRequest request =
                WebRequest.Create(baseUri + this.city);

            WebResponse response = request.GetResponse();

            string responseString = "";
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                // Выводим ответ сервера
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    responseString += line;
                }
            }

            response.Dispose();

            string city = responseString.Substring(responseString.IndexOf("<h1>") + "<h1>".Length,
                responseString.IndexOf("</h1>") - (responseString.IndexOf("<h1>") + "<h1>".Length));

            city = city.Replace("<strong>", " ").Replace("</strong>", "");

            lbl_City.Content = city;

            responseString = responseString.Substring(responseString.IndexOf("<div class=\"tabs\""),
                responseString.IndexOf("</div> <div class=\"tabsTop\">") -
                responseString.IndexOf("<div class=\"tabs\""));

            for (int i = 1; i < 7; i++)
            {
                int startIndex = responseString.IndexOf("<div class=\"main \" id=\"bd" + i);

                if (startIndex == -1)
                {
                    startIndex = responseString.IndexOf("<div class=\"main loaded\" id=\"bd" + i);
                }

                int endIndex = responseString.IndexOf("</div> <div class=\"mid" + i);

                if (endIndex == -1)
                {
                    endIndex = responseString.IndexOf("</div> <div class=\"last");
                }

                string currTab = responseString.Substring(startIndex, endIndex - startIndex);

                try
                {

                    startIndex = currTab.IndexOf(">", currTab.IndexOf("<p class=\"day-link")) + ">".Length;
                    endIndex = currTab.IndexOf("</p>", startIndex);
                }
                catch
                {
                    startIndex = currTab.IndexOf(">", currTab.IndexOf("<a class=\"day-link")) + ">".Length;
                    endIndex = currTab.IndexOf("</a>", startIndex);
                }

                string day = currTab.Substring(startIndex, endIndex - startIndex);

                dayLabels[i - 1].Content = day;

                startIndex = currTab.IndexOf("<span>", currTab.IndexOf("мин.")) + "<span>".Length;
                endIndex = currTab.IndexOf("</span>", startIndex);

                string minTemp = currTab.Substring(startIndex, endIndex - startIndex).Replace("&deg;", "°");

                tempLabels[(i - 1) * 2].Content = "↓ " + minTemp;

                startIndex = currTab.IndexOf("<span>", currTab.IndexOf("макс.")) + "<span>".Length;
                endIndex = currTab.IndexOf("</span>", startIndex);

                string maxTemp = currTab.Substring(startIndex, endIndex - startIndex).Replace("&deg;", "°");

                tempLabels[(i - 1) * 2 + 1].Content = "↑ " + maxTemp;
            }
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void MI_Close_OnClick(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void MI_Options_OnClick(object sender, RoutedEventArgs e)
        {
            Options opts = new Options(this);
            opts.Show();
        }
    }
}
