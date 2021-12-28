using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Job
{
    public class JobM3000ViewModel
    {
        public Models.Job CurrentJob { get; set; }
        public M3000 M3000 { get; set; }
        public MotorInfo MotorInfo { get; set; }
        public OperatingFeatures OperatingFeatures { get; set; }
        public List<SpecialFeatures> SpecialFeatureslist { get; set; }
        public List<SpecialFeaturesEX> SpecialFeaturesTable { get; set; }
        public List<PO> POList { get; set; }
        public string CurrentTab { get; set; }
        public string buttonAction { get; set; }
        public int CurrentUserID { get; set; }
        public int fieldID { get; set; }
        public string JobTypeName { get; set; }
        public string JobFolder { get; set; }
    }
}
