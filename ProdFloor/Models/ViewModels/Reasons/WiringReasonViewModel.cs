using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Reasons
{
    public class WiringReasonViewModel
    {
        public List<WiringReason1> WiringReasons1 { get; set; }
        public List<WiringReason2> WiringReasons2 { get; set; }
        public List<WiringReason3> WiringReasons3 { get; set; }
        public List<WiringReason4> WiringReasons4 { get; set; }
        public List<WiringReason5> WiringReasons5 { get; set; }
        public WiringReason1 WiringReason1 { get; set; }
        public WiringReason2 WiringReason2 { get; set; }
        public WiringReason3 WiringReason3 { get; set; }
        public WiringReason4 WiringReason4 { get; set; }
        public WiringReason5 WiringReason5 { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentSeparator { get; set; }
        public int TotalItems { get; set; }

        [Display(Name = "Reason 1")]
        public int WiringReason1ID { get; set; }
        [Display(Name = "Reason 2")]
        public int WiringReason2ID { get; set; }
        [Display(Name = "Reason 3")]
        public int WiringReason3ID { get; set; }
        [Display(Name = "Reason 4")]
        public int WiringReason4ID { get; set; }
        [Display(Name = "Reason 5")]
        public int WiringReason5ID { get; set; }

        public string Description { get; set; }

        public bool CleanFields { get; set; }
    }
}
