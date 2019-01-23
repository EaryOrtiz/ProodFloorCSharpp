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
    }
}
