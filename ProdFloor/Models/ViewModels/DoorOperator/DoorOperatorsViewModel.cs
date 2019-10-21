using System.Collections.Generic;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class DoorOperatorsListViewModel
    {
        public List<DoorOperator> DoorOperators { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentBrand { get; set; }

        public string Brand { get; set; }
        public string Style { get; set; }
        public string Name { get; set; }

        public bool CleanFields { get; set; }
        public int TotalItems { get; set; }
    }
}
