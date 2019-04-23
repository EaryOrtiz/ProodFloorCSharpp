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
        IQueryable<SpecialFeatures> SpecialFeatures { get; }
        IQueryable<PO> POs { get; }
        IQueryable<CustomSoftware> CustomSoftwares { get;}
        IQueryable<TriggeringCustSoft> TriggeringCustSofts { get;}
        IQueryable<CustomFeature> CustomFeatures { get;}

        void SaveJob(Job job);
        void SaveJobExtension(JobExtension jobExtension);
        void SaveHydroSpecific(HydroSpecific hydroSpecific);
        void SaveGenericFeatures(GenericFeatures genericFeatures);
        void SaveIndicator(Indicator indicator);
        void SaveHoistWayData(HoistWayData hoistWayData);
        void SaveSpecialFeatures(SpecialFeatures specialFeatures);
        void SavePO(PO po);
        void SaveEngJobView(JobViewModel viewModelToSave);
        void SaveCustomSoftware(CustomSoftware customSoftware);
        void SaveTriggeringCustSoft(TriggeringCustSoft triggeringCustSoft);
        void SaveCustomFeature(CustomFeature customFeature);

        Job DeleteJob(int jobID);
        Job DeleteEngJob(int JobID);
        JobExtension DeleteJobExtension(int jobExtensionID);
        HydroSpecific DeleteHydroSpecific(int hydroSpecificID);
        GenericFeatures DeleteGenericFeatures(int genericFeaturesID);
        Indicator DeleteIndicator(int indicatorID);
        HoistWayData DeleteHoistWayData(int hoistWatDataID);
        SpecialFeatures DeleteSpecialFeatures(int specialFeaturesID);
        PO DeletePO(int POID);
        CustomSoftware DeleteCustomSoftware(int CustomSoftwareID);
        TriggeringCustSoft DeleteTriggeringCustSoft(int TriggeringCustSoftID);
        CustomFeature DeleteCustomFeature(int CustomFeatureID);
    }
}
