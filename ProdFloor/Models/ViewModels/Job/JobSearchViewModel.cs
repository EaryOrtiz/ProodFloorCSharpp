﻿using Microsoft.AspNetCore.Mvc.Rendering;
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
        //Atributos de Job
        public string NameJobSearch { get; set; }
        public int NumJobSearch { get; set; }
        public int POJobSearch { get; set; }
        public string CustJobSearch { get; set; }
        public string ContractorJobSearch { get; set; }
        public int EngID { get; set; }
        public int CrossAppEngID { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public int CityID { get; set; }
        public int FireCodeID { get; set; }
        public int JobTypeID { get; set; }
        public SelectList Status;
        public string StatusJobSearch { get; set; }

        //Atributos de JobExtensions
        public string AuxCop { get; set; }
        public string CartopDoorButtons { get; set; }
        public string DoorHold { get; set; }
        public string HeavyDoors { get; set; }
        public string InfDetector { get; set; }
        public string JobTypeAdd { get; set; }
        public string JobTypeMain { get; set; }
        public string Scop { get; set; }
        public string Shc { get; set; }
        public string MechSafEdge { get; set; }
        public int InputFrecuency { get; set; }
        public int InputPhase { get; set; }
        public int InputVoltage { get; set; }
        public int NumOfStops { get; set; }
    }
}
