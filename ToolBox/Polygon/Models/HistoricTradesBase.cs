using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.ToolBox.Polygon.Models
{
    public abstract class HistoricTradesBase<T>
        where T : ITickResult
    {
        public abstract int ResultsCount { get; set; }

        public abstract List<T> Results { get; set; }

        public HistoricTradesBase<T> Add(HistoricTradesBase<T> newEquitiesInstance)
        {
            this.ResultsCount += newEquitiesInstance.ResultsCount;
            this.Results.AddRange(newEquitiesInstance.Results);

            return this;
        }
    }
}
