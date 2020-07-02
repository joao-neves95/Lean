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

namespace QuantConnect.ToolBox.Polygon.Constants
{
    /// <summary>
    /// Polygon's API endpoint constant values.
    /// <para></para>
    /// Docs: <see href="https://polygon.io/docs"/>
    /// 
    /// <para></para>
    /// Original by @joao-neves95.
    /// </summary>
    public static class PolygonEndpoints
    {
        public const int ResponseLimit_StockHistoricTrades = 50000;

        public const int ResponseLimit_CryptoHistoricTrades = 10000;

        public const int ResponseLimit_ForexHistoricTrades = 10000;

        public const string DateFormat = "yyyy-MM-dd";

        public const string Protocol_Rest = "https";

        public const string Protocol_WS = "wss";

        public const string Host_Rest = "api.polygon.io";

        public const string Host_WS = "socket.polygon.io";

        public const string QueryKey_ApiKey_Rest = "apiKey";

        public const string QueryKey_OffsetTimestamp_Rest = "offset";

        public const string Path_StockHistoricTrades_V2 = "v2/ticks/stocks/trades";

        public const string Path_CryptoHistoricTrades_V1 = "v1/historic/crypto";

        public const string Path_ForexHistoricTrades_V1 = "v1/historic/forex";

        public const string Path_Aggregates_V2 = "v2/aggs/ticker";
    }
}
