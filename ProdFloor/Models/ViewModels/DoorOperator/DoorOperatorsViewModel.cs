using System.Collections.Generic;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class DoorOperatorsListViewModel
    {
        public IEnumerable<DoorOperator> DoorOperators { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentBrand { get; set; }
    }
}
