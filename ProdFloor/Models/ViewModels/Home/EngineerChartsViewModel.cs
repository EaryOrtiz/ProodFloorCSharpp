using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Home
{
    public class EngineerChartsViewModel
    {

        public List<AppUser> users { get; set; }

        public string ChartName { get; set; }


        //M2000 charts details
        public IEnumerable<Models.Job> JobsWorkingOnItM2000 { get; set; }
        public IEnumerable<Models.Job> JobsCrossPendingM2000 { get; set; }
        public IEnumerable<Models.Job> JobsOnCrossM2000 { get; set; }
        public IEnumerable<Models.Job> JobsCrossCompleteM2000 { get; set; }

        public PagingInfo PagingInfoWorkingOnItM2000 { get; set; }
        public PagingInfo PagingInfoCrossPendingM2000 { get; set; }
        public PagingInfo PagingInfoOnCrossM2000 { get; set; }
        public PagingInfo PagingInfoCrossCompleteM2000 { get; set; }

        //Hydro charts details
        public IEnumerable<Models.Job> JobsWorkingOnItHydro { get; set; }
        public IEnumerable<Models.Job> JobsCrossCompleteHydro { get; set; }

        public PagingInfo PagingInfoWorkingOnItHydro { get; set; }
        public PagingInfo PagingInfoCrossCompleteHydro { get; set; }


        //Traction charts details
        public IEnumerable<Models.Job> JobsWorkingOnItTraction { get; set; }
        public IEnumerable<Models.Job> JobsCrossCompleteTraction { get; set; }

        public PagingInfo PagingInfoWorkingOnItTraction { get; set; }
        public PagingInfo PagingInfoCrossCompleteTraction { get; set; }


        public List<JobAdditional> MyJobAdditionals { get; set; }
        public List<PO> POs { get; set; }


        public string CurrentItem { get; set; }
        public string CurrentCategory { get; set; }
        public string buttonAction { get; set; }
        public List<JobType> JobTypes { get; set; }

        
        public List<JobAdditional> ActiveJobAdditionals { get; set; }
        public List<JobAdditional> OnCrossJobAdditionals { get; set; }
        public List<JobAdditional> PendingJobAdditionals { get; set; }
        public int CurrentEngID { get; set; }
        public int CurrentCrosAppEngID { get; set; }
        public string CurrentStatus { get; set; }
        public int JobID { get; set; }


        public List<Station> StationList { get; set; }
    }
}
