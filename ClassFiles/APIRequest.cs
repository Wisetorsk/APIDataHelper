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
        public int? Page { get; set; } = null;
        public int? RowsToReturn { get; set; } = null;
        //public IBaseDTO<object> ReturnedData { get; set; }
        public string FullRequest
        {
            get
            {
                string url = "";
                int index = 0;
                if (!(Parameters.Count() == 0)) return url;
                url += "?";
                foreach (var item in Parameters)
                {
                    url += $"{item.Key}={item.Value}";
                    if (!(Parameters.Count == index)) url += "&";
                    index++;
                }
                if (!(RowsToReturn is null)) url += $"antallraderiretur={RowsToReturn}";
                if (!(Page is null)) url += $"side={Page}";
                return url;
            }
        }

        public APIRequest(string apiurl, string controllerName, Dictionary<string, object> parameters)
        {
            ApiBaseUrl = apiurl;
            Controller = controllerName;
            Parameters = parameters;
            if (Parameters.ContainsKey("side")) Page = (int)Parameters["side"];
            if (Parameters.ContainsKey("Side")) Page = (int)Parameters["Side"];
            if (Parameters.ContainsKey("page")) Page = (int)Parameters["page"];
            if (Parameters.ContainsKey("Page")) Page = (int)Parameters["Page"];
            if (Parameters.ContainsKey("antallraderiretur")) RowsToReturn = (int)Parameters["antallraderiretur"];
            if (Parameters.ContainsKey("AntallRaderIRetur")) RowsToReturn = (int)Parameters["AntallRaderIRetur"];
            if (Parameters.ContainsKey("rowstoreturn")) RowsToReturn = (int)Parameters["rowstoreturn"];
            if (Parameters.ContainsKey("RowsToReturn")) RowsToReturn = (int)Parameters["RowsToReturn"];
        }
    }
}
