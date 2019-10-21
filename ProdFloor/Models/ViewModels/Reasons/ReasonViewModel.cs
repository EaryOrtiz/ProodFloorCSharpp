using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int TotalItems { get; set; }

        [Display(Name = "Reason 1")]
        public int Reason1ID { get; set; }
        [Display(Name = "Reason 2")]
        public int Reason2ID { get; set; }
        [Display(Name = "Reason 3")]
        public int Reason3ID { get; set; }
        [Display(Name = "Reason 4")]
        public int Reason4ID { get; set; }
        [Display(Name = "Reason 5")]
        public int Reason5ID { get; set; }

        public string Description { get; set; }

        public bool CleanFields { get; set; }
    }
}
