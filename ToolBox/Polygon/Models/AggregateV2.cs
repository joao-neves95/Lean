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

using System;

using Newtonsoft.Json;

using QuantConnect.Data.Market;

namespace QuantConnect.ToolBox.Polygon.Models
{
    public class AggregateV2
    {
        [JsonProperty("t")]
        public int Timestamp { get; set; }

        [JsonProperty("v")]
        public int Volume { get; set; }

        [JsonProperty("o")]
        public decimal Open { get; set; }

        [JsonProperty("c")]
        public decimal Close { get; set; }

        [JsonProperty("h")]
        public decimal High { get; set; }

        [JsonProperty("l")]
        public decimal Low { get; set; }

        public TradeBar ToTradeBar(TimeSpan period, Symbol symbol = null)
        {
            return new TradeBar()
            {
                High = this.High,
                Low = this.Low,
                Open = this.Open,
                Close = this.Close,
                Value = this.Close,
                Volume = this.Volume,
                Time = Time.UnixTimeStampToDateTime(this.Timestamp),

                Period = period,
                Symbol = symbol,
                DataType = MarketDataType.TradeBar
            };
        }
    }
}
