using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class DashboardIndexViewModel
    {
        public IEnumerable<Models.Job> PendingJobs { get; set; }
        public IEnumerable<Models.TestJob> PendingTestJobs { get; set; }
        public PagingInfo PendingJobsPagingInfo { get; set; }
        public IEnumerable<Models.Job> ProductionJobs { get; set; }
        public PagingInfo ProductionJobsPagingInfo { get; set; }
        public IEnumerable<Models.Job> ActiveJobs { get; set; }
        public PagingInfo ActiveJobsPagingInfo { get; set; }
        public List<JobType> JobTypesList { get; set; }

        public string Side { get; set; }

        public List<AppUser> users { get; set; }


        public IEnumerable<Models.Job> MyJobs { get; set; }
        public PagingInfo MyJobsPagingInfo { get; set; }
        public IEnumerable<Models.Job> OnCrossJobs { get; set; }
        public PagingInfo OnCrossJobsPagingInfo { get; set; }
        public IEnumerable<Models.Job> PendingToCrossJobs { get; set; }
        public PagingInfo PendingToCrossJobsPagingInfo { get; set; }
        public string CurrentItem { get; set; }
        public string CurrentCategory { get; set; }
        public string buttonAction { get; set; }
        public List<JobType> JobTypes { get; set; }
        public List<PO> POs { get; set; }
        public List<StatusPO> StatusPOs { get; set; }
        public List<JobAdditional> MyJobAdditionals { get; set; }
        public List<JobAdditional> ActiveJobAdditionals { get; set; }
        public List<JobAdditional> OnCrossJobAdditionals { get; set; }
        public List<JobAdditional> PendingJobAdditionals { get; set; }
        public int CurrentEngID { get; set; }
        public int CurrentCrosAppEngID { get; set; }
        public string CurrentStatus { get; set; }
        public int JobID { get; set; }

        [Display(Name = "Job Number")]
        [Range(2015000000, 3030000000, ErrorMessage = "Job number is out of range")]
        public int jobnumber { get; set; }

        public List<Station> StationList { get; set; }

        public bool isEngAdmin { get; set; }

        public int CurrentUserID { get; set; }

        [Range(3000000, 4900000, ErrorMessage = "PO number is out of range")]
        [Required(ErrorMessage = "Please enter a PO")]
        public int POJobSearch { get; set; }



        //para el nuevo search 
        public PO PO { get; set; }
        public Models.Job Job { get; set; }
        public JobExtension JobExtension { get; set; }
        public HydroSpecific HydroSpecific { get; set; }
        public HoistWayData HoistWayData { get; set; }
        public GenericFeatures GenericFeatures { get; set; }
        public Indicator Indicator { get; set; }
        public SpecialFeatures SpecialFeature { get; set; }

        public Element Element { get; set; }
        public ElementHydro ElementHydro {get; set;}
        public ElementTraction ElementTraction {get; set;}

        public string JobTypeName { get; set; }

        //for WiringPXP
        public IEnumerable<WiringPXP> MyWiringPXPs { get; set; }
        public PagingInfo MyWiringPXPsPagingInfo { get; set; }
        public IEnumerable<WiringPXP> OnCrossWiringPXPS { get; set; }
        public PagingInfo OnCrossWiringPXPsPagingInfo { get; set; }
        public IEnumerable<WiringPXP> PendingToCrossWiringPXPs { get; set; }
        public PagingInfo PendingToCrossWiringPXPsPagingInfo { get; set; }


        public WiringPXP WiringPXP { get; set; }

        public int StatusPOCount { get; set; }

        //for Wiring
        public IEnumerable<Models.Wiring> MyWirings { get; set; }
        public PagingInfo MyWiringsPagingInfo { get; set; }
        public IEnumerable<Models.Wiring> OnCrossWirings { get; set; }
        public PagingInfo OnCrossWiringsPagingInfo { get; set; }
        public IEnumerable<Models.Wiring> PendingToCrossWirings { get; set; }
        public PagingInfo PendingToCrossWiringsPagingInfo { get; set; }


        public Models.Wiring Wiring { get; set; }



    }
}
