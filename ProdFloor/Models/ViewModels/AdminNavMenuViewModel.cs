using System.Collections.Generic;
using ProdFloor.Models;


namespace ProdFloor.Models.ViewModels
{
    public class AdminNavMenuViewModel
    {
        public IEnumerable<string> Jobtypes { get; set; } // job type
        public Dictionary<string,List<string>> Cities { get; set; } //country , state
        public IEnumerable<string> States { get; set; } // country
        public IEnumerable<string> DoorOperators { get; set; } // brand
        public IEnumerable<string> LandingSystems { get; set; } // used in
    }
}
