using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    public class WiringPXP
    {
        public int WiringXPID { get; set; }

        public string Status { get; set; }

        public int JobID { get; set; }

        public int WiringStationID { get; set; }

        public int SinglePO { get; set; }

        public int WirerID { get; set; }

        public List<int> WirersID { get; set; }

        public List<PXPError> _PXPErrors { get; set; }
    }

    public class PXPError
    {
        public int PXPErrorID { get; set; }

        public int WiringXPID { get; set; }

        public int PXPReasonID { get; set; }

        public int GuiltyWirerID { get; set; }
    }

    public class PXPReason
    {
        public int PXPReasonID { get; set; }

        public string Description { get; set; }
    }

    public class WiringStation
    {
        public int WiringStationID { get; set; }
        public string Label { get; set; }

        //wiring or pxp
        public string Type { get; set; }

        [Display(Name = "Job Type")]
        public int JobTypeID { get; set; }
    }
}
