using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Job
{
    public class ElementSearchViewModel
    {
        public List<Models.Job> JobsSearchList { get; set; }
        public List<Element> ElementList { get; set; }
        public List<ElementHydro> ElementTractionList { get; set; }
        public List<ElementHydro> ElementHydroList { get; set; }
        public List<SpecialFeatures> SpecialFeatureslist { get; set; }
        public List<JobType> JobTypelist { get; set; }
        public List<City> Citylist { get; set; }
        public List<State> Statelist { get; set; }
        public List<LandingSystem> Landinglist { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public bool CleanFields { get; set; }
        public int LastSearch { get; set; }

        public string JobTypeName { get; set; }

        //Atributos de Job
        [Display(Name ="Job Name")]
        public string NameJobSearch { get; set; }
        [Display(Name = "Job Number")]
        public int NumJobSearch { get; set; }
        [Display(Name = "PO Number")]
        public int POJobSearch { get; set; }
        [Display(Name = "Cust Number")]
        public string CustJobSearch { get; set; }
        [Display(Name = "Contractor")]
        public string ContractorJobSearch { get; set; }

        public int EngID { get; set; }
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

        //Atributos de JobExtensions

        public int DoorOperatorID { get;  set; }
        [Display(Name = "Car Gate")]
        public string DoorGate { get; set; }
        [Display(Name = "Door Brand")]
        public string DoorBrand { get; set; }

        //Atributos de HydroSpecifics

        public string LOS { get; set; }
        [Display(Name = "Life Jacket")]
        public string PSS { get; set; }
        [Display(Name = "Resync operation")]
        public string Starter { get; set; }
        [Display(Name = "Viscosity Control")]
        public string VCI { get; set; }
        [Display(Name = "Valve's Brand")]
        public string ValveBrand { get; set; }

        public int FLA { get; set; }
        public int HP { get; set; }
        [Display(Name = "Number of motors")]
        public int SPH { get; set; }

        //Atributos de GenericFeatureList
        [Display(Name = "Security")]
        public string CallEnable { get; set; }
        [Display(Name = "Car to lobby")]
        public string CarToLobby { get; set; }
        public string EMT { get; set; }
        [Display(Name = "Emergency Power")]
        public string EP { get; set; }
        [Display(Name = "Earthquake")]
        public string EQ { get; set; }
        [Display(Name = "Additional Fire Recall")]
        public string FRON2 { get; set; }
        [Display(Name = "Hospital")]
        public string INA { get; set; }
        [Display(Name = "In-Car Inspection")]
        public string INCP { get; set; }
        public string LoadWeigher { get; set; }
        [Display(Name = "CRO")]
        public string CRO { get; set; }
        [Display(Name = "Car Key")]
        public string CarKey { get; set; }
        public string HCRO { get; set; }
        [Display(Name = "Hall Key")]
        public string HallKey { get; set; }
        [Display(Name = "Bottom Access Switch")]
        public string CTINSPST { get; set; }
        [Display(Name = "EP contact during normal power?")]

        public int Capacity { get; set; }
        [Display(Name = "Landing System")]
        public int LandingSystemID { get; set; }
        public string  CarCardReader { get; set; }
        public string  HallCardReader { get; set; }

        public string  HAPS { get; set; }
        public string  PTFLD { get; set; }
        public string  CReg { get; set; }
        public string Egress { get; set; }
        public string PHECutOut { get; set; }
        public string Traveler { get; set; }
        public string  PFGE { get; set; }

        [Display(Name = "Car To Lobby")]
        public string  CTL { get; set; }

        [Display(Name = "Car Shutdown")]
        public string  CSD { get; set; }

        [Display(Name = "Car to Floor")]
        public string  CTF { get; set; }

        [Display(Name = "Life Jacket")]
        public string LJ { get; set; }

        [Display(Name = "Door Hold")]
        public string DHLD { get; set; }

        public int Speed { get; set; }
        public int Voltage { get; set; }
        public int Phase { get; set; }
        public int Frequency { get; set; }



        public string MachineLocation { get; set; }
        public string VVVF { get; set; }
        public string MotorBrand { get; set; }
        public string Contact { get; set; }

        public string Encoder { get; set; }
        public string ISO { get; set; }

        public int PickVoltage { get; set; }
        public int HoldVoltage { get; set; }
        public int Resistance { get; set; }
        public float Current { get; set; }

        //Atributos de Special Features
        public string Description { get; set; }

    }
}
