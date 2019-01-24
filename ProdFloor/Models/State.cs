using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ProdFloor.Models
{
    public class State
    { 
        public int StateID { get; set; }

        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Name { get; set; }

        public int CountryID { get; set; }
        //public string Country { get; set; }
        public List<City> _Cities { get; set; }
    }
}
