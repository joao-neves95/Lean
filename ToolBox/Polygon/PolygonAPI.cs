﻿/*
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
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using QuantConnect.Data.Market;
using QuantConnect.ToolBox.Polygon.Constants;
using QuantConnect.ToolBox.Polygon.Models;

namespace QuantConnect.ToolBox.Polygon
{
    /// <summary>
    /// 
    /// Class to interact with Polygon's API.
    /// Original by @joao-neves95.
    /// 
    /// </summary>
    /// <author> https://github.com/joao-neves95 </author>
    public class PolygonAPI : IDisposable
    {
        #region CONSTRUCTOR / DESTRUCTOR

        public PolygonAPI(string apiKey)
        {
            this.ApiKey = apiKey;
            this.UriBuilder = new UriBuilder(PolygonEndpoints.Protocol_Rest, PolygonEndpoints.Host_Rest);
            this.UriBuilder.Query = PolygonEndpoints.ApiKeyQueryKey_Rest + "=" + this.ApiKey;
        }

        ~PolygonAPI()
        {
            if (!this.Disposed)
            {
                this.Dispose();
            }
        }

        public void Dispose()
        {
            if (!this.Disposed)
            {
                this.HttpClient.Dispose();
                this.Disposed = true;
            }
        }

        #endregion CONSTRUCTOR / DESTRUCTOR

        #region PRIVATE PROPERTIES

        private string ApiKey { get; set; }

        private UriBuilder UriBuilder { get; set; }

        private HttpClient HttpClient { get; } = new HttpClient();

        #endregion PRIVATE PROPERTIES

        #region PUBLIC PROPERTIES

        public bool Disposed { get; private set; }

        #endregion PUBLIC PROPERTIES

        #region PRIVATE METHODS

        /// <summary>
        /// Wrapper method to control GET requests.
        /// 
        /// <para></para>
        /// Original author @joao-neves95.
        /// </summary>
        /// <typeparam name="T"> Polygon's endpoint data model </typeparam>
        /// <param name="pathEndpoint"> The endpoint to request. E.g.: "v2/ticks/stocks/trades" </param>
        /// <param name="additionalQueryParams"> Adds adicional parameters to the request's query string. </param>
        /// <returns> T | null </returns>
        private async Task<T> GetAsync<T>(string pathEndpoint, string[] additionalQueryParams = null)
        {
            this.UriBuilder.Path = pathEndpoint;

            if (additionalQueryParams != null && additionalQueryParams.Length > 0)
            {
                this.UriBuilder.Query = this.UriBuilder.Query.Substring(1) + string.Join("&", additionalQueryParams);
            }

            T result = default(T);
            HttpResponseMessage response = await this.HttpClient.GetAsync(this.UriBuilder.Uri);

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }

            // TODO: (_SHIVAYL_) - Add error logging.

            return result;
        }

        #endregion PRIVATE METHODS

        #region PUBLIC METHODS

        public async Task<List<Tick>> GetEquitiesHistoricTradesAsync(Symbol symbol, DateTime startDate, DateTime endDate)
        {
            // TODO: (_SHIVAYL_) - Turn this pagination logic generic enought to be used by every (or most) endpoints.

            DateTime currentDate = startDate;

            Equities<HistoricTradeV2> completeResponse = new Equities<HistoricTradeV2>();
            Equities<HistoricTradeV2> currentResponse = null;

            while (currentDate <= endDate)
            {
                int lastRequestCount = -1;
                int lastResultTimestamp = -1;
                do
                {
                    currentResponse = await this.GetAsync<Equities<HistoricTradeV2>>(
                        $"{PolygonEndpoints.Path_EquitiesHistoricTrades_V2}/" +
                        $"{symbol.Value}/" +
                        $"{startDate.ToString(PolygonEndpoints.DateFormat, CultureInfo.InvariantCulture)}",
                        lastResultTimestamp == -1 ? null : new[] { $"{PolygonEndpoints.TimestampQueryKey_Rest}={lastResultTimestamp}" }
                    );

                    if (currentResponse == null || currentResponse.Results.Count == 0)
                    {
                        // TODO: (_SHIVAYL_) - Log invalid response.
                        continue;
                    }

                    if (lastRequestCount == -1)
                    {
                        completeResponse = currentResponse;
                    }
                    else
                    {
                        completeResponse.Add(currentResponse);
                    }

                    lastRequestCount = currentResponse.ResultsCount;

                } while (lastRequestCount >= PolygonEndpoints.MaxResponseSizeLimit);

                currentDate.AddDays(1);
            }

            List<Tick> result = new List<Tick>();

            for (int i = 0; i < completeResponse.Results.Count; ++i)
            {
                result.Add(completeResponse.Results[i].ToTick());
            }

            return result;
        }

        #endregion PUBLIC METHODS
    }
}
