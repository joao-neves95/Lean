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
using System.Linq;

using QuantConnect.Data;
using QuantConnect.Logging;
using QuantConnect.ToolBox.Polygon.Constants;
using QuantConnect.ToolBox.Polygon.Enums;

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
        /// Entry point to the Polygon Data Downloader.
        /// </summary>
        /// <param name="tickers"></param>
        /// <param name="resolution"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
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

                AssetType assetType;

                if (!Enum.TryParse(@params.AssetType, true, out assetType))
                {
                    throw new Exception(PolygonMessages.InvalidAssetType(@params.AssetType));
                }

                IDataDownloader polygonDownloader;

                switch (assetType)
                {
                    case AssetType.Stock:
                    case AssetType.Equity:
                        polygonDownloader = new PolygonStockDataDownloader(@params.ApiKey);
                        break;
                    case AssetType.Crypto:
                        polygonDownloader = new PolygonCryptoDataDownloader(@params.ApiKey);
                        break;
                    case AssetType.Forex:
                        polygonDownloader = new PolygonForexDataDownloader(@params.ApiKey);
                        break;
                    default:
                        throw new Exception(PolygonMessages.ErrorNotice + PolygonMessages.NotImplementedAssetType);
                }

                IEnumerable<BaseData> data = new List<BaseData>();

                for (int iTicker = 0; iTicker < @params.Tickers.Count; ++iTicker)
                {
                    for (int iResolution = 0; iResolution < resolutions.Length; ++iResolution)
                    {
                        data.Concat(polygonDownloader.Get(
                            @params.Tickers[iTicker], resolutions[iResolution], @params.FromDate, @params.ToDate
                        ));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                Environment.Exit(1);
            }

            throw new NotImplementedException();
        }
    }
}
