using QuantConnect.ToolBox.Polygon.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
