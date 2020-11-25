﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Report
{
    public class TestStatsForReportViewModel
    {
        public List<Station> StationsM2000List { get; set; }
        public List<Station> StationsM4000List { get; set; }

        public List<TestStats> TestStatsList { get; set; }
    }
}
