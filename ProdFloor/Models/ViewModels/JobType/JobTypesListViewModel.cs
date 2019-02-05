using System.Collections.Generic;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class JobTypesListViewModel
    {
        public List<JobType> JobTypes { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
