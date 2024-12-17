using Microsoft.Win32;
using MosBirzha_23var.Companies;
using MosBirzha_23var.Objects;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MosBirzha_23var
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CandleTimeSpan CurrentDisplaySpan { get; private set; } = CandleTimeSpan.MINUTE;
        private TestCompany Company { get; } = new();
        private DispatcherTimer _companyUpdateTimer = new();

        public MainWindow()
        {
            InitializeComponent();


            _companyUpdateTimer.Tick += (s, e) => { Company.Tick(DateTimeOffset.UtcNow.ToUnixTimeSeconds()); Update(); };
            _companyUpdateTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);

            _companyUpdateTimer.Start();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentDisplaySpan = (CandleTimeSpan)(Enum.GetValues(typeof(CandleTimeSpan))).GetValue((sender as ComboBox).SelectedIndex);
            Update();
        }

        private void Update()
        {

            Chart.DisplaySpan = CurrentDisplaySpan;
            Chart.Candles = Company.Scalps.ToCandles(CurrentDisplaySpan);
            Chart.Draw();




            PriceDisplay.Text = $"{Company.Price}$";
            if (Chart.Candles.Count > 0)
            {
                DynamicDisplay.Text = $"{Company.PriceDynamic * 100:F2}%";
                DynamicDescription.Text = $"Динамика";
            }

            if (Company.PriceDynamic > 0)
            {
                DynamicDisplay.Foreground = Brushes.Green;
            }
            else if (Company.PriceDynamic < 0)
            {
                DynamicDisplay.Foreground = Brushes.Red;
            }
            else
            {
                DynamicDisplay.Foreground = Brushes.Black;
            }
        }

        private void StoreData_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder data = new StringBuilder();
            foreach (PriceScalp scalp in Company.Scalps)
                data.AppendLine($"{scalp.Timestamp};{scalp.Price}");

            SaveFileDialog d = new()
            {
                Title = "Сохранение данных компании",
                FileName = "dump.txt"
            };

            if (d.ShowDialog() == true)
                File.WriteAllText(d.FileName, data.ToString());
        }

        private void LoadData_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog d = new()
            {
                Title = "Загрузка данных компании",
                FileName = "dump.txt"
            };

            if (d.ShowDialog() == true)
            {
                _companyUpdateTimer.Stop();
                Company.Reset();

                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                string[] lines = File.ReadAllLines(d.FileName).Reverse().ToArray();
                long maxTimestamp = long.Parse(lines[0].Split(';')[0]);
                foreach (string line in lines)
                {
                    var a = line.Split(';');
                    if (a.Length == 2)
                    {
                        try
                        {
                            Company.Scalps.Add(new PriceScalp(now + (long.Parse(a[0]) - maxTimestamp), double.Parse(a[1])));
                        }
                        catch
                        {
                            MessageBox.Show($"Ошибка парсинга файла - некорректное значение в строке {lines.ToList().IndexOf(line)}");
                        }
                    }

                    else
                    {
                        MessageBox.Show($"Ошибка парсинга файла - некорректное количество аргументов в строке {lines.ToList().IndexOf(line)}");
                    }
                }
                // костыль вселенских масштабов
                var speculatedTick = new LastTickResult()
                {
                    Magic = 0,
                    IsBuying = false,
                    IsMoveHappend = true,
                    Timestamp = Company.Scalps.First().Timestamp
                };
                speculatedTick.SetPrice(Company.Scalps.First().Price);
                Company.SpeculateTick(speculatedTick.Timestamp, speculatedTick);

                MemorySafety();
                _companyUpdateTimer.Start();
            }
        }

        private void GenerateData_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Приложение может зависнуть!\nЗапускать на свой страх и риск!", "Экспериментальная функция", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Stopwatch justForLulz = new();

                _companyUpdateTimer.Stop();
                Company.Reset();
                justForLulz.Start();

                long renderTime = 0;

                while(renderTime <= 86400 * 14)
                {
                    for (int i = 0; i < 4; i++)
                        Company.Tick(renderTime);
                    renderTime++;
                }

                justForLulz.Stop();
                MessageBox.Show($"Рендер успешно выполнен!\nВремя выполнения - {justForLulz.Elapsed.Minutes} минут {justForLulz.Elapsed.Seconds} секунд и {justForLulz.Elapsed.Milliseconds} милисекунд");
                StoreData_Click(sender, e);
                Company.Reset();
                _companyUpdateTimer.Start();
            }
        }

        private void MemorySafety()
        {
            // 3 дня максимум
            if (Company.Scalps.Count > 86400 * 7)
            {
                if (MessageBox.Show("Обнаружен большой объём данных!\nОбрезать все данные, возрастом более 7 дней?", "Безопасноть памяти", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Company.Scalps.RemoveRange(86400 * 7 * 4, Company.Scalps.Count - 86400 * 7 * 4);
                    GC.Collect();
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Company.Reset();
        }
    }
}