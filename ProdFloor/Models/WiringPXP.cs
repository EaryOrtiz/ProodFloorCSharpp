using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    public class WiringPXP
    {
        public int WiringPXPID { get; set; }

        public int POID { get; set; }

        public int StationID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int SinglePO { get; set; }

        public int WirerPXPID { get; set; }

        public List<PXPError> _PXPErrors { get; set; }
    }

    public class WirersPXPInvolved
    {
        public int WirersPXPInvolvedID { get; set; }

        public int WiringPXPID { get; set; }

        public int WirerPXPID { get; set; }
    }

    public class PXPError
    {
        public int PXPErrorID { get; set; }

        public int WiringPXPID { get; set; }

        public int PXPReasonID { get; set; }

        public int GuiltyWirerID { get; set; }
    }

    public class PXPReason
    {
        public int PXPReasonID { get; set; }

        public string Description { get; set; }
    }

}
