using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace APIDataHelper 
{ 
    public class APIDataHelperFactory
    {
        //private readonly string _baseUrl = "https://localhost:5001";
        public IHttpClientFactory Factory { get; set; }
        public APIDataHelperFactory(IHttpClientFactory httpFactory)
        {
            Factory = httpFactory;
        }
        public HelperBase<object, IBaseDTO<object>> CreateNew(string apiUrl, string apiCall)
        {
            return new GenericFactory(Factory, apiUrl, apiCall);
        }

        public HelperBase<object, IBaseDTO<object>> CreateNew(APIRequest request)
        {
            return new GenericFactory(Factory, request);
        }



        public class GenericFactory : HelperBase<object, IBaseDTO<object>>
        {
            public GenericFactory(IHttpClientFactory httpFactory, string apiurl, string apiCall, int? waitOverload = null) : base(httpFactory, apiurl, apiCall, waitOverload)
            {
            }

            public GenericFactory(IHttpClientFactory httpFactory, APIRequest request, int? waitOverload = null) : base(httpFactory, request, waitOverload)
            {
            }
        }

    }
}
