using ProdFloor.Models.ViewModels.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    public class EFTestingRepository : ITestingRepository
    {
        private ApplicationDbContext context;

        public EFTestingRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<TestJob> TestJobs => context.TestJobs;
        public IQueryable<TestFeature> TestFeatures => context.TestFeatures;
        public IQueryable<Step> Steps => context.Steps;
        public IQueryable<TriggeringFeature> TriggeringFeatures => context.TriggeringFeatures;
        public IQueryable<StepsForJob> StepsForJobs => context.StepsForJobs;
        public IQueryable<Reason1> Reasons1 => context.Reasons1;
        public IQueryable<Reason2> Reasons2 => context.Reasons2;
        public IQueryable<Reason3> Reasons3 => context.Reasons3;
        public IQueryable<Reason4> Reasons4 => context.Reasons4;
        public IQueryable<Reason5> Reasons5 => context.Reasons5;
        public IQueryable<Stop> Stops => context.Stops;

        public void SaveTestJob(TestJob testJob)
        {
            if (testJob.TestJobID == 0)
            {
                context.TestJobs.Add(testJob);
            }
            else
            {
                TestJob dbEntry = context.TestJobs
                .FirstOrDefault(p => p.TestJobID == testJob.TestJobID);
                if (dbEntry != null)
                {
                    dbEntry.JobID = testJob.JobID;
                    dbEntry.TechnicianID = testJob.TechnicianID;
                    dbEntry.Status = testJob.Status;
                }
            }
            context.SaveChanges();

        }
        public void SaveTestFeature(TestFeature testFeature)
        {
            if (testFeature.TestFeatureID == 0)
            {
                context.TestFeatures.Add(testFeature);
            }
            else
            {
                TestFeature dbEntry = context.TestFeatures
                .FirstOrDefault(p => p.TestFeatureID == testFeature.TestFeatureID);
                if (dbEntry != null)
                {
                    dbEntry.TestJobID = testFeature.TestJobID;
                    dbEntry.Overlay = testFeature.Overlay;
                    dbEntry.Group = testFeature.Group;
                    dbEntry.PC = testFeature.PC;
                    dbEntry.BrakeCoilVoltageMoreThan10 = testFeature.BrakeCoilVoltageMoreThan10;
                    dbEntry.MBrake = testFeature.MBrake;
                    dbEntry.EMCO = testFeature.EMCO;
                    dbEntry.R6 = testFeature.R6;
                    dbEntry.Local = testFeature.Local;
                    dbEntry.ShortFloor = testFeature.ShortFloor;
                    dbEntry.Custom = testFeature.Custom;
                    dbEntry.MRL = testFeature.MRL;
                    dbEntry.CTL2 = testFeature.CTL2;
                }
            }
            context.SaveChanges();

        }
        public void SaveStep(Step step)
        {
            if (step.StepID == 0)
            {
                context.Steps.Add(step);
            }
            else
            {
                Step dbEntry = context.Steps
                .FirstOrDefault(p => p.StepID == step.StepID);
                if (dbEntry != null)
                {
                    dbEntry.JobTypeID = step.JobTypeID;
                    dbEntry.Stage = step.Stage;
                    dbEntry.ExpectedTime = step.ExpectedTime;
                    dbEntry.Description = step.Description;
                    dbEntry.Order = step.Order;
                }
            }
            context.SaveChanges();

        }
        public void SaveTriggeringFeature(TriggeringFeature triggering)
        {
            if (triggering != null && triggering.TriggeringFeatureID == 0)
            {
                List<TriggeringFeature> specials = context.TriggeringFeatures.Where(p => p.TriggeringFeatureID == triggering.TriggeringFeatureID).ToList();
                if (specials.Count != 1 || triggering.Name != null)
                {
                    context.TriggeringFeatures.Add(triggering);
                }

            }
            else
            {
                TriggeringFeature dbEntry = context.TriggeringFeatures
                .FirstOrDefault(p => p.TriggeringFeatureID == triggering.TriggeringFeatureID);
                if (dbEntry != null)
                {
                    dbEntry.StepID = triggering.StepID;
                    dbEntry.Name = triggering.Name;
                    dbEntry.IsSelected = triggering.IsSelected;
                }
            }
            context.SaveChanges();

        }
        public void SaveStepsForJob(StepsForJob stepsForJob)
        {
            if (stepsForJob.StepsForJobID == 0)
            {
                context.StepsForJobs.Add(stepsForJob);
            }
            else
            {
                StepsForJob dbEntry = context.StepsForJobs
                .FirstOrDefault(p => p.StepsForJobID == stepsForJob.StepsForJobID);
                if (dbEntry != null)
                {
                    dbEntry.StepID = stepsForJob.StepID;
                    dbEntry.TestJobID = stepsForJob.TestJobID;
                    dbEntry.Start = stepsForJob.Start;
                    dbEntry.Stop = stepsForJob.Stop;
                    dbEntry.Elapsed = stepsForJob.Elapsed;
                    dbEntry.Complete = stepsForJob.Complete;
                    dbEntry.Consecutivo = stepsForJob.Consecutivo;
                }
            }
            context.SaveChanges();

        }
        public void SaveReason1(Reason1 reason1)
        {
            if (reason1.Reason1ID == 0)
            {
                context.Reasons1.Add(reason1);
            }
            else
            {
                Reason1 dbEntry = context.Reasons1
                .FirstOrDefault(p => p.Reason1ID == reason1.Reason1ID);
                if (dbEntry != null)
                {
                    dbEntry.Description = reason1.Description;
                }
            }
            context.SaveChanges();

        }
        public void SaveReason2(Reason2 reason2)
        {
            if (reason2.Reason2ID == 0)
            {
                context.Reasons2.Add(reason2);
            }
            else
            {
                Reason2 dbEntry = context.Reasons2
                .FirstOrDefault(p => p.Reason2ID == reason2.Reason2ID);
                if (dbEntry != null)
                {
                    dbEntry.Reason1ID = reason2.Reason1ID;
                    dbEntry.Description = reason2.Description;
                }
            }
            context.SaveChanges();
        }
        public void SaveReason3(Reason3 reason3)
        {
            if (reason3.Reason3ID == 0)
            {
                context.Reasons3.Add(reason3);
            }
            else
            {
                Reason3 dbEntry = context.Reasons3
                .FirstOrDefault(p => p.Reason3ID == reason3.Reason3ID);
                if (dbEntry != null)
                {
                    dbEntry.Reason2ID = reason3.Reason2ID;
                    dbEntry.Description = reason3.Description;
                }
            }
            context.SaveChanges();
        }
        public void SaveReason4(Reason4 reason4)
        {
            if (reason4.Reason4ID == 0)
            {
                context.Reasons4.Add(reason4);
            }
            else
            {
                Reason4 dbEntry = context.Reasons4
                .FirstOrDefault(p => p.Reason4ID == reason4.Reason4ID);
                if (dbEntry != null)
                {
                    dbEntry.Reason3ID = reason4.Reason3ID;
                    dbEntry.Description = reason4.Description;
                }
            }
            context.SaveChanges();

        }
        public void SaveReason5(Reason5 reason5)
        {
            if (reason5.Reason5ID == 0)
            {
                context.Reasons5.Add(reason5);
            }
            else
            {
                Reason5 dbEntry = context.Reasons5
                .FirstOrDefault(p => p.Reason5ID == reason5.Reason5ID);
                if (dbEntry != null)
                {
                    dbEntry.Reason4ID = reason5.Reason4ID;
                    dbEntry.Description = reason5.Description;
                }
            }
            context.SaveChanges();
        }
        public void SaveStop(Stop stop)
        {
            if (stop.StopID == 0)
            {
                context.Stops.Add(stop);
            }
            else
            {
                Stop dbEntry = context.Stops
                .FirstOrDefault(p => p.StopID == stop.StopID);
                if (dbEntry != null)
                {
                    dbEntry.TestJobID = stop.TestJobID;
                    dbEntry.Reason1ID = stop.Reason1ID;
                    dbEntry.Reason2ID = stop.Reason2ID;
                    dbEntry.Reason3ID = stop.Reason3ID;
                    dbEntry.Reason4ID = stop.Reason4ID;
                    dbEntry.Reason5ID = stop.Reason5ID;
                    dbEntry.Start = stop.Start;
                    dbEntry.StopDate = stop.StopDate;
                    dbEntry.Description = stop.Description;
                }
            }
            context.SaveChanges();

        }

        public TestJob DeleteTestJob(int TestJobID)
        {
            TestJob dbEntry = context.TestJobs
                .FirstOrDefault(p => p.TestJobID == TestJobID);
            if (dbEntry != null)
            {
                context.TestJobs.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public TestFeature DeleteTestFeature(int TestFeatureID)
        {
            TestFeature dbEntry = context.TestFeatures
                .FirstOrDefault(p => p.TestFeatureID == TestFeatureID);
            if (dbEntry != null)
            {
                context.TestFeatures.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public Step DeleteStep(int StepID)
        {
            Step dbEntry = context.Steps
                .FirstOrDefault(p => p.StepID == StepID);
            if (dbEntry != null)
            {
                context.Steps.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public Step DeleteTestStep(int StepID)
        {
            Step dbEntry = context.Steps
                .FirstOrDefault(p => p.StepID == StepID);
            TriggeringFeature triggering = context.TriggeringFeatures
                .FirstOrDefault(p => p.StepID == StepID);
            try
            {
                if (dbEntry != null)
                {
                    context.Steps.Remove(dbEntry);
                    context.SaveChanges();
                }
                if (triggering != null)
                {
                    context.TriggeringFeatures.Remove(triggering);
                    context.SaveChanges();
                }

            }
            catch (Exception e)
            {

            }

            return dbEntry;
        }
        public TriggeringFeature DeleteTriggeringFeature(int TriggeringFeatureID)
        {
            TriggeringFeature dbEntry = context.TriggeringFeatures
                .FirstOrDefault(p => p.TriggeringFeatureID == TriggeringFeatureID);
            if (dbEntry != null)
            {
                context.TriggeringFeatures.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public StepsForJob DeleteStepsForJob(int StepsForJobID)
        {
            StepsForJob dbEntry = context.StepsForJobs
                .FirstOrDefault(p => p.StepsForJobID == StepsForJobID);
            if (dbEntry != null)
            {
                context.StepsForJobs.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public Reason1 DeleteReason1(int Reason1ID)
        {
            Reason1 dbEntry = context.Reasons1
                .FirstOrDefault(p => p.Reason1ID == Reason1ID);
            if (dbEntry != null)
            {
                context.Reasons1.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public Reason2 DeleteReason2(int Reason2ID)
        {
            Reason2 dbEntry = context.Reasons2
                .FirstOrDefault(p => p.Reason2ID == Reason2ID);
            if (dbEntry != null)
            {
                context.Reasons2.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public Reason3 DeleteReason3(int Reason3ID)
        {
            Reason3 dbEntry = context.Reasons3
                .FirstOrDefault(p => p.Reason3ID == Reason3ID);
            if (dbEntry != null)
            {
                context.Reasons3.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public Reason4 DeleteReason4(int Reason4ID)
        {
            Reason4 dbEntry = context.Reasons4
                .FirstOrDefault(p => p.Reason4ID == Reason4ID);
            if (dbEntry != null)
            {
                context.Reasons4.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public Reason5 DeleteReason5(int Reason5ID)
        {
            Reason5 dbEntry = context.Reasons5
                .FirstOrDefault(p => p.Reason5ID == Reason5ID);
            if (dbEntry != null)
            {
                context.Reasons5.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public Stop DeleteStop(int StopID)
        {
            Stop dbEntry = context.Stops
                .FirstOrDefault(p => p.StopID == StopID);
            if (dbEntry != null)
            {
                context.Stops.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public void SaveTestStep(StepViewModel viewModelToSave)
        {
            SaveStep(viewModelToSave.Step);
            if (viewModelToSave.TriggeringList != null)
            {
                for (int i = 0; i < viewModelToSave.TriggeringList.Count; i++)
                {
                    if (viewModelToSave.TriggeringList[i].StepID != 0)
                    {
                        if (viewModelToSave.TriggeringList == null)
                        {
                            SaveTriggeringFeature(viewModelToSave.TriggeringList[i]);
                        }
                        else
                        {
                            viewModelToSave.TriggeringList[i].TriggeringFeatureID = viewModelToSave.TriggeringList[i].TriggeringFeatureID;
                            SaveTriggeringFeature(viewModelToSave.TriggeringList[i]);
                        }
                    }
                }
            }
        }
    }
}
