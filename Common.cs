using MosBirzha_23var.Objects;

namespace MosBirzha_23var
{
    public static class Common
    {
        public static Random Rng { get; } = new();

        /*[Obsolete("Более не используется")]
        private static Dictionary<CandleTimeSpan, string> _locale { get; } =
            new(){ { CandleTimeSpan.MINUTE, "минуту"},
                   { CandleTimeSpan.FIVE_MINUTE, "5 минут" }, 
                   { CandleTimeSpan.FIFTEEN_MINUTES, "15 минут" }, 
                   { CandleTimeSpan.THIRTY_MINUTES, "полчаса" }, 
                   { CandleTimeSpan.HOUR, "час" }, 
                   { CandleTimeSpan.SIX_HOURS, "6 часов" }, 
                   { CandleTimeSpan.DAY, "день" } };

        [Obsolete("Более не используется")]
        public static string SpanToString(this CandleTimeSpan span) => _locale[span];
        */
        public static List<Candle> ToCandles(this List<PriceScalp> priceScalps, CandleTimeSpan timeSpan)
        {
            if (priceScalps == null || priceScalps.Count == 0)
                return [];

            IEnumerable<PriceScalp> sortedPriceScalps = priceScalps.OrderBy(ps => ps.Timestamp);
            List<Candle> result = [];

            long currentCandleStartTime = sortedPriceScalps.First().Timestamp;
            long candleDuration = (long)timeSpan;
            List<PriceScalp> currentGroup = [];


            foreach (PriceScalp scalp in sortedPriceScalps)
            {
                if (scalp.Timestamp < currentCandleStartTime + candleDuration)
                {
                    currentGroup.Add(scalp);
                }
                else
                {
                    if (currentGroup.Count > 0)
                        result.Add(currentGroup.ToCandle());

                    currentCandleStartTime += candleDuration;
                    currentGroup = [scalp];

                    while (scalp.Timestamp >= currentCandleStartTime + candleDuration)
                    {
                        result.Add(new Candle(0, 0, 0, 0));
                        currentCandleStartTime += candleDuration;
                    }
                }
            }

            if (currentGroup.Count > 0)
                result.Add(currentGroup.ToCandle());

            return result;
        }

        public static Candle ToCandle(this List<PriceScalp> priceScalps)
        {
            double open = priceScalps.First().Price;  // Цена открытия
            double close = priceScalps.Last().Price;  // Цена закрытия
            double high = priceScalps.Max(ps => ps.Price); // Максимальная цена
            double low = priceScalps.Min(ps => ps.Price);  // Минимальная цена

            return new Candle(open, close, high, low);
        }
    }
}
