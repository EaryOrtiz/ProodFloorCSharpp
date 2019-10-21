using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.SlowDown
{
    public class SlowdownListViewModel
    {
        public List<Slowdown> Slowdowns { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public bool CleanFields { get; set; }
        public int TotalItems { get; set; }

        [Display(Name = "Car Speed (FPM)")]
        public int CarSpeedFPM { get; set; }

        public int Distance { get; set; }

        [Display(Name = "Landing Page (A)")]
        public int A { get; set; }

        [Display(Name = "Slow Limit")]
        public int SlowLimit { get; set; }

        [Display(Name = "Minium Floor Heigth")]
        public int MiniumFloorHeight { get; set; }
    }
}
