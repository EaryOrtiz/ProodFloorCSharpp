using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ProdFloor.Models
{
    public class FireCode
    {
        public int FireCodeID { get; set; }

        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter {0} field")]
        public string Name { get; set; }

        public List<Job> _Jobs { get; set; }
        public List<City> _Cities { get; set; }
    }
}
