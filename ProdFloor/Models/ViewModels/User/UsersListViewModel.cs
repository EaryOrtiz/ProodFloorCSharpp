using System.Collections.Generic;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class UsersListViewModel
    {
        public IEnumerable<AppUser> Users { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}