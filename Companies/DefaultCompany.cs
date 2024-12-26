using System.Diagnostics;
using MosBirzha_23var.Objects;

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
    public class DefaultCompany
    {
        public List<PriceScalp> Scalps = [];
        protected LastTickResult _tResult;

        public DefaultCompany() => Reset();

        public double Price
        {
            get
            {
                if (Scalps.Count > 0)
                    return Math.Round(Scalps.Last().Price, 2);
                return 0;
            }
        }
        public double PriceDynamic
        {
            get
            {
                if (Scalps.Count > 0)
                    return (Scalps.Last().Price - Scalps[0].Price) / Scalps[0].Price ;
                return 0;
            }
        }

        // по факту это нужно для того, чтобы загрузка работала
        public LastTickResult SpeculateTick(long currentTime, LastTickResult result)
        {
            _tResult = result;
            return Tick(currentTime);
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
                result.IsBuying = (result.Magic + 0.2f) > 0.7f + (_tResult.IsBuying ? 0.2475f : 0);
                double PriceDeviation = Common.Rng.NextSingle();
                result.SetPrice(_tResult.Scalp.Price - PriceDeviation * (result.IsBuying ? -1 : 1));
            }
            else
            {
                result.SetPrice(_tResult.Scalp.Price);
            }
            _tResult = result;
            Scalps.Add(result.Scalp);
            return result;
        }

        public void Reset()
        {
            Scalps.Clear();
            LastTickResult result = new()
            {
                Magic = -1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                IsMoveHappend = false,
                IsBuying = false,
            };
            result.SetPrice(100);
            _tResult = result;
        }
    }
}
