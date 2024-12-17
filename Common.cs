using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosBirzha_23var
{
    class Common
    {
        public static Random Rng = new();

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public static List<Candle> ConvertToCandles(List<PriceScalp> priceScalps, CandleTimeSpan timeSpan)
        {
            if (priceScalps == null || priceScalps.Count == 0)
            {
                return [];
            }

            // Группируем данные по временным промежуткам
            var candles = new List<Candle>();

            // Сортируем данные для корректной группировки
            var sortedPriceScalps = priceScalps.OrderBy(ps => ps.Timestamp).ToList();

            long currentGroupEndTime = sortedPriceScalps.First().Timestamp + (long)timeSpan;

            List<PriceScalp> currentGroup = [];

            foreach (var scalp in sortedPriceScalps)
            {
                if (scalp.Timestamp < currentGroupEndTime)
                {
                    currentGroup.Add(scalp);
                }
                else
                {
                    if (currentGroup.Count > 0)
                    {
                        candles.Add(CreateCandle(currentGroup));
                    }
                    // Обновляем временные рамки для следующей группы
                    currentGroupEndTime = scalp.Timestamp + (long)timeSpan;
                    currentGroup.Clear();
                    currentGroup.Add(scalp);
                }
            }

            // Добавляем последнюю группу, если она не пустая
            if (currentGroup.Count > 0)
            {
                candles.Add(CreateCandle(currentGroup));
            }

            return candles;
        }

        private static Candle CreateCandle(List<PriceScalp> priceScalps)
        {
            double open = priceScalps.First().Price;  // Цена открытия
            double close = priceScalps.Last().Price;  // Цена закрытия
            double high = priceScalps.Max(ps => ps.Price); // Максимальная цена
            double low = priceScalps.Min(ps => ps.Price);  // Минимальная цена

            return new Candle(open, close, high, low);
        }
    }
}
