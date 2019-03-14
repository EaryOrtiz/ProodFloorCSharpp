using Microsoft.AspNetCore.Mvc.Rendering;
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
        public bool CleanFields { get; set; }

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
        public int DoorOperatorID { get;  set; }
        public string DoorGate { get; set; }
        public string DoorBrand { get; set; }

        //Atributos de HydroSpecifics
        public string Battery { get; set; }
        public string BatteryBrand { get; set; }
        public string LOS { get; set; }
        public string LifeJacket { get; set; }
        public string OilCool { get; set; }
        public string OilTank { get; set; }
        public string PSS { get; set; }
        public string Resync { get; set; }
        public string Roped { get; set; }
        public string Starter { get; set; }
        public string VCI { get; set; }
        public string ValveBrand { get; set; }

        public int FLA { get; set; }
        public int HP { get; set; }
        public int MotorsNum { get; set; }
        public int SPH { get; set; }

        //Atributos de GenericFeatureList
        public string Attendant { get; set; }
        public string CallEnable { get; set; }
        public string CarToLobby { get; set; }
        public string EMT { get; set; }
        public string EP { get; set; }
        public string EQ { get; set; }
        public string FLO { get; set; }
        public string FRON2 { get; set; }
        public string Hosp { get; set; }
        public string EPVoltage { get; set; }
        public string INA { get; set; }
        public string INCP { get; set; }
        public string Ind { get; set; }
        public string LoadWeigher { get; set; }
        public string TopAccess { get; set; }
        public string SwitchStyle { get; set; }
        public string CRO { get; set; }
        public string CarCallRead { get; set; }
        public string CarKey { get; set; }
        public string HCRO { get; set; }
        public string HallCallRead { get; set; }
        public string HallKey { get; set; }
        public string BottomAccess { get; set; }
        public string BottomAccessLocation { get; set; }
        public string CTINSPST { get; set; }
        public string EPContact { get; set; }
        public string EPSelect { get; set; }
        public string GovModel { get; set; }
        public string INCPButtons { get; set; }
        public string Monitoring { get; set; }
        public string EPCarsNumber { get; set; }
        public string PTI { get; set; }
        public string TopAccessLocation { get; set; }

        //Atributos de Indicators
        public string CarLanterns { get; set; }
        public string HallCallsType { get; set; }
        public string HallCallsVoltageType { get; set; }
        public string HallLanterns { get; set; }
        public string HallPI { get; set; }
        public string IndicatorsVoltageType { get; set; }
        public string PassingFloor { get; set; }
        public string VoiceAnnunciationPI { get; set; }
        public string CarCallsVoltageType { get; set; }

        public string CarCallsVoltage { get; set; }
        public string HallCallsVoltage { get; set; }
        public int IndicatorsVoltage { get; set; }

        //Atributos de HoistWayData
        public string AnyRear { get; set; }
        public string IndependentRearOpenings { get; set; }
        public int TopFloor { get; set; }
        public int RearFloorOpenings { get; set; }
        public int FrontFloorOpenings { get; set; }
        public int Capacity { get; set; }
        public int DownSpeed { get; set; }
        public int LandingSystemID { get; set; }
        public int UpSpeed { get; set; }
        public int HoistWaysNumber { get; set; }
        public int MachineRooms  { get; set; }

        //Atributos de Special Features
        public string Description { get; set; }
    }
}
