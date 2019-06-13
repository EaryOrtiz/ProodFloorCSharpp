using Microsoft.EntityFrameworkCore;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.Job;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProdFloor.Models
{
    public class EFJobRepository : IJobRepository
    {
        private ApplicationDbContext context;

        public EFJobRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<Job> Jobs => context.Jobs;
        public IQueryable<JobExtension> JobsExtensions => context.JobsExtensions;
        public IQueryable<HydroSpecific> HydroSpecifics => context.HydroSpecifics;
        public IQueryable<GenericFeatures> GenericFeaturesList => context.GenericFeaturesList;
        public IQueryable<Indicator> Indicators => context.Indicators;
        public IQueryable<HoistWayData> HoistWayDatas => context.HoistWayDatas;
        public IQueryable<SpecialFeatures> SpecialFeatures => context.SpecialFeatures;
        public IQueryable<PO> POs => context.POs;
        public IQueryable<CustomSoftware> CustomSoftwares => context.CustomSoftwares;
        public IQueryable<TriggeringCustSoft> TriggeringCustSofts => context.TriggeringCustSofts;
        public IQueryable<CustomFeature> CustomFeatures => context.CustomFeatures;
        public IQueryable<Element> Elements => context.Elements;
        public IQueryable<ElementHydro> ElementHydros => context.ElementHydros;
        public IQueryable<ElementTraction> ElementTractions => context.ElementTractions;

        public void SaveJob(Job job)
        {
            if (job.JobID == 0)
            {
                context.Jobs.Add(job);
            }
            else
            {
                Job dbEntry = context.Jobs
                .FirstOrDefault(p => p.JobID == job.JobID);
                if (dbEntry != null)
                {
                    dbEntry.Name = job.Name;
                    dbEntry.Name2 = job.Name2;
                    dbEntry.JobNum = job.JobNum;
                    dbEntry.JobTypeID = job.JobTypeID;
                    dbEntry.ShipDate = job.ShipDate;
                    dbEntry.LatestFinishDate = job.LatestFinishDate;
                    dbEntry.Cust = job.Cust;
                    dbEntry.Contractor = job.Contractor;
                    dbEntry.CityID = job.CityID;
                    dbEntry.FireCodeID = job.FireCodeID;
                    dbEntry.Status = job.Status;
                    dbEntry.EngID = job.EngID;
                    dbEntry.CrossAppEngID = job.CrossAppEngID;
                }
            }
            context.SaveChanges();
        }
        public void SaveJobExtension(JobExtension jobExtension)
        {
            if (jobExtension.JobExtensionID == 0)
            {
                context.JobsExtensions.Add(jobExtension);
            }
            else
            {
                JobExtension dbEntry = context.JobsExtensions
                .FirstOrDefault(p => p.JobExtensionID == jobExtension.JobExtensionID);
                if (dbEntry != null)
                {
                    dbEntry.JobID = jobExtension.JobID;
                    dbEntry.DoorOperatorID = jobExtension.DoorOperatorID;
                    dbEntry.NumOfStops = jobExtension.NumOfStops;
                    dbEntry.JobTypeMain = jobExtension.JobTypeMain;
                    dbEntry.JobTypeAdd = jobExtension.JobTypeAdd;
                    dbEntry.InputVoltage = jobExtension.InputVoltage;
                    dbEntry.InputPhase = jobExtension.InputPhase;
                    dbEntry.InputFrecuency = jobExtension.InputFrecuency;
                    dbEntry.DoorGate = jobExtension.DoorGate;
                    dbEntry.DoorHoist = jobExtension.DoorHoist;
                    dbEntry.InfDetector = jobExtension.InfDetector;
                    dbEntry.MechSafEdge = jobExtension.MechSafEdge;
                    dbEntry.HeavyDoors = jobExtension.HeavyDoors;
                    dbEntry.CartopDoorButtons = jobExtension.CartopDoorButtons;
                    dbEntry.DoorHold = jobExtension.DoorHold;
                    dbEntry.Nudging = jobExtension.Nudging;
                    dbEntry.AltRis = jobExtension.AltRis;
                    dbEntry.BackUpDisp = jobExtension.BackUpDisp;
                    dbEntry.SwingOp = jobExtension.SwingOp;
                    dbEntry.SCOP = jobExtension.SCOP;
                    dbEntry.SHC = jobExtension.SHC;
                    dbEntry.SHCRisers = jobExtension.SHCRisers;
                    dbEntry.AUXCOP = jobExtension.AUXCOP;
                }
            }
            context.SaveChanges();
        }
        public void SaveHydroSpecific(HydroSpecific hydroSpecific)
        {
            if (hydroSpecific.HydroSpecificID == 0)
            {
                context.HydroSpecifics.Add(hydroSpecific);
            }
            else
            {
                HydroSpecific dbEntry = context.HydroSpecifics
                .FirstOrDefault(p => p.HydroSpecificID == hydroSpecific.HydroSpecificID);
                if (dbEntry != null)
                {
                    dbEntry.JobID = hydroSpecific.JobID;
                    dbEntry.Starter = hydroSpecific.Starter;
                    dbEntry.HP = hydroSpecific.HP;
                    dbEntry.FLA = hydroSpecific.FLA;
                    dbEntry.SPH = hydroSpecific.SPH;
                    dbEntry.MotorsNum = hydroSpecific.MotorsNum;
                    dbEntry.ValveBrand = hydroSpecific.ValveBrand;
                    dbEntry.ValveCoils = hydroSpecific.ValveCoils;
                    dbEntry.ValveNum = hydroSpecific.ValveNum;
                    dbEntry.ValveVoltage = hydroSpecific.ValveVoltage;
                    dbEntry.Battery = hydroSpecific.Battery;
                    dbEntry.BatteryBrand = hydroSpecific.BatteryBrand;
                    dbEntry.LifeJacket = hydroSpecific.LifeJacket;
                    dbEntry.LOS = hydroSpecific.LOS;
                    dbEntry.OilCool = hydroSpecific.OilCool;
                    dbEntry.OilTank = hydroSpecific.OilTank;
                    dbEntry.PSS = hydroSpecific.PSS;
                    dbEntry.Resync = hydroSpecific.Resync;
                    dbEntry.VCI = hydroSpecific.VCI;
                }
            }
            context.SaveChanges();
        }
        public void SaveGenericFeatures(GenericFeatures genericFeatures)
        {
            if (genericFeatures.GenericFeaturesID == 0)
            {
                context.GenericFeaturesList.Add(genericFeatures);
            }
            else
            {
                GenericFeatures dbEntry = context.GenericFeaturesList
                .FirstOrDefault(p => p.GenericFeaturesID == genericFeatures.GenericFeaturesID);
                if (dbEntry != null)
                {
                    dbEntry.GenericFeaturesID = genericFeatures.GenericFeaturesID;
                    dbEntry.JobID = genericFeatures.JobID;
                    dbEntry.FRON2 = genericFeatures.FRON2;
                    dbEntry.Attendant = genericFeatures.Attendant;
                    dbEntry.CarToLobby = genericFeatures.CarToLobby;
                    dbEntry.EQ = genericFeatures.EQ;
                    dbEntry.EMT = genericFeatures.EMT;
                    dbEntry.EP = genericFeatures.EP;
                    dbEntry.EPVoltage = genericFeatures.EPVoltage;
                    dbEntry.EPOtherCars = genericFeatures.EPOtherCars;
                    dbEntry.EPCarsNumber = genericFeatures.EPCarsNumber;
                    dbEntry.EPContact = genericFeatures.EPContact;
                    dbEntry.PTI = genericFeatures.PTI;
                    dbEntry.EPSelect = genericFeatures.EPSelect;
                    dbEntry.FLO = genericFeatures.FLO;
                    dbEntry.Hosp = genericFeatures.Hosp;
                    dbEntry.Pit = genericFeatures.Pit;
                    dbEntry.INA = genericFeatures.INA;
                    dbEntry.TopAccess = genericFeatures.TopAccess;
                    dbEntry.TopAccessLocation = genericFeatures.TopAccessLocation;
                    dbEntry.BottomAccess = genericFeatures.BottomAccess;
                    dbEntry.BottomAccessLocation = genericFeatures.BottomAccessLocation;
                    dbEntry.INCP = genericFeatures.INCP;
                    dbEntry.INCPButtons = genericFeatures.INCPButtons;
                    dbEntry.CTINSPST = genericFeatures.CTINSPST;
                    dbEntry.Roped = genericFeatures.Roped;
                    dbEntry.GovModel = genericFeatures.GovModel;
                    dbEntry.LoadWeigher = genericFeatures.LoadWeigher;
                    dbEntry.SwitchStyle = genericFeatures.SwitchStyle;
                    dbEntry.Monitoring = genericFeatures.Monitoring;
                    dbEntry.CallEnable = genericFeatures.CallEnable;
                    dbEntry.CarCallCodeSecurity = genericFeatures.CarCallCodeSecurity;
                    dbEntry.SpecialInstructions = genericFeatures.SpecialInstructions;
                    dbEntry.CarCallRead = genericFeatures.CarCallRead;
                    dbEntry.HallCallRead = genericFeatures.HallCallRead;
                    dbEntry.CarKey = genericFeatures.CarKey;
                    dbEntry.CRO = genericFeatures.CRO;
                    dbEntry.HCRO = genericFeatures.HCRO;
                    dbEntry.BSI = genericFeatures.BSI;

                }
                else
                {
                    context.GenericFeaturesList.Add(genericFeatures);
                }
            }
            context.SaveChanges();
        }
        public void SaveIndicator(Indicator indicator)
        {
            if (indicator.IndicatorID == 0)
            {
                context.Indicators.Add(indicator);
            }
            else
            {
                Indicator dbEntry = context.Indicators
                .FirstOrDefault(p => p.IndicatorID == indicator.IndicatorID);
                if (dbEntry != null)
                {
                    dbEntry.CarCallsVoltage = indicator.CarCallsVoltage;
                    dbEntry.CarCallsVoltageType = indicator.CarCallsVoltageType;
                    dbEntry.CarCallsType = indicator.CarCallsType;
                    dbEntry.HallCallsVoltage = indicator.HallCallsVoltage;
                    dbEntry.HallCallsVoltageType = indicator.HallCallsVoltageType;
                    dbEntry.HallCallsType = indicator.HallCallsType;
                    dbEntry.CarPI = indicator.CarPI;
                    dbEntry.CarPIType = indicator.CarPIType;
                    dbEntry.CarPIDiscreteType = indicator.CarPIDiscreteType;
                    dbEntry.HallPI = indicator.HallPI;
                    dbEntry.HallPIType = indicator.HallPIType;
                    dbEntry.HallPIDiscreteType = indicator.HallPIDiscreteType;
                    dbEntry.VoiceAnnunciationPI = indicator.VoiceAnnunciationPI;
                    dbEntry.VoiceAnnunciationPIType = indicator.VoiceAnnunciationPIType;
                    dbEntry.CarLanterns = indicator.CarLanterns;
                    dbEntry.CarLanternsStyle = indicator.CarLanternsStyle;
                    dbEntry.CarLanternsType = indicator.CarLanternsType;
                    dbEntry.HallLanterns = indicator.HallLanterns;
                    dbEntry.HallLanternsStyle = indicator.HallLanternsStyle;
                    dbEntry.HallLanternsType = indicator.HallLanternsType;
                    dbEntry.PassingFloor = indicator.PassingFloor;
                    dbEntry.PassingFloorType = indicator.PassingFloorType;
                    dbEntry.PassingFloorDiscreteType = indicator.PassingFloorDiscreteType;
                    dbEntry.PassingFloorEnable = indicator.PassingFloorEnable;
                    dbEntry.IndicatorsVoltage = indicator.IndicatorsVoltage;
                    dbEntry.IndicatorsVoltageType = indicator.IndicatorsVoltageType;
                    dbEntry.JobID = indicator.JobID;
                }
            }
            context.SaveChanges();
        }
        public void SaveHoistWayData(HoistWayData hoistWayData)
        {
            if (hoistWayData.HoistWayDataID == 0)
            {
                context.HoistWayDatas.Add(hoistWayData);
            }
            else
            {
                HoistWayData dbEntry = context.HoistWayDatas
                .FirstOrDefault(p => p.HoistWayDataID == hoistWayData.HoistWayDataID);
                if (dbEntry != null)
                {
                    dbEntry.JobID = hoistWayData.JobID;
                    dbEntry.FrontFirstServed = hoistWayData.FrontFirstServed;
                    dbEntry.RearFirstServed = hoistWayData.RearFirstServed;
                    dbEntry.FrontSecondServed = hoistWayData.FrontSecondServed;
                    dbEntry.RearSecondServed = hoistWayData.RearSecondServed;
                    dbEntry.FrontThirdServed = hoistWayData.FrontThirdServed;
                    dbEntry.RearThirdServed = hoistWayData.RearThirdServed;
                    dbEntry.FrontFourthServed = hoistWayData.FrontFourthServed;
                    dbEntry.RearFourthServed = hoistWayData.RearFourthServed;
                    dbEntry.FrontFifthServed = hoistWayData.FrontFifthServed;
                    dbEntry.RearFifthServed = hoistWayData.RearFifthServed;
                    dbEntry.FrontSexthServed = hoistWayData.FrontSexthServed;
                    dbEntry.RearSexthServed = hoistWayData.RearSexthServed;
                    dbEntry.FrontSeventhServed = hoistWayData.FrontSeventhServed;
                    dbEntry.RearSeventhServed = hoistWayData.RearSeventhServed;
                    dbEntry.FrontEightServed = hoistWayData.FrontEightServed;
                    dbEntry.RearEightServed = hoistWayData.RearEightServed;
                    dbEntry.FrontNinthServed = hoistWayData.FrontNinthServed;
                    dbEntry.RearNinthServed = hoistWayData.RearNinthServed;
                    dbEntry.FrontTenthServed = hoistWayData.FrontTenthServed;
                    dbEntry.RearTenthServed = hoistWayData.RearTenthServed;
                    dbEntry.FrontEleventhServed = hoistWayData.FrontEleventhServed;
                    dbEntry.RearEleventhServed = hoistWayData.RearEleventhServed;
                    dbEntry.FrontTwelvethServed = hoistWayData.FrontTwelvethServed;
                    dbEntry.RearTwelvethServed = hoistWayData.RearTwelvethServed;
                    dbEntry.FrontThirteenthServed = hoistWayData.FrontThirteenthServed;
                    dbEntry.RearThirteenthServed = hoistWayData.RearThirteenthServed;
                    dbEntry.FrontFourteenthServed = hoistWayData.FrontFourteenthServed;
                    dbEntry.RearFourteenthServed = hoistWayData.RearFourteenthServed;
                    dbEntry.FrontFifteenthServed = hoistWayData.FrontFifteenthServed;
                    dbEntry.RearFifteenthServed = hoistWayData.RearFifteenthServed;
                    dbEntry.FrontSixteenthServed = hoistWayData.FrontSixteenthServed;
                    dbEntry.RearSixteenthServed = hoistWayData.RearSixteenthServed;
                    dbEntry.Capacity = hoistWayData.Capacity;
                    dbEntry.UpSpeed = hoistWayData.UpSpeed;
                    dbEntry.DownSpeed = hoistWayData.DownSpeed;
                    dbEntry.TotalTravel = hoistWayData.TotalTravel;
                    dbEntry.LandingSystemID = hoistWayData.LandingSystemID;
                }
            }
            context.SaveChanges();
        }
        public void SaveSpecialFeatures(SpecialFeatures specialFeatures)
        {
            if (specialFeatures != null && specialFeatures.SpecialFeaturesID == 0)
            {
                List<SpecialFeatures> specials = context.SpecialFeatures.Where(p => p.JobID == specialFeatures.JobID).ToList();
                if (specials.Count == 0) context.SpecialFeatures.Add(specialFeatures);
                else if (specials.Count == 1 && specialFeatures.Description != null)context.SpecialFeatures.Add(specialFeatures);
                else if (specials.Count > 1 && !string.IsNullOrEmpty(specialFeatures.Description)) context.SpecialFeatures.Add(specialFeatures);

            }
            else
            {
                SpecialFeatures dbEntry = context.SpecialFeatures
                .FirstOrDefault(p => p.SpecialFeaturesID == specialFeatures.SpecialFeaturesID);
                if (dbEntry != null)
                {
                    dbEntry.JobID = specialFeatures.JobID;
                    dbEntry.Description = specialFeatures.Description;

                }
            }
            context.SaveChanges();

        }
        public void SavePO(PO po)
        {
            if (po != null && po.POID == 0)
            {
                List<int> AllPoS = context.POs.Select(m => m.PONumb).ToList();
                if (!AllPoS.Contains(po.PONumb))
                {
                    context.POs.Add(po);
                }

            }
            else
            {
                PO dbEntry = context.POs
                .FirstOrDefault(p => p.POID == po.POID);
                if (dbEntry != null)
                {
                    dbEntry.JobID = po.JobID;
                    dbEntry.PONumb = po.PONumb;

                }
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {

            }

        }
        public void SaveCustomSoftware(CustomSoftware customSoftware)
        {
            if (customSoftware.CustomSoftwareID == 0)
            {
                context.CustomSoftwares.Add(customSoftware);
            }
            else
            {
                CustomSoftware dbEntry = context.CustomSoftwares
                .FirstOrDefault(p => p.CustomSoftwareID == customSoftware.CustomSoftwareID);
                if (dbEntry != null)
                {
                    dbEntry.Description = customSoftware.Description;
                }
            }
            context.SaveChanges();
        }
        public void SaveCustomFeature(CustomFeature customFeature)
        {
            if (customFeature.CustomFeatureID == 0)
            {
                context.CustomFeatures.Add(customFeature);
            }
            else
            {
                CustomFeature dbEntry = context.CustomFeatures
                .FirstOrDefault(p => p.CustomFeatureID == customFeature.CustomFeatureID);
                if (dbEntry != null)
                {
                    dbEntry.CustomSoftwareID = customFeature.CustomSoftwareID;
                    dbEntry.JobID = customFeature.JobID;
                }
            }
            context.SaveChanges();
        }
        public void SaveTriggeringCustSoft(TriggeringCustSoft triggering)
        {
            if (triggering != null && triggering.TriggeringCustSoftID == 0)
            {
                List<TriggeringCustSoft> specials = context.TriggeringCustSofts.Where(p => p.TriggeringCustSoftID == triggering.TriggeringCustSoftID).ToList();
                if (specials.Count != 1 || triggering.Name != null)
                {
                    context.TriggeringCustSofts.Add(triggering);
                }

            }
            else
            {
                TriggeringCustSoft dbEntry = context.TriggeringCustSofts
                .FirstOrDefault(p => p.TriggeringCustSoftID == triggering.TriggeringCustSoftID);
                if (dbEntry != null)
                {
                    dbEntry.CustomSoftwareID = triggering.CustomSoftwareID;
                    dbEntry.Name = triggering.Name;
                    dbEntry.isSelected = triggering.isSelected;
                    dbEntry.itemToMatch = triggering.itemToMatch;
                }
            }
            context.SaveChanges();

        }
        public void SaveElement(Element element)
        {
            if (element.ElementID == 0)
            {
                context.Elements.Add(element);
            }
            else
            {
                Element dbEntry = context.Elements
                .FirstOrDefault(p => p.ElementID == element.ElementID);
                if (dbEntry != null)
                {
                    dbEntry.JobID = element.JobID;
                    dbEntry.DoorGate = element.DoorGate;
                    dbEntry.DoorOperatorID = element.DoorOperatorID;

                    dbEntry.INA = element.INA;
                    dbEntry.INCP = element.INCP;
                    dbEntry.CarKey = element.CarKey;
                    dbEntry.CarCardReader = element.CarCardReader;
                    dbEntry.CRO = element.CRO;
                    dbEntry.HallKey = element.HallKey;
                    dbEntry.HallCardReader = element.HallCardReader;
                    dbEntry.HCRO = element.HCRO;

                    dbEntry.HAPS = element.HAPS;
                    dbEntry.LoadWeigher = element.LoadWeigher;
                    dbEntry.EP = element.EP;
                    dbEntry.EMT = element.EMT;
                    dbEntry.PSS = element.PSS;
                    dbEntry.PTFLD = element.PTFLD;
                    dbEntry.VCI = element.VCI;
                    dbEntry.DHLD = element.DHLD;
                    dbEntry.CReg = element.CReg;
                    dbEntry.Egress = element.Egress;
                    dbEntry.PHECutOut = element.PHECutOut;
                    dbEntry.CTINSPST = element.CTINSPST;
                    dbEntry.Traveler = element.Traveler;
                    dbEntry.Capacity = element.Capacity;
                    dbEntry.Speed = element.Speed;
                    dbEntry.Voltage = element.Voltage;
                    dbEntry.Phase = element.Phase;
                    dbEntry.Frequency = element.Frequency;
                    dbEntry.CallEnable = element.CallEnable;
                    dbEntry.CTL = element.CTL;
                    dbEntry.CTF = element.CTF;
                    dbEntry.CSD = element.CSD;
                    dbEntry.EQ = element.EQ;
                    dbEntry.LJ = element.LJ;
                    dbEntry.LOS = element.LOS;
                    dbEntry.PFGE = element.PFGE;
                    dbEntry.FRON2 = element.FRON2;

                }
            }
            context.SaveChanges();
        }
        public void SaveElementHydro(ElementHydro element)
        {
            if (element.ElementHydroID == 0)
            {
                context.ElementHydros.Add(element);
            }
            else
            {
                ElementHydro dbEntry = context.ElementHydros
                .FirstOrDefault(p => p.ElementHydroID == element.ElementHydroID);
                if (dbEntry != null)
                {
                    dbEntry.JobID = element.JobID;

                    dbEntry.Starter = element.Starter;
                    dbEntry.HP = element.HP;
                    dbEntry.FLA = element.FLA;
                    dbEntry.SPH = element.SPH;
                    dbEntry.ValveBrand = element.ValveBrand;
                }
            }
            context.SaveChanges();
        }
        public void SaveElementTraction(ElementTraction element)
        {
            if (element.ElementTractionID == 0)
            {
                context.ElementTractions.Add(element);
            }
            else
            {
                ElementTraction dbEntry = context.ElementTractions
                .FirstOrDefault(p => p.ElementTractionID == element.ElementTractionID);
                if (dbEntry != null)
                {
                    dbEntry.JobID = element.JobID;

                    dbEntry.MachineLocation = element.MachineLocation;
                    dbEntry.VVVF = element.VVVF;
                    dbEntry.MotorBrand = element.MotorBrand;
                    dbEntry.HP = element.HP;
                    dbEntry.FLA = element.FLA;
                    dbEntry.Encoder = element.Encoder;
                    dbEntry.ISO = element.ISO;
                    dbEntry.PickVoltage = element.PickVoltage;
                    dbEntry.HoldVoltage = element.HoldVoltage;
                    dbEntry.Resistance = element.Resistance;
                    dbEntry.Current = element.Current;
                    dbEntry.Contact = element.Contact;

                }
            }
            context.SaveChanges();
        }

        public Job DeleteJob(int JobID)
        {
            Job dbEntry = context.Jobs
                .FirstOrDefault(p => p.JobID == JobID);
            if (dbEntry != null)
            {
                context.Jobs.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public Job DeleteEngJob(int JobID)
        {
            Job dbEntry = context.Jobs
                .FirstOrDefault(p => p.JobID == JobID);
            JobExtension jobExtension = context.JobsExtensions
                .FirstOrDefault(p => p.JobID == JobID);
            HydroSpecific hydroSpecific = context.HydroSpecifics
                .FirstOrDefault(p => p.JobID == JobID);
            GenericFeatures genericFeatures = context.GenericFeaturesList
                .FirstOrDefault(p => p.JobID == JobID);
            Indicator indicator = context.Indicators
                .FirstOrDefault(p => p.JobID == JobID);
            HoistWayData hoistWayData = context.HoistWayDatas
                .FirstOrDefault(p => p.JobID == JobID);
            SpecialFeatures specialFeatures = context.SpecialFeatures
                .FirstOrDefault(p => p.JobID == JobID);
            PO pos = context.POs
                .FirstOrDefault(p => p.JobID == JobID);

            try
            {

                if (dbEntry != null)
                {
                    context.Jobs.Remove(dbEntry);
                    context.SaveChanges();
                }
                if (jobExtension != null)
                {
                    context.JobsExtensions.Remove(jobExtension);
                    context.SaveChanges();
                }
                if (hydroSpecific != null)
                {
                    context.HydroSpecifics.Remove(hydroSpecific);
                    context.SaveChanges();
                }
                if (genericFeatures != null)
                {
                    context.GenericFeaturesList.Remove(genericFeatures);
                    context.SaveChanges();
                }
                if (indicator != null)
                {
                    context.Indicators.Remove(indicator);
                    context.SaveChanges();
                }
                if (hoistWayData != null)
                {
                    context.HoistWayDatas.Remove(hoistWayData);
                    context.SaveChanges();
                }
                if (specialFeatures != null)
                {
                    context.SpecialFeatures.Remove(specialFeatures);
                    context.SaveChanges();
                }
                if (pos != null)
                {
                    context.POs.Remove(pos);
                    context.SaveChanges();
                }

            }
            catch (Exception e)
            {

            }

            return dbEntry;
        }
        public JobExtension DeleteJobExtension(int jobExtensionID)
        {
            JobExtension dbEntry = context.JobsExtensions
                .FirstOrDefault(p => p.JobExtensionID == jobExtensionID);
            if (dbEntry != null)
            {
                context.JobsExtensions.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public HydroSpecific DeleteHydroSpecific(int hydroSpecificID)
        {
            HydroSpecific dbEntry = context.HydroSpecifics
                .FirstOrDefault(p => p.HydroSpecificID == hydroSpecificID);
            if (dbEntry != null)
            {
                context.HydroSpecifics.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public GenericFeatures DeleteGenericFeatures(int genericFeaturesID)
        {
            GenericFeatures dbEntry = context.GenericFeaturesList
                .FirstOrDefault(p => p.GenericFeaturesID == genericFeaturesID);
            if (dbEntry != null)
            {
                context.GenericFeaturesList.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public Indicator DeleteIndicator(int indicatorID)
        {
            Indicator dbEntry = context.Indicators
                .FirstOrDefault(p => p.IndicatorID == indicatorID);
            if (dbEntry != null)
            {
                context.Indicators.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public HoistWayData DeleteHoistWayData(int hoistWayDataID)
        {
            HoistWayData dbEntry = context.HoistWayDatas
                .FirstOrDefault(p => p.HoistWayDataID == hoistWayDataID);
            if (dbEntry != null)
            {
                context.HoistWayDatas.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public Element DeleteElement(int ElementID)
        {
            Element dbEntry = context.Elements
                .FirstOrDefault(p => p.ElementID == ElementID);
            if (dbEntry != null)
            {
                context.Elements.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public ElementHydro DeleteElementHydro(int ElementHydroID)
        {
            ElementHydro dbEntry = context.ElementHydros
                .FirstOrDefault(p => p.ElementHydroID == ElementHydroID);
            if (dbEntry != null)
            {
                context.ElementHydros.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public ElementTraction DeleteElementTraction(int ElementTractionID)
        {
            ElementTraction dbEntry = context.ElementTractions
                .FirstOrDefault(p => p.ElementTractionID == ElementTractionID);
            if (dbEntry != null)
            {
                context.ElementTractions.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }


        public SpecialFeatures DeleteSpecialFeatures(int specialFeaturesID)
        {
            SpecialFeatures dbEntry = context.SpecialFeatures
                .FirstOrDefault(p => p.SpecialFeaturesID == specialFeaturesID);
            if (dbEntry != null)
            {
                context.SpecialFeatures.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public PO DeletePO(int POID)
        {
            PO dbEntry = context.POs
                .FirstOrDefault(p => p.POID == POID);
            if (dbEntry != null)
            {
                context.POs.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public CustomSoftware DeleteCustomSoftware(int CustomSoftwareID)
        {
            CustomSoftware dbEntry = context.CustomSoftwares
                .FirstOrDefault(p => p.CustomSoftwareID == CustomSoftwareID);
            if (dbEntry != null)
            {
                context.CustomSoftwares.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public CustomFeature DeleteCustomFeature(int CustomFeatureID)
        {
            CustomFeature dbEntry = context.CustomFeatures
                .FirstOrDefault(p => p.CustomFeatureID == CustomFeatureID);
            if (dbEntry != null)
            {
                context.CustomFeatures.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public TriggeringCustSoft DeleteTriggeringCustSoft(int TriggeringCustSoftID)
        {
            TriggeringCustSoft dbEntry = context.TriggeringCustSofts
                .FirstOrDefault(p => p.TriggeringCustSoftID == TriggeringCustSoftID);
            if (dbEntry != null)
            {
                context.TriggeringCustSofts.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public CustomSoftware DeleteJobCustomSoftware(int CustomSoftwareID)
        {
            CustomSoftware dbEntry = context.CustomSoftwares
                .FirstOrDefault(p => p.CustomSoftwareID == CustomSoftwareID);
            TriggeringCustSoft triggering = context.TriggeringCustSofts
                .FirstOrDefault(p => p.CustomSoftwareID == CustomSoftwareID);
            try
            {
                if (dbEntry != null)
                {
                    context.CustomSoftwares.Remove(dbEntry);
                    context.SaveChanges();
                }
                if (triggering != null)
                {
                    context.TriggeringCustSofts.Remove(triggering);
                    context.SaveChanges();
                }

            }
            catch (Exception e)
            {

            }

            return dbEntry;
        }

        public void SaveEngJobView(JobViewModel viewModelToSave)
        {
            SaveJob(viewModelToSave.CurrentJob);
            if (viewModelToSave.CurrentJobExtension != null)
            {
                if (viewModelToSave.CurrentJobExtension.JobID != 0)
                {
                    JobExtension jobExtension = JobsExtensions.FirstOrDefault(j => j.JobID == viewModelToSave.CurrentJobExtension.JobID);
                    if (jobExtension == null)
                    {
                        SaveJobExtension(viewModelToSave.CurrentJobExtension);
                    }
                    else
                    {
                        viewModelToSave.CurrentJobExtension.JobExtensionID = jobExtension.JobExtensionID;
                        SaveJobExtension(viewModelToSave.CurrentJobExtension);
                    }
                }
            }
            if (viewModelToSave.CurrentHydroSpecific != null)
            {
                if (viewModelToSave.CurrentHydroSpecific.JobID != 0)
                {
                    HydroSpecific hydroSpecific = HydroSpecifics.FirstOrDefault(j => j.JobID == viewModelToSave.CurrentHydroSpecific.JobID);
                    if (hydroSpecific == null)
                    {
                        SaveHydroSpecific(viewModelToSave.CurrentHydroSpecific);
                    }
                    else
                    {
                        viewModelToSave.CurrentHydroSpecific.HydroSpecificID = hydroSpecific.HydroSpecificID;
                        SaveHydroSpecific(viewModelToSave.CurrentHydroSpecific);
                    }
                }
            }
            if (viewModelToSave.CurrentGenericFeatures != null)
            {
                if (viewModelToSave.CurrentGenericFeatures.JobID != 0)
                {
                    GenericFeatures genericFeatures = GenericFeaturesList.FirstOrDefault(j => j.JobID == viewModelToSave.CurrentGenericFeatures.JobID);
                    if (genericFeatures == null)
                    {
                        SaveGenericFeatures(viewModelToSave.CurrentGenericFeatures);
                    }
                    else
                    {
                        viewModelToSave.CurrentGenericFeatures.GenericFeaturesID = genericFeatures.GenericFeaturesID;
                        SaveGenericFeatures(viewModelToSave.CurrentGenericFeatures);
                    }
                }
            }
            if (viewModelToSave.CurrentIndicator != null)
            {
                if (viewModelToSave.CurrentIndicator.JobID != 0)
                {
                    Indicator indicator = Indicators.FirstOrDefault(j => j.JobID == viewModelToSave.CurrentIndicator.JobID);
                    if (indicator == null)
                    {
                        SaveIndicator(viewModelToSave.CurrentIndicator);
                    }
                    else
                    {
                        viewModelToSave.CurrentIndicator.IndicatorID = indicator.IndicatorID;
                        SaveIndicator(viewModelToSave.CurrentIndicator);
                    }
                }
            }
            if (viewModelToSave.CurrentHoistWayData != null)
            {
                if (viewModelToSave.CurrentHoistWayData.JobID != 0)
                {
                    HoistWayData hoistway = HoistWayDatas.FirstOrDefault(j => j.JobID == viewModelToSave.CurrentHoistWayData.JobID);
                    if (hoistway == null)
                    {
                        SaveHoistWayData(viewModelToSave.CurrentHoistWayData);
                    }
                    else
                    {
                        viewModelToSave.CurrentHoistWayData.HoistWayDataID = hoistway.HoistWayDataID;
                        SaveHoistWayData(viewModelToSave.CurrentHoistWayData);
                    }
                }
            }

            if (viewModelToSave.SpecialFeatureslist != null)
            {
                for (int i = 0; i < viewModelToSave.SpecialFeatureslist.Count; i++)
                {
                    if (viewModelToSave.SpecialFeatureslist[i].JobID != 0)
                    {
                        if (viewModelToSave.SpecialFeatureslist == null)
                        {
                            SaveSpecialFeatures(viewModelToSave.SpecialFeatureslist[i]);
                        }
                        else
                        {
                            viewModelToSave.SpecialFeatureslist[i].SpecialFeaturesID = viewModelToSave.SpecialFeatureslist[i].SpecialFeaturesID;
                            SaveSpecialFeatures(viewModelToSave.SpecialFeatureslist[i]);
                        }
                    }
                }
            }
            if (viewModelToSave.POList != null)
            {
                for (int i = 0; i < viewModelToSave.POList.Count; i++)
                {
                    if (viewModelToSave.POList[i].JobID != 0)
                    {
                        if (viewModelToSave.POList == null)
                        {
                            SavePO(viewModelToSave.POList[i]);
                        }
                        else
                        {
                            viewModelToSave.POList[i].POID = viewModelToSave.POList[i].POID;
                            SavePO(viewModelToSave.POList[i]);
                        }
                    }
                }
            }
        }
        public void SaveJobCustomSoftware(CustomSoftwareViewModel viewModelToSave)
        {
            SaveCustomSoftware(viewModelToSave.CustomSoftware);
            if (viewModelToSave.TriggeringList != null)
            {
                for (int i = 0; i < viewModelToSave.TriggeringList.Count; i++)
                {
                    if (viewModelToSave.TriggeringList[i].CustomSoftwareID != 0)
                    {
                        if (viewModelToSave.TriggeringList == null)
                        {
                            SaveTriggeringCustSoft(viewModelToSave.TriggeringList[i]);
                        }
                        else
                        {
                            viewModelToSave.TriggeringList[i].TriggeringCustSoftID = viewModelToSave.TriggeringList[i].TriggeringCustSoftID;
                            SaveTriggeringCustSoft(viewModelToSave.TriggeringList[i]);
                        }
                    }
                }
            }
        }

        public void SaveEngElementHydroJobView(JobElementHydroViewModel viewModelToSave)
        {
            SaveJob(viewModelToSave.CurrentJob);
            if (viewModelToSave.Element != null)
            {
                if (viewModelToSave.Element.JobID != 0)
                {
                    Element element = Elements.FirstOrDefault(j => j.JobID == viewModelToSave.Element.ElementID);
                    if (element == null)
                    {
                        SaveElement(viewModelToSave.Element);
                    }
                    else
                    {
                        viewModelToSave.Element.ElementID = element.ElementID;
                        SaveElement(viewModelToSave.Element);
                    }
                }
            }
            if (viewModelToSave.ElementHydro != null)
            {
                if (viewModelToSave.ElementHydro.JobID != 0)
                {
                    ElementHydro hydro = ElementHydros.FirstOrDefault(j => j.JobID == viewModelToSave.ElementHydro.JobID);
                    if (hydro == null)
                    {
                        SaveElementHydro(viewModelToSave.ElementHydro);
                    }
                    else
                    {
                        viewModelToSave.ElementHydro.ElementHydroID = hydro.ElementHydroID;
                        SaveElementHydro(viewModelToSave.ElementHydro);
                    }
                }
            }

            if (viewModelToSave.SpecialFeatureslist != null)
            {
                for (int i = 0; i < viewModelToSave.SpecialFeatureslist.Count; i++)
                {
                    if (viewModelToSave.SpecialFeatureslist[i].JobID != 0)
                    {
                        if (viewModelToSave.SpecialFeatureslist == null)
                        {
                            SaveSpecialFeatures(viewModelToSave.SpecialFeatureslist[i]);
                        }
                        else
                        {
                            viewModelToSave.SpecialFeatureslist[i].SpecialFeaturesID = viewModelToSave.SpecialFeatureslist[i].SpecialFeaturesID;
                            SaveSpecialFeatures(viewModelToSave.SpecialFeatureslist[i]);
                        }
                    }
                }
            }
            if (viewModelToSave.POList != null)
            {
                for (int i = 0; i < viewModelToSave.POList.Count; i++)
                {
                    if (viewModelToSave.POList[i].JobID != 0)
                    {
                        if (viewModelToSave.POList == null)
                        {
                            SavePO(viewModelToSave.POList[i]);
                        }
                        else
                        {
                            viewModelToSave.POList[i].POID = viewModelToSave.POList[i].POID;
                            SavePO(viewModelToSave.POList[i]);
                        }
                    }
                }
            }
        }
        public void SaveEngElementTractionJobView(JobElementTractionViewModel viewModelToSave)
        {
            SaveJob(viewModelToSave.CurrentJob);
            if (viewModelToSave.Element != null)
            {
                if (viewModelToSave.Element.JobID != 0)
                {
                    Element element = Elements.FirstOrDefault(j => j.JobID == viewModelToSave.Element.ElementID);
                    if (element == null)
                    {
                        SaveElement(viewModelToSave.Element);
                    }
                    else
                    {
                        viewModelToSave.Element.ElementID = element.ElementID;
                        SaveElement(viewModelToSave.Element);
                    }
                }
            }
            if (viewModelToSave.ElementTraction != null)
            {
                if (viewModelToSave.ElementTraction.JobID != 0)
                {
                    ElementTraction hydro = ElementTractions.FirstOrDefault(j => j.JobID == viewModelToSave.ElementTraction.JobID);
                    if (hydro == null)
                    {
                        SaveElementTraction(viewModelToSave.ElementTraction);
                    }
                    else
                    {
                        viewModelToSave.ElementTraction.ElementTractionID = hydro.ElementTractionID;
                        SaveElementTraction(viewModelToSave.ElementTraction);
                    }
                }
            }

            if (viewModelToSave.SpecialFeatureslist != null)
            {
                for (int i = 0; i < viewModelToSave.SpecialFeatureslist.Count; i++)
                {
                    if (viewModelToSave.SpecialFeatureslist[i].JobID != 0)
                    {
                        if (viewModelToSave.SpecialFeatureslist == null)
                        {
                            SaveSpecialFeatures(viewModelToSave.SpecialFeatureslist[i]);
                        }
                        else
                        {
                            viewModelToSave.SpecialFeatureslist[i].SpecialFeaturesID = viewModelToSave.SpecialFeatureslist[i].SpecialFeaturesID;
                            SaveSpecialFeatures(viewModelToSave.SpecialFeatureslist[i]);
                        }
                    }
                }
            }

            if (viewModelToSave.POList != null)
            {
                for (int i = 0; i < viewModelToSave.POList.Count; i++)
                {
                    if (viewModelToSave.POList[i].JobID != 0)
                    {
                        if (viewModelToSave.POList == null)
                        {
                            SavePO(viewModelToSave.POList[i]);
                        }
                        else
                        {
                            viewModelToSave.POList[i].POID = viewModelToSave.POList[i].POID;
                            SavePO(viewModelToSave.POList[i]);
                        }
                    }
                }
            }

        }


        public Job DeleteEngElementHydroJob(int JobID)
        {
            Job dbEntry = context.Jobs
                .FirstOrDefault(p => p.JobID == JobID);
            Element element = context.Elements
                .FirstOrDefault(p => p.JobID == JobID);
            ElementHydro hydro= context.ElementHydros
                .FirstOrDefault(p => p.JobID == JobID);
            SpecialFeatures specialFeatures = context.SpecialFeatures
                .FirstOrDefault(p => p.JobID == JobID);
            PO pos = context.POs
                .FirstOrDefault(p => p.JobID == JobID);

            try
            {

                if (dbEntry != null)
                {
                    context.Jobs.Remove(dbEntry);
                    context.SaveChanges();
                }
                if (element != null)
                {
                    context.Elements.Remove(element);
                    context.SaveChanges();
                }
                if (hydro != null)
                {
                    context.ElementHydros.Remove(hydro);
                    context.SaveChanges();
                }
                if (specialFeatures != null)
                {
                    context.SpecialFeatures.Remove(specialFeatures);
                    context.SaveChanges();
                }

                if (pos != null)
                {
                    context.POs.Remove(pos);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {

            }

            return dbEntry;
        }
        public Job DeleteEngElementTractionJob(int JobID)
        {
            Job dbEntry = context.Jobs
                .FirstOrDefault(p => p.JobID == JobID);
            Element element = context.Elements
                .FirstOrDefault(p => p.JobID == JobID);
            ElementTraction traction = context.ElementTractions
                .FirstOrDefault(p => p.JobID == JobID);
            SpecialFeatures specialFeatures = context.SpecialFeatures
                .FirstOrDefault(p => p.JobID == JobID);
            PO pos = context.POs
                .FirstOrDefault(p => p.JobID == JobID);

            try
            {

                if (dbEntry != null)
                {
                    context.Jobs.Remove(dbEntry);
                    context.SaveChanges();
                }
                if (element != null)
                {
                    context.Elements.Remove(element);
                    context.SaveChanges();
                }
                if (traction != null)
                {
                    context.ElementTractions.Remove(traction);
                    context.SaveChanges();
                }
                if (specialFeatures != null)
                {
                    context.SpecialFeatures.Remove(specialFeatures);
                    context.SaveChanges();
                }
                if (pos != null)
                {
                    context.POs.Remove(pos);
                    context.SaveChanges();
                }

            }
            catch (Exception e)
            {

            }

            return dbEntry;
        }
    }
}
