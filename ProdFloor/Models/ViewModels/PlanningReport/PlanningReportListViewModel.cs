using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels
{
    public class PlanningReportListViewModel
    {
        public PlanningReport planningReport { get; set; }

        public List<PlanningReportRow> planningReportRows { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public string Name { get; set; }
    }
}
