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
using System.Globalization;

using QuantConnect.Configuration;
using QuantConnect.Data;
using QuantConnect.Logging;
using QuantConnect.ToolBox.Polygon.Constants;

namespace QuantConnect.ToolBox.Polygon.PolygonDownloader
{
    /// <summary>
    /// Polygon Data Downloader Program.
    /// 
    /// <para></para>
    /// Original by @joao-neves95.
    /// </summary>
    /// <author> https://github.com/joao-neves95 </author>
    public static class PolygonDownloaderProgram
    {
        /// <summary>
        // Entry point to the Polygon Data Downloader.
        /// </summary>
        /// <param name="params"></param>
        public static void PolygonDownloader(PolygonDownloaderParams @params)
        {
            try
            {
                if (@params.ToDate < @params.FromDate)
                {
                    throw new ArgumentException(PolygonMessages.EndDateBiggerStartDate);
                }

                Resolution[] resolutions;
                bool resolutionAll = @params.Resolution.ToUpper(CultureInfo.InvariantCulture) == "ALL";

                if (resolutionAll)
                {
                    resolutions = PolygonAPIClient.ImplementedResolutions;
                }
                else
                {
                    Resolution resolution;

                    if (!Parse.TryParseResolution(@params.Resolution, out resolution))
                    {
                        throw new Exception(PolygonMessages.InvalidResolution(@params.Resolution));
                    }

                    resolutions = new Resolution[1] { resolution };
                }

                SecurityType securityType;

                if (!Enum.TryParse(@params.SecurityType, true, out securityType))
                {
                    throw new Exception(PolygonMessages.InvalidAssetType(@params.SecurityType));
                }

                IDataDownloader polygonDownloader = new PolygonDataDownloader(@params.ApiKey);
                Symbol symbol = null;

                for (int iTicker = 0; iTicker < @params.Tickers.Count; ++iTicker)
                {
                    symbol = Symbol.Create(@params.Tickers[iTicker], securityType, "empty");

                    for (int iResolution = 0; iResolution < resolutions.Length; ++iResolution)
                    {
                        IEnumerable<BaseData> data = polygonDownloader.Get(
                            symbol, resolutions[iResolution], @params.FromDate, @params.ToDate
                        );

                        LeanDataWriter dataWriter = new LeanDataWriter(
                            resolutions[iResolution],
                            symbol,
                            Config.Get("data-directory", "../../../../Data"),
                            TickType.Trade
                        );

                        dataWriter.Write(data);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                Environment.Exit(1);
            }
        }
    }
}
