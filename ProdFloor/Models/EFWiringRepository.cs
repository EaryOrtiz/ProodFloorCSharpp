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

        public IQueryable<WiringPXP> wiringPXPs => context.wiringPXPs;
        public IQueryable<PXPError> pXPErrors => context.pXPErrors;
        public IQueryable<PXPReason> pXPReasons => context.pXPReasons;

        public void SaveWiringPXP(WiringPXP wiringPXP)
        {
            if (wiringPXP.WiringPXPID == 0)
            {
                context.wiringPXPs.Add(wiringPXP);
            }
            else
            {
                WiringPXP dbEntry = context.wiringPXPs
                .FirstOrDefault(p => p.WirerID == wiringPXP.WiringPXPID);
                if (dbEntry != null)
                {
                    dbEntry.WirerID = wiringPXP.WirerID;
                    dbEntry.JobID = wiringPXP.JobID;
                    dbEntry.SinglePO = wiringPXP.SinglePO;
                    dbEntry.Status = wiringPXP.Status;
                    dbEntry.StationID = wiringPXP.StationID;
                }
            }
            context.SaveChanges();

        }

        public void SavePXPError(PXPError pXPError)
        {
            if (pXPError.PXPErrorID == 0)
            {
                context.pXPErrors.Add(pXPError);
            }
            else
            {
                PXPError dbEntry = context.pXPErrors
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
                context.pXPReasons.Add(pXPReason);
            }
            else
            {
                PXPReason dbEntry = context.pXPReasons
                .FirstOrDefault(p => p.PXPReasonID == pXPReason.PXPReasonID);
                if (dbEntry != null)
                {
                    dbEntry.Description = pXPReason.Description;
                }
            }
            context.SaveChanges();

        }


        public WiringPXP DeleteWirerPXP(int WirerPXPID)
        {
            WiringPXP dbEntry = context.wiringPXPs
                 .FirstOrDefault(p => p.WiringPXPID == WirerPXPID);
            if (dbEntry != null)
            {
                context.wiringPXPs.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public PXPError DeletePXPError(int PXPErrorID)
        {
            PXPError dbEntry = context.pXPErrors
                 .FirstOrDefault(p => p.PXPErrorID == PXPErrorID);
            if (dbEntry != null)
            {
                context.pXPErrors.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public PXPReason DeletePXPReason(int PXPReasonID)
        {
            PXPReason dbEntry = context.pXPReasons
                  .FirstOrDefault(p => p.PXPReasonID == PXPReasonID);
            if (dbEntry != null)
            {
                context.pXPReasons.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }


    }
}
