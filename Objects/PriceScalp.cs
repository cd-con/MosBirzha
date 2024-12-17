namespace MosBirzha_23var.Objects
{
    public class PriceScalp(long timestamp, double price)
    {
        public long Timestamp { get; private set; } = timestamp;
        public double Price { get; private set; } = price;
    }
}
