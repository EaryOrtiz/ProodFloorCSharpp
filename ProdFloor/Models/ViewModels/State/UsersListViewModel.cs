using System.Collections.Generic;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class StateListViewModel
    {
        public List<State> States { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentCountry { get; set; }
    }
}