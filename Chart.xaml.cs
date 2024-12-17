using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MosBirzha_23var
{
    /// <summary>
    /// Логика взаимодействия для Chart.xaml
    /// </summary>
    public partial class Chart : UserControl
    {
        public List<Candle> _candles = new();
        private double _zoomFactor = 1.0;
            double candleWidth = 10;
            double spacing = 5;

        bool disableFollow = false;
        public Chart()
        {
            InitializeComponent();
        }

        private void DrawCandles(double offset)
        {
            if (_candles.Count == 0)
                return;

            // Определяем минимум и максимумы для масштабирования
            double minPrice = _candles.Min(c => c.Low);
            double maxPrice = _candles.Max(c => c.High);
            double avgPrice = (minPrice + maxPrice) / 2;
            double scaledMinPrice = avgPrice - ((avgPrice - minPrice) * _zoomFactor);
            double scaledMaxPrice = avgPrice + ((maxPrice - avgPrice) * _zoomFactor);

            // Масштаб высоты
            double scaleHeight = (CandleCanvas.ActualHeight / (scaledMaxPrice - scaledMinPrice));

            CandleCanvas.Children.Clear(); // Очищаем холст

            if ((maxPrice - minPrice) == 0)
                return;

            for (int i = 0; i < _candles.Count; i++)
            {
                Candle candle = _candles[i];
                double x = i * (candleWidth + spacing) - offset; // Применяем прокрутку

                // Масштабируем цены для отрисовки на холсте
                double openY = CandleCanvas.ActualHeight - (candle.Open - scaledMinPrice) * scaleHeight;
                double closeY = CandleCanvas.ActualHeight - (candle.Close - scaledMinPrice) * scaleHeight;
                double highY = CandleCanvas.ActualHeight - (candle.High - scaledMinPrice) * scaleHeight;
                double lowY = CandleCanvas.ActualHeight - (candle.Low - scaledMinPrice) * scaleHeight;

                // Рисуем линии (тени) от high до low
                Line line = new Line
                {
                    X1 = x + candleWidth / 2,
                    Y1 = highY,
                    X2 = x + candleWidth / 2,
                    Y2 = lowY,
                    Stroke = Brushes.Black
                };
                CandleCanvas.Children.Add(line);

                // Рисуем тело свечи
                Rectangle rectangle = new Rectangle
                {
                    Width = candleWidth,
                    Height = Math.Abs(openY - closeY),
                    Fill = candle.Close >= candle.Open ? Brushes.Green : Brushes.Red
                };
                Canvas.SetLeft(rectangle, x);
                Canvas.SetTop(rectangle, Math.Min(openY, closeY));
                CandleCanvas.Children.Add(rectangle);
            }
        }

        private void InitializeScrollBar()
        {
            TimeBar.Maximum = (_candles.Count * (candleWidth + spacing)) - CandleCanvas.ActualWidth;
            TimeBar.Minimum = 0;
            if (!disableFollow)
                TimeBar.Value = TimeBar.Maximum;
        }

        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DrawCandles(TimeBar.Value);
        }

        public void Draw()
        {
            InitializeScrollBar();
            DrawCandles(TimeBar.Value);
        }

        private void Zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _zoomFactor = Zoom.Value; // Обновляем масштаб
            DrawCandles(TimeBar.Value); // Перерисовываем свечи с новым масштабом
        }

        private void TimeBar_GotFocus(object sender, RoutedEventArgs e)
        {
            disableFollow = true;
        }

        private void TimeBar_LostFocus(object sender, RoutedEventArgs e)
        {
            disableFollow = false;
        }
    }
}
