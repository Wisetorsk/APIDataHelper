using Microsoft.Extensions.Configuration;
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
        public IConfiguration Config { get; set; }
        public HelperSettings GeneralSettings { get; set; } 
        public APIDataHelperFactory(IHttpClientFactory httpFactory, IConfiguration config)
        {
            Factory = httpFactory;
            Config = config;
            SetConfiguration(Config);
        }
        
        public HelperBase<TDto, TModel> CreateNew<TDto, TModel>(string apiCall) where TModel : IBaseModel<TDto>
        {
            return new HelperBase<TDto, TModel>(new HelperConfiguration(Factory, apiCall, GeneralSettings.ApiUrl, GeneralSettings.ApiKey)) { 
                RowReturnParamName = GeneralSettings.RowReturnParamName,
                PagesReturnParamName = GeneralSettings.PagesReturnParamName,
                ReconnectPause = GeneralSettings.ReconnectPause,
                FetchReconnectionDelayTime = GeneralSettings.FetchReconnectionDelayTime,
                ReconnectionBreaker = GeneralSettings.ReconnectionBreaker,
                MaxOverload = GeneralSettings.MaxOverload,
                MinOverload = GeneralSettings.MinOverload,
                DefaultPageSize = GeneralSettings.DefaultPageSize
            };
        }

        private void SetConfiguration(IConfiguration config)
        {
            GeneralSettings = new HelperSettings(config["Api:BaseUrl"], config["Api:ApiKey"])
            {
                RowReturnParamName = config["ApiHelper:Settings:RowReturnParamName"],
                PagesReturnParamName = config["ApiHelper:Settings:PagesReturnParamName"],
                ReconnectPause = int.Parse(config["ApiHelper:Settings:ReconnectPause"]),
                FetchReconnectionDelayTime = int.Parse(config["ApiHelper:Settings:ReconnectPause"]),
                ReconnectionBreaker = int.Parse(config["ApiHelper:Settings:ReconnectionBreaker"]),
                MaxOverload = int.Parse(config["ApiHelper:Settings:MaxWaitOverload"]),
                MinOverload = int.Parse(config["ApiHelper:Settings:MinWaitOverload"]),
                DefaultPageSize = int.Parse(config["ApiHelper:Settings:DefaultPageSize"])
            };

        }
    }
}
