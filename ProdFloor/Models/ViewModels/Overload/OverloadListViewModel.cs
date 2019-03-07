using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Overload
{
    public class OverloadListViewModel
    {
        public List<Models.Overload> Overloads { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
