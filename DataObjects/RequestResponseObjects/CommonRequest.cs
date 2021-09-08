using System;
using System.Collections.Generic;

namespace APIDataHelper
{
    public class CommonRequest
    {
        public String RequestID { get; set; }
        public Guid CorrelationID { get; set; }
        public SortedList<String, Object> QueryParameters { get; set; }
        public SortedList<String, Object> ResultParameters { get; set; }
        public SortedList<String, Object> ACLParameters { get; set; }

        public CommonRequest(string requestID, Guid correlationID)
        {
            QueryParameters = new SortedList<String, object>();
            ResultParameters = new SortedList<String, object>();
            ACLParameters = new SortedList<String, object>();
            CorrelationID = correlationID;
            RequestID = requestID;
        }
    }
}
