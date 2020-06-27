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

using QuantConnect.Data;
using QuantConnect.Data.Market;

namespace QuantConnect.ToolBox.Polygon.PolygonDownloader
{
    /// <summary>
    /// Polygon Data Downloader Class.
    /// 
    /// <para></para>
    /// Original by @joao-neves95.
    /// </summary>
    /// <author> https://github.com/joao-neves95 </author>
    public class PolygonDataDownloader : IDataDownloader
    {
        public PolygonDataDownloader(string apiKey)
        {
            this.ApiKey = apiKey;
            this.PolygonAPI = new PolygonAPI(apiKey);
        }

        private PolygonAPI PolygonAPI { get; set; }

        public string ApiKey { get; set; }

        public IEnumerable<BaseData> Get(Symbol symbol, Resolution resolution, DateTime startDate, DateTime endDate)
        {
            if (resolution == Resolution.Tick)
            {
                return this.PolygonAPI.GetEquitiesHistoricTradesAsync(symbol, startDate, endDate).GetAwaiter().GetResult();
            }

            throw new NotImplementedException();
        }
    }
}
