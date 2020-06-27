using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.ToolBox.Polygon.Constants
{
    public static class PolygonMessages
    {
        public const string ErrorNotice = "PolygonDownloader ERROR: ";

        private const string _InvalidResolution = "Invalid resoltion ";

        public const string NotImplementedResolution = "Resoltion not yet implemented.";

        public static string ResolutionPossibleValues
        {
            get
            {
                return $"--resolution=${PolygonAPIClient.ImplementedResolutionsStr}/All";
            }
        }

        public static string InvalidResolution(string invalidValue)
        {
            return PolygonMessages.ErrorNotice + PolygonMessages._InvalidResolution + $"'{invalidValue}'." +
                   Environment.NewLine +
                   "Please use: " + PolygonMessages.ResolutionPossibleValues;
        }
    }
}
