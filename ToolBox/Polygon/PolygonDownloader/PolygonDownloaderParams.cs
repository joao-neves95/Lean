using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.ToolBox.Polygon.PolygonDownloader
{
    public class PolygonDownloaderParams
    {
        public IList<string> Tickers { get; set; }

        public string Resolution { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string ApiKey { get; set; }
    }
}
