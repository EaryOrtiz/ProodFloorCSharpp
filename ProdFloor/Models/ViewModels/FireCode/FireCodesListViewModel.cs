using System.Collections.Generic;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class FireCodesListViewModel
    {
        public IEnumerable<FireCode> FireCodes { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
