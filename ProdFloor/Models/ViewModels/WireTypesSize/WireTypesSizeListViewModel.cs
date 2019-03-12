using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.WireTypesSize
{
    public class WireTypesSizeListViewModel
    {
        public List<Models.WireTypesSize> WireTypes { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
