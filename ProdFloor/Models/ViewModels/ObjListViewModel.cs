using System.Collections.Generic;
using System;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class ObjListViewModel
    {
        public IEnumerable<Object> Objects { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentSeparator { get; set; }
        public string Title { get; set; }
        public string ViewSummary { get; set; }
    }
}
