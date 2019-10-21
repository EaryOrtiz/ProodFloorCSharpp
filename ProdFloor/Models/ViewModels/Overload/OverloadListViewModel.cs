using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.Overload
{
    public class OverloadListViewModel
    {
        public List<Models.Overload> Overloads { get; set; }

        public PagingInfo PagingInfo { get; set; }
        public int TotalItems { get; set; }
        public bool CleanFields { get; set; }

        [Display(Name = "AMP Minium")]
        public float AMPMin { get; set; }

        [Display(Name = "AMP Maximum")]
        public float AMPMax { get; set; }

        [Display(Name = "Overload Part Number")]
        public int OverTableNum { get; set; }

        [Display(Name = "MCE Part Number")]
        public string MCPart { get; set; }

        [Display(Name = "Siemens Part Number")]
        public string SiemensPart { get; set; }
    }
}
