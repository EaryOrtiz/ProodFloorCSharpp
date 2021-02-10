using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.TestJob
{
    public class TestJobWorkSheetViewModel
    {
        public string JobNumber { get; set; }
        public int PO { get; set; }

        public string ShippingDate { get; set; }

        public string ElapsedTimeSteps { get; set; } 
        public int StopsNumber { get; set; } 
        public string ElapsedTimeStop { get; set; } 
        public int ReassignsNumber { get; set; } 
        public string TechsInThisTestJob { get; set; } 
        public string StationsInThisJob { get; set; }
    }
}
