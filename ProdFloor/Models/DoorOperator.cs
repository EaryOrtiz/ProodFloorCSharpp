using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ProdFloor.Models
{
    public class DoorOperator
    {
        public int DoorOperatorID { get; set; }

        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter {0} field")]
        public string Style { get; set; }

        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter {0} field")]
        public string Brand { get; set; }

        [StringLength(50, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter {0} field")]
        public string Name { get; set; }

        public List<JobExtension> _JobExtensions { get; set; }

    }
}
