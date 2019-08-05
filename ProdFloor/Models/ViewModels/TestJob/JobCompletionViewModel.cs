using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.TestJob
{
    public class JobCompletionViewModel
    {
        public Models.TestJob TestJob { get; set; }
        public List<Models.TestJob> TestJobList { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime FinishDate { get; set; }
        
        public List<Step> StepList { get; set; }
        public List<StepsForJob> StepsForJobList { get; set; }
        public Step Step { get; set; }
        public StepsForJob StepsForJob { get; set; }
        public Stop Stop { get; set; }
        public List<Stop> StopList { get; set; }

        [Required(ErrorMessage = "Please enter the hours to complete")]
        [Range(1, 720, ErrorMessage = "The hours are out of range")]
        [Display(Name = "Hours to complete")]
        public double ElapsedTimeHours { get; set; }

        [Range(1, 60, ErrorMessage = "The minutes are out of range")]
        [Required(ErrorMessage = "Please enter the minutes to complete")]
        [Display(Name = "Minutes to complete")]
        public double ElapsedTimeMinutes { get; set; }
    }
}
