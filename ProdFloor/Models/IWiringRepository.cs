using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    public interface IWiringRepository
    {
        IQueryable<WiringPXP> wirerPXPs { get; }
        IQueryable<PXPError> pXPErrors { get; }
        IQueryable<PXPReason> pXPReasons { get; }

        void SaveWirerPXP(WiringPXP wirerPXP);
        void SavePXPError(PXPError pXPError);
        void SavePXPReason(PXPReason pXPReason);

        WiringPXP DeleteWirerPXP(int WiringPXPID);
        PXPError DeletePXPError(int PXPErrorID);
        PXPReason DeletePXPReason(int PXPReasonID);
    }
}
