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
    public interface IHelperBase<Tdata>
    {
        //Tdto[] Data { get; set; }
        Tdata QueryResult { get; set; }
        HttpClient Http { get; set; }
        Task GetNewData(int amount = 100, int page = 1);
        //event Action DataLoaded;
    }
}