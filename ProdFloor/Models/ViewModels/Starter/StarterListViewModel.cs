using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Starter
{
    public class StarterListViewModel
    {
        public List<Models.Starter> Starters { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public bool CleanFields { get; set; }
        public int TotalItems { get; set; }


        public float FLA { get; set; }
        public float HP { get; set; }


        public string StarterType { get; set; }

        public string Volts { get; set; }

        [Display(Name = "MCE Part Number")]
        public string MCPart { get; set; }

        [Display(Name = "New Manufacturer")]
        public string NewManufacturerPart { get; set; }

        [Display(Name = "Overload Number")]
        public string OverloadTable { get; set; }
    }
}
