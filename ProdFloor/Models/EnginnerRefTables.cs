using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    public class Slowdown
    {
        public int SlowdownID { get; set; }

        [Display(Name ="Car Speed (FPM)")]
        [Required(ErrorMessage = "Please enter a Car Speed")]
        public int CarSpeedFPM { get; set; }

        [Required(ErrorMessage = "Please enter a Distance")]
        public int Distance { get; set; }

        [Display(Name = "Landing Page (A)")]
        [Required(ErrorMessage = "Please enter an A")]
        public int A { get; set; }

        [Display(Name = "Slow Limit")]
        [Required(ErrorMessage = "Please enter a Slow Limit")]
        public int SlowLimit { get; set; }

        [Display(Name = "Minium Floor Heigth")]
        [Required(ErrorMessage = "Please enter a Minium Floor Height")]
        public int MiniumFloorHeight { get; set; }
    }

    public class WireTypesSize
    {
        public int WireTypesSizeID { get; set; }

        [Required(ErrorMessage = "Please enter a Type")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Please enter an Size")]
        public string Size { get; set; }

        [Display(Name = "AMP Rating")]
        [Required(ErrorMessage = "Please enter an AMP Rating")]
        public int AMPRating { get; set; }
    }

    public class Starter
    {
        public int StarterID { get; set; }

        [Required(ErrorMessage = "Please enter a Brand")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Please enter a FLA")]
        public int FLA { get; set; }

        [Required(ErrorMessage = "Please enter a Type")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Please enter a Volt Number")]
        public string Volts { get; set; }

        [Required(ErrorMessage = "Please enter a HP Number")]
        public float HP { get; set; }

        [Display(Name = "MCE Part Number")]
        [Required(ErrorMessage = "Please enter a MCE Part Number")]
        public string MCPart { get; set; }

        [Display(Name = "New Manufacturer Part Number")]
        [Required(ErrorMessage = "Please enter a New Manufacturer Part Number")]
        public string NewManufacturerPart { get; set; }

        [Display(Name = "Overload Table Number")]
        [Required(ErrorMessage = "Please enter a Overload Table Number")]
        public string OverloadTable { get; set; }
    }

    public class Overload
    {
        public int OverloadID { get; set; }

        [Display(Name = "AMP Minium")]
        [Required(ErrorMessage = "Please enter a AMP Min")]
        public float AMPMin { get; set; }

        [Display(Name = "AMP Maximum")]
        [Required(ErrorMessage = "Please enter a AMP Max")]
        public float AMPMax { get; set; }

        [Display(Name = "Overload Table Number")]
        [Required(ErrorMessage = "Please enter a Overload Table Number")]
        public int OverTableNum { get; set; }

        [Display(Name = "MCE Part Number")]
        [Required(ErrorMessage = "Please enter a MCE Part Number")]
        public string MCPart { get; set; }

        [Display(Name = "Siemens Part Number")]
        [Required(ErrorMessage = "Please enter a Siemens Part")]
        public string SiemensPart { get; set; }
    }
}
