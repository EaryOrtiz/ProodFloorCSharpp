using ProdFloor.Models.ViewModels;
using System.Linq;

namespace ProdFloor.Models
{
    public interface IJobRepository
    {
        IQueryable<Job> Jobs { get; }
        IQueryable<JobExtension> JobsExtensions { get; }
        IQueryable<HydroSpecific> HydroSpecifics { get; }
        IQueryable<GenericFeatures> GenericFeaturesList { get; }
        IQueryable<Indicator> Indicators { get; }
        IQueryable<HoistWayData> HoistWayDatas { get; }


        void SaveJob(Job job);
        void SaveJobExtension(JobExtension jobExtension);
        void SaveHydroSpecific(HydroSpecific hydroSpecific);
        void SaveGenericFeatures(GenericFeatures genericFeatures);
        void SaveIndicator(Indicator indicator);
        void SaveHoistWayData(HoistWayData hoistWayData);
        void SaveEngJobView(JobViewModel viewModelToSave);

        Job DeleteJob(int jobID);
        Job DeleteEngJob(int JobID);
        JobExtension DeleteJobExtension(int jobExtensionID);
        HydroSpecific DeleteHydroSpecific(int hydroSpecificID);
        GenericFeatures DeleteGenericFeatures(int genericFeaturesID);
        Indicator DeleteIndicator(int indicatorID);
        HoistWayData DeleteHoistWayData(int hoistWatDataID);
    }
}
