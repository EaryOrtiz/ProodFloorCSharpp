using System.Collections.Generic;
using System;
using ProdFloor.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProdFloor.Models.ViewModels
{
    public class JobViewModel
    {
        public Models.Job CurrentJob { get; set; }
        public JobExtension CurrentJobExtension { get; set; }
        public HydroSpecific CurrentHydroSpecific { get; set; }
        public GenericFeatures CurrentGenericFeatures { get; set; }
        public Indicator CurrentIndicator { get; set; }
        public HoistWayData CurrentHoistWayData { get; set; }
        //public SpecialFeatures CurrentSpecialFeatures { get; set; }
        public List<SpecialFeatures> SpecialFeatureslist { get; set; }
        public string CurrentTab { get; set; }
        public string buttonAction { get; set; }
    }
}
