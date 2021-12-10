using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Wiring
{
    public class WiringOptionsViewModel
    {
        public List<WiringOption> WiringOptions { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentSeparator { get; set; }

        public string Description { get; set; }
        public bool isDeleted { get; set; }

        public bool CleanFields { get; set; }
        public int TotalItems { get; set; }
    }
}
