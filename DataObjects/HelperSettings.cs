using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIDataHelper
{
    public class HelperSettings
    {
        public string ApiUrl { get; private set; }
        public string RowReturnParamName { get; set; }
        public string PagesReturnParamName { get; set; }
        public string ApiKey { get; private set; }
        public int ReconnectPause { get; set; } 
        public string ApiCall { get; set; }
        public int FetchReconnectionDelayTime { get; set; } 
        public int ReconnectionBreaker { get; set; }
        public int MaxOverload { get; set; }
        public int MinOverload { get; set; }
        public int DefaultPageSize { get; set; }
        public HelperSettings(string apiurl, string apikey)
        {
            ApiUrl = apiurl;
            ApiKey = apikey;
        }
    }
}
