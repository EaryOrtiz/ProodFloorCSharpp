using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProdFloor.Models
{
    public class Job
    {
        public int JobID { get; set; }

        [StringLength(26, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Status { get; set; }

        public int EngID { get; set; }

        [StringLength(78, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }

        [Display(Name = "Job #")]
        [Range(2015000000, 2021000000, ErrorMessage = "Job number is out of range")]
        [Required(ErrorMessage = "Please enter a Job Num")]
        public int JobNum { get; set; }

        [Display(Name = "PO #")]
        [Range(3000000, 4900000, ErrorMessage = "PO number is out of range")]
        [Required(ErrorMessage = "Please enter a PO")]
        public int PO { get; set; }

        [Display(Name = "Shipping Date")]
        [Required(ErrorMessage = "Please enter a Shipping Date")]
        public DateTime ShipDate { get; set; }

        [Display(Name = "Finish Date")]
        [Required(ErrorMessage = "Please enter a Latest Finish Date")]
        public DateTime LatestFinishDate { get; set; }

        [Display(Name = "Customer ID")]
        [StringLength(10, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter a Customer Number")]
        public string Cust { get; set; }

        [Display(Name = "Contractor Name")]
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter a Contractor")]
        public string Contractor { get; set; }
        /*
        [Display(Name = "Country")]
        [Required(ErrorMessage = "Please enter a Country")]
        public string JobCountry { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please enter a State")]
        public string JobState { get; set; }

        
        [Display(Name = "Job Type")]
        [Required(ErrorMessage = "Please enter a Job Type")]
        public string JobType { get; set; }
            
        [Display(Name = "City")]
        [Required(ErrorMessage = "Please enter a City")]
        public string JobCity { get; set; }

        [Display(Name = "Safety Code")]
        [Required(ErrorMessage = "Please enter a Safety Code")]
        public string SafetyCode { get; set; }

        */

        public JobExtension jobExtension { get; set; }
        public HydroSpecific HydroSpecific { get; set; }
        public GenericFeatures GenericFeatures { get; set; }
        public Indicator Indicator { get; set; }
        public HoistWayData HoistWayData { get; set; }
        public int JobTypeID { get; set; }
        public int CityID { get; set; }
        public int FireCodeID { get; set; }

    }

    public class JobExtension
    {
        public int JobExtensionID { get; set; }
        public int JobID { get; set; }

        [Display(Name = "Number of Stops")]
        [Range(1,32, ErrorMessage = "Stops are out of range")]
        [Required(ErrorMessage = "Please enter the number of stops")]
        public int NumOfStops { get; set; }

        [Display(Name = "Type of operation")]
        [StringLength(26, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter the type of operation")]
        public string JobTypeMain { get; set; }

        [Display(Name = "Type of operation#2")]
        [StringLength(26, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter the type of operation")]
        public string JobTypeAdd { get; set; }

        [Display(Name = "Input Voltage")]
        [Range(114, 600, ErrorMessage = "Voltage out of range")]
        [Required(ErrorMessage = "Please enter the input voltage")]
        public int InputVoltage { get; set; }

        [Range(1, 3, ErrorMessage = "Phase out of range")]
        [Display(Name = "Input Phase")]
        [Required(ErrorMessage = "Please enter the input phase")]
        public int InputPhase { get; set; }

        [Range(50, 61, ErrorMessage = "Frequency out of range")]
        [Display(Name = "Input Frequency")]
        [Required(ErrorMessage = "Please enter the input frequency")]
        public int InputFrecuency { get; set; }

        [Display(Name = "Car Gate")]
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter type of car gate")]
        public string DoorGate { get; set; }

        [Display(Name = "Hoistway Doors")]
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter the type of hoistway gate")]
        public string DoorHoist { get; set; }

        [Display(Name = "Infrared detector/photo eye")]
        public bool InfDetector { get; set; }

        [Display(Name = "Mechanical Safety Edge")]
        public bool MechSafEdge { get; set; }

        [Display(Name = "Heavy Doors")]
        public bool HeavyDoors { get; set; }

        [Display(Name = "Cartop Door Open/Close Buttons")]
        public bool CartopDoorButtons { get; set; }

        [Display(Name = "Door Hold Operation")]
        public bool DoorHold { get; set; }

        [Display(Name = "Nudging")]
        public bool Nudging { get; set; }

        /*[Display(Name = "Door Operator Brand")]
        [Required(ErrorMessage = "Please enter the door operator brand")]
        public string DoorBrand { get; set; }
        
        [Display(Name = "Door Operator Model")]
        [Required(ErrorMessage = "Please enter the door operator model")]
        public string DoorModel { get; set; }
        */

        [Display(Name = "Serial COP")]
        public bool SCOP { get; set; }

        [Display(Name = "Serial Hall Calls")]
        public bool SHC { get; set; }

        [Display(Name = "SHC Risers #")]
        public int SHCRisers { get; set; }

        [Display(Name = "Auxiliary COP")]
        public bool AUXCOP { get; set; }

        public int DoorOperatorID { get; set; } 
    }

    public class HydroSpecific
    {
        public int HydroSpecificID { get; set; }
        public int JobID { get; set; }

        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter the starter model")]
        public string Starter { get; set; }

        [Required(ErrorMessage = "Please enter HP")]
        public int HP { get; set; }

        [Required(ErrorMessage = "Please enter the FLA")]
        public int FLA { get; set; }

        [Required(ErrorMessage = "Please enter the starts per hour")]
        public int SPH { get; set; }

        [Required(ErrorMessage = "Please enter the starts number of motors")]
        [Display(Name = "Number of motors")]
        public int MotorsNum { get; set; }

        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter the brand of the valves")]
        [Display(Name = "Valve's Brand")]
        public string ValveBrand { get; set; }

        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter the model of the valves")]
        [Display(Name = "Valve's Model")]
        public string ValveModel { get; set; }

        [Required(ErrorMessage = "Please enter the number of coils per valve")]
        [Display(Name = "Coils per valve")]
        public int ValveCoils { get; set; }

        [Required(ErrorMessage = "Please enter the number valves")]
        [Display(Name = "Number of valves")]
        public int ValveNum { get; set; }

        [Required(ErrorMessage = "Please enter the voltage of the valves")]
        [Display(Name = "Valve's Voltage")]
        public int ValveVoltage { get; set; }
        
        public bool Battery { get; set; }

        [Display(Name = "Battery's Brand")]
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter a {0}")]
        public string BatteryBrand { get; set; }

        [Display(Name = "Life Jacket")]
        public bool LifeJacket { get; set; }
        [Display(Name = "Low oil sw.")]
        public bool LOS { get; set; }
        [Display(Name = "Oil Cooler")]
        public bool OilCool { get; set; }
        [Display(Name = "OTTS")]
        public bool OilTank { get; set; }
        [Display(Name = "Pressure sw.")]
        public bool PSS { get; set; }
        [Display(Name = "Resync operation")]
        public bool Resync { get; set; }
        [Display(Name = "Roped Hydro")]
        public bool Roped { get; set; }
        [Display(Name = "Viscosity Control")]
        public bool VCI { get; set; }
    }

    public class GenericFeatures
    {
        public int GenericFeaturesID { get; set; }
        public int JobID { get; set; }
        
        [Display(Name = "Additional Fire Recall")]
        public bool FRON2 { get; set; }
        [Display(Name = "Attendant Service")]
        public bool Attendant { get; set; }
        [Display(Name = "Car to lobby")]
        public bool CarToLobby { get; set; }
        [Display(Name = "Earthquake")]
        public bool EQ { get; set; }
        [Display(Name = "EMT")]
        public bool EMT { get; set; }

        //Emergency Power Options
        [Display(Name = "Emergency Power")]
        public bool EP { get; set; }//*
        [Display(Name = "Generator same as line voltage?")]
        public bool EPVoltage { get; set; }//*
        [Display(Name = "Does same generator power other cars?")]
        public bool EPOtherCars { get; set; } //*
        [Display(Name = "Number of cars to run at the same time?")]
        public int EPCarsNumber { get; set; } //* 1,2,3,4+

        [Display(Name = "EP contact during normal power?")]
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string EPContact { get; set; } //* NO or NC

        [Display(Name = "Power pre transfer contact?")]
        public bool PTI { get; set; } //* 
        [Display(Name = "Manual Select Switch")]
        public bool EPSelect { get; set; } //* 


        [Display(Name = "Fan/Light Timer Option")]
        public bool FLO { get; set; }
        [Display(Name = "Hospital")]
        public bool Hosp { get; set; }
        [Display(Name = "Independent")]
        public bool Ind { get; set; }

        // Hoistway Access Options
        [Display(Name = "Hoistway Access")]
        public bool INA { get; set; }

        [Display(Name = "Top Access Switch")] //*
        [StringLength(25, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public bool TopAccess { get; set; } //*

        [Display(Name = "Top Access Switch Location")]//*
        public string TopAccessLocation { get; set; } //* Front Or Rear
        [Display(Name = "Bottom Access Switch")]//*
        public bool BottomAccess { get; set; }//*

        [Display(Name = "Bottom Access Switch Location")]//*
        [StringLength(25, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string BottomAccessLocation { get; set; } //* Front Or Rear

        // In-car Inspection Options
        [Display(Name = "In-Car Inspection")]
        public bool INCP { get; set; }

        [Display(Name = "In-Car Inspection Buttons")] //*
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string INCPButtons { get; set; } //* Using car calls or up/down buttons

        [Display(Name = "Switch Style")]
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string SwitchStyle { get; set; }

        [Display(Name = "Load Weigher")]
        public bool LoadWeigher { get; set; }
        [Display(Name = "Cartop Inspection Station")]//*
        public bool CTINSPST { get; set; }//*
        [Display(Name = "Roped Hydro")]//*
        public bool Roped { get; set; }//*

        [Display(Name = "Roped Hydro Governor Model")]//*
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string GovModel { get; set; }//*

        [StringLength(100, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string Monitoring { get; set; } //* "None", MView Complete, MView Interface, IMonitor Complete, IMonitor Interface, IDS Liftnet

        [Display(Name = "Security")]
        public bool CallEnable { get; set; }
        [Display(Name = "Car Card Reader")]
        public bool CarCallRead { get; set; }
        [Display(Name = "Hall Card Reader")]
        public bool HallCallRead { get; set; }
        [Display(Name = "Car Key")]
        public bool CarKey { get; set; }
        [Display(Name = "Hall Key")]
        public bool HallKey { get; set; }
        [Display(Name = "CRO")]
        public bool CRO { get; set; }
        [Display(Name = "HCRO")]
        public bool HCRO { get; set; }
        [Display(Name = "Car call code security")]

        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string CarCallCodeSecurity { get; set; }

        [Display(Name = "Special Instructions")]
        [StringLength(250, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string SpecialInstructions { get; set; }
    }

    public class Indicator
    {
        public int IndicatorID { get; set; }
        public int JobID { get; set; }

        // if JobExtension.SCOP is true these are 24 dc led

        [Required(ErrorMessage = "Please enter the car calls voltage")]
        public string CarCallsVoltage { get; set; } //24,48,120

        [StringLength(5, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter the car calls voltage type")]
        public string CarCallsVoltageType { get; set; } //AC, DC

        [StringLength(25, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter the car calls type")]
        public string CarCallsType { get; set; } //LED, Incandescent

        // if JobExtension.SHC is true these are 24 dc led
        [Required(ErrorMessage = "Please enter the hall calls voltage")]
        public string HallCallsVoltage { get; set; } //24,48,120

        [StringLength(5, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter the hall calls voltage type")]
        public string HallCallsVoltageType { get; set; } //AC, DC

        [StringLength(25, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter the hall calls type")]
        public string HallCallsType { get; set; } //LED, Incandescent


        public bool CarPI { get; set; } // check if required
        [StringLength(25, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string CarPIType { get; set; } // CE, Emotive, Discrete
        [StringLength(25, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string CarPIDiscreteType { get; set; } //Multi-light, One line, binary 00, binary 01

        public bool HallPI { get; set; } // check if required

        [StringLength(25, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string HallPIType { get; set; } // CE, Emotive, Discrete
        [StringLength(25, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string HallPIDiscreteType { get; set; } //Multi-light, One line, binary 00, binary 01

        public bool VoiceAnnunciationPI { get; set; } // check if required
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string VoiceAnnunciationPIType { get; set; } // CE, Emotive, Other

        public bool CarLanterns { get; set; } //check if required
        [StringLength(25, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string CarLanternsStyle { get; set; } // CE, Emotive, Discrete

        [StringLength(10, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string CarLanternsType { get; set; } //Chime, Gong

        public bool HallLanterns { get; set; } //check if required
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string HallLanternsStyle { get; set; } // CE, Emotive, Discrete

        [StringLength(10, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string HallLanternsType { get; set; } //Chime, Gong

        public bool PassingFloor { get; set; } // check if required

        [StringLength(25, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter a {0}")]
        public string PassingFloorType { get; set; } // CE, Emotive, Discrete

        [StringLength(25, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter a {0}")]
        public string PassingFloorDiscreteType { get; set; } //Chime, Gong

        public bool PassingFloorEnable { get; set; } // check if required

        [Required(ErrorMessage = "Please enter the indicators voltage")]
        public int IndicatorsVoltage { get; set; } //24,48,120
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter the indicators voltage type")]
        public string IndicatorsVoltageType { get; set; } //AC, DC
    }

    public class HoistWayData
    {
        public int HoistWayDataID { get; set; }
        public int JobID { get; set; }

        public bool FrontFirstServed { get; set; }
        public bool RearFirstServed { get; set; }
        public bool FrontSecondServed { get; set; }
        public bool RearSecondServed { get; set; }
        public bool FrontThirdServed { get; set; }
        public bool RearThirdServed { get; set; }
        public bool FrontFourthServed { get; set; }
        public bool RearFourthServed { get; set; }
        public bool FrontFifthServed { get; set; }
        public bool RearFifthServed { get; set; }
        public bool FrontSexthServed { get; set; }
        public bool RearSexthServed { get; set; }
        public bool FrontSeventhServed { get; set; }
        public bool RearSeventhServed { get; set; }
        public bool FrontEightServed { get; set; }
        public bool RearEightServed { get; set; }
        public bool FrontNinthServed { get; set; }
        public bool RearNinthServed { get; set; }
        public bool FrontTenthServed { get; set; }
        public bool RearTenthServed { get; set; }
        public bool FrontEleventhServed { get; set; }
        public bool RearEleventhServed { get; set; }
        public bool FrontTwelvethServed { get; set; }
        public bool RearTwelvethServed { get; set; }
        public bool FrontThirteenthServed { get; set; }
        public bool RearThirteenthServed { get; set; }
        public bool FrontFourteenthServed { get; set; }
        public bool RearFourteenthServed { get; set; }
        public bool FrontFifteenthServed { get; set; }
        public bool RearFifteenthServed { get; set; }
        public bool FrontSixteenthServed { get; set; }
        public bool RearSixteenthServed { get; set; }

        [Required(ErrorMessage = "Please enter the capacity")]
        public int Capacity { get; set; }
        [Required(ErrorMessage = "Please enter the up direction speed")]
        [Display(Name = "Up Direction Speed")]
        public int UpSpeed { get; set; }
        [Required(ErrorMessage = "Please enter the down direction speed")]
        [Display(Name = "Down Direction Speed")]
        public int DownSpeed { get; set; }
        [Required(ErrorMessage = "Please enter the total travel")]
        [Display(Name = "Total Travel")]
        public int TotalTravel { get; set; }
        [Required(ErrorMessage = "Please enter the landing system")]
        [Display(Name = "Landing System")]
        //public string LandingSystem { get; set; }
        public int LandingSystemID { get; set; }
    }

    /*It is not need it
    public class StatusIndicator
    {
        public int StatusIndicatorID { get; set; }
        public int JobID { get; set; }

        public string Name { get; set; }
        public int Voltage { get; set; }
        public string VoltageType { get; set; }
    }
    */
}
