using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels
{
    public class PlanningReportListViewModel
    {
        public PlanningReport planningReport { get; set; }

        public PlanningReportRow ReportRow { get; set; }


        public List<PlanningReportRow> planningReportRows { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public string Name { get; set; }

        [Range(3000000, 4900000, ErrorMessage = "PO number is out of range")]
        [Required(ErrorMessage = "Please enter a PO")]
        public int POSearch { get; set; }

        public string PrintableType { get; set; }

        public bool Custom { get; set; }

        public string PrintableNotes { get; set; }

        public DateTime DueDate { get; set; }

        public string CarNumber { get; set; }
       
        public string ConfigGuy { get; set; }

    }
}
