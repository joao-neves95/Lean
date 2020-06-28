/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * https://github.com/joao-neves95
*/

using System.Collections.Generic;

using Newtonsoft.Json;

namespace QuantConnect.ToolBox.Polygon.Models
{
    public class StockHistoricTrades<T>
        where T : IPolygonAPIResult
    {
        [JsonProperty("results_count")]
        public int ResultsCount { get; set; } = 0;

        [JsonProperty("db_latency")]
        public int MsLatency { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("ticker")]
        public string Ticker { get; set; }

        [JsonProperty("results")]
        public List<T> Results { get; set; } = new List<T>();

        public StockHistoricTrades<T> Add(StockHistoricTrades<T> newEquitiesInstance)
        {
            this.ResultsCount += newEquitiesInstance.ResultsCount;
            this.Results.AddRange(newEquitiesInstance.Results);

            return this;
        }
    }
}
