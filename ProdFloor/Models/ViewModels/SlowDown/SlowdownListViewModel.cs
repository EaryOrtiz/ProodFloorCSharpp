using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.SlowDown
{
    public class SlowdownListViewModel
    {
        public List<Slowdown> Slowdowns { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
