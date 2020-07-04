
namespace QuantConnect.ToolBox.Polygon.Models.WS
{
    public class Request
    {
        public Request()
        {
        }

        public Request(string _action, string _params)
        {
            this.Action = _action;
            this.Params = _params;
        }

        public string Action { get; set; }

        public string Params { get; set; }
    }
}
