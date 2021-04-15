using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    public class WirerPXP
    {
        public int WirerPXPID { get; set; }

        public int JobID { get; set; }

        public int SinglePO { get; set; }

        public int WirerID { get; set; }

        public List<PXPError> _PXPErrors { get; set; }
    }

    public class PXPError
    {
        public int PXPErrorID { get; set; }

        public int WirerPXPID { get; set; }

        public int PXPReasonID { get; set; }

        public int GuiltyWirerID { get; set; }


    }

    public class PXPReason
    {
        public int PXPReasonID { get; set; }

        public int Description { get; set; }

        public List<PXPError> _PXPErrors { get;set;}

    }
}
