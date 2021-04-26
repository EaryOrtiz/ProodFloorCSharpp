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

        public IQueryable<WiringPXP> wirerPXPs => context.wiringPXPs;
        public IQueryable<PXPError> pXPErrors => context.pXPErrors;
        public IQueryable<PXPReason> pXPReasons => context.pXPReasons;
        public IQueryable<WiringStation> wiringStations => context.wiringStations;

        public void SaveWirerPXP(WiringPXP wirerPXP)
        {
            if (wirerPXP.WiringXPID == 0)
            {
                context.wiringPXPs.Add(wirerPXP);
            }
            else
            {
                WiringPXP dbEntry = context.wiringPXPs
                .FirstOrDefault(p => p.WirerID == wirerPXP.WiringXPID);
                if (dbEntry != null)
                {
                    dbEntry.WirerID = wirerPXP.WirerID;
                    dbEntry.JobID = wirerPXP.JobID;
                    dbEntry.SinglePO = wirerPXP.SinglePO;
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
                    dbEntry.WiringXPID = pXPError.WiringXPID;
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

        public void SaveWiringStation(WiringStation wiringStation)
        {
            if (wiringStation.WiringStationID == 0)
            {
                context.wiringStations.Add(wiringStation);
            }
            else
            {
                WiringStation dbEntry = context.wiringStations
                .FirstOrDefault(p => p.WiringStationID == wiringStation.WiringStationID);
                if (dbEntry != null)
                {
                    dbEntry.JobTypeID = wiringStation.JobTypeID;
                    dbEntry.Label = wiringStation.Label;
                    dbEntry.Type = wiringStation.Type;
                }
            }
            context.SaveChanges();

        }

        public WiringPXP DeleteWirerPXP(int WirerPXPID)
        {
            WiringPXP dbEntry = context.wiringPXPs
                 .FirstOrDefault(p => p.WiringXPID == WirerPXPID);
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

        public WiringStation DeleteWiringStation(int WiringStationID)
        {
            WiringStation dbEntry = context.wiringStations
                  .FirstOrDefault(p => p.WiringStationID == WiringStationID);
            if (dbEntry != null)
            {
                context.wiringStations.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

    }
}
