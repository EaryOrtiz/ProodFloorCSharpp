using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProdFloor.Models
{
    public class AppUser : IdentityUser
    {
        [Display(Name = "Engineer Number(only if required)")]
        public int EngID { get; set; }
    }
}