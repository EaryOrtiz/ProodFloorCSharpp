using System.Collections.Generic;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class JobsListViewModel
    {
        public IEnumerable<Job> Jobs { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentJobType { get; set; }
    }
}
