using System;

namespace ProdFloor.Models.ViewModels
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public string sort { get; set; }
        public int TotalItemsFromLastSearch { get; set; }
        public string JobNumb { get;set; } 
        public string JobTypeName { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages =>
        (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
    }
}
