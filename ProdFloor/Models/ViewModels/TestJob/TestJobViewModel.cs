using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.TestJob
{
    public class TestJobViewModel
    {
        public Models.TestJob TestJob { get; set; }
        public TestFeature TestFeature { get; set; }
        public Step Step { get; set; }
        public StepsForJob StepsForJob { get; set; }
        public Stop Stop { get; set; }
        public int CurrentTechnicianID { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public Models.Job Job { get; set; }
        public JobExtension JobExtension { get; set; }
        public HydroSpecific HydroSpecific { get; set; }
        public HoistWayData HoistWayData { get; set; }
        public GenericFeatures GenericFeatures { get; set; }
        public Indicator Indicator { get; set; }
        public SpecialFeatures SpecialFeature { get; set; }
        public PO PO { get; set; }
        [Range(3000000, 4900000, ErrorMessage = "PO number is out of range")]
        [Required(ErrorMessage = "Please enter a PO")]
        public int POJobSearch { get; set; }

        public List<Models.TestJob> TestJobList { get; set; }
        public List<Models.Job> JobList { get; set; }
        public List<TriggeringFeature> TriggerList { get; set; }
        public List<TestFeature> TestFeatureList { get; set; }
        public List<Step> StepList { get; set; }
        public List<StepsForJob> StepsForJobList { get; set; }
        public List<Station> StationsList { get; set; }
        public List<PO> POList { get; set; }
        public List<Stop> StopList { get; set; }
        public List<Reason1> Reasons1List { get; set; }
        public List<Reason2> Reasons2List { get; set; }
        public List<Reason3> Reasons3List { get; set; }
        public List<Reason4> Reasons4List { get; set; }
        public List<Reason5> Reasons5List { get; set; }
        public string Reason1Name { get; set; }
        public int CurrentStep { get; set; }
        public int TotalStepsPerStage { get; set; }

        public string buttonAction { get; set; }
        public string CurrentTab { get; set; }


        //Dummy fields
        public bool Canada { get; set; }
        public bool Ontario { get; set; }
        public bool MOD { get; set; }
        public bool Manual { get; set; }
        public bool IMonitor { get; set; }
        public bool MView { get; set; }
        [Display(Name = "+2 Starters")]
        public bool TwosStarters { get; set; }
        public bool StopNC { get; set; }
        public int NewTechnicianID { get; set; }
        public int NewStationID { get; set; }

        public bool Clean { get; set; }

        //Dos dashboard
        public PagingInfo PagingInfoIncompleted { get; set; }
        public PagingInfo PagingInfoCompleted { get; set; }

        public List<Models.TestJob> TestJobIncompletedList { get; set; }
        public List<Models.TestJob> TestJobCompletedList { get; set; }

        public bool isDummy { get; set; }


    }
}
