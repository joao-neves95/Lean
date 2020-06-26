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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using QuantConnect.ToolBox.Polygon.Constants;

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
        /// <param name="pathEndpoint"> The endpoint to request. Eg.: "" </param>
        /// <returns> T | null </returns>
        private async Task<T> GetAsync<T>(string pathEndpoint)
        {
            this.UriBuilder.Path = pathEndpoint;

            T result = default(T);
            HttpResponseMessage response = await this.HttpClient.GetAsync(this.UriBuilder.Uri);

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }

            return result;
        }

        #endregion PRIVATE METHODS
    }
}
