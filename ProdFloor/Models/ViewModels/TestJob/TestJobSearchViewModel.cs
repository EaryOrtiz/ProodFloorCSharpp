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
        public Models.Job Job { get; set; }
        public JobExtension JobExtension { get; set; }
        public HydroSpecific HydroSpecific { get; set; }
        public HoistWayData HoistWayData { get; set; }
        public GenericFeatures GenericFeatures { get; set; }

        public Models.TestJob TestJob { get; set; }
        public Stop Stop { get; set; }

        public PagingInfo PagingInfo { get; set; }
        public bool CleanFields { get; set; }

        //Dummy fields
        public string Canada { get; set; }
        public string Ontario { get; set; }
        public string MOD { get; set; }
        public string Manual { get; set; }
        public string IMonitor { get; set; }
        public string HAPS { get; set; }
        public string Duplex { get; set; }
        public string SHC { get; set; }
        public string EDGELS { get; set; }
        public string RailLS { get; set; }
        public string MView { get; set; }
        [Display(Name = "+2 Starters")]
        public string TwosStarters { get; set; }
        public string Critical { get; set; }

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

    }
}
