using System;
using System.Collections.Generic;
using System.Text;

namespace APIDataHelper
{ 
    public interface IBaseModel<TDto>
    {
        int CurrentPage { get; set; }
        TDto[] Data { get; set; }
        int NextPage { get; set; }
        int NoOfPagesForQuery { get; set; }
        int NoOfRowsInDataset { get; set; }
        int NoOfRowsInQuery { get; set; }
        int PageSize { get; set; }
        int PreviousPage { get; set; }
    }
}
