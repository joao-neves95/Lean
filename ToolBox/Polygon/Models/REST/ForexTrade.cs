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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using QuantConnect.Data.Market;

namespace QuantConnect.ToolBox.Polygon.Models.REST
{
    public class ForexTrade : ITickResult
    {
        [JsonProperty("a")]
        public int AskPrice { get; set; }

        [JsonProperty("b")]
        public int BidPrice { get; set; }

        [JsonProperty("t")]
        public int Timestamp { get; set; }

        public Tick ToTick(Symbol symbol = null)
        {
            return new Tick()
            {
                AskPrice = this.AskPrice,
                BidPrice = this.BidPrice,

                DataType = MarketDataType.Tick,
                Symbol = symbol
            };
        }
    }
}
