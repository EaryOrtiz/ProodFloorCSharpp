using System.Linq;
using ProdFloor.Models.ViewModels;

namespace ProdFloor.Models
{
    public interface IItemRepository
    {
        IQueryable<Country> Countries { get; }

        IQueryable<State> States { get; }

        IQueryable<City> Cities { get; }

        IQueryable<DoorOperator> DoorOperators { get; }

        IQueryable<FireCode> FireCodes { get; }

        IQueryable<JobType> JobTypes { get; }

        IQueryable<LandingSystem> LandingSystems { get; }

        void SaveCountry(Country country);
        void SaveState(State state);
        void SaveCity(City city);
        void SaveDoorOperator(DoorOperator doorOperator);
        void SaveFireCode(FireCode fireCode);
        void SaveJobType(JobType jobType);
        void SaveLandingSystem(LandingSystem landingSystem);

        Country DeleteCountry(int CountryID);
        State DeleteState(int StateID);
        City DeleteCity(int CityID);
        DoorOperator DeleteDoorOperator(int DoorOperatorID);
        FireCode DeleteFireCode(int FireCodeID);
        JobType DeleteJobType(int JobTypeID);
        LandingSystem DeleteLandingSystem(int LandingSystemID);
    }
}
