using System;
using System.Collections.Generic;
using System.Text;

namespace APIDataHelper
{ 
    public interface IBaseDTO<Tdata>
    {
        int CurrentPage { get; set; }
        Tdata[] Data { get; set; }
        int NextPage { get; set; }
        int NoOfPagesForQuery { get; set; }
        int NoOfRowsInDataset { get; set; }
        int NoOfRowsInQuery { get; set; }
        int PageSize { get; set; }
        int PreviousPage { get; set; }
    }
}
