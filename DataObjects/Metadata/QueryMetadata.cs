using System;
using System.Collections.Generic;
using System.Text;

namespace APIDataHelper
{
    
    public class QueryMetadata
    {
        public DateTime Fetched { get; set; }
        public int NoOfRowsInDataset { get; set; }
        public int NoOfRowsInQuery { get; set; }
        public int PageSize { get; set; }
        public int NoOfPagesForQuery { get; set; }
        public int Pages => NoOfRowsInQuery / PageSize + (NoOfRowsInQuery % PageSize == 0 ? 0 : 1);
    }
    
}
