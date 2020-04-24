using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public List<JobType> JobTypelist { get; set; }
        public List<City> Citylist { get; set; }
        public List<State> Statelist { get; set; }
        public List<LandingSystem> Landinglist { get; set; }

        public List<SpecialFeaturesEX> SpecialFeaturesTable { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public bool CleanFields { get; set; }

        public int LastSearch { get; set; }

        public int JobTotalCount { get; set; }

        public string JobTypeName { get; set; }

        public JobType jobTypeAux { get; set; }

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

        //Atributos de JobExtensions
        [Display(Name = "Auxiliary COP")]
        public string AuxCop { get; set; }
        [Display(Name = "Cartop Door Open/Close Buttons")]
        public string CartopDoorButtons { get; set; }
        [Display(Name = "Door Hold Operation")]
        public string DoorHold { get; set; }
        [Display(Name = "Heavy Doors")]
        public string HeavyDoors { get; set; }
        [Display(Name = "Infrared detector/photo eye")]
        public string InfDetector { get; set; }
        [Display(Name = "Type of operation#2")]
        public string JobTypeAdd { get; set; }
        [Display(Name = "Type of operation")]
        public string JobTypeMain { get; set; }
        [Display(Name = "Serial COP")]
        public string Scop { get; set; }
        [Display(Name = "Serial Hall Calls")]
        public string Shc { get; set; }
        [Display(Name = "Mechanical Safety Edge")]
        public string MechSafEdge { get; set; }
        [Display(Name = "Input Frequency")]
        public int InputFrecuency { get; set; }
        [Display(Name = "Input Phase")]
        public int InputPhase { get; set; }
        [Display(Name = "Input Voltage")]
        public int InputVoltage { get; set; }
        [Display(Name = "Number of Stops")]
        public int NumOfStops { get; set; }
        [Display(Name = "Door Operator")]
        public int DoorOperatorID { get;  set; }
        [Display(Name = "Car Gate")]
        public string DoorGate { get; set; }
        [Display(Name = "Door Brand")]
        public string DoorBrand { get; set; }

        //Atributos de HydroSpecifics
        public String Battery { get; set; }
        [Display(Name = "Battery's Brand")]
        public string BatteryBrand { get; set; }
        [Display(Name = "Low oil sw.")]
        public string LOS { get; set; }
        [Display(Name = "Life Jacket")]
        public string LifeJacket { get; set; }
        [Display(Name = "Oil Cooler")]
        public string OilCool { get; set; }
        [Display(Name = "OTTS")]
        public string OilTank { get; set; }
        [Display(Name = "Pressure sw.")]
        public string PSS { get; set; }
        [Display(Name = "Resync operation")]
        public string Resync { get; set; }
        public string Starter { get; set; }
        [Display(Name = "Viscosity Control")]
        public string VCI { get; set; }
        [Display(Name = "Valve's Brand")]
        public string ValveBrand { get; set; }


        [Display(Name = "Number of motors disconnects")]
        public int MotorsDisconnect { get; set; }

        [Display(Name = "Coils per valve")]
        public int ValveCoils { get; set; }

        [Display(Name = "Number of valves")]
        public int ValveNum { get; set; }

        [Display(Name = "Valve's Voltage")]
        public int ValveVoltage { get; set; }



        public int FLA { get; set; }
        public int HP { get; set; }
        [Display(Name = "Number of motors")]
        public int MotorsNum { get; set; }
        public int SPH { get; set; }

        //Atributos de GenericFeatureList
        [Display(Name = "Attendant Service")]
        public string Attendant { get; set; }
        [Display(Name = "Security")]
        public string CallEnable { get; set; }
        [Display(Name = "Car to lobby")]
        public string CarToLobby { get; set; }
        public string EMT { get; set; }
        [Display(Name = "Emergency Power")]
        public string EP { get; set; }
        [Display(Name = "Earthquake")]
        public string EQ { get; set; }
        [Display(Name = "Fan/Light Timer Option")]
        public string FLO { get; set; }
        [Display(Name = "Additional Fire Recall")]
        public string FRON2 { get; set; }
        [Display(Name = "Hospital")]
        public string Hosp { get; set; }
        [Display(Name = "Generator same as line voltage?")]
        public string EPVoltage { get; set; }
        [Display(Name = "Hoistway Access")]
        public string INA { get; set; }
        [Display(Name = "In-Car Inspection")]
        public string INCP { get; set; }

        [Display(Name = "Pit Flood")]
        public string Pit { get; set; }
        [Display(Name = "Load Weigher")]
        public string LoadWeigher { get; set; }
        [Display(Name = "Top Access Switch")]
        public string TopAccess { get; set; }
        [Display(Name = "Switch Style")]
        public string SwitchStyle { get; set; }
        [Display(Name = "CRO")]
        public string CRO { get; set; }
        [Display(Name = "Car Card Reader")]
        public string CarCallRead { get; set; }
        [Display(Name = "Car Key")]
        public string CarKey { get; set; }
        public string HCRO { get; set; }
        [Display(Name = "Hall Card Reader")]
        public string HallCallRead { get; set; }
        [Display(Name = "Hall Key")]
        public string HallKey { get; set; }
        [Display(Name = "Bottom Access Switch")]
        public string BottomAccess { get; set; }

        [Display(Name = "Bottom Access Switch Location")]
        public string BottomAccessLocation { get; set; }
        [Display(Name = "Cartop Inspection Station")]
        public string CTINSPST { get; set; }
        [Display(Name = "EP contact during normal power?")]
        public string EPContact { get; set; }
        [Display(Name = "Manual Select Switch")]
        public string EPSelect { get; set; }
        [Display(Name = "Roped Hydro Governor Model")]
        public string GovModel { get; set; }
        [Display(Name = "In-Car Inspection Buttons")]
        public string INCPButtons { get; set; }
        public string Monitoring { get; set; }
        [Display(Name = "Number of cars to run at the same time?")]
        public string EPCarsNumber { get; set; }
        [Display(Name = "Power pre transfer contact?")]
        public string PTI { get; set; }
        [Display(Name = "Top Access Switch Location")]
        public string TopAccessLocation { get; set; }

        [Display(Name = "BSI")]
        public String BSI { get; set; }

        [Display(Name = "Car call code security")]
        public string CarCallCodeSecurity { get; set; }


        //Atributos de Indicators
        public string CarLanterns { get; set; }
        public string HallLanterns { get; set; }
        public string HallPI { get; set; }
        public string CarPI { get; set; }
        public string IndicatorsVoltageType { get; set; }
        public string PassingFloor { get; set; }
        public string VoiceAnnunciationPI { get; set; }

        public int IndicatorsVoltage { get; set; }


        public string CarPIType { get; set; } 
        public string CarPIDiscreteType { get; set; } 

        public string HallPIType { get; set; } 
        public string HallPIDiscreteType { get; set; } 

        public string VoiceAnnunciationPIType { get; set; } 

        public string CarLanternsStyle { get; set; } 

        public string CarLanternsType { get; set; } 


        public string HallLanternsStyle { get; set; } 

        public string HallLanternsType { get; set; } 


        public string PassingFloorType { get; set; } 

        public string PassingFloorDiscreteType { get; set; } 

        public string PassingFloorEnable { get; set; } 

        public string HallPIAll { get; set; }

        //Atributos de HoistWayData
        public string AnyRear { get; set; }
        public string IndependentRearOpenings { get; set; }
        public int TopFloor { get; set; }
        public int RearFloorOpenings { get; set; }
        public int FrontFloorOpenings { get; set; }
        public int Capacity { get; set; }
        [Display(Name = "Down Direction Speed")]
        public int DownSpeed { get; set; }
        [Display(Name = "Landing System")]
        public int LandingSystemID { get; set; }
        [Display(Name = "Up Direction Speed")]
        public int UpSpeed { get; set; }
        [Display(Name = "Hoistways Number")]
        public int HoistWaysNumber { get; set; }
        [Display(Name = "Machine Rooms Number")]
        public int MachineRooms  { get; set; }

        //Atributos de Special Features
        public string Description { get; set; }
    }
}
