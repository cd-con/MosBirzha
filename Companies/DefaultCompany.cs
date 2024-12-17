using System.Diagnostics;

namespace MosBirzha_23var.Companies
{
    public struct LastTickResult
    {
        public long Timestamp { get; set; }
        public bool IsMoveHappend { get; set; }
        public float Magic { get; set; }
        public bool IsBuying { get; set; }
        public PriceScalp Scalp { get; private set; }

        public void SetPrice(double price)
        {
            Scalp = new PriceScalp(Timestamp, price);
        }

    }
    public abstract class DefaultCompany
    {
        protected LastTickResult tResult;
        public List<PriceScalp> graph = new();
        public double Price
        {
            get
            {
                return Math.Round(graph.Last().Price, 2);
            }
        }
        public LastTickResult Tick(long currentTime)
        {
            LastTickResult result = new()
            {
                Magic = Common.Rng.NextSingle(),
                Timestamp = currentTime
            };
            result.IsMoveHappend = result.Magic > 0.2f;
            // Check if someone decided to buy/sell
            if (result.IsMoveHappend)
            {
                // Deciding - buy or sell
                result.IsBuying = (result.Magic + 0.2f) > 0.7f + (tResult.IsBuying ? 0.3f : 0);
                double PriceDeviation = Common.Rng.NextSingle();
                result.SetPrice(tResult.Scalp.Price - PriceDeviation * (result.IsBuying ? -1 : 1));
            }
            else
            {
                result.SetPrice(tResult.Scalp.Price);
            }
            tResult = result;
            graph.Add(result.Scalp);
            return result;
        }
    }

    public class Test : DefaultCompany
    {
        public Test()
        {
            LastTickResult result = new()
            {
                Magic = -1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                IsMoveHappend = false,
                IsBuying = false,
            };
            result.SetPrice(100);
            tResult = result;
        }
    }
}
