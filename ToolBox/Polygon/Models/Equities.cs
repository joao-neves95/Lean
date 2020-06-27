using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.ToolBox.Polygon.Models
{
    public class Equities<T>
        where T : IPolygonAPIResult
    {
        public int ResultsCount { get; set; } = 0;

        public int DBLatency { get; set; }

        public bool Success { get; set; }

        public string Ticker { get; set; }

        public List<T> Results { get; set; } = new List<T>();

        public Equities<T> Add(Equities<T> newEquitiesInstance)
        {
            this.ResultsCount += newEquitiesInstance.ResultsCount;
            this.Results.AddRange(newEquitiesInstance.Results);

            return this;
        }
    }
}
