using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ProdFloor.Models
{
    public class LandingSystem
    {
        public int LandingSystemID { get; set; }

        [StringLength(15, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter a {0}")]
        public string UsedIn { get; set; }

        [StringLength(20, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Name { get; set; }

        public List<HoistWayData> HoistWayDatas { get; set; }
    }
}
