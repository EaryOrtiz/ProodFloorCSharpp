using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Item
{
    public class ReferencesSrchElementViewModel
    {

        public List<Models.Job> JobsSearchList { get; set; }
        public List<HydroSpecific> HydroSpecificList { get; set; }
        public List<HoistWayData> HoistWayDataList { get; set; }
        public List<PO> POList { get; set; }
        public List<CustomSoftware> CustomSoftList { get; set; }
        public bool RefernceData { get; set; }
        public int JobID { get; set; }

        //Hydro
        public string NumJobSearch { get; set; }

        //Aux fields for New JobNumber
        public string JobNumFirstDigits { get; set; }
        public int JobNumLastDigits { get; set; }

        public int PO { get; set; }
        public int Voltage { get; set; }
        public string Volts { get; set; }
        public string DoorOperator { get; set; }
        public bool HAPS { get; set; }

        public string Starter { get; set; }
        public float FLA { get; set; }
        public int SPH { get; set; }
        public string StarterType { get; set; }
        public string MCPart { get; set; }
        public string NewManufacturerPart { get; set; }

        //Traction
        public float HP { get; set; }
        public int Speed { get; set; }
        public int Capacity { get; set; }

        public int PickVoltage { get; set; }
        public int HoldVoltage { get; set; }
        public int Resistance { get; set; }

        public int IdealVIn { get; set; }
        public int MinVIn { get; set; }
        public int MaxVIn { get; set; }
        public string BrakeResistor { get; set; }

    }
}
