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

using QuantConnect.ToolBox.Polygon.Enums;

namespace QuantConnect.ToolBox.Polygon.Constants
{
    public static class PolygonMessages
    {
        #region PROPERTIES

        public const string ErrorNotice = "PolygonDownloader ERROR: ";

        public const string EndDateBiggerStartDate = "The end date must be greater or equal than the start date.";

        private const string _InvalidResolution = "Invalid resoltion ";

        public const string NotImplementedResolution = "Resoltion not yet implemented.";

        private const string _InvalidAssetType = "Invalid asset type ";

        public const string NotImplementedAssetType = "Asset Type not yet implemented.";

        public const string InvalidResolutionTimespan = PolygonMessages._InvalidResolution + "/ timespan.";

        public const string RequestError = PolygonMessages.ErrorNotice + "Polygon.io API request error.";

        public const string InvalidSymbolTickData = PolygonMessages.ErrorNotice +
                                                    "Invalid symbol value. For forex/crypto data, you will have to use a slash. " +
                                                    "E.g.: \"USD/EUR\" \"USD/BTC\"";

        public static string ResolutionUse
        {
            get
            {
                return $"--resolution=${PolygonAPIClient.ImplementedResolutionsStr}/All";
            }
        }

        public static string AssetTypesUse
        {
            get
            {
                return $"--asset-type=${String.Join("/", Enum.GetNames(typeof(AssetType)))}";
            }
        }

        #endregion PROPERTIES

        public static string InvalidResolution(string invalidValue)
        {
            return PolygonMessages.ErrorNotice + PolygonMessages._InvalidResolution + $"'{invalidValue}'." +
                   Environment.NewLine +
                   "Please use: " + PolygonMessages.ResolutionUse;
        }

        public static string InvalidAssetType(string invalidValue)
        {
            return PolygonMessages.ErrorNotice + PolygonMessages._InvalidAssetType + $"'{invalidValue}'." +
                   Environment.NewLine +
                   "Please use:" + PolygonMessages.AssetTypesUse;
        }
    }
}
