using System.Collections.Generic;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class CountryListViewModel
    {
        public List<Country> Countries { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}