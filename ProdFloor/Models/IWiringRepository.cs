using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    public interface IWiringRepository
    {
        IQueryable<WirerPXP> wirerPXPs { get; }
        IQueryable<PXPError> pXPErrors { get; }
        IQueryable<PXPReason> pXPReasons { get; }

        void SaveWirerPXP(WirerPXP wirerPXP);
        void SavePXPError(PXPError pXPError);
        void SavePXPReason(PXPReason pXPReason);

        TestJob DeleteWirerPXP(int WirerPXPID);
        TestJob DeletePXPError(int PXPErrorID);
        TestJob DeletePXPReason(int PXPReasonID);
    }
}
