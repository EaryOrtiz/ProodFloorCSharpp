using System;
using System.Collections.Generic;
using ProdFloor.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Home
{
    public class CrossAppHubViewModel
    {
        public IEnumerable<Models.Job> JobsToCross { get; set; }
        public PagingInfo ToCrossPagingInfo { get; set; }
        public IEnumerable<Models.Job> JobsInCross { get; set; }
        public PagingInfo InCrossPagingInfo { get; set; }
    }
}
