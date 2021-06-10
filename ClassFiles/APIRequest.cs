using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIDataHelper
{
    public class APIRequest
    {
        public string ApiBaseUrl { get; set; }
        public string Controller { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public string FullRequest
        {
            get
            {
                string url = $"{ApiBaseUrl}/{Controller}";
                int index = 0;
                if (!(Parameters.Count() == 0)) return url;
                url += "?";
                foreach (var item in Parameters)
                {
                    url += $"{item.Key}={item.Value}";
                    if (!(Parameters.Count == index)) url += "&";
                    index++;
                }
                return url;
            }
        }

        public string ControllerRequest
        {
            get
            {
                string url = $"{Controller}";
                int index = 0;
                if (!(Parameters.Count() == 0)) return url;
                url += "?";
                foreach (var item in Parameters)
                {
                    url += $"{item.Key}={item.Value}";
                    if (!(Parameters.Count == index)) url += "&";
                    index++;
                }
                return url;
            }
        }

        public APIRequest(string apiurl, string controllerName, Dictionary<string, object> parameters = null)
        {
            ApiBaseUrl = apiurl;
            Controller = controllerName;
            if (parameters != null) Parameters = parameters;
        }
    }
}
