
using QuantConnect.Data.Market;

namespace QuantConnect.ToolBox.Polygon.Models
{
    public class HistoricTradeV2 : IPolygonAPIResult
    {
        public string T { get; set; }

        public int t { get; set; }

        public int y { get; set; }

        public int f { get; set; }

        public int q { get; set; }

        public string i { get; set; }

        public int x { get; set; }

        public int s { get; set; }

        public decimal p { get; set; }

        public int z { get; set; }

        public Tick ToTick()
        {
            return new Tick()
            {
                Quantity = this.s,
                Value = this.p,
                Symbol = this.T,
                Time = Time.UnixTimeStampToDateTime(this.t),
                DataType = MarketDataType.Tick,
                TickType = TickType.Trade
            };
        }
    }
}
