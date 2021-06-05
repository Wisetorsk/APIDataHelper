using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIDataHelper
{
    /// <summary>
    /// Helper object!  Should be moved to common project... Along with BaseDTO
    /// This object facillitates the aquisition and processing of data following the NK "model" pattern.
    ///
    /// COMMENT Helper object should keep track of the amount of data available and keep fetching and
    /// updating its own stored data accordinlgy if more is available on the database
    ///
    /// </summary>
    /// <typeparam name="Tdto">DTO object representing the rows of data in database/datastore</typeparam>
    /// <typeparam name="Tdata">Model object representing the returned payload from api call with metadata</typeparam>
    public abstract class HelperBase<Tdto, Tdata> : IHelperBase<Tdata> where Tdata : IBaseDTO<Tdto>
    {
        public Tdto[] Data { get; set; }
        public Tdata QueryResult { get; set; }
        public HttpClient Http { get; set; }

        public APIRequest RequestObject { get; set; }

        public readonly string apiUrl;
        private readonly string _rowReturnParamName = "AntallRaderIRetur"; // Should be read from file, or given externally!
        private readonly string _pagesReturnParamName = "side"; // Should be read from file, or given externally!
        private readonly string _apiCall;
        private readonly int _retryWaitTime = 500; //500ms default
        private bool _waiting = false;
        private DateTime? _lastFetched; //Time of last datafetch
        private readonly int _fetchReconnectionDelayTime = 2000; // Minimum time between refreshes of data. (in milliseconds)

        public event Action FetchingData;

        public event Action DoneFecthing;

        private string AmountToGet(int numberToGet) => $"?{_rowReturnParamName}={numberToGet}";

        private string Page(int pageNum) => $"&{_pagesReturnParamName}={pageNum}";

        public int? CurrentPage => QueryResult?.CurrentPage;
        public int? NextPage => QueryResult?.NextPage;
        public int? PreviousPage => QueryResult?.PreviousPage;

        private void NotifyFetchingData() => FetchingData?.Invoke();

        private void NotifyDoneFetching() => DoneFecthing?.Invoke();

        public HelperBase(IHttpClientFactory httpFactory, string apiurl, string apiCall, int? waitOverload = null)
        {
            Http = httpFactory.CreateClient();
            apiUrl = apiurl;
            _apiCall = apiCall;
            if (waitOverload > 10000 || waitOverload < 10)
            {
                throw new Exception("The reconnection wait period cannot be over 10 seconds or under 10ms!!!");
            }
            else if (waitOverload.HasValue)
            {
                _retryWaitTime = (int)waitOverload;
            }
        }

        public HelperBase(IHttpClientFactory httpFactory, APIRequest request, int? waitOverload = null)
        {
            Http = httpFactory.CreateClient();
            RequestObject = request;
            if (waitOverload > 10000 || waitOverload < 10)
            {
                throw new Exception("The reconnection wait period cannot be over 10 seconds or under 10ms!!!");
            }
            else if (waitOverload.HasValue)
            {
                _retryWaitTime = (int)waitOverload;
            }
        }

        /// <summary>
        /// Fetches new data from api, and stores it locally in object.
        /// </summary>
        /// <param name="amount">Number of rows of data to fetch</param>
        /// <param name="page">Current page of data to fetch</param>
        /// <returns></returns>
        public async Task GetNewData(int amount = 100, int page = 1)
        {
            int connectionAttempts = 0;
            int reconnectionBreaker = 3; // max three reconnection attepmts. Find way to separate breaker logic to external method
            if (_lastFetched != null)
            {
                var now = DateTime.Now;
                var diff = now.Subtract((DateTime)_lastFetched).TotalMilliseconds;
                if (page == CurrentPage && diff < _fetchReconnectionDelayTime)
                {   // No need to refresh data every second, so break out
                    // Change to throwing of exception that can be caught by higher layer??
                    Console.WriteLine($"No need to refresh data after: {diff} milliseconds. Limit is set to: {_fetchReconnectionDelayTime}");
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
                        HttpResponseMessage response;
                        if (RequestObject is null)
                        {
                            response = await Http.GetAsync($"{apiUrl}/{_apiCall}{AmountToGet(amount)}{Page(page)}");
                        } else
                        {
                            response = await Http.GetAsync(RequestObject.FullRequest);
                        }
                        if (response.IsSuccessStatusCode)
                        {
                            QueryResult = JsonConvert.DeserializeObject<Tdata>(await response.Content.ReadAsStringAsync());
                            Data = QueryResult.Data; // Kinda creates a copy of the data tho... right? bad?
                            NotifyDoneFetching(); // Needs to ensure that ALL data has been fetched before being fired.
                        }
                        _lastFetched = DateTime.Now;
                        Console.WriteLine($"Response code: {response.StatusCode}"); // Change to use of ILogger!!
                    }
                }
                catch (TimeoutException)
                {
                    // Set a timer and wait until a set time has passed, and retry the connection to API
                    var time = new System.Timers.Timer(_retryWaitTime) { Enabled = true };
                    _waiting = true;
                    time.Elapsed += SetWaitingToDone;
                    // Find a way to "block" the thread. without.... blocking it...
                    // Maybe separate the api call and fetching logic into a separate method that can be attached to the timer.elapsed event???
                    Console.WriteLine($"Unable to connect to api, reconnecting...");
                }
                catch (Exception) // Most generic error.  Can error handlinng and processing be passed to an external class/middleware?
                {
                    Console.WriteLine("Unable to get data from API");
                    throw; // Add proper exception handling if the api responds with an error that can be handled or if it need to retry the connection.
                }
            } while (Data is null && connectionAttempts < reconnectionBreaker);
            if (connectionAttempts >= reconnectionBreaker)
            {
                Console.WriteLine("Connection timed out");
                throw new TooManyReconnectAttemptsException("Too many reconnection attempts", null);
            }
        }

        private void SetWaitingToDone(object sender, System.Timers.ElapsedEventArgs e)
        {
            _waiting = false;
        }
    }
}