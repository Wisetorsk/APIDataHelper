using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIDataHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public interface IHelperBase<TDto, TModel>
    {
        TDto[] Data { get; set; }
        TModel Search { get; set; }
        HttpClient Http { get; set; }
        QueryMetadata Metadata { get; set; }
        int DefaultPageSize { get; set; }
        int? NextPage { get; }
        int? PreviousPage { get; }
        int? CurrentPage { get; }
        int? NumberOfEntries { get; }

        event EventHandler<DataHelperEventArgs<TModel>> DoneFecthingWithReturn;
        event Action FetchFailed;
        event Action FetchingData;
        event Action DoneFetching;

        Task GetNewData(int? amount = null, int page = 1, bool rapid = false);
        Task<QueryMetadata> GetMetadata();
        bool UpdateCall(string newCall);
        bool UpdateCall(ApiCall call);

        Task GetNextPage();
        Task GetPreviousPage();
        Task GetPage(int page);
    }
}