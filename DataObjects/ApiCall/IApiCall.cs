using System.Collections.Generic;

namespace APIDataHelper
{
    public interface IApiCall
    {
        string ApiUrl { get; set; }
        string Call { get; }
        string FullCall { get; }
        Dictionary<string, object> CallParameters { get; set; }
        string Controller { get; set; }
    }
}