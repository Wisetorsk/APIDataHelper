using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace APIDataHelper
{
    public class HelperConfiguration
    {
        public HelperConfiguration(IHttpClientFactory httpFactory, string apiCall, string apiUrl, string apikey, int? waitOverload = null)
        {
            HttpFactory = httpFactory;
            ApiCall = apiCall;
            ApiUrl = apiUrl;
            Apikey = apikey;
            WaitOverload = waitOverload;
        }

        public IHttpClientFactory HttpFactory { get; private set; }
        public string ApiCall { get; private set; }
        public string ApiUrl { get; private set; }
        public string Apikey { get; private set; }
        public int? WaitOverload { get; private set; }
    }

    /// <summary>
    /// Helper object!  Should be moved to common project... Along with BaseDTO
    /// This object facillitates the aquisition and processing of data following the NK "model" pattern.
    ///
    /// COMMENT Helper object should keep track of the amount of data available and keep fetching and
    /// updating its own stored data accordinlgy if more is available on the database
    ///
    /// </summary>
    /// <typeparam name="TDto">DTO object representing the rows of data in database/datastore</typeparam>
    /// <typeparam name="TModel">Model object representing the returned payload from api call with metadata</typeparam>
    public class HelperBase<TDto, TModel> : IHelperBase<TDto, TModel> where TModel : IBaseModel<TDto>
    {
        public TDto[] Data { get; set; }
        public TModel Search { get; set; }
        public HttpClient Http { get; set; }
        public QueryMetadata Metadata { get; set; } = null;
        public int DefaultPageSize { get; set; }
        public DateTime? LastFetched { get; set; } //Time of last datafetch


        public string ApiUrl { get; private set; }
        public string RowReturnParamName { get; set; }
        public string PagesReturnParamName { get; set; }
        private string _apikey;
        public int ReconnectPause { get; set; } //500ms default wait time between next attempt when api times out
        private string _apiCall;
        private bool _waiting = false;
        public int FetchReconnectionDelayTime { get; set; } // Minimum time between refreshes of data. (in milliseconds) 
        public int ReconnectionBreaker { get; set; }
        public int MaxOverload { get; set; }
        public int MinOverload { get; set; }

        private ApiCall _apiCallObject;

        public int? CurrentPage => Search?.CurrentPage;
        public int? NextPage => Search?.NextPage;
        public int? PreviousPage => Search?.PreviousPage;
        public int? NumberOfEntries => Data?.Length;
        // ReSharper disable once HeapView.BoxingAllocation
        private string AmountToGet(int numberToGet) => $"?{RowReturnParamName}={numberToGet}";
        // ReSharper disable once HeapView.BoxingAllocation
        private string Page(int pageNum) => $"&{PagesReturnParamName}={pageNum:int}";


        public event Action FetchingData;
        public event Action FetchFailed;
        public event Action DoneFetching;
        public event EventHandler<DataHelperEventArgs<TModel>> DoneFecthingWithReturn;


        private void NotifyFetchingData() => FetchingData?.Invoke();
        private void NotifyFetchFailed() => FetchFailed?.Invoke();
        private void NotifyDoneFetching(TModel result) => DoneFecthingWithReturn?
            .Invoke(this, new DataHelperEventArgs<TModel>(result));

        public TDto[] ReturnData()
        {
            return Data;
        }

        /// <summary>
        /// Updates the controller and action call used on the api.
        /// </summary>
        /// <param name="newCall">String representation of controller call</param>
        /// <returns></returns>
        public bool UpdateCall(string newCall)
        {
            // Needs validation!!!
            _apiCall = newCall;
            return true;
        }

        /// <summary>
        /// Updates the controller and action call used on the api
        /// </summary>
        /// <param name="call">ApiCall object</param>
        /// <returns></returns>
        public bool UpdateCall(ApiCall call)
        {
            if (call is null) return false;
            _apiCall = call.Call;
            return true;
        }

        private void ReadConfig(IConfiguration config)
        {
            ApiUrl = config["KundeInnsynApi:BaseUrl"];
            _apikey = config["KundeInnsynApi:ApiKey"];
            RowReturnParamName = config["ApiHelper:Settings:RowReturnParamName"];
            PagesReturnParamName = config["ApiHelper:Settings:PagesReturnParamName"];
            ReconnectPause = int.Parse(config["ApiHelper:Settings:ReconnectPause"]);
            FetchReconnectionDelayTime = int.Parse(config["ApiHelper:Settings:ReconnectPause"]);
            ReconnectionBreaker = int.Parse(config["ApiHelper:Settings:ReconnectionBreaker"]);
            MaxOverload = int.Parse(config["ApiHelper:Settings:MaxWaitOverload"]);
            MinOverload = int.Parse(config["ApiHelper:Settings:MinWaitOverload"]);
            DefaultPageSize = int.Parse(config["ApiHelper:Settings:DefaultPageSize"]);
        }

        private void ConfigureHttpClient(IHttpClientFactory httpFactory)
        {
            Http = httpFactory.CreateClient();
            Http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Http.DefaultRequestHeaders.Add("apiToken", _apikey);
            Http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true,
                MustRevalidate = true,
                Private = true,
            };
        }

        public HelperBase(HelperConfiguration helperConfiguration)
        {
            _apiCall = helperConfiguration.ApiCall;
            ApiUrl = helperConfiguration.ApiUrl;
            _apikey = helperConfiguration.Apikey;
            ConfigureHttpClient(helperConfiguration.HttpFactory);

            if (helperConfiguration.WaitOverload > 10000 || helperConfiguration.WaitOverload < 10)
            {
                throw new Exception("The reconnection wait period cannot be over 10 seconds or under 10ms!!!");
            }
            else if (helperConfiguration.WaitOverload.HasValue)
            {
                ReconnectPause = (int)helperConfiguration.WaitOverload;
            }
        }

        public HelperBase(IHttpClientFactory httpFactory, string apiCall, IConfiguration config,
            int? waitOverload = null)
        {
            ReadConfig(config);
            _apiCall = apiCall;
            ConfigureHttpClient(httpFactory);

            if (waitOverload > 10000 || waitOverload < 10)
            {
                throw new Exception("The reconnection wait period cannot be over 10 seconds or under 10ms!!!");
            }
            else if (waitOverload.HasValue)
            {
                ReconnectPause = (int)waitOverload;
            }
        }


        public HelperBase(IHttpClientFactory httpFactory, ApiCall call, IConfiguration config, int? waitOverload = null)
        {
            _apiCallObject = call;
            ReadConfig(config);
            _apiCall = call.Call;
            ConfigureHttpClient(httpFactory);

            if (waitOverload > 10000 || waitOverload < 10)
            {
                throw new Exception("The reconnection wait period cannot be over 10 seconds or under 10ms!!!");
            }
            else if (waitOverload.HasValue)
            {
                ReconnectPause = (int)waitOverload;
            }
        }

        /// <summary>
        /// Fetches new data from api, and stores it locally in object.
        /// </summary>
        /// <param name="amount">Number of rows of data to fetch</param>
        /// <param name="page">Current page of data to fetch</param>
        /// <returns></returns>
        public async Task GetNewData(int? amount = null, int page = 1, bool rapid = false)
        {
            int connectionAttempts = 0;
            // max three reconnection attepmts. Find way to separate breaker logic to external method
            if (LastFetched != null)
            {
                var now = DateTime.Now;
                var diff = now.Subtract((DateTime)LastFetched).TotalMilliseconds;
                if (page == CurrentPage && diff < FetchReconnectionDelayTime && !rapid)
                {   // No need to refresh data every second, so break out
                    // Change to throwing of exception that can be caught by higher layer??
                    Console.WriteLine(
                        $"No need to refresh data after: {diff} milliseconds. " +
                        $"Limit is set to: {FetchReconnectionDelayTime}");
                    return;
                }
            }
            do
            {
                try
                {
                    NotifyFetchingData();
                    if (!_waiting)
                    {
                        connectionAttempts++;
                        Stopwatch watch = new();
                        watch.Start();
                        //Console.WriteLine($"Sending request to: {_apiCall}\t request Id: {requestID}");
                        var response =
                            await Http.GetAsync(
                                $"{ApiUrl}/{_apiCall}{AmountToGet(amount ?? DefaultPageSize)}{Page(page)}");
                        if (response.IsSuccessStatusCode)
                        {
                            String content = await response.Content.ReadAsStringAsync();
                            if (content != null && content.Length > 0)
                            {
                                Search = JsonConvert.DeserializeObject<TModel>(content);
                                //Console.WriteLine($"Recieved response\t request Id: {requestID}");
                                Data = Search.Data; // Kinda creates a copy of the data tho... right? bad?
                                NotifyDoneFetching(Search); // Needs to ensure that ALL data has been fetched before being fired.
                            }
                            DoneFetching?.Invoke();
                            watch.Stop();
                            Console.WriteLine(
                                $"\n\n Call: {_apiCall} \nFetch and deserialization took: " +
                                $"{watch.ElapsedMilliseconds} milliseconds\n\n");
                        }
                        LastFetched = DateTime.Now;
                        Console.WriteLine($"Response code: {response.StatusCode}"); // Change to use of ILogger!!
                    }


                }
                catch (TimeoutException)
                {
                    // Set a timer and wait until a set time has passed, and retry the connection to API
                    //var time = new System.Timers.Timer(_retryWaitTime) { Enabled = true };
                    //_waiting = true;
                    //time.Elapsed += SetWaitingToDone;
                    NotifyFetchFailed();
                    //Console.WriteLine($"Unable to connect to api, reconnecting...");
                }
                catch (Exception) // Most generic error.  Can error handlinng and processing be passed to an external class/middleware?
                {
                    NotifyFetchFailed();
                    //Console.WriteLine("Unable to get data from API");
                    throw; // Add proper exception handling if the api responds with an error that can be handled or if it need to retry the connection.
                }
            } while (Data is null && connectionAttempts < ReconnectionBreaker);
            if (connectionAttempts >= ReconnectionBreaker) Console.WriteLine("Connection timed out");
        }

        /// <summary>
        /// Gets all available data from controller in a single page.
        /// </summary>
        /// <returns></returns>
        public async Task GetAllData()
        {

            QueryMetadata metadata = await GetMetadata();
            if (metadata != null)
                await GetNewData(metadata.NoOfRowsInQuery, 1);
            else
            {
                //-- her må det inn logikk for alternativ flow 
                await GetNewData(DefaultPageSize, 1);
            }

        }


        private void SetWaitingToDone(object sender, System.Timers.ElapsedEventArgs e)
        {
            _waiting = false;
        }


        /// <summary>
        /// Sends a simple request to the server to get the available metadata.
        /// </summary>
        /// <returns></returns>
        public async Task<QueryMetadata> GetMetadata()
        {
            var response = await Http.GetAsync($"{ApiUrl}/{_apiCall}{AmountToGet(1)}{Page(1)}");
            if (response.IsSuccessStatusCode)
            {
                var responseBody = JsonConvert.DeserializeObject<TModel>(await response.Content.ReadAsStringAsync());
                if (responseBody != null)
                {
                    QueryMetadata metadata = CreateMetadataObject(responseBody);
                    return metadata;
                }
            }
            return null;
        }

        private QueryMetadata CreateMetadataObject(TModel responseBody)
        {
            QueryMetadata metadata = new();
            metadata.NoOfPagesForQuery = responseBody.NoOfPagesForQuery;
            metadata.NoOfRowsInDataset = responseBody.NoOfRowsInDataset;
            metadata.NoOfRowsInQuery = responseBody.NoOfRowsInQuery;
            metadata.PageSize = responseBody.PageSize;
            metadata.Fetched = DateTime.Now;
            Metadata = metadata;
            return metadata;
        }

        public async Task GetNextPage()
        {
            await GetNewData(DefaultPageSize, (int)NextPage);
        }

        public async Task GetPreviousPage()
        {
            await GetNewData(DefaultPageSize, (int)PreviousPage);
        }

        public async Task GetPage(int page)
        {
            await GetNewData(DefaultPageSize, page);
        }
    }
}
