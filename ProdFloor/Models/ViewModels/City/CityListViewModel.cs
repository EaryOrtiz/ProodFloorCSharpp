using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class CityListViewModel
    {
        public List<City> Cities { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentSeparator { get; set; }

        [Display(Name = "State")]
        public int StateID { get; set; }
        [Display(Name = "Country")]
        public int CountryID { get; set; }
        public string Name { get; set; }
        public int TotalItems { get; set; }

        public bool CleanFields { get; set; }
    }
}