using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MosBirzha_23var.Objects;

namespace MosBirzha_23var
{
    public partial class Chart : UserControl
    {
        public List<Candle> Candles = [];
        public CandleTimeSpan DisplaySpan = CandleTimeSpan.MINUTE;
        private double _candleWidth = 12;
        private double _spacing = 18;
        private int _mDisplay = 120;

        public Chart() => InitializeComponent();

        private void DrawCandles(double offset)
        {
            if (Candles.Count == 0) return;

            double minPrice = Candles.Min(c => c.Low);
            double maxPrice = Candles.Max(c => c.High);
            double avgPrice = Candles.Average(c => (c.High + c.Low) / 2);
            double priceRange = maxPrice - minPrice;
            double heightScale = CandleCanvas.ActualHeight / priceRange;

            if (!double.IsFinite(heightScale)) return;

            CandleCanvas.Children.Clear();

            double canvasWidth = CandleCanvas.ActualWidth;
            double totalWidth = Candles.Count * (_candleWidth + _spacing);

            DrawGrid(minPrice, maxPrice);



            for (int i = 0; i < Candles.Count; i++)
            {
                TimeSpan howAgo = TimeSpan.FromSeconds((Candles.Count - i - 1) * (int)DisplaySpan);
                Candle candle = Candles[i];
                double x = canvasWidth - totalWidth + i * (_candleWidth + _spacing) + offset;

                if (x < 0 || x > canvasWidth - _spacing) continue;

                double openY = CandleCanvas.ActualHeight - (candle.Open - minPrice) * heightScale;
                double closeY = CandleCanvas.ActualHeight - (candle.Close - minPrice) * heightScale;
                double highY = CandleCanvas.ActualHeight - (candle.High - minPrice) * heightScale;
                double lowY = CandleCanvas.ActualHeight - (candle.Low - minPrice) * heightScale;

                // Линия High-Low
                var line = new Line
                {
                    X1 = x + _candleWidth / 2,
                    Y1 = highY,
                    X2 = x + _candleWidth / 2,
                    Y2 = lowY,
                    Stroke = Brushes.Black
                };
                CandleCanvas.Children.Add(line);

                // Тело свечи
                var rectangle = new Rectangle
                {
                    Width = _candleWidth,
                    Height = Math.Abs(openY - closeY),
                    Fill = candle.Close >= candle.Open ? Brushes.Green : Brushes.Red,
                    ToolTip = $"Open: {candle.Open:F4}\nClose: {candle.Close:F4}\nHigh: {candle.High:F4}\nLow: {candle.Low:F4}"
                };
                Canvas.SetLeft(rectangle, x);
                Canvas.SetTop(rectangle, Math.Min(openY, closeY));
                CandleCanvas.Children.Add(rectangle);
                var timeLabel = new TextBlock
                {
                    Text = $"T-{(DisplaySpan == CandleTimeSpan.DAY ? howAgo.TotalDays : (int)DisplaySpan > 3599 ? howAgo.TotalHours : howAgo.TotalMinutes)  }",
                    Foreground = Brushes.Gray,
                    FontSize = 10
                };
                Canvas.SetLeft(timeLabel, x);
                Canvas.SetTop(timeLabel, CandleCanvas.ActualHeight + 2);
                CandleCanvas.Children.Add(timeLabel);
            }
        }

        // Сетка
        private void DrawGrid(double minPrice, double maxPrice)
        {
            int steps = (int)CandleCanvas.ActualHeight / 32; // Количество шагов на сетке
            double priceStep = (maxPrice - minPrice) / steps;

            for (int i = 0; i <= steps; i++)
            {
                double y = CandleCanvas.ActualHeight - (i * CandleCanvas.ActualHeight / steps);

                // Горизонтальная линия
                var gridLine = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = CandleCanvas.ActualWidth,
                    Y2 = y,
                    Stroke = Brushes.DimGray,
                    StrokeDashArray = new DoubleCollection { 2, 2 }
                };
                CandleCanvas.Children.Add(gridLine);

                // Шаг цены справа
                double priceLabelValue = minPrice + i * priceStep;
                var priceLabel = new TextBlock
                {
                    Text = $"{priceLabelValue:F2}",
                    Foreground = Brushes.Black,
                    FontSize = 10
                };
                Canvas.SetRight(priceLabel, -26);
                Canvas.SetTop(priceLabel, y - 8);
                CandleCanvas.Children.Add(priceLabel);
            }
        }

        // Обновляем прокрутку времени
        private void TimeBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeBar.Maximum = Math.Max(0, Candles.Count * (_candleWidth + _spacing) - CandleCanvas.ActualWidth); // Ограничиваем прокрутку
            DrawCandles(TimeBar.Value);
        }
        
        // Метод для инициализации
        public void Draw()
        {
            TimeBar.Maximum = Math.Max(0, Candles.Count * (_candleWidth + _spacing) - CandleCanvas.ActualWidth);
            DrawCandles(TimeBar.Value);
        }
    }
}