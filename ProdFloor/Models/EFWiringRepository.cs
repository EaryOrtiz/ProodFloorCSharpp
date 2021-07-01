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

        //Wiring
        public IQueryable<Wiring> Wirings => context.Wirings;
        public IQueryable<WirersInvolved> WirersInvolveds => context.WirersInvolveds;
        public IQueryable<WiringStop> WiringStops => context.WiringStops;
        public IQueryable<WiringStep> WiringSteps => context.WiringSteps;
        public IQueryable<WiringStepForJob> WiringStepsForJobs => context.WiringStepsForJobs;
        public IQueryable<WiringReason1> WiringReason1s => context.WiringReason1s;
        public IQueryable<WiringReason2> WiringReason2s => context.WiringReason2s;
        public IQueryable<WiringReason3> WiringReason3s => context.WiringReason3s;
        public IQueryable<WiringReason4> WiringReason4s => context.WiringReason4s;
        public IQueryable<WiringReason5> WiringReason5s => context.WiringReason5s;
        public IQueryable<WiringOption> WiringOptions => context.WiringOptions;
        public IQueryable<WiringFeatures> WiringFeatures => context.WiringFeatures;
        public IQueryable<WiringTriggeringFeature> WiringTriggeringFeatures => context.WiringTriggeringFeatures;

        public void SaveWiring(Wiring wiring)
        {
            if (wiring.WiringID == 0)
            {
                context.Wirings.Add(wiring);
            }
            else
            {
                Wiring dbEntry = context.Wirings
                .FirstOrDefault(p => p.WiringID == wiring.WiringID);
                if (dbEntry != null)
                {
                    dbEntry.WirerID = wiring.WirerID;
                    dbEntry.POID = wiring.POID;
                    dbEntry.StationID = wiring.StationID;
                    dbEntry.StartDate = wiring.StartDate;
                    dbEntry.CompletedDate = wiring.CompletedDate;
                }
            }
            context.SaveChanges();

        }
        public void SaveWirersInvolved(WirersInvolved wirersInvolved)
        {
            if (wirersInvolved.WirersInvolvedID == 0)
            {
                context.WirersInvolveds.Add(wirersInvolved);
            }
            else
            {
                WirersInvolved dbEntry = context.WirersInvolveds
                .FirstOrDefault(p => p.WirersInvolvedID == wirersInvolved.WirersInvolvedID);
                if (dbEntry != null)
                {
                    dbEntry.WiringID = wirersInvolved.WiringID;
                    dbEntry.WirerID = wirersInvolved.WirerID;
                }
            }
            context.SaveChanges();

        }
        public void SaveWiringStop(WiringStop wiringStop)
        {
            if (wiringStop.WiringStopID == 0)
            {
                context.WiringStops.Add(wiringStop);
            }
            else
            {
                WiringStop dbEntry = context.WiringStops
                .FirstOrDefault(p => p.WiringStopID == wiringStop.WiringStopID);
                if (dbEntry != null)
                {
                    dbEntry.WiringID = wiringStop.WiringID;
                    dbEntry.Reason1 = wiringStop.Reason1;
                    dbEntry.Reason2 = wiringStop.Reason2;
                    dbEntry.Reason3 = wiringStop.Reason3;
                    dbEntry.Reason4 = wiringStop.Reason4;
                    dbEntry.Reason5ID = wiringStop.Reason5ID;
                    dbEntry.StartDate = wiringStop.StartDate;
                    dbEntry.StopDate = wiringStop.StopDate;
                    dbEntry.Elapsed = wiringStop.Elapsed;
                    dbEntry.Description = wiringStop.Description;
                    dbEntry.AuxWirerID = wiringStop.AuxWirerID;
                    dbEntry.AuxStationID = wiringStop.AuxStationID;
                    dbEntry.Critical = wiringStop.Critical;
                }
            }
            context.SaveChanges();

        }
        public void SaveWiringStep(WiringStep wiringStep)
        {
            if (wiringStep.WiringStepID == 0)
            {
                context.WiringSteps.Add(wiringStep);
            }
            else
            {
                WiringStep dbEntry = context.WiringSteps
                .FirstOrDefault(p => p.WiringStepID == wiringStep.WiringStepID);
                if (dbEntry != null)
                {
                    dbEntry.JobTypeID = wiringStep.JobTypeID;
                    dbEntry.Stage = wiringStep.Stage;
                    dbEntry.ExpectedTime = wiringStep.ExpectedTime;
                    dbEntry.Description = wiringStep.Description;
                    dbEntry.Order = wiringStep.Order;
                }
            }
            context.SaveChanges();

        }
        public void SaveWiringStepForJob(WiringStepForJob wiringStepForJob)
        {
            if (wiringStepForJob.WiringStepForJobID == 0)
            {
                context.WiringStepsForJobs.Add(wiringStepForJob);
            }
            else
            {
                WiringStepForJob dbEntry = context.WiringStepsForJobs
                .FirstOrDefault(p => p.WiringStepForJobID == wiringStepForJob.WiringStepForJobID);
                if (dbEntry != null)
                {
                    dbEntry.WiringStepID = wiringStepForJob.WiringStepID;
                    dbEntry.WiringID = wiringStepForJob.WiringID;
                    dbEntry.Start = wiringStepForJob.Start;
                    dbEntry.Stop = wiringStepForJob.Stop;
                    dbEntry.Elapsed = wiringStepForJob.Elapsed;
                    dbEntry.Complete = wiringStepForJob.Complete;
                    dbEntry.Consecutivo = wiringStepForJob.Consecutivo;
                    dbEntry.AuxWirerID = wiringStepForJob.AuxWirerID;
                    dbEntry.AuxStationID = wiringStepForJob.AuxStationID;
                    dbEntry.Obsolete = wiringStepForJob.Obsolete;
                }
            }
            context.SaveChanges();

        } 
        public void SaveWiringReason1(WiringReason1 wiringReason1)
        {
            if (wiringReason1.WiringReason1ID == 0)
            {
                context.WiringReason1s.Add(wiringReason1);
            }
            else
            {
                WiringReason1 dbEntry = context.WiringReason1s
                .FirstOrDefault(p => p.WiringReason1ID == wiringReason1.WiringReason1ID);
                if (dbEntry != null)
                {
                    dbEntry.Description = wiringReason1.Description;
                }
            }
            context.SaveChanges();

        }
        public void SaveWiringReason2(WiringReason2 wiringReason2)
        {
            if (wiringReason2.WiringReason2ID == 0)
            {
                context.WiringReason2s.Add(wiringReason2);
            }
            else
            {
                WiringReason2 dbEntry = context.WiringReason2s
                .FirstOrDefault(p => p.WiringReason2ID == wiringReason2.WiringReason2ID);
                if (dbEntry != null)
                {
                    dbEntry.WiringReason1ID = wiringReason2.WiringReason1ID;
                    dbEntry.Description = wiringReason2.Description;
                }
            }
            context.SaveChanges();
        }
        public void SaveWiringReason3(WiringReason3 wiringReason3)
        {
            if (wiringReason3.WiringReason3ID == 0)
            {
                context.WiringReason3s.Add(wiringReason3);
            }
            else
            {
                WiringReason3 dbEntry = context.WiringReason3s
                .FirstOrDefault(p => p.WiringReason3ID == wiringReason3.WiringReason3ID);
                if (dbEntry != null)
                {
                    dbEntry.WiringReason2ID = wiringReason3.WiringReason2ID;
                    dbEntry.Description = wiringReason3.Description;
                }
            }
            context.SaveChanges();
        }
        public void SaveWiringReason4(WiringReason4 reason4)
        {
            if (reason4.WiringReason4ID == 0)
            {
                context.WiringReason4s.Add(reason4);
            }
            else
            {
                WiringReason4 dbEntry = context.WiringReason4s
                .FirstOrDefault(p => p.WiringReason4ID == reason4.WiringReason4ID);
                if (dbEntry != null)
                {
                    dbEntry.WiringReason3ID = reason4.WiringReason3ID;
                    dbEntry.Description = reason4.Description;
                }
            }
            context.SaveChanges();

        }
        public void SaveWiringReason5(WiringReason5 reason5)
        {
            if (reason5.WiringReason5ID == 0)
            {
                context.WiringReason5s.Add(reason5);
            }
            else
            {
                WiringReason5 dbEntry = context.WiringReason5s
                .FirstOrDefault(p => p.WiringReason5ID == reason5.WiringReason5ID);
                if (dbEntry != null)
                {
                    dbEntry.WiringReason4ID = reason5.WiringReason4ID;
                    dbEntry.Description = reason5.Description;
                }
            }
            context.SaveChanges();
        }
        public void SaveWiringOption(WiringOption wiringOption)
        {
            if (wiringOption.WiringOptionID == 0)
            {
                context.WiringOptions.Add(wiringOption);
            }
            else
            {
                WiringOption dbEntry = context.WiringOptions
                .FirstOrDefault(p => p.WiringOptionID == wiringOption.WiringOptionID);
                if (dbEntry != null)
                {
                    dbEntry.Description = wiringOption.Description;
                    dbEntry.isDeleted = wiringOption.isDeleted;
                }
            }
            context.SaveChanges();
        }
        public void SaveWiringFeatures(WiringFeatures wiringFeatures)
        {
            if (wiringFeatures.WiringFeaturesID == 0)
            {
                context.WiringFeatures.Add(wiringFeatures);
            }
            else
            {
                WiringFeatures dbEntry = context.WiringFeatures
                .FirstOrDefault(p => p.WiringOptionID == wiringFeatures.WiringOptionID);
                if (dbEntry != null)
                {
                    dbEntry.WiringID = wiringFeatures.WiringID;
                    dbEntry.WiringOptionID = wiringFeatures.WiringOptionID;
                    dbEntry.Quantity = wiringFeatures.Quantity;
                }
            }
            context.SaveChanges();
        }
        public void SaveWiringTriggeringFeature(WiringTriggeringFeature wiringTriggeringFeature)
        {
            if (wiringTriggeringFeature.WiringTriggeringFeatureID == 0)
            {
                context.WiringTriggeringFeatures.Add(wiringTriggeringFeature);
            }
            else
            {
                WiringTriggeringFeature dbEntry = context.WiringTriggeringFeatures
                .FirstOrDefault(p => p.WiringTriggeringFeatureID == wiringTriggeringFeature.WiringTriggeringFeatureID);
                if (dbEntry != null)
                {
                    dbEntry.WiringStepID = wiringTriggeringFeature.WiringStepID;
                    dbEntry.WiringOptionID = wiringTriggeringFeature.WiringOptionID;
                    dbEntry.Quantity = wiringTriggeringFeature.Quantity;
                    dbEntry.Equality = wiringTriggeringFeature.Equality;
                }
            }
            context.SaveChanges();
        }

        public Wiring DeleteWiring(int WiringID)
        {
            Wiring dbEntry = context.Wirings
                .FirstOrDefault(p => p.WiringID == WiringID);
            if (dbEntry != null)
            {
                context.Wirings.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public WirersInvolved DeleteWirersInvolved(int WirersInvolvedID)
        {
            WirersInvolved dbEntry = context.WirersInvolveds
                .FirstOrDefault(p => p.WirersInvolvedID == WirersInvolvedID);
            if (dbEntry != null)
            {
                context.WirersInvolveds.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public WiringStop DeleteWiringStop(int WiringStopID)
        {
            WiringStop dbEntry = context.WiringStops
                .FirstOrDefault(p => p.WiringStopID == WiringStopID);
            if (dbEntry != null)
            {
                context.WiringStops.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public WiringStep DeleteWiringStep(int WiringStepID)
        {
            WiringStep dbEntry = context.WiringSteps
                .FirstOrDefault(p => p.WiringStepID == WiringStepID);
            if (dbEntry != null)
            {
                context.WiringSteps.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public WiringStepForJob DeleteWiringStepForJob(int WiringStepForJobID)
        {
            WiringStepForJob dbEntry = context.WiringStepsForJobs
                .FirstOrDefault(p => p.WiringStepForJobID == WiringStepForJobID);
            if (dbEntry != null)
            {
                context.WiringStepsForJobs.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public WiringReason1 DeleteWiringReason1(int WiringReason1ID)
        {
            WiringReason1 dbEntry = context.WiringReason1s
                .FirstOrDefault(p => p.WiringReason1ID == WiringReason1ID);
            if (dbEntry != null)
            {
                context.WiringReason1s.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public WiringReason2 DeleteWiringReason2(int WiringReason2ID)
        {
            WiringReason2 dbEntry = context.WiringReason2s
                .FirstOrDefault(p => p.WiringReason2ID == WiringReason2ID);
            if (dbEntry != null)
            {
                context.WiringReason2s.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public WiringReason3 DeleteWiringReason3(int WiringReason3ID)
        {
            WiringReason3 dbEntry = context.WiringReason3s
                .FirstOrDefault(p => p.WiringReason3ID == WiringReason3ID);
            if (dbEntry != null)
            {
                context.WiringReason3s.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public WiringReason4 DeleteWiringReason4(int WiringReason4ID)
        {
            WiringReason4 dbEntry = context.WiringReason4s
                .FirstOrDefault(p => p.WiringReason4ID == WiringReason4ID);
            if (dbEntry != null)
            {
                context.WiringReason4s.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public WiringReason5 DeleteWiringReason5(int WiringReason5ID)
        {
            WiringReason5 dbEntry = context.WiringReason5s
                .FirstOrDefault(p => p.WiringReason5ID == WiringReason5ID);
            if (dbEntry != null)
            {
                context.WiringReason5s.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public WiringOption DeleteWiringOption(int WiringOptionID)
        {
            WiringOption dbEntry = context.WiringOptions
                .FirstOrDefault(p => p.WiringOptionID == WiringOptionID);
            if (dbEntry != null)
            {
                context.WiringOptions.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public WiringFeatures DeleteWiringFeatures(int WiringFeaturesID)
        {
            WiringFeatures dbEntry = context.WiringFeatures
                .FirstOrDefault(p => p.WiringFeaturesID == WiringFeaturesID);
            if (dbEntry != null)
            {
                context.WiringFeatures.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public WiringTriggeringFeature DeleteWiringTriggeringFeature(int WiringTriggeringFeatureID)
        {
            WiringTriggeringFeature dbEntry = context.WiringTriggeringFeatures
                .FirstOrDefault(p => p.WiringTriggeringFeatureID == WiringTriggeringFeatureID);
            if (dbEntry != null)
            {
                context.WiringTriggeringFeatures.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }


        //PXP
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
                    dbEntry.POID = wiringPXP.POID;
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
