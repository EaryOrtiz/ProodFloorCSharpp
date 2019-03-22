using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Reasons
{
    public class ReasonViewModel
    {
        public List<Reason1> Reasons1 { get; set; }
        public List<Reason2> Reasons2 { get; set; }
        public List<Reason3> Reasons3 { get; set; }
        public List<Reason4> Reasons4 { get; set; }
        public List<Reason5> Reasons5 { get; set; }
        public Reason1 Reason1 { get; set; }
        public Reason2 Reason2 { get; set; }
        public Reason3 Reason3 { get; set; }
        public Reason4 Reason4 { get; set; }
        public Reason5 Reason5 { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentSeparator { get; set; }
    }
}
