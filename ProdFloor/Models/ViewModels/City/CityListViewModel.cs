using System.Collections.Generic;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class CityListViewModel
    {
        public List<City> Cities { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentSeparator { get; set; }
    }
}