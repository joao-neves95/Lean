using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using QuantConnect.Data.Market;
using QuantConnect.ToolBox.Polygon.Enums;

namespace QuantConnect.ToolBox.Polygon.Interfaces
{
    public interface IPolygonAPIClient
    {
        Task<List<Tick>> GetStockHistoricTradesAsync(string ticker, DateTime startDate, DateTime endDate);

        Task<List<Tick>> GetCryptoHistoricTradesAsync(string fromTicker, string toTicker, DateTime startDate, DateTime endDate);

        Task<List<Tick>> GetForexHistoricTradesAsync(string fromTicker, string toTicker, DateTime startDate, DateTime endDate);

        Task<List<TradeBar>> GetAggregatesAsync(string ticker, Timespan timespan, DateTime startDate, DateTime endDate);
    }
}
