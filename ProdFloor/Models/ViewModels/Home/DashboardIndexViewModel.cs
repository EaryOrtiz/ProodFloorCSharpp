using System.Collections.Generic;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class DashboardIndexViewModel
    {
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
        public int CurrentEngID { get; set; }
        public int CurrentCrosAppEngID { get; set; }
        public string CurrentStatus { get; set; }
        public int JobID { get; set; }
    }
}
