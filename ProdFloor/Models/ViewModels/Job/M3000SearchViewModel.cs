using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Job
{
    public class M3000SearchViewModel
    {
        public List<Models.Job> JobsSearchList { get; set; }
        public List<M3000> M3000List { get; set; }
        public List<MotorInfo> MotorInfoList { get; set; }
        public List<OperatingFeatures> OperatingFeaturesList { get; set; }
        public List<SpecialFeatures> SpecialFeatureslist { get; set; }
        public List<JobType> JobTypelist { get; set; }
        public List<City> Citylist { get; set; }
        public List<State> Statelist { get; set; }
        public List<LandingSystem> Landinglist { get; set; }

        public List<SpecialFeaturesEX> SpecialFeaturesTable { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public bool CleanFields { get; set; }
        public int LastSearch { get; set; }

        public JobType jobTypeAux { get; set; }

        public string JobTypeName { get; set; }

        //Atributos de Job
        [Display(Name ="Job Name")]
        public string NameJobSearch { get; set; }

        [Display(Name = "Job Number")]
        public string NumJobSearch { get; set; }

        //Aux fields for New JobNumber
        public string JobNumFirstDigits { get; set; }
        public int JobNumLastDigits { get; set; }


        [Display(Name = "PO Number")]
        public int POJobSearch { get; set; }
        [Display(Name = "Cust Number")]
        public string CustJobSearch { get; set; }
        [Display(Name = "Contractor")]
        public string ContractorJobSearch { get; set; }

        public int EngID { get; set; }
        public int CurrentUserEngID { get; set; }
        public int CrossAppEngID { get; set; }

        [Display(Name = "Country")]
        public int CountryID { get; set; }
        [Display(Name = "State")]
        public int StateID { get; set; }
        [Display(Name = "City")]
        public int CityID { get; set; }
        [Display(Name = "FireCode")]
        public int FireCodeID { get; set; }
        [Display(Name = "JobType")]
        public int JobTypeID { get; set; }
        public SelectList Status;
        [Display(Name = "Status")]
        public string StatusJobSearch { get; set; }
        [Display(Name = "Job Name #2")]
        public string Name2 { get; set; }

        //M3000
        [Display(Name = "Input Voltage")]
        public int InputVoltage { get; set; }

        [Display(Name = "Input Phase")]
        public int InputPhase { get; set; }

        [Display(Name = "Input Frequency")]
        public int InputFrecuency { get; set; }

        [Display(Name = "Speed")]
        public int Speed { get; set; }

        [Display(Name = "Length")]
        public int Length { get; set; }

        //bool
        [Display(Name = "EC-RCT")]
        public string ECRCT { get; set; }

        [Display(Name = "Control Type")]
        public string ControlType { get; set; }

        [Display(Name = "NEMA")]
        public string NEMA { get; set; }

        [Display(Name = "Control Panel")]
        public string ControlPanel { get; set; }


        //MotorInfo
        public int Voltage { get; set; }

        public int HP { get; set; }

        public float FLA { get; set; }

        [Display(Name = "Contactor")]
        public string Contactor { get; set; }

        //bool 4152 3135 7582 5174
        [Display(Name = "Main brake")]
        public string MainBrake { get; set; }

        [Display(Name = "Brake Type")]
        public string MainBrakeType { get; set; }

        [Display(Name = "Brake Contact")]
        public string MainBrakeContact { get; set; }
        //bool
        [Display(Name = "Aux Brake")]
        public string AuxBrake { get; set; }

        [Display(Name = "Aux Brake Type")]
        public string AuxBrakeType { get; set; }

        [Display(Name = "Aux Brake Contact")]
        public string AuxBrakeContact { get; set; }



        //OpratingFeatures
        //bool
        [Display(Name = "Tandem Operation")]
        public string TandemOperation { get; set; }
        //bool
        [Display(Name = "Auto Chain Lubrication")]
        public string AutoChainLubrication { get; set; }

        [Display(Name = "Auto Chain Lubrication Voltage")]
        public int AutoChainLubriVoltage { get; set; }

        [Display(Name = "Display Module")]
        public string DisplayModule { get; set; }
        //bool
        [Display(Name = "Smoke Detector")]
        public string SmokeDetector { get; set; }
        //bool
        [Display(Name = "Remote Monitoring")]
        public string RemoteMonitoring { get; set; }

        [Display(Name = "Remote Monitoring Type")]
        public string RemoteMonitoringType { get; set; }
        //bool
        [Display(Name = "Thermistor")]
        public string Thermistor { get; set; }


        //Atributos de Special Features
        public string Description { get; set; }

    }
}
