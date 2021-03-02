using System.Collections.Generic;
using System.Linq;
using ProdFloor.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ProdFloor.Models
{
    public class EFItemRepository:IItemRepository
    {
        private ApplicationDbContext context;

        public EFItemRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<Country> Countries => context.Countries;
        public IQueryable<State> States => context.States;
        public IQueryable<City> Cities => context.Cities;
        public IQueryable<DoorOperator> DoorOperators => context.DoorOperators;
        public IQueryable<FireCode> FireCodes => context.FireCodes;
        public IQueryable<JobType> JobTypes => context.JobTypes;
        public IQueryable<LandingSystem> LandingSystems => context.LandingSystems;
        public IQueryable<PlanningReport> PlanningReports => context.PlanningReports;
        public IQueryable<PlanningReportRow> PlanningReportRows => context.PlanningReportRows;

        public void SaveCountry(Country country)
        {
            if (country.CountryID == 0)
            {
                context.Countries.Add(country);
            }
            else
            {
                Country dbEntry = context.Countries
                .FirstOrDefault(p => p.CountryID == country.CountryID);
                if (dbEntry != null)
                {
                    dbEntry.Name = country.Name;
                }
            }
            context.SaveChanges();

        }

        public void SaveState(State state)
        {
            if (state.StateID == 0)
            {
                context.States.Add(state);
            }
            else
            {
                State dbEntry = context.States
                .FirstOrDefault(p => p.StateID == state.StateID);
                if (dbEntry != null)
                {
                    dbEntry.CountryID = state.CountryID;
                    dbEntry.Name = state.Name;
                }
            }
            context.SaveChanges();
        }

        public void SaveCity(City city)
        {
            if (city.CityID == 0)
            {
                context.Cities.Add(city);
            }
            else
            {
                City dbEntry = context.Cities
                .FirstOrDefault(p => p.CityID == city.CityID);
                if (dbEntry != null)
                {
                    dbEntry.Name = city.Name;
                    dbEntry.StateID = city.StateID;
                    dbEntry.FirecodeID = city.FirecodeID;
                }
            }
            context.SaveChanges();
        }

        public void SaveDoorOperator(DoorOperator doorOperator)
        {
            if (doorOperator.DoorOperatorID == 0)
            {
                context.DoorOperators.Add(doorOperator);
            }
            else
            {
                DoorOperator dbEntry = context.DoorOperators
                .FirstOrDefault(p => p.DoorOperatorID == doorOperator.DoorOperatorID);
                if (dbEntry != null)
                {
                    dbEntry.Name = doorOperator.Name;
                    dbEntry.Style = doorOperator.Style;
                    dbEntry.Brand = doorOperator.Brand;
                }
            }
            context.SaveChanges();
        }

        public void SaveFireCode(FireCode fireCode)
        {
            if (fireCode.FireCodeID == 0)
            {
                context.FireCodes.Add(fireCode);
            }
            else
            {
                FireCode dbEntry = context.FireCodes
                .FirstOrDefault(p => p.FireCodeID == fireCode.FireCodeID);
                if (dbEntry != null)
                {
                    dbEntry.Name = fireCode.Name;
                }
            }
            context.SaveChanges();
        }

        public void SaveJobType(JobType jobType)
        {
            if (jobType.JobTypeID == 0)
            {
                context.JobTypes.Add(jobType);
            }
            else
            {
                JobType dbEntry = context.JobTypes
                .FirstOrDefault(p => p.JobTypeID == jobType.JobTypeID);
                if (dbEntry != null)
                {
                    dbEntry.Name = jobType.Name;
                }
            }
            context.SaveChanges();
        }

        public void SaveLandingSystem(LandingSystem landingSystem)
        {
            if (landingSystem.LandingSystemID == 0)
            {
                context.LandingSystems.Add(landingSystem);
            }
            else
            {
                LandingSystem dbEntry = context.LandingSystems
                .FirstOrDefault(p => p.LandingSystemID == landingSystem.LandingSystemID);
                if (dbEntry != null)
                {
                    dbEntry.Name = landingSystem.Name;
                    dbEntry.UsedIn = landingSystem.UsedIn;
                }
            }
            context.SaveChanges();
        }

        public void SavePlanningReport(PlanningReport planningReport)
        {
            if (planningReport.PlanningReportID == 0)
            {
                context.PlanningReports.Add(planningReport);
            }
            else
            {
                PlanningReport dbEntry = context.PlanningReports
                .FirstOrDefault(p => p.PlanningReportID == planningReport.PlanningReportID);
                if (dbEntry != null)
                {
                    dbEntry.PlanningDate = planningReport.PlanningDate;
                    dbEntry.DateTimeLoad = planningReport.DateTimeLoad;
                    dbEntry.Busy = planningReport.Busy;

                }
            }
            context.SaveChanges();
        }

        public void SavePlanningReportRow(PlanningReportRow planningReportRow)
        {
            if (planningReportRow.PlanningReportRowID == 0)
            {
                context.PlanningReportRows.Add(planningReportRow);
            }
            else
            {
                PlanningReportRow dbEntry = context.PlanningReportRows
                .FirstOrDefault(p => p.PlanningReportID == planningReportRow.PlanningReportID);
                if (dbEntry != null)
                {
                    dbEntry.PlanningReportID = planningReportRow.PlanningReportID;
                    dbEntry.Consecutive = planningReportRow.Consecutive;
                    dbEntry.JobNumber = planningReportRow.JobNumber;
                    dbEntry.PO = planningReportRow.PO;
                    dbEntry.JobName = planningReportRow.JobName;
                    dbEntry.PreviousWorkCenter = planningReportRow.PreviousWorkCenter;
                    dbEntry.WorkCenter = planningReportRow.WorkCenter;
                    dbEntry.Notes = planningReportRow.Notes;
                    dbEntry.Priority = planningReportRow.Priority;
                    dbEntry.Material = planningReportRow.Material;
                    dbEntry.MRP = planningReportRow.MRP;
                    dbEntry.SoldTo = planningReportRow.SoldTo;

                }
            }
            context.SaveChanges();
        }

        public Country DeleteCountry(int CountryID)
        {
            Country dbEntry = context.Countries
                .FirstOrDefault(p => p.CountryID == CountryID);
            if (dbEntry != null)
            {
                context.Countries.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public State DeleteState(int StateID)
        {
            State dbEntry = context.States
                .FirstOrDefault(p => p.StateID == StateID);
            if (dbEntry != null)
            {
                context.States.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public City DeleteCity(int CityID)
        {
            City dbEntry = context.Cities
                .FirstOrDefault(p => p.CityID == CityID);
            if (dbEntry != null)
            {
                context.Cities.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public DoorOperator DeleteDoorOperator(int DoorOperatorID)
        {
            DoorOperator dbEntry = context.DoorOperators
                .FirstOrDefault(p => p.DoorOperatorID == DoorOperatorID);
            if (dbEntry != null)
            {
                context.DoorOperators.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public FireCode DeleteFireCode(int FireCodeID)
        {
            FireCode dbEntry = context.FireCodes
                .FirstOrDefault(p => p.FireCodeID == FireCodeID);
            if (dbEntry != null)
            {
                context.FireCodes.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public JobType DeleteJobType(int JobTypeID)
        {
            JobType dbEntry = context.JobTypes
                .FirstOrDefault(p => p.JobTypeID == JobTypeID);
            if (dbEntry != null)
            {
                context.JobTypes.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public LandingSystem DeleteLandingSystem(int LandingSystemID)
        {
            LandingSystem dbEntry = context.LandingSystems
                .FirstOrDefault(p => p.LandingSystemID == LandingSystemID);
            if (dbEntry != null)
            {
                context.LandingSystems.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public PlanningReport DeletePlanningReport(int PlanningReportID)
        {
            PlanningReport dbEntry = context.PlanningReports
                .FirstOrDefault(p => p.PlanningReportID == PlanningReportID);
            if (dbEntry != null)
            {
                context.PlanningReports.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public PlanningReportRow DeletePlanningReportRow(int PlanningReportRowID)
        {
            PlanningReportRow dbEntry = context.PlanningReportRows
                .FirstOrDefault(p => p.PlanningReportID == PlanningReportRowID);
            if (dbEntry != null)
            {
                context.PlanningReportRows.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public bool DeleteAllPlanningRowsTable()
        {
            IQueryable<PlanningReportRow> dbEntries = context.PlanningReportRows;

            context.PlanningReportRows.RemoveRange(dbEntries);

            bool isTableDeleted = !context.PlanningReports.Any();

            return isTableDeleted;
        }


        //********Enginner Refernces Table

        public IQueryable<Slowdown> Slowdowns => context.Slowdowns;
        public IQueryable<WireTypesSize> WireTypesSizes => context.WireTypesSizes;
        public IQueryable<Starter> Starters => context.Starters;
        public IQueryable<Overload> Ovearloads => context.Overloads;

        public void SaveSlowdown(Slowdown slowdown)
        {
            if (slowdown.SlowdownID == 0)
            {
                context.Slowdowns.Add(slowdown);
            }
            else
            {
                Slowdown dbEntry = context.Slowdowns
                .FirstOrDefault(p => p.SlowdownID == slowdown.SlowdownID);
                if (dbEntry != null)
                {
                    dbEntry.CarSpeedFPM = slowdown.CarSpeedFPM;
                    dbEntry.Distance = slowdown.Distance;
                    dbEntry.A = slowdown.A;
                    dbEntry.SlowLimit = slowdown.SlowLimit;
                    dbEntry.MiniumFloorHeight = slowdown.MiniumFloorHeight;
                }
            }
            context.SaveChanges();
        }

        public void SaveWireTypesSize(WireTypesSize wireTypesSize)
        {
            if (wireTypesSize.WireTypesSizeID == 0)
            {
                context.WireTypesSizes.Add(wireTypesSize);
            }
            else
            {
                WireTypesSize dbEntry = context.WireTypesSizes
                .FirstOrDefault(p => p.WireTypesSizeID == wireTypesSize.WireTypesSizeID);
                if (dbEntry != null)
                {
                    dbEntry.Type = wireTypesSize.Type;
                    dbEntry.Size = wireTypesSize.Size;
                    dbEntry.AMPRating = wireTypesSize.AMPRating;
                }
            }
            context.SaveChanges();
        }

        public void SaveStarter(Starter starter)
        {
            if (starter.StarterID == 0)
            {
                context.Starters.Add(starter);
            }
            else
            {
                Starter dbEntry = context.Starters
                .FirstOrDefault(p => p.StarterID == starter.StarterID);
                if (dbEntry != null)
                {
                    dbEntry.FLA = starter.FLA;
                    dbEntry.StarterType = starter.StarterType;
                    dbEntry.StarterType = starter.StarterType;
                    dbEntry.FLA = starter.FLA;
                    dbEntry.Volts = starter.Volts;
                    dbEntry.HP = starter.HP;
                    dbEntry.MCPart = starter.MCPart;
                    dbEntry.NewManufacturerPart = starter.NewManufacturerPart;
                    dbEntry.OverloadTable = starter.OverloadTable;
                }
            }
            context.SaveChanges();
        }

        public void SaveOverload(Overload overload)
        {
            if (overload.OverloadID == 0)
            {
                context.Overloads.Add(overload);
            }
            else
            {
                Overload dbEntry = context.Overloads
                .FirstOrDefault(p => p.OverloadID == overload.OverloadID);
                if (dbEntry != null)
                {
                    dbEntry.AMPMin = overload.AMPMin;
                    dbEntry.AMPMax = overload.AMPMax;
                    dbEntry.OverTableNum = overload.OverTableNum;
                    dbEntry.MCPart = overload.MCPart;
                    dbEntry.SiemensPart = overload.SiemensPart;
                }
            }
            context.SaveChanges();
        }

        public Slowdown DeleteSlowdown(int slowdownID)
        {
            Slowdown dbEntry = context.Slowdowns
                .FirstOrDefault(p => p.SlowdownID == slowdownID);
            if (dbEntry != null)
            {
                context.Slowdowns.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public WireTypesSize DeleteWireTypeSize(int WireTypeSizeID)
        {
            WireTypesSize dbEntry = context.WireTypesSizes
                .FirstOrDefault(p => p.WireTypesSizeID == WireTypeSizeID);
            if (dbEntry != null)
            {
                context.WireTypesSizes.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public Starter DeleteStarter(int StarterID)
        {
            Starter dbEntry = context.Starters
                .FirstOrDefault(p => p.StarterID == StarterID);
            if (dbEntry != null)
            {
                context.Starters.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public Overload DeleteOverload(int OverloadID)
        {
            Overload dbEntry = context.Overloads
                .FirstOrDefault(p => p.OverloadID == OverloadID);
            if (dbEntry != null)
            {
                context.Overloads.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
    }
}
