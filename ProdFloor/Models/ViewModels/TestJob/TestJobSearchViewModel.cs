using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.TestJob
{
    public class TestJobSearchViewModel
    {
        public List<Models.Job> JobsSearchList { get; set; }
        public List<Models.TestJob> TestJobsSearchList { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public bool CleanFields { get; set; }

        //Atributos de Job
        [Display(Name = "Job Name")]
        public string NameJobSearch { get; set; }
        [Display(Name = "Job Number")]
        public int NumJobSearch { get; set; }
        [Display(Name = "PO Name")]
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
        [Display(Name = "Status")]
        public string StatusJobSearch { get; set; }
        [Display(Name = "Job Name #2")]
        public string Name2 { get; set; }


        //Atributos de TestJob
        public int JobID { get; set; }
        public int TechnicianID { get; set; }
        public string Status { get; set; }
        public int SinglePO { get; set; }
        [Display(Name = "Job Label")]
        public string JobLabel { get; set; }
        [Display(Name = "Station")]
        public int StationID { get; set; }

        //Atributos de Features
        public string Overlay { get; set; }
        public string Group { get; set; }
        [Display(Name = "PC de Cliente")]
        public string PC { get; set; }
        [Display(Name = "Brake Coil Voltage > 10")]
        public string BrakeCoilVoltageMoreThan10 { get; set; }
        [Display(Name = "EMBrake Module")]
        public string EMBrake { get; set; }
        [Display(Name = "EMCO Board")]
        public string EMCO { get; set; }
        [Display(Name = "R6 Regen Unit")]
        public string R6 { get; set; }
        public string Local { get; set; }
        [Display(Name = "Short Floor")]
        public string ShortFloor { get; set; }
        public string Custom { get; set; }
        public string MRL { get; set; }
        public string CTL2 { get; set; }
        [Display(Name = "Tarjeta CPI Incluida")]
        public string TrajetaCPI { get; set; }
        [Display(Name = "Door Control en Cartop")]
        public string Cartop { get; set; }

        //Stop
        [Display(Name = "Reason 1")]
        public int Reason1 { get; set; }
        [Display(Name = "Reason 2")]
        public int Reason2 { get; set; }
        [Display(Name = "Reason 3")]
        public int Reason3 { get; set; }
        [Display(Name = "Reason 4")]
        public int Reason4 { get; set; }
        [Display(Name = "Reason 5")]
        public int Reason5ID { get; set; }
        public string Description { get; set; }
    }
}
