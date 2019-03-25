using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.TestJob
{
    public class TestJobViewModel
    {
        public Models.TestJob TestJob { get; set; }
        public TestFeature TestFeature { get; set; }
        public Step Step { get; set; }
        public StepsForJob StepsForJob { get; set; }
        public int CurrentTechnicianID { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public Models.Job Job { get; set; }
        [Range(2015000000, 3030000000, ErrorMessage = "Job number is out of range")]
        [Required(ErrorMessage = "Please enter a Job Num")]
        public int NumJobSearch { get; set; }

        public List<Models.TestJob> TestJobList { get; set; }
        public List<Models.Job> JobList { get; set; }
        public List<TriggeringFeature> TriggerList { get; set; }
        public List<TestFeature> TestFeatureList { get; set; }
        public List<Step> StepList { get; set; }
        public List<StepsForJob> StepsForJobList { get; set; }
    }
}
