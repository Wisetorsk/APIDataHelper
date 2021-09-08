using System.Collections.Generic;

namespace APIDataHelper
{
    public class ApiCall : IApiCall
    {
        public string ApiUrl { get; set; }
        public string Controller { get; set; }
        public string FullCall => $"{ApiUrl}/{Controller}{ConstructCallParameters()}";
        public string Call => $"{Controller}{ConstructCallParameters()}";
        public Dictionary<string, object> CallParameters { get; set; }

        private string ConstructCallParameters()
        {
            int index = 0;
            string output = "?";
            if (CallParameters.Count == 0) return "";
            foreach (var parameter in CallParameters)
            {
                output += $"{parameter.Key}={parameter.Value}";
                if (CallParameters.Count > 1 && index != CallParameters.Count) output += "&";
                index++;
            }
            return output;
        }
    }
}
