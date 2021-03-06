﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ProdFloor.Models
{
    public class JobType
    {
        public int JobTypeID { get; set; }

        [StringLength(10, ErrorMessage = "The maximum length of the {0} field is {1}")]
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Name { get; set; }

        public List<Job> _Jobs { get; set; }
        public List<Station> _Stations { get; set; }
        public List<Step> Steps { get; set; }

    }
}
