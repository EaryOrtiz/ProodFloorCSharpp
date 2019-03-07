using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Starter
{
    public class StarterListViewModel
    {
        public List<Models.Starter> Starters { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
