namespace MosBirzha_23var
{
    public enum CandleTimeSpan
    {
        DEBUG = 1,
        MINUTE = 60,
        FIVE_MINUTE = 300,
        FIFTEEN_MINUTES = 900,
        THIRTY_MINUTES = 1800,
        HOUR = 3600,
        SIX_HOURS = 21600,
        DAY = 86400
    }
    public class Candle
    {
            public double Open { get; set; }
            public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }

            public Candle(double open, double close, double high, double low)
            {
                Open = open;
                Close = close;
                High = high;
                Low = low;
            }
    }
}
