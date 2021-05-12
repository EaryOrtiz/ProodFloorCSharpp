using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    public class EFWiringRepository : IWiringRepository
    {
        private ApplicationDbContext context;

        public EFWiringRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<WiringPXP> WiringPXPs => context.WiringPXPs;
        public IQueryable<PXPError> PXPErrors => context.PXPErrors;
        public IQueryable<PXPReason> PXPReasons => context.PXPReasons;
        public IQueryable<WirersPXPInvolved> WirersPXPInvolveds => context.WirersPXPInvolveds;

        public void SaveWiringPXP(WiringPXP wiringPXP)
        {
            if (wiringPXP.WiringPXPID == 0)
            {
                context.WiringPXPs.Add(wiringPXP);
            }
            else
            {
                WiringPXP dbEntry = context.WiringPXPs
                .FirstOrDefault(p => p.WirerPXPID == wiringPXP.WiringPXPID);
                if (dbEntry != null)
                {
                    dbEntry.WirerPXPID = wiringPXP.WirerPXPID;
                    dbEntry.JobID = wiringPXP.JobID;
                    dbEntry.SinglePO = wiringPXP.SinglePO;
                    dbEntry.StationID = wiringPXP.StationID;
                    dbEntry.StartDate = wiringPXP.StartDate;
                    dbEntry.EndDate = wiringPXP.EndDate;
                }
            }
            context.SaveChanges();

        }

        public void SavePXPError(PXPError pXPError)
        {
            if (pXPError.PXPErrorID == 0)
            {
                context.PXPErrors.Add(pXPError);
            }
            else
            {
                PXPError dbEntry = context.PXPErrors
                .FirstOrDefault(p => p.PXPErrorID == pXPError.PXPErrorID);
                if (dbEntry != null)
                {
                    dbEntry.WiringPXPID = pXPError.WiringPXPID;
                    dbEntry.PXPReasonID = pXPError.PXPReasonID;
                    dbEntry.GuiltyWirerID = pXPError.GuiltyWirerID;
                }
            }
            context.SaveChanges();

        }

        public void SavePXPReason(PXPReason pXPReason)
        {
            if (pXPReason.PXPReasonID == 0)
            {
                context.PXPReasons.Add(pXPReason);
            }
            else
            {
                PXPReason dbEntry = context.PXPReasons
                .FirstOrDefault(p => p.PXPReasonID == pXPReason.PXPReasonID);
                if (dbEntry != null)
                {
                    dbEntry.Description = pXPReason.Description;
                }
            }
            context.SaveChanges();

        }

        public void SaveWirersPXPInvolved(WirersPXPInvolved wirersPXPInvolved)
        {
            if (wirersPXPInvolved.WirersPXPInvolvedID == 0)
            {
                context.WirersPXPInvolveds.Add(wirersPXPInvolved);
            }
            else
            {
                WirersPXPInvolved dbEntry = context.WirersPXPInvolveds
                .FirstOrDefault(p => p.WirersPXPInvolvedID == wirersPXPInvolved.WirersPXPInvolvedID);
                if (dbEntry != null)
                {
                    dbEntry.WiringPXPID = wirersPXPInvolved.WiringPXPID;
                    dbEntry.WirerPXPID = wirersPXPInvolved.WirerPXPID;
                }
            }
            context.SaveChanges();

        }


        public WiringPXP DeleteWiringPXP(int WirerPXPID)
        {
            WiringPXP dbEntry = context.WiringPXPs
                 .FirstOrDefault(p => p.WiringPXPID == WirerPXPID);
            if (dbEntry != null)
            {
                context.WiringPXPs.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public PXPError DeletePXPError(int PXPErrorID)
        {
            PXPError dbEntry = context.PXPErrors
                 .FirstOrDefault(p => p.PXPErrorID == PXPErrorID);
            if (dbEntry != null)
            {
                context.PXPErrors.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public PXPReason DeletePXPReason(int PXPReasonID)
        {
            PXPReason dbEntry = context.PXPReasons
                  .FirstOrDefault(p => p.PXPReasonID == PXPReasonID);
            if (dbEntry != null)
            {
                context.PXPReasons.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public WirersPXPInvolved DeleteWirersPXPInvolved(int WirersPXPInvolvedID)
        {
            WirersPXPInvolved dbEntry = context.WirersPXPInvolveds
                  .FirstOrDefault(p => p.WirersPXPInvolvedID == WirersPXPInvolvedID);
            if (dbEntry != null)
            {
                context.WirersPXPInvolveds.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }


    }
}
