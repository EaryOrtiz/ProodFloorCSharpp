﻿using System.Collections.Generic;
using System;
using ProdFloor.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

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
        public JobAdditional CurrentJobAdditional { get; set; }
        //public SpecialFeatures CurrentSpecialFeatures { get; set; }
        public List <JobAdditional> JobAdditionalsList { get; set; }
        public List<SpecialFeatures> SpecialFeatureslist { get; set; }
        public List<SpecialFeaturesEX> SpecialFeaturesTable { get; set; }

        public List<PO> POList { get; set; }
        public string CurrentTab { get; set; }
        public string buttonAction { get; set; }
        public int CurrentUserID { get; set; }
        public int fieldID { get; set; }
        public string JobTypeName { get; set; }
        public string JobFolder { get; set; }
    }
}
