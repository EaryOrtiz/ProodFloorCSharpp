using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Item
{
    public class ReferencesSearchvViewModel
    {

        public List<Models.Job> JobsSearchList { get; set; }
        public List<HydroSpecific> HydroSpecificList { get; set; }
        public List<HoistWayData> HoistWayDataList { get; set; }
        public bool RefernceData { get; set; }

        //Atributos de Job
        public int NumJobSearch { get; set; }

        //SlowLimit Table
        public int CarSpeedFPM { get; set; } ///Downs Speed
        public int Distance { get; set; }
        public int A { get; set; }
        public int SlowLimit { get; set; }
        public int MiniumFloorHeight { get; set; }

        //Wiretypesize Table
        public string Type { get; set; }
        public string Size { get; set; }
        public int AMPRating { get; set; } //FLA

        //Starter Table
        public string Brand { get; set; }
        public int FLA { get; set; }
        public string TypeS { get; set; }
        public string Volts { get; set; }
        public float HP { get; set; }
        public string MCPart { get; set; }
        public string NewManufacturerPart { get; set; }
        public string OverloadTable { get; set; }

        //Overload
        public float AMPMin { get; set; } //FLA
        public float AMPMax { get; set; }  //FLA
        public int OverTableNum { get; set; }
        public string MCPartOver { get; set; }
        public string SiemensPart { get; set; }
    }
}
