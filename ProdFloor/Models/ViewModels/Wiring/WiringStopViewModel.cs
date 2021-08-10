using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Wiring
{
    public class WiringStopViewModel
    {
        public List<WiringStop> StopList { get; set; }
        public List<Station> StationsList { get; set; }
        public List<JobType> JobTypeList { get; set; }
        public List<WiringReason1> Reasons1List { get; set; }
        public List<WiringReason2> Reasons2List { get; set; }
        public List<WiringReason3> Reasons3List { get; set; }
        public List<WiringReason4> Reasons4List { get; set; }
        public List<WiringReason5> Reasons5List { get; set; }
        public string Reason1Name { get; set; }
        public WiringStop Stop { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public bool CleanFields { get; set; }
        public string Critical { get; set; }
        public string JobNum { get; set; }
        public int PONum { get; set; }

        [Display(Name = "Shift End")]
        public bool WithShiftEnd { get; set; }
        [Display(Name = "Job Reassignment ")]
        public bool WithReassignment { get; set; }
        [Display(Name = "Returned From Complete ")]
        public bool WithReturnedFromComplete { get; set; }

        public int TotalOnDB { get; set; }
    }
}
