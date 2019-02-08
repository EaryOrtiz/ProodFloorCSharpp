using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFloor.Models
{
    public class City
    {
        
        public int CityID { get; set; }
        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter {0} field")]
        public string Name { get; set; }

        [Display(Name = "State")]
        public int StateID { get; set; }

        [NotMapped]
        [Display(Name = "Country")]
        public string CountryID { get; set; }

        [Display(Name = "Fire Code")]
        public int FirecodeID { get; set; }
        public List<Job> _Jobs { get; set; }


        /*public string Country { get; set; }
        //public string State { get; set; }
        public string CurrentFireCode { get; set; }
        */
    }
}
