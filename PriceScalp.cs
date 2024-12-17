namespace MosBirzha_23var
{
    public class PriceScalp
    {
        public long Timestamp {  get; private set; }
        public double Price { get; private set; }

        public PriceScalp(long timestamp, double price)
        {
            Timestamp = timestamp;
            Price = price;
        }
    }
}
