using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuantConnect.Data;
using QuantConnect.Interfaces;
using QuantConnect.Packets;

namespace QuantConnect.ToolBox.Polygon.PolygonDataQueueHandler
{
    public class PolygonDataQueueHandler : IDataQueueHandler
    {
        public bool IsConnected => false;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BaseData> GetNextTicks()
        {
            throw new NotImplementedException();
        }

        public void Subscribe(LiveNodePacket job, IEnumerable<Symbol> symbols)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(LiveNodePacket job, IEnumerable<Symbol> symbols)
        {
            throw new NotImplementedException();
        }
    }
}
