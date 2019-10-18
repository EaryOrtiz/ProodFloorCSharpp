using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class LandingSystemsListViewModel
    {
        public List<LandingSystem> LandingSystems { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public bool CleanFields { get; set; }

        public string UsedIn { get; set; }
        public string Name { get; set; }

        [Display(Name = "JobType")]
        public int JobTypeID { get; set; }
    }
}
