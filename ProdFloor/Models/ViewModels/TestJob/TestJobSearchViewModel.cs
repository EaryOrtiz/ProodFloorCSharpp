﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.TestJob
{
    public class TestJobSearchViewModel
    {
        public List<Models.Job> JobsSearchList { get; set; }
        public List<Models.Job> jobListAux { get; set; }
        public List<Models.TestJob> TestJobsSearchList { get; set; }
        public List<Stop> StopList { get; set; }
        public List<Station> StationsList { get; set; }
        public List<JobType> JobTypeList { get; set; }
        public Models.Job Job { get; set; }
        public JobExtension JobExtension { get; set; }
        public HydroSpecific HydroSpecific { get; set; }
        public HoistWayData HoistWayData { get; set; }
        public GenericFeatures GenericFeatures { get; set; }

        public List<Reason1> Reasons1List { get; set; }
        public List<Reason2> Reasons2List { get; set; }
        public List<Reason3> Reasons3List { get; set; }
        public List<Reason4> Reasons4List { get; set; }
        public List<Reason5> Reasons5List { get; set; }
        public string Reason1Name { get; set; }

        public Models.TestJob TestJob { get; set; }
        public Stop Stop { get; set; }

        public PagingInfo PagingInfo { get; set; }
        public bool CleanFields { get; set; }

        //Dummy fields
        public string JobName { get; set; }

        public string JobNum { get; set; }
        //Aux fields for New JobNumber
        public string JobNumFirstDigits { get; set; }
        public int JobNumLastDigits { get; set; }


        public string Canada { get; set; }
        public string Ontario { get; set; }
        public string MOD { get; set; }
        public string Manual { get; set; }
        public string IMonitor { get; set; }
        public string HAPS { get; set; }
        public string Duplex { get; set; }
        public string SHC { get; set; }
        public string EDGELS { get; set; }
        public string RailLS { get; set; }
        public string MView { get; set; }
        [Display(Name = "+2 Starters")]
        public string TwosStarters { get; set; }
        public string Critical { get; set; }

        //Atributos de Features
        public string Overlay { get; set; }
        public string Group { get; set; }
        [Display(Name = "PC de Cliente")]
        public string PC { get; set; }
        [Display(Name = "Brake Coil Voltage > 10")]
        public string BrakeCoilVoltageMoreThan10 { get; set; }
        [Display(Name = "EMBrake Module")]
        public string EMBrake { get; set; }
        [Display(Name = "EMCO Board")]
        public string EMCO { get; set; }
        [Display(Name = "R6 Regen Unit")]
        public string R6 { get; set; }
        public string Local { get; set; }
        [Display(Name = "Short Floor")]
        public string ShortFloor { get; set; }
        public string Custom { get; set; }
        public string MRL { get; set; }
        public string CTL2 { get; set; }
        [Display(Name = "Tarjeta CPI Incluida")]
        public string TrajetaCPI { get; set; }
        [Display(Name = "Door Control en Cartop")]
        public string Cartop { get; set; }


        [Display(Name = "Shift End")]
        public bool WithShiftEnd { get; set; }
        [Display(Name = "Job Reassignment ")]
        public bool WithReassignment { get; set; }
        [Display(Name = "Returned From Complete ")]
        public bool WithReturnedFromComplete { get; set; }

        public int TotalOnDB { get; set; }

    }
}
