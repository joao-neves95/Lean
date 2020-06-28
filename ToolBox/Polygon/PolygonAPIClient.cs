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
using System.Threading.Tasks;
using System.Globalization;
using System.Net.Http;

using Newtonsoft.Json;

using QuantConnect.Data.Market;
using QuantConnect.ToolBox.Polygon.Constants;
using QuantConnect.ToolBox.Polygon.Models;

namespace QuantConnect.ToolBox.Polygon
{
    // https://polygon.io/docs/swagger.json
    /// <summary>
    /// 
    /// Class to interact with Polygon's API.
    /// Original by @joao-neves95.
    /// 
    /// </summary>
    /// <author> https://github.com/joao-neves95 </author>
    public class PolygonAPIClient : IDisposable
    {
        #region CONSTRUCTOR / DESTRUCTOR

        public PolygonAPIClient(string apiKey)
        {
            this.ApiKey = apiKey;
            this.UriBuilder = new UriBuilder(PolygonEndpoints.Protocol_Rest, PolygonEndpoints.Host_Rest)
            {
                Query = PolygonEndpoints.QueryKey_ApiKey_Rest + "=" + this.ApiKey
            };
        }

        ~PolygonAPIClient()
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

        #region PUBLIC STATIC PROPERTIES

        public static string[] ImplementedResolutionsStr
        {
            get
            {
                return new string[] { "Tick" };
            }
        }

        public static Resolution[] ImplementedResolutions
        {
            get
            {
                return new Resolution[] { Resolution.Tick };
            }
        }

        #endregion PUBLIC STATIC PROPERTIES

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
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync(), new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Include,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Auto
                });
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

            StockHistoricTrades<HistoricTrade> completeResponse = null;
            StockHistoricTrades<HistoricTrade> currentResponse = null;

            while (currentDate <= endDate)
            {
                int lastResultCount = -1;
                int lastResultTimestamp = -1;
                do
                {
                    currentResponse = await this.GetAsync<StockHistoricTrades<HistoricTrade>>(
                        $"{PolygonEndpoints.Path_EquitiesHistoricTrades_V2}/" +
                        $"{symbol.Value}/" +
                        $"{startDate.ToString(PolygonEndpoints.DateFormat, CultureInfo.InvariantCulture)}",
                        lastResultTimestamp == -1 ? null : new[] { $"{PolygonEndpoints.QueryKey_Timestamp_Rest}={lastResultTimestamp}" }
                    );

                    if (currentResponse == null || currentResponse.Results.Count == 0)
                    {
                        // TODO: (_SHIVAYL_) - Log invalid response.
                        continue;
                    }

                    if (lastResultCount == -1)
                    {
                        completeResponse = currentResponse;
                    }
                    else
                    {
                        completeResponse.Add(currentResponse);
                    }

                    lastResultCount = currentResponse.Results.Count;

                } while (lastResultCount >= PolygonEndpoints.ResponseSizeLimit_EquitiesHistoricTrades);

                currentDate.AddDays(1);
            }

            List<Tick> result = new List<Tick>();

            for (int i = 0; i < completeResponse.Results.Count; ++i)
            {
                result.Add(completeResponse.Results[i].ToTick());
            }

            return result;
        }

        public async Task<List<Tick>> GetCryptoHistoricTrades(Symbol symbol, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        #endregion PUBLIC METHODS
    }
}
