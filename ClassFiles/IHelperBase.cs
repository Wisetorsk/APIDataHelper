using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIDataHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Tdto"></typeparam>
    /// <typeparam name="Tdata"></typeparam>
    public interface IHelperBase<Tdto, Tdata>
    {
        Tdto[] Data { get; set; }
        Tdata Search { get; set; }
        HttpClient Http { get; set; }
        int? NextPage { get; }
        int? PreviousPage { get; }
        int? CurrentPage { get; }

        event EventHandler<DataHelperEventArgs<Tdata>> DoneFecthing;
        event Action FetchFailed;
        event Action FetchingData;

        Task GetNewData(int amount = 100, int page = 1);
        Task<QueryMetadata> GetMetadata();
    }
}