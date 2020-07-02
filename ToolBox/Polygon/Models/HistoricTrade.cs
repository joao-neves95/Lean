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

using Newtonsoft.Json;

using QuantConnect.Data.Market;

namespace QuantConnect.ToolBox.Polygon.Models
{
    public class HistoricTrade : ITickResult
    {
        [JsonProperty("t")]
        public int Timestamp { get; set; }

        [JsonProperty("i")]
        public string TradeID { get; set; }

        [JsonProperty("x")]
        public int ExchangeID { get; set; }

        [JsonProperty("s")]
        public int Size { get; set; }

        [JsonProperty("p")]
        public decimal Price { get; set; }

        public Tick ToTick(Symbol symbol = null)
        {
            return new Tick()
            {
                Quantity = this.Size,
                Value = this.Price,
                Time = Time.UnixTimeStampToDateTime(this.Timestamp),

                DataType = MarketDataType.Tick,
                TickType = TickType.Trade,
                Symbol = symbol,
            };
        }
    }
}
