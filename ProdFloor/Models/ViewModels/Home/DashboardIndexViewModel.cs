using System.Collections.Generic;
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
        public List<JobType> JobTypesList { get; set; }
        
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
        public List<JobAdditional> MyJobAdditionals { get; set; }
        public List<JobAdditional> OnCrossJobAdditionals { get; set; }
        public List<JobAdditional> PendingJobAdditionals { get; set; }
        public int CurrentEngID { get; set; }
        public int CurrentCrosAppEngID { get; set; }
        public string CurrentStatus { get; set; }
        public int JobID { get; set; }
    }
}
