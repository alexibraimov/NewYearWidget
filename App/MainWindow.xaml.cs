using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace NewYearWidget
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        private readonly string filePath;
        public MainWindow()
        {
            InitializeComponent();
            filePath = System.IO.Path.Combine(Environment.CurrentDirectory, "positions.txt");
            GetPositions();
            Days.Content = "0";
            Hours.Content = "0";
            Minutes.Content = "0";
            Seconds.Content = "0";
            DaysLabel.Content = "ДНЕЙ";
            HoursLabel.Content = "ЧАСОВ";
            MinutesLabel.Content = "МИНУТ";
            SecondsLabel.Content = "СЕКУНД";
            timer.Tick += OnTick;
            timer.Start();
        }


        private void OnTick(object sender, EventArgs e)
        {
            try
            {
                UpdateView();
            }
            catch
            {
            }
        }

        protected void UpdateView()
        {
            var date1 = DateTime.Now;
            var newYerDate = new DateTime(date1.Year + 1, 1, 1);
            var toNewYear = newYerDate - date1;

            Days.Content = toNewYear.Days;
            Hours.Content = toNewYear.Hours;
            Minutes.Content = toNewYear.Minutes;
            Seconds.Content = toNewYear.Seconds;
            DaysLabel.Content = RussianWordDeclension.GetWordGetWithDeclension(toNewYear.Days, RussianWordDeclension.WordType.DAY);
            HoursLabel.Content = RussianWordDeclension.GetWordGetWithDeclension(toNewYear.Hours, RussianWordDeclension.WordType.HOUR);
            MinutesLabel.Content = RussianWordDeclension.GetWordGetWithDeclension(toNewYear.Minutes, RussianWordDeclension.WordType.MINUTE);
            SecondsLabel.Content = RussianWordDeclension.GetWordGetWithDeclension(toNewYear.Seconds, RussianWordDeclension.WordType.SECOND);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            timer.Tick -= OnTick;
            base.OnClosing(e);
            Process.GetCurrentProcess().Kill();
        }

        private Point old;
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            old = e.GetPosition(null);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var cur = e.GetPosition(null);

                this.Left += cur.X - old.X;
                this.Top += cur.Y - old.Y;
                Save();
            }
        }

        private void Save()
        {
            try
            {
                using (var sw = new StreamWriter(filePath, append: false))
                {
                    sw.Write($"{this.Left};{this.Top}");
                }
            }
            catch
            {

            }
        }

        private void GetPositions()
        {
            try
            {
                using (var sr = new StreamReader(filePath))
                {
                    var text = sr.ReadToEnd();

                    if (!string.IsNullOrEmpty(text))
                    {
                        var positions = text?.Split(';')?.Select(x => double.Parse(x))?.ToArray();
                        if (positions!=null && positions.Length == 2)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                this.Left = positions[0];
                                this.Top = positions[1];
                            });
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            switch (MessageBox.Show("Закрыть виджет?", "Question", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                case MessageBoxResult.OK:
                    {
                        this.Close();
                    }
                    break;
                case MessageBoxResult.Cancel:
                    { 
                    
                    }
                    break;
            }
        }

        public static class RussianWordDeclension
        {
            public enum WordType
            {
                DAY,
                HOUR,
                MINUTE,
                SECOND
            }

            private static Dictionary<WordType, List<string>> _wordsMap = new Dictionary<WordType, List<string>>()
            {
                [WordType.DAY] = new List<string>() 
                {
                    "ДЕНЬ","ДНЯ","ДНЕЙ"
                },
                [WordType.HOUR] = new List<string>()
                {
                    "ЧАС","ЧАСА","ЧАСОВ"
                },
                [WordType.MINUTE] = new List<string>()
                {
                    "МИНУТА","МИНУТЫ","МИНУТ"
                },
                [WordType.SECOND] = new List<string>()
                {
                    "СЕКУНДА","СЕКУНДЫ","СЕКУНД"
                },
            };

            public static string GetWordGetWithDeclension(int count, WordType wordType)
            {
                if (_wordsMap.ContainsKey(wordType))
                {
                    var lastDigit = count / 1 % 10;
                    var prelastDigit = count / 10 % 10;

                    if (lastDigit == 1 && prelastDigit != 1)
                    {
                        return _wordsMap[wordType][0];
                    }
                    else if(lastDigit > 1 && lastDigit < 5 && prelastDigit != 1)
                    {
                        return _wordsMap[wordType][1];
                    }
                    else
                    {
                        return _wordsMap[wordType][2];
                    }
                }

                return null;
            }

        }

    }
}
