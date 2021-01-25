using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFloor.Models
{
    public class AppUser : IdentityUser
    {
        [Display(Name = "Engineer Number(only if required)")]
        public int EngID { get; set; }

        [NotMapped]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [NotMapped]
        public string FullName {
            get
            {
                string[] FullNameArray = UserName.Split('.');
                if(FullNameArray.Length == 2)
                {
                    string FirstName = FullNameArray[0].Substring(0, 1).ToUpper() + FullNameArray[0].Substring(1);
                    string LastName = FullNameArray[1].Substring(0, 1).ToUpper() + FullNameArray[1].Substring(1);
                    string FullNameAux = FirstName + " " + LastName;
                    return FullNameAux;
                }
                else
                {
                    return "Default User";
                }
                

            }
        
        }

        [NotMapped]
        public string ShortFullName
        {
            get
            {
                string[] FullNameArray = UserName.Split('.');
                if (FullNameArray.Length == 2)
                {
                    string FirstName = FullNameArray[0].Substring(0, 1).ToUpper() + FullNameArray[0].Substring(1);
                    string LastName = FullNameArray[1].Substring(0, 1).ToUpper() + FullNameArray[1].Substring(1);
                    string FullNameAux = FirstName + " " + LastName.Remove(1);
                    return FullNameAux;
                }
                else
                {
                    return "Default User";
                }


            }

        }
    }
}