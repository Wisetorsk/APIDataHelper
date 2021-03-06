using System;
using System.Collections.Generic;
using System.Text;

namespace APIDataHelper
{
    public class BaseModel<TDto> : IBaseModel<TDto>
    {
        public int NoOfRowsInDataset { get; set; } = 0;
        public int NoOfRowsInQuery { get; set; } = 0;
        public int PageSize { get; set; } = 100;
        public int NoOfPagesForQuery { get; set; } = 1;

        public int CurrentPage { get; set; } = 1;
        public int NextPage { get; set; } = 1;
        public int PreviousPage { get; set; } = 1;
        public TDto[] Data { get; set; }
    }
}
