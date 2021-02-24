using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Report
{
    public class ReportsViewModel
    {
        public List<Station> StationsM2000List { get; set; }
        public List<Station> StationsM4000List { get; set; }

        public List<TestStats> TestStatsList { get; set; }

        public List<DailyReport> dailyReports { get;  set; }
        public List<DailyReport> WeekReports { get;  set; }
        public List<DailyReport> PastMonthReports { get;  set; }
        public List<DailyReport> MonthReports { get;  set; }


        public string CurrentTab { get; set; }
    }
}
