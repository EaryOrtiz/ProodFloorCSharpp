using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Job
{
    public class JobSearchViewModel
    {
        public List<Models.Job> JobsSearchList { get; set; }
        public List<JobExtension> JobExtensionList { get; set; }
        public List<HydroSpecific> HydroSpecificList { get; set; }
        public List<GenericFeatures> GenericFeaturesList { get; set; }
        public List<Indicator> IndicatorList { get; set; }
        public List<HoistWayData> HoistWayDataList { get; set; }
        public List<SpecialFeatures> SpecialFeatureslist { get; set; }
        public PagingInfo PagingInfo { get; set; }

        public string NameJobSearch { get; set; }
        public int NumJobSearch { get; set; }
        public int POJobSearch { get; set; }
        public string CustJobSearch { get; set; }
        public string ContractorJobSearch { get; set; }
        public int EngID { get; set; }
        public int CrossAppEngID { get; set; }
        public int CityID { get; set; }
        public int FireCodeID { get; set; }
        public int JobTypeID { get; set; }

        public SelectList Status;
        public string StatusJobSearch { get; set; }
    }
}
