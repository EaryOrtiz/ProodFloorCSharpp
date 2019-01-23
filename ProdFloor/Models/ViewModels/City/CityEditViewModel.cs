using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class CityEditViewModel
    {
        public City CurrentCity { get; set; }
        public SelectList Countries { get; set; }
        public SelectList States { get; set; }
        public SelectList FireCodes { get; set; }
    }
}
