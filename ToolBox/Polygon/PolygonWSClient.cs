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
using System.Security.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocket4Net;
using Newtonsoft.Json;

using QuantConnect.Util;
using QuantConnect.ToolBox.Polygon.Enums;
using QuantConnect.ToolBox.Polygon.Interfaces;
using QuantConnect.ToolBox.Polygon.Constants;
using QuantConnect.ToolBox.Polygon.Models.WS;

namespace QuantConnect.ToolBox.Polygon
{
    /// <summary>
    /// 
    /// A wrapper to interact with Polygon's WebSockets API.
    /// Original by @joao-neves95.
    /// 
    /// </summary>
    /// <author> https://github.com/joao-neves95 </author>
    public class PolygonWSClient : IPolygonWSClient, IDisposable
    {
        /// <summary>
        /// Creates a new WebSocket client wrapper for a Polygon's channel.
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="assetType"></param>
        public PolygonWSClient(string apiKey, AssetType assetType)
        {
            this.ApiKey = apiKey;
            this.AssetType = assetType;
            this.UriBuilder = new UriBuilder(PolygonEndpoints.Protocol_WS, PolygonEndpoints.Host_WS);
        }

        ~PolygonWSClient()
        {
            this.Dispose();
        }

        #region PRIVATE PROPERTIES

        private string ApiKey { get; set; }

        private AssetType AssetType { get; set; }

        private UriBuilder UriBuilder { get; set; }

        #endregion PRIVATE PROPERTIES

        #region PUBLIC PROPERTIES

        public WebSocket WebSocket { get; private set; } = null;

        #endregion PUBLIC PROPERTIES

        #region PRIVATE METHODS

        private string ComputeChannelType(WSChannelType wsChannelType)
        {
            switch (this.AssetType)
            {
                case AssetType.Equity:
                    switch (wsChannelType)
                    {
                        case WSChannelType.Trades:
                            return PolygonEndpoints.Channel_Stocks_Trades;
                        case WSChannelType.Quotes:
                            return PolygonEndpoints.Channel_Stocks_Quotes;
                    }

                    break;
                case AssetType.Crypto:
                    switch (wsChannelType)
                    {
                        case WSChannelType.Trades:
                            return PolygonEndpoints.Channel_Crypto_Trades;
                        case WSChannelType.Quotes:
                            return PolygonEndpoints.Channel_Crypto_Quotes;
                    }

                    break;
                case AssetType.Forex:
                    switch (wsChannelType)
                    {
                        case WSChannelType.Trades:
                            throw new Exception(); // TODO: (_SHIVAYL_) - Create Exception.
                        case WSChannelType.Quotes:
                            return PolygonEndpoints.Channel_Forex_Quotes;
                    }

                    break;
            }

            return null;
        }

        private void SendRequest(Request request)
        {
            if (WebSocket == null)
            {
                throw new Exception(); // Not connected.
            }

            this.WebSocket.Send(JsonConvert.SerializeObject(
                request,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Include,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Auto
                }
            ));
        }

        #endregion PRIVATE METHODS

        #region PUBLIC METHODS

        public void Dispose()
        {
            if (this.WebSocket.State == WebSocketState.Open)
            {
                this.WebSocket.Close();
            }

            this.WebSocket.DisposeSafely();
        }

        public void ConnectToChannel()
        {
            if (this.WebSocket.State == WebSocketState.Open)
            {
                throw new Exception(); // (_SHIVAYL_) - Create Exception.
            }

            switch (this.AssetType)
            {
                case AssetType.Equity:
                    this.UriBuilder.Path = PolygonEndpoints.Path_WS_Stocks;
                    break;
                case AssetType.Crypto:
                    this.UriBuilder.Path = PolygonEndpoints.Path_WS_Crypto;
                    break;
                case AssetType.Forex:
                    this.UriBuilder.Path = PolygonEndpoints.Path_WS_Forex;
                    break;
            }

            this.WebSocket = new WebSocket(
                this.UriBuilder.Uri.AbsoluteUri.ToStringInvariant(),
                sslProtocols: SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
            );

            // TODO: (_SHIVAYL_) - Add event listeners.

            this.WebSocket.Open();
        }

        public void Authenticate()
        {
            this.SendRequest(new Request(PolygonEndpoints.Action_WS_Auth, this.ApiKey));
        }

        public void Subscribe(string ticker, WSChannelType wsChannelType)
        {
            this.Subscribe(new[] { ticker }, wsChannelType);
        }

        public void Subscribe(string[] tickers, WSChannelType wsChannelType)
        {
            string channelType = this.ComputeChannelType(wsChannelType);
            StringBuilder streamParams = new StringBuilder(tickers.Length);

            // Let's use a loop to do everything at the same time and faster, instead of using LINQ.
            // We want something like this: "T.MSFT,T.AAPL,T.AMD,T.NVDA".
            for (int i = 0; i < tickers.Length; ++i)
            {
                streamParams.Append(channelType);
                streamParams.Append(tickers[i]);

                if (i < tickers.Length - 1)
                {
                    streamParams.Append(",");
                }
            }

            this.SendRequest(new Request(
                PolygonEndpoints.Action_WS_Sub, streamParams.ToString()
            ));
        }

        #endregion PUBLIC METHODS
    }
}
