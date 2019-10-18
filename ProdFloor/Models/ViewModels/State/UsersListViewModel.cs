using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class StateListViewModel
    {
        public List<State> States { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentCountry { get; set; }


        [Display(Name = "Country")]
        public int CountryID { get; set; }
        public string Name { get; set; }

        public bool CleanFields { get; set; }
    }
}