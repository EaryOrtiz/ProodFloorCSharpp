using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Wiring
{
    public class WiringViewModel
    {
        public Models.Wiring Wiring { get; set; }
        public List<WiringFeatures> FeatureList { get; set; }
        public WiringFeatures Feature { get; set; }
        public WiringStep StepInfo { get; set; }
        public WiringStepForJob StepsForJob { get; set; }
        public WiringStop Stop { get; set; }
        public int CurrentWirerID { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public Models.Job Job { get; set; }

        public List<JobType> JobTypeList { get; set; }
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

        public List<Models.Wiring> WiringJobList { get; set; }
        public List<Models.Job> JobList { get; set; }
        public List<WiringTriggeringFeature> WiringTriggerList { get; set; }
        public List<WiringStep> StepList { get; set; }
        public List<WiringStepForJob> StepsForJobList { get; set; }
        public List<Station> StationsList { get; set; }
        public List<PO> POList { get; set; }
        public List<StatusPO> StatusPOList { get; set; }
        public List<WiringStop> StopList { get; set; }
        public List<WiringReason1> Reasons1List { get; set; }
        public List<WiringReason2> Reasons2List { get; set; }
        public List<WiringReason3> Reasons3List { get; set; }
        public List<WiringReason4> Reasons4List { get; set; }
        public List<WiringReason5> Reasons5List { get; set; }
        public string Reason1Name { get; set; }
       
        public string buttonAction { get; set; }
        public string CurrentTab { get; set; }

        //Dummy fields
        public int NewWirerID { get; set; }
        public int NewStationID { get; set; }
        public bool Clean { get; set; }

        //Dos dashboard
        public PagingInfo PagingInfoIncompleted { get; set; }
        public PagingInfo PagingInfoCompleted { get; set; }
        public PagingInfo PagingInfoWorkingOnIt { get; set; }
        public List<Models.Wiring> WiringJobIncompletedList { get; set; }
        public List<Models.Wiring> WiringJobCompletedList { get; set; }
        public List<Models.Wiring> WiringJobWorkingOnItList { get; set; }
        public bool isNotDummy { get; set; }
        public string JobTypeName { get; set; }


        //para el nuevo search 


        public Element Element { get; set; }
        public ElementHydro ElementHydro { get; set; }
        public ElementTraction ElementTraction { get; set; }

        //Steps for Job
        public int CurrentStepInStage { get; set; }
        public int TotalStepsPerStage { get; set; }
        public int StepsLeft { get; set; }
        public string JobNum { get; set; }
        public bool StopNC { get; set; }
       
        public WiringStepForJob prevStep { get; set; }
        public WiringStepForJob nextStep { get; set; }
        public WiringStepForJob currentStep { get; set; }


        //JobCompletion
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime FinishDate { get; set; }

        [Required(ErrorMessage = "Please enter the hours to complete")]
        [Range(1, 720, ErrorMessage = "The hours are out of range")]
        [Display(Name = "Hours to complete")]
        public double ElapsedTimeHours { get; set; }

        [Range(1, 60, ErrorMessage = "The minutes are out of range")]
        [Required(ErrorMessage = "Please enter the minutes to complete")]
        [Display(Name = "Minutes to complete")]
        public double ElapsedTimeMinutes { get; set; }
    }
}
