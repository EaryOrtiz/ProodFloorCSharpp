using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models.ViewModels.WireTypesSize
{
    public class WireTypesSizeListViewModel
    {
        public List<Models.WireTypesSize> WireTypes { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public bool CleanFields { get; set; }

        public string Type { get; set; }

        public string Size { get; set; }

        [Display(Name = "AMP Rating")]
        public int AMPRating { get; set; }
    }
}
