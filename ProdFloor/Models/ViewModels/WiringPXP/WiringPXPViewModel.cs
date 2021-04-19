using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Wiring
{
    public class WiringPXPViewModel
    {
        public WiringPXP wiringPXP { get; set; }

        public PXPError  pXPError { get; set; }
        public PXPReason pXPReason { get; set; }


        public List<WiringPXP> wiringPXPList { get; set; }

        public List<PXPError> pXPErrorList { get; set; }
        public List<PXPReason> pXPReasonList { get; set; }

        public PagingInfo PagingInfo { get; set; }


        //Maybe required

        public Models.Job Job { get; set; }

        public List<JobType> JobTypeList { get; set; }

        [Range(3000000, 4900000, ErrorMessage = "PO number is out of range")]
        [Required(ErrorMessage = "Please enter a PO")]
        public int POJobSearch { get; set; }

        public bool isNotDummy { get; set; }

        public bool CleanFields { get; set; }
        public int TotalItems { get; set; }
    }
}
