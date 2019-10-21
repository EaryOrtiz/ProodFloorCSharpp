using System;
using System.Collections.Generic;
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
        public int JobTypeID { get; set; }
    }
}
