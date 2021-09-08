using System;

namespace APIDataHelper
{
    public class CommonResponse
    {
        public Object Metadata { get; set; }
        public Object Payload { get; set; }

        public int RowCount { get; set; }
        public bool Error { get; set; }
        public String ErrorDescription { get; set; }
        public String Status { get; set; }
    }
}
