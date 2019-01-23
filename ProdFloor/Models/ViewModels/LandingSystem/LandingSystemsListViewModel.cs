﻿using System.Collections.Generic;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class LandingSystemsListViewModel
    {
        public IEnumerable<LandingSystem> LandingSystems { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
