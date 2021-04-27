using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    public interface IWiringRepository
    {
        IQueryable<WiringPXP> wiringPXPs { get; }
        IQueryable<PXPError> pXPErrors { get; }
        IQueryable<PXPReason> pXPReasons { get; }

        void SaveWiringPXP(WiringPXP wiringPXP);
        void SavePXPError(PXPError pXPError);
        void SavePXPReason(PXPReason pXPReason);

        WiringPXP DeleteWirerPXP(int WiringPXPID);
        PXPError DeletePXPError(int PXPErrorID);
        PXPReason DeletePXPReason(int PXPReasonID);
    }
}
