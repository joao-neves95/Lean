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

using QuantConnect.Data;
using QuantConnect.ToolBox.Polygon.Constants;
using QuantConnect.ToolBox.Polygon.Interfaces;

namespace QuantConnect.ToolBox.Polygon.PolygonDownloader
{
    /// <summary>
    /// Polygon Equity/Stock Data Downloader Class.
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
            this.PolygonAPI = new PolygonAPIClient(apiKey);
        }

        private IPolygonAPIClient PolygonAPI { get; set; }

        public string ApiKey { get; set; }

        public IEnumerable<BaseData> Get(Symbol symbol, Resolution resolution, DateTime startDate, DateTime endDate)
        {
            switch (resolution)
            {
                case Resolution.Tick:
                    string toTicker = string.Empty;
                    string fromTicker = string.Empty;

                    if (symbol.SecurityType == SecurityType.Forex || symbol.SecurityType == SecurityType.Crypto)
                    {
                        string[] splitedSymbol = symbol.Value.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                        if (splitedSymbol.Length != 2)
                        {
                            throw new Exception(PolygonMessages.InvalidSymbolTickData);
                        }

                        toTicker = splitedSymbol[0];
                        fromTicker = splitedSymbol[1];
                    }

                    switch (symbol.SecurityType)
                    {
                        case SecurityType.Equity:
                            return this.PolygonAPI.GetStockHistoricTradesAsync(symbol.Value, startDate, endDate)
                                                  .GetAwaiter().GetResult();
                        case SecurityType.Forex:
                            return this.PolygonAPI.GetForexHistoricTradesAsync(toTicker, fromTicker, startDate, endDate)
                                       .GetAwaiter().GetResult();
                        case SecurityType.Crypto:
                            return this.PolygonAPI.GetCryptoHistoricTradesAsync(toTicker, fromTicker, startDate, endDate)
                                                  .GetAwaiter().GetResult();
                    }

                    throw this.ThrowInvalidResolution(resolution);

                case Resolution.Minute:
                case Resolution.Hour:
                case Resolution.Daily:
                    return this.PolygonAPI.GetAggregatesAsync(symbol.Value, PolygonAPIClient.ComputeTimespan(resolution), startDate, endDate)
                                          .GetAwaiter().GetResult();
                default:
                    throw this.ThrowInvalidResolution(resolution);
            }
        }

        private Exception ThrowInvalidResolution(Resolution resolution)
        {
            throw new NotImplementedException(
                PolygonMessages.NotImplementedResolution +
                PolygonMessages.InvalidResolution(nameof(resolution))
            );
        }
    }
}
