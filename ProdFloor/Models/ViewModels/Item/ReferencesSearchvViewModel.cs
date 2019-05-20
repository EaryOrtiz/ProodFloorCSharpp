using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Item
{
    public class ReferencesSearchvViewModel
    {

        public List<Models.Job> JobsSearchList { get; set; }
        public List<HydroSpecific> HydroSpecificList { get; set; }
        public List<HoistWayData> HoistWayDataList { get; set; }
        public List<PO> POList { get; set; }
        public List<CustomSoftware> CustomSoftList { get; set; }
        public bool RefernceData { get; set; }

        //Atributos de Job
        [Range(2015000000, 3030000000, ErrorMessage = "Job number is out of range")]
        [Required(ErrorMessage = "Please enter a Job Num")]
        public int NumJobSearch { get; set; }
        public string status { get; set; }

        public int SPH { get; set; }
        public string JobName { get; set; }
        public string JobName2 { get; set; }
        public string StarterType { get; set; }
        public string Cust { get; set; }
        public string Contractor { get; set; }
        public string FireCode { get; set; }
        public string JobTypeMain { get; set; }
        public string ValveBrand { get; set; }
        public string LandingName { get; set; }
        public int PO { get; set; }
        public int InputVoltage { get; set; }
        public int LandingID { get; set; }
        public string FireCodeName { get; set; }
        public int DownSpeed { get; set; }
        public int UpSpeed { get; set; }
        public int TotalTravel { get; set; }


        //SlowLimit Table

        //Down speed
        public int CarSpeedFPM { get; set; } ///Downs Speed
        public int Distance { get; set; }
        public int A { get; set; }
        public int SlowLimit { get; set; }
        public int MiniumFloorHeight { get; set; }

        //Up speed
        public int CarUpSpeedFPM { get; set; } ///UP Speed
        public int UPDistance { get; set; }
        public int UPA { get; set; }
        public int UPSlowLimit { get; set; }
        public int UPMiniumFloorHeight { get; set; }

        //Wiretypesize Table
        public string Type { get; set; }
        public string Size { get; set; }
        public int AMPRating { get; set; } //FLA

        //Starter Table
        public string Brand { get; set; }
        public float FLA { get; set; }
        public string TypeS { get; set; }
        public string Volts { get; set; }
        public float HP { get; set; }
        public string MCPart { get; set; }
        public string NewManufacturerPart { get; set; }
        public string OverloadTable { get; set; }
        public int InputPhase { get; set; }

        //Overload
        public float AMPMin { get; set; } //FLA
        public float AMPMax { get; set; }  //FLA
        public int OverTableNum { get; set; }
        public string MCPartOver { get; set; }
        public string SiemensPart { get; set; }

        //SHC Calculator
        public int calculatedFrontSHC { get; set; }
        public int calculatedRearSHC { get; set; }
        public int FrontFloor { get; set; }
        public int RearFloor { get; set; }
        public bool anyRear { get; set; }
        public bool SHCisSelected { get; set;}
    }
}
