using System;
using System.Collections.Generic;
using ProdFloor.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Home
{
    public class CrossAppHubViewModel
    {
        public IEnumerable<Job> JobsToCross { get; set; }
        public PagingInfo ToCrossPagingInfo { get; set; }
        public IEnumerable<Job> JobsInCross { get; set; }
        public PagingInfo InCrossPagingInfo { get; set; }
    }
}
