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

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime FinishDate { get; set; }


        public double ElapsedTimeHours { get; set; }
        public double ElapsedTimeMinutes { get; set; }
    }
}
