using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProdFloor.Models;

namespace ProdFloor.Models.ViewModels
{
    public class StateEditViewModel
    {
        public State CurrentState { get; set; }
        public List<SelectListItem> Countries { get; set; } 
    }
}