using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema;
using NPOI.SS.Formula.Functions;

namespace ProdFloor.Models
{
    public class Job
    {
        public int JobID { get; set; }

        [StringLength(26, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string Status { get; set; }

        public int EngID { get; set; }
        public int CrossAppEngID { get; set; }
        [NotMapped]
        public int CurrentUserID { get; set; }

        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }

        [Display(Name = "Name #2")]
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string Name2 { get; set; }

        [Display(Name = "Job Number")]
        public string JobNum { get; set; }

        //Aux fields for New JobNumber
        [NotMapped]
        [Required(ErrorMessage = "Please enter the first digits")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "First digits is out of range")]
        public string JobNumFirstDigits { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please enter the last digits")]
        [Range(10000, 99999, ErrorMessage = "Last digits is out of range")]
        public int JobNumLastDigits { get; set; }



        [Display(Name = "Shipping Date")]
        [Required(ErrorMessage = "Please enter a Shipping Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ShipDate { get; set; }

        [Display(Name = "Finish Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
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

        [NotMapped]
        [Display(Name = "Country")]
        [Required(ErrorMessage = "Please enter a Country")]
        public string CountryID { get; set; }

        [NotMapped]
        [Display(Name = "State")]
        [Required(ErrorMessage = "Please enter a State")]
        public string StateID { get; set; }

        [NotMapped]
        [Display(Name = "Current Fire Code")]
        public int CurrentFirecode { get; set; }

        public JobExtension _jobExtension { get; set; }

        public List<WiringPXP> _WiringPXPs { get; set; }
        public HydroSpecific _HydroSpecific { get; set; }
        public GenericFeatures _GenericFeatures { get; set; }
        public Indicator _Indicator { get; set; }
        public HoistWayData _HoistWayData { get; set; }
        public JobAdditional _JobAdditional { get; set; }
        public List<SpecialFeatures> _SpecialFeatureslist { get; set; }
        public List<CustomFeature> _CustomFeatures { get; set; }
        public List<Element> _Elements { get; set; }
        public List<ElementHydro> _ElementHydros { get; set; }
        public List<ElementTraction> _EmentTractions { get; set; }

        public List<PO> _PO { get; set; }
        public List<TestJob> _TestJobs { get; set; }
        [Display(Name = "Job Type")]
        public int JobTypeID { get; set; }
        [Display(Name = "City")]
        public int CityID { get; set; }
        [Display(Name = "Fire Code")]
        public int FireCodeID { get; set; }

    }

    public class JobNumber
    {
        public string firstDigits { get; set; }

        public int lastDigits { get; set; }
    }

    public class PO
    {
        public int POID { get; set; }
        public int JobID { get; set; }
        [Display(Name = "PO #")]
        [Range(3000000, 4900000, ErrorMessage = "PO number is out of range")]
        [Required(ErrorMessage = "Please enter a PO")]
        public int PONumb { get; set; }
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
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
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

        [NotMapped]
        [Display(Name = "Door Operator Brand")]
        [Required(ErrorMessage = "Please enter the door operator brand")]
        public string DoorBrand { get; set; }

        [Display(Name = "Serial COP")]
        public bool SCOP { get; set; }

        [Display(Name = "Serial Hall Calls")]
        public bool SHC { get; set; }

        [Display(Name = "Swing Operation")]
        public bool SwingOp { get; set; }

        [Display(Name = "Back Up Dispatcher")]
        public bool BackUpDisp { get; set; }

        [Display(Name = "Alternate Riser")]
        public bool AltRis { get; set; }

        [Display(Name = "SHC Risers #")]
        public int SHCRisers { get; set; }

        [Display(Name = "Auxiliary COP")]
        public bool AUXCOP { get; set; }

        [Display(Name = "Door Operator")]
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
        public float FLA { get; set; }

        [Required(ErrorMessage = "Please enter the starts per hour")]
        public int SPH { get; set; }

        [Required(ErrorMessage = "Please enter the starts number of motors")]
        [Display(Name = "Number of motors")]
        public int MotorsNum { get; set; }

        [Required(ErrorMessage = "Please enter the starts number of motors disconnects")]
        [Display(Name = "Number of motors disconnects")]
        public int MotorsDisconnect { get; set; }

        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter the brand of the valves")]
        [Display(Name = "Valve's Brand")]
        public string ValveBrand { get; set; }

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
        public string BatteryBrand { get; set; }

        [NotMapped]
        [Display(Name = "Other Brand")]
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string OtherBatteryBrand { get; set; }

        [NotMapped]
        [Display(Name = "Other Valve Brand")]
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string OtherValveBrand { get; set; }

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
        public string EPCarsNumber { get; set; } //* 1,2,3,4+

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
        [Display(Name = "Pit Flood")]
        public bool Pit { get; set; }

        // Hoistway Access Options
        [Display(Name = "Hoistway Access")]
        public bool INA { get; set; }

        [Display(Name = "Top Access Switch")] //*
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
        [Display(Name = "BSI")]
        public bool BSI { get;set;}
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
        public string PassingFloorType { get; set; } // CE, Emotive, Discrete

        [StringLength(25, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string PassingFloorDiscreteType { get; set; } //Chime, Gong

        public bool PassingFloorEnable { get; set; } // check if required

        [Required(ErrorMessage = "Please enter the indicators voltage")]
        public int IndicatorsVoltage { get; set; } //24,48,120
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter the indicators voltage type")]
        public string IndicatorsVoltageType { get; set; } //AC, DC

        public bool HallPIAll { get; set; }
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

        [NotMapped]
        public bool AnyRear
        {
            get {
                    if (RearFirstServed == true || RearSecondServed == true || RearThirdServed == true || RearFourthServed == true || RearFifthServed == true || RearSexthServed == true
                        || RearSeventhServed == true || RearEightServed == true || RearNinthServed == true || RearTenthServed == true
                        || RearEleventhServed == true || RearTwelvethServed == true || RearThirteenthServed == true || RearFourteenthServed == true
                        || RearFifteenthServed == true || RearSixteenthServed == true)
                    {
                        return true;
                    }
                        return false;
                }
        }

        [NotMapped]
        public bool IndependentRearOpenings
        {
            get
            {
                bool resp = false;
                if( AnyRear == true)
                {
                    if (RearFirstServed == true && FrontFirstServed == false) resp = true;
                    if (RearSecondServed == true && FrontSecondServed == false) resp = true;
                    if (RearThirdServed == true && FrontThirdServed == false) resp = true;
                    if (RearFourthServed == true && FrontFourthServed == false) resp = true;
                    if (RearFifthServed == true && FrontFifthServed == false) resp = true;
                    if (RearSexthServed == true && FrontSexthServed == false) resp = true;
                    if (RearSeventhServed == true && FrontSeventhServed == false) resp = true;
                    if (RearEightServed == true && FrontEightServed == false) resp = true;
                    if (RearNinthServed == true && FrontNinthServed == false) resp = true;
                    if (RearTenthServed == true && FrontTenthServed == false) resp = true;
                    if (RearEleventhServed == true && FrontEleventhServed == false) resp = true;
                    if (RearTwelvethServed == true && FrontTwelvethServed == false) resp = true;
                    if (RearThirteenthServed == true && FrontThirteenthServed == false) resp = true;
                    if (RearFourteenthServed == true && FrontFourteenthServed == false) resp = true;
                    if (RearFifteenthServed == true && FrontFifteenthServed == false) resp = true;
                    if (RearSixteenthServed == true && FrontSixteenthServed == false) resp = true;

                    return resp;
                }
                
                return resp;
            }
        }

        [NotMapped]
        public int TopFloor
        {
            get
            {
                int count = 0;
                if (RearFirstServed == true || FrontFirstServed == true) count++;
                if (RearSecondServed == true || FrontSecondServed == true) count++;
                if (RearThirdServed == true || FrontThirdServed == true) count++;
                if (RearFourthServed == true || FrontFourthServed == true) count++;
                if (RearFifthServed == true || FrontFifthServed == true) count++;
                if (RearSexthServed == true || FrontSexthServed == true) count++;
                if (RearSeventhServed == true || FrontSeventhServed == true) count++;
                if (RearEightServed == true || FrontEightServed == true) count++;
                if (RearNinthServed == true || FrontNinthServed == true) count++;
                if (RearTenthServed == true || FrontTenthServed == true) count++;
                if (RearEleventhServed == true || FrontEleventhServed == true) count++;
                if (RearTwelvethServed == true || FrontTwelvethServed == true) count++;
                if (RearThirteenthServed == true || FrontThirteenthServed == true) count++;
                if (RearFourteenthServed == true || FrontFourteenthServed == true) count++;
                if (RearFifteenthServed == true || FrontFifteenthServed == true) count++;
                if (RearSixteenthServed == true || FrontSixteenthServed == true) count++;

                return count;
            }
        }

        [NotMapped]
        public int RearFloorOpenings
        {
            get
            {
                int count = 0;
                if (RearFirstServed == true) count++;
                if (RearSecondServed == true) count++;
                if (RearThirdServed == true) count++;
                if (RearFourthServed == true) count++;
                if (RearFifthServed == true) count++;
                if (RearSexthServed == true) count++;
                if (RearSeventhServed == true) count++;
                if (RearEightServed == true) count++;
                if (RearNinthServed == true) count++;
                if (RearTenthServed == true) count++;
                if (RearEleventhServed == true) count++;
                if (RearTwelvethServed == true) count++;
                if (RearThirteenthServed == true) count++;
                if (RearFourteenthServed == true) count++;
                if (RearFifteenthServed == true) count++;
                if (RearSixteenthServed == true) count++;
                
                return count;
            }
        }

        [NotMapped]
        public int FrontFloorOpenings
        {
            get
            {
                int count = 0;
                if (FrontFirstServed == true) count++;
                if (FrontSecondServed == true) count++;
                if (FrontThirdServed == true) count++;
                if (FrontFourthServed == true) count++;
                if (FrontFifthServed == true) count++;
                if (FrontSexthServed == true) count++;
                if (FrontSeventhServed == true) count++;
                if (FrontEightServed == true) count++;
                if (FrontNinthServed == true) count++;
                if (FrontTenthServed == true) count++;
                if (FrontEleventhServed == true) count++;
                if (FrontTwelvethServed == true) count++;
                if (FrontThirteenthServed == true) count++;
                if (FrontFourteenthServed == true) count++;
                if (FrontFifteenthServed == true) count++;
                if (FrontSixteenthServed == true) count++;

                return count;
            }
        }

        [Required(ErrorMessage = "Please enter a number of hoistways")]
        [Display(Name = "Hoistways Number")]
        public int HoistWaysNumber { get; set; }
        [Required(ErrorMessage = "Please enter a number of Machine Rooms")]
        [Display(Name = "Machine Rooms Number")]
        public int MachineRooms { get; set; }

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
        public int LandingSystemID { get; set; }
    }

    public class SpecialFeatures
    {
        public int SpecialFeaturesID { get; set; }
        public int JobID { get; set; }
        [StringLength(250, ErrorMessage = "The maximum length of the {0} field is {1}")]
        public string Description { get; set; }
        public Job Job { get; set; }
    }

   public class CustomSoftware
    {
        public int CustomSoftwareID { get; set; }
        public string Description { get; set; }
        
        public List<TriggeringCustSoft> _TriggeringCustSofts { get; set; }
        public List<CustomFeature> _CustomFeatures { get; set; }
    }

    public class TriggeringCustSoft
    {
        public int TriggeringCustSoftID { get; set; }
        public int CustomSoftwareID { get; set; }
        public string Name { get; set; }
        public bool isSelected { get; set; }
        public string itemToMatch { get; set; }
    }

    public class CustomFeature
    {
        public int CustomFeatureID { get; set; }
        public int CustomSoftwareID { get; set; }
        public int JobID { get; set; }
    }

    public class Element
    {
        public int ElementID { get; set; }
        public int JobID { get; set; }

        [Display(Name = "Car Gate")]
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter type of car gate")]
        public string DoorGate { get; set; }

        [NotMapped]
        [Display(Name = "Door Operator Brand")]
        [Required(ErrorMessage = "Please enter the door operator brand")]
        public string DoorBrand { get; set; }

        [Display(Name = "Door Operator")]
        public int DoorOperatorID { get; set; }

        [Required(ErrorMessage = "Please enter the INA")]
        public string INA { get; set; }
        public bool INCP { get; set; }
        public bool  CarKey { get; set; }
        public bool  CarCardReader { get; set; }
        public bool  CRO { get; set; }
        public bool  HallKey { get; set; }
        public bool  HallCardReader { get; set; }
        public bool HCRO { get; set; }
        [Display(Name = "Security")]
        public bool CallEnable { get; set; }

        public bool HAPS { get; set; }
        public bool EP { get; set; }
        public bool EMT { get; set; }
        public bool PSS { get; set; }
        public bool PTFLD { get; set; }
        public bool VCI { get; set; }
        public bool CReg { get; set; }
        public bool Egress { get; set; }
        public bool PHECutOut { get; set; }
        public bool CTINSPST { get; set; }
        public bool Traveler { get; set; }
        public bool LOS { get; set; }
        public bool PFGE { get; set; }
        public bool FRON2 { get; set; }

        [Display(Name = "Car To Lobby")]
        public bool CTL { get; set; }

        [Display(Name = "Car Shutdown")]
        public bool CSD { get; set; }

        [Display(Name = "Car to Floor")]
        public bool CTF { get; set; }

        [Display(Name = "Earthquake")]
        public bool EQ { get; set; }

        [Display(Name = "Life Jacket")]
        public bool LJ { get; set; }

        [Display(Name = "Door Hold")]
        public bool DHLD { get; set; }

        [Required(ErrorMessage = "Please enter the Capacity")]
        public int Capacity { get; set; }
        [Required(ErrorMessage = "Please enter the Speed")]
        public int Speed { get; set; }
        [Required(ErrorMessage = "Please enter the Voltage")]
        public int Voltage { get; set; }
        [Required(ErrorMessage = "Please enter the Phase")]
        public int Phase { get; set; }
        [Required(ErrorMessage = "Please enter the Frequency")]
        public int Frequency { get; set; }
        [Required(ErrorMessage = "Please enter the LoadWeigher")]
        public string LoadWeigher { get; set; }

        [Required(ErrorMessage = "Please enter the landing system")]
        [Display(Name = "Landing System")]
        public int LandingSystemID { get; set; }

    }

    public class ElementHydro
    {
        public int ElementHydroID { get; set; }
        public int JobID { get; set; }

        [Required(ErrorMessage = "Please enter the Starter")]
        public string Starter { get; set; }
        [Required(ErrorMessage = "Please enter the HP")]
        public float HP { get; set; }
        [Required(ErrorMessage = "Please enter the FLA")]
        public float FLA { get; set; }
        [Required(ErrorMessage = "Please enter the SPH")]
        public int SPH { get; set; }
        [Required(ErrorMessage = "Please enter the ValveBrand")]
        public string ValveBrand { get; set; }
    }

    public class ElementTraction
    {
        public int ElementTractionID { get; set; }
        public int JobID { get; set; }

        [Required(ErrorMessage = "Please enter the MachineLocation")]
        public string MachineLocation { get; set; }
        [Required(ErrorMessage = "Please enter the VVVF")]
        public string VVVF { get; set; }
        [Required(ErrorMessage = "Please enter the MotorBrand")]
        public string MotorBrand { get; set; }
        [Required(ErrorMessage = "Please enter the Contact")]
        public string Contact { get; set; }

        public bool Encoder { get; set; }
        public bool ISO { get; set; }

        [Required(ErrorMessage = "Please enter the HP")]
        public float HP { get; set; }
        [Required(ErrorMessage = "Please enter the FLA")]
        public float FLA { get; set; }
        [Required(ErrorMessage = "Please enter the PickVoltage")]
        public int PickVoltage { get; set; }
        [Required(ErrorMessage = "Please enter the HoldVoltage")]
        public int HoldVoltage { get; set; }
        [Required(ErrorMessage = "Please enter the Resistance")]
        public int Resistance { get; set; }
        [Required(ErrorMessage = "Please enter the Current")]
        public float Current { get; set; }
        
    }

    public class JobAdditional
    {
        public int JobAdditionalID { get; set; }
        public int JobID { get; set; }

        [Required(ErrorMessage = "Please enter a status")]
        public string Status { get; set; }

        [Display(Name = "Corrective Actions")]
        public string Action { get; set; }

        public int Priority { get; set; }

        [Display(Name = "Expected Release Date")]
        [Required(ErrorMessage = "Please enter an expected release date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ERDate { get; set; }
    }
}
