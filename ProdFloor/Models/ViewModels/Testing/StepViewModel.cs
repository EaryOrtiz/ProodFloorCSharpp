using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Testing
{
    public class StepViewModel
    {
        public List<Step> StepList { get; set; }
        public List<JobType> JobTypesList { get; set; }
        public List<TriggeringFeature> TriggeringList { get; set; }
        public Step Step { get; set; }
        public string buttonAction { get; set; }
        public string CurrentTab { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string JobTypeSelected { get; set; }

        public List<Step> StepsForElmHydroList { get; set; }
        public List<Step> StepsForElmTractionList { get; set; }
        public List<Step> StepsForM200List { get; set; }
        public List<Step> StepsForM4000List { get; set; }

        //timessssssssssssss
        public TimeSpan Time { get; set; }


        public PagingInfo ElmHydroPagingInfo { get; set; }
        public PagingInfo ElmTractionPagingInfo { get; set; }
        public PagingInfo M2000PagingInfo { get; set; }
        public PagingInfo M4000PagingInfo { get; set; }
        
    }
}
