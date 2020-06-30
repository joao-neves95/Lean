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
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;

using Newtonsoft.Json;

using QuantConnect.Util;
using QuantConnect.Data.Market;
using QuantConnect.ToolBox.Polygon.Constants;
using QuantConnect.ToolBox.Polygon.Models;
using QuantConnect.ToolBox.Polygon.Enums;

namespace QuantConnect.ToolBox.Polygon
{
    /// <summary>
    /// 
    /// Class to interact with Polygon's API.
    /// Original by @joao-neves95.
    /// 
    /// </summary>
    /// <author> https://github.com/joao-neves95 </author>
    public class PolygonAPIClient : IPolygonAPIClient, IDisposable
    {
        #region CONSTRUCTOR / DESTRUCTOR

        public PolygonAPIClient(string apiKey)
        {
            this.ApiKey = apiKey;
            this.UriBuilder = new UriBuilder(PolygonEndpoints.Protocol_Rest, PolygonEndpoints.Host_Rest);
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
                this.HttpClient.DisposeSafely();
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

        public static Resolution[] ImplementedResolutions
        {
            get
            {
                return new Resolution[] {
                    Resolution.Tick, Resolution.Minute, Resolution.Hour, Resolution.Daily
                };
            }
        }

        public static string[] ImplementedResolutionsStr
        {
            get
            {
                return PolygonAPIClient.ImplementedResolutions
                                       .Select((resolution) => Enum.GetName(typeof(Resolution), resolution))
                                       .ToArray();
            }
        }

        #endregion PUBLIC STATIC PROPERTIES

        #region PRIVATE METHODS

        private string ParseDateString(DateTime dateTime)
        {
            return dateTime.ToString(PolygonEndpoints.DateFormat, CultureInfo.InvariantCulture);
        }

        private TimeSpan ComputePeriod(Timespan timespan)
        {
            switch (timespan)
            {
                case Timespan.Minute:
                    return Time.OneMinute;
                case Timespan.Hour:
                    return Time.OneHour;
                case Timespan.Day:
                    return Time.OneDay;
                default:
                    throw new Exception(PolygonMessages.InvalidResolutionTimespan);
            }
        }

        /// <summary>
        /// Wrapper method to control GET requests with authorization.
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
            this.UriBuilder.Query = PolygonEndpoints.QueryKey_ApiKey_Rest + "=" + this.ApiKey;

            if (additionalQueryParams != null && additionalQueryParams.Length > 0)
            {
                this.UriBuilder.Query = this.UriBuilder.Query.Substring(1) + string.Join("&", additionalQueryParams);
            }

            HttpResponseMessage response = await this.HttpClient.GetAsync(this.UriBuilder.Uri);

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(
                    await response.Content.ReadAsStringAsync(),
                    new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Include,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Auto
                    }
                );
            }

            throw new Exception(
                PolygonMessages.RequestError + Environment.NewLine +
                "REQUEST INFO:" + Environment.NewLine +
                "Status Code:" + response.StatusCode +
                "Reason Phrase:" + response.ReasonPhrase +
                "Content:" + response.Content
            );
        }

        private async Task<List<Tick>> GetHistoricTradesPaginatedAsync<TWrap, TRes>(
            Func<DateTime, string> pathEndpointBuilder, DateTime startDate, DateTime endDate, int maxLimit = 10000)

            where TRes : ITickResult
            where TWrap : HistoricTradesBase<TRes>
        {
            DateTime currentDate = startDate;

            TWrap completeResponse = default(TWrap);
            TWrap currentResponse = default(TWrap);

            while (currentDate <= endDate)
            {
                int lastResultCount = -1;
                int lastResultTimestamp = -1;
                do
                {
                    currentResponse = await this.GetAsync<TWrap>(
                        pathEndpointBuilder(currentDate),
                        lastResultTimestamp == -1 ? null : new[] { $"{PolygonEndpoints.QueryKey_OffsetTimestamp_Rest}={lastResultTimestamp}" }
                    );

                    if (currentResponse == null || currentResponse.Results.Count == 0)
                    {
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

                } while (lastResultCount >= maxLimit);

                currentDate.AddDays(1);
            }

            List<Tick> result = new List<Tick>();

            for (int i = 0; i < completeResponse.Results.Count; ++i)
            {
                result.Add(completeResponse.Results[i].ToTick());
            }

            return result;
        }

        #endregion PRIVATE METHODS

        #region PUBLIC STATIC METHODS

        public static Timespan ComputeTimespan(Resolution resolution)
        {
            switch (resolution)
            {
                case Resolution.Minute:
                    return Timespan.Minute;
                case Resolution.Hour:
                    return Timespan.Hour;
                case Resolution.Daily:
                    return Timespan.Day;
                default:
                    throw new Exception(PolygonMessages.InvalidResolutionTimespan);
            }
        }

        #endregion PUBLIC STATIC METHODS

        #region PUBLIC METHODS

        public async Task<List<Tick>> GetStockHistoricTradesAsync(string ticker, DateTime startDate, DateTime endDate)
        {
            return await this.GetHistoricTradesPaginatedAsync<HistoricTradesV2<HistoricTrade>, HistoricTrade>(
                (currentDate) =>
                {
                    return $"{PolygonEndpoints.Path_StockHistoricTrades_V2}/" +
                           $"{ticker}/" +
                           $"{this.ParseDateString(currentDate)}";
                },
                startDate, endDate, PolygonEndpoints.ResponseLimit_StockHistoricTrades
            );
        }

        public async Task<List<Tick>> GetCryptoHistoricTradesAsync(string fromTicker, string toTicker, DateTime startDate, DateTime endDate)
        {
            return await this.GetHistoricTradesPaginatedAsync<HistoricTradesV1<HistoricTrade>, HistoricTrade>(
                (currentDate) =>
                {
                    return $"{PolygonEndpoints.Path_CryptoHistoricTrades_V1}/" +
                           $"{fromTicker}/{toTicker}/" +
                           $"{this.ParseDateString(currentDate)}";
                },
                startDate, endDate, PolygonEndpoints.ResponseLimit_CryptoHistoricTrades
            );
        }

        public async Task<List<Tick>> GetForexHistoricTradesAsync(string fromTicker, string toTicker, DateTime startDate, DateTime endDate)
        {
            return await this.GetHistoricTradesPaginatedAsync<HistoricTradesV1<ForexTrade>, ForexTrade>(
                (currentDate) =>
                {
                    return $"{PolygonEndpoints.Path_ForexHistoricTrades_V1}/" +
                           $"{fromTicker}/{toTicker}/" +
                           $"{this.ParseDateString(currentDate)}";
                },
                startDate, endDate, PolygonEndpoints.ResponseLimit_ForexHistoricTrades
            );
        }

        public async Task<List<TradeBar>> GetAggregatesAsync(string ticker, Timespan timespan, DateTime startDate, DateTime endDate)
        {
            AggregatesV2 response = await this.GetAsync<AggregatesV2>(
                $"{PolygonEndpoints.Path_Aggregates_V2}/" +
                $"{ticker}/1/{Enum.GetName(typeof(Timespan), timespan).ToLowerInvariant()}" +
                $"{this.ParseDateString(startDate)}/{this.ParseDateString(endDate)}/" +
                $"false"
            );

            List<TradeBar> result = new List<TradeBar>();
            TimeSpan period = this.ComputePeriod(timespan);

            for (int i = 0; i < response.Results.Count; ++i)
            {
                result.Add(response.Results[i].ToTradeBar(period));
            }

            return result;
        }

        #endregion PUBLIC METHODS
    }
}
