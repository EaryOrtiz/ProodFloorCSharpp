using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Wiring
{
    public class WiringStepViewModel
    {
        public List<WiringStep> WiringStepList { get; set; }
        public List<JobType> JobTypesList { get; set; }
        public List<WiringTriggeringFeature> WiringTriggeringList { get; set; }
        public WiringStep WiringStep { get; set; }
        public string buttonAction { get; set; }
        public string CurrentTab { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string JobTypeSelected { get; set; }

        public List<WiringStep> StepsForElmHydroList { get; set; }
        public List<WiringStep> StepsForElmTractionList { get; set; }
        public List<WiringStep> StepsForM200List { get; set; }
        public List<WiringStep> StepsForM4000List { get; set; }
        public List<WiringStep> AllStepsList { get; set; }

        //timessssssssssssss
        public TimeSpan Time { get; set; }


        public PagingInfo ElmHydroPagingInfo { get; set; }
        public PagingInfo ElmTractionPagingInfo { get; set; }
        public PagingInfo M2000PagingInfo { get; set; }
        public PagingInfo M4000PagingInfo { get; set; }
    }
}
