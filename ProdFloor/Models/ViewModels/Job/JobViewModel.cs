﻿using System.Collections.Generic;
using System;
using ProdFloor.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProdFloor.Models.ViewModels
{
    public class JobViewModel
    {
        public Job CurrentJob { get; set; }
        public JobExtension CurrentJobExtension { get; set; }
        public HydroSpecific CurrentHydroSpecific { get; set; }
        public GenericFeatures CurrentGenericFeatures { get; set; }
        public Indicator CurrentIndicator { get; set; }
        public HoistWayData CurrentHoistWayData { get; set; }
        public string CurrentTab { get; set; }
    }
}
