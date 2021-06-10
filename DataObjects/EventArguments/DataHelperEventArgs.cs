using System;
using System.Collections.Generic;
using System.Text;

namespace APIDataHelper
{
    public class DataHelperEventArgs<DataObject> : EventArgs
    {
        public DataObject Data { get; set; }
        public DataHelperEventArgs(DataObject data)
        {
            Data = data;
        }
    }
}
