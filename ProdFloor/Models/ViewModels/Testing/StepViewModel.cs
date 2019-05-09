using System;
using System.Collections.Generic;
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
    }
}
