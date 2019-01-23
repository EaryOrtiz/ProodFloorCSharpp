using System.Collections.Generic;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class JobTypesListViewModel
    {
        public IEnumerable<JobType> JobTypes { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
