using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Job
{
    public class CustomSoftwareViewModel
    {

        public List<CustomSoftware> CustomSoftwareList { get; set; }
        public List<TriggeringCustSoft> TriggeringList { get; set; }
        public CustomSoftware CustomSoftware { get; set; }
        public string buttonAction { get; set; }
        public string CurrentTab { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
