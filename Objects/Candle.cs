namespace MosBirzha_23var.Objects
{
    public enum CandleTimeSpan
    {
        MINUTE = 60,
        FIVE_MINUTE = 300,
        FIFTEEN_MINUTES = 900,
        THIRTY_MINUTES = 1800,
        HOUR = 3600,
        SIX_HOURS = 21600,
        DAY = 86400
    }
    public class Candle(double open, double close, double high, double low)
    {
        public double Open { get; set; } = open;
        public double Close { get; set; } = close;
        public double High { get; set; } = high;
        public double Low { get; set; } = low;
    }
}
