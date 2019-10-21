using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Stations
{
    public class StationListViewModel
    {
        public List<Station> Stations { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentSeparator { get; set; }

        public bool CleanFields { get; set; }

        public int StationID { get; set; }
        public string Label { get; set; }

        [Display(Name = "JobType")]
        public int JobTypeID { get; set; }

        public int TotalItems { get; set; }
    }
}
