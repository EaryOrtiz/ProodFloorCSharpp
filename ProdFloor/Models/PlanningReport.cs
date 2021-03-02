using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFloor.Models
{
    public class PlanningReport
    {
        public int PlanningReportID { get; set; }

        public DateTime PlanningDate { get; set; }

        public DateTime DateTimeLoad { get; set; }

        public bool Busy { get; set; }

        public List<PlanningReportRow> Rows { get; set; }

        /*
        public PlanningReport(DateTime planningDate, DateTime dateTimeLoad, bool busy)
        {
            PlanningDate = planningDate;
            DateTimeLoad = dateTimeLoad;
            Busy = busy;
        }
        */
    }

    public class PlanningReportRow
    {
        public int PlanningReportRowID { get; set; }

        public int PlanningReportID { get; set; }

        public int Consecutive { get; set; }

        public string JobNumber { get; set; }

        public int PO { get; set; }

        public string JobName { get; set; }

        public string PreviousWorkCenter { get; set; }

        public string WorkCenter { get; set; }

        public string Notes { get; set; }

        public string Priority { get; set; }

        public string Material { get; set; }

        public string MRP { get; set; }

        public string SoldTo { get; set; }

        public bool Custom { get; set; }

        /*

        public PlanningReportRow(int consecutive, string jobNumber, int pO, string jobName, string previousWorkCenter,
            string workCenter, string notes, string priority, string material, string mRP, string soldTo)
        {
            Consecutive = consecutive;
            JobNumber = jobNumber;
            PO = pO;
            JobName = jobName;
            PreviousWorkCenter = previousWorkCenter;
            WorkCenter = workCenter;
            Notes = notes;
            Priority = priority;
            Material = material;
            MRP = mRP;
            SoldTo = soldTo;
        }
        */

        [NotMapped]
        public bool WorkCenterChanged
        {
            get
            {
                if (WorkCenter == PreviousWorkCenter)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
