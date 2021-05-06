using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    public interface IWiringRepository
    {
        IQueryable<WiringPXP> WiringPXPs { get; }
        IQueryable<PXPError> PXPErrors { get; }
        IQueryable<PXPReason> PXPReasons { get; }
        IQueryable<WirersPXPInvolved> WirersPXPInvolveds { get; }

        void SaveWiringPXP(WiringPXP wiringPXP);
        void SavePXPError(PXPError pXPError);
        void SavePXPReason(PXPReason pXPReason);
        void SaveWirersPXPInvolved(WirersPXPInvolved wirersPXPInvolved);

        WiringPXP DeleteWirerPXP(int WiringPXPID);
        PXPError DeletePXPError(int PXPErrorID);
        PXPReason DeletePXPReason(int PXPReasonID);
        WirersPXPInvolved DeleteWirersPXPInvolved(int WirersPXPInvolvedID);
    }
}
