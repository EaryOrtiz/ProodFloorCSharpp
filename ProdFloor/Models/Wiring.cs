using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{

    //clasess that are mostly the same
    public class Wiring
    {
        public int WiringID { get; set; }
        public int POID { get; set; }
        public int WirerID { get; set; }

        [Display(Name = "Station")]
        public int StationID { get; set; }

        //Total time
        public DateTime StartDate { get; set; }
        public DateTime CompletedDate { get; set; }

        public List<WiringStop> _WiringStops { get; set; }
        public List<WiringStepForJob> _WiringStepsForJobs { get; set; }
        public List<WirersInvolved> _WirersInvolved { get; set; }
        public WiringFeatures _WiringFeatures { get; set; }
    }

    public class WirersInvolved
    {
        public int WirersInvolvedID { get; set; }

        public int WiringID { get; set; }

        public int WirerID { get; set; }
    }

    public class WiringStop
    {
        public int WiringStopID { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public int WiringID { get; set; }
        [Display(Name = "Reason 1")]
        public int Reason1 { get; set; }
        [Display(Name = "Reason 2")]
        public int Reason2 { get; set; }
        [Display(Name = "Reason 3")]
        public int Reason3 { get; set; }
        [Display(Name = "Reason 4")]
        public int Reason4 { get; set; }
        [Display(Name = "Reason 5")]
        [Required(ErrorMessage = "Please enter a {0}")]
        public int Reason5ID { get; set; }
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Please enter a Stop Date")]
        public DateTime StopDate { get; set; }
        public DateTime Elapsed { get; set; }
        public string Description { get; set; }

        //New Boooelan to critical stops
        public bool Critical { get; set; }

        //Auxiliaries fields
        public int AuxWirerID { get; set; }
        public int AuxStationID { get; set; }

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.

        [NotMapped]
        public int GetWeekOfYear
        {
            get
            {
                // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
                // be the same week# as whatever Thursday, Friday or Saturday are,
                // and we always get those right
                DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(StartDate);
                if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
                {
                    StartDate = StartDate.AddDays(3);
                }

                // Return the week of our adjusted day
                return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(StartDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            }

        }

    }

    public class WiringStep
    {
        public int WiringStepID { get; set; }

        [Display(Name = "Job Type")]
        public int JobTypeID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Stage { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm:ss}")]
        public DateTime ExpectedTime { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public int Order { get; set; }

        //public List<TriggeringFeature> _TriggeringFeatures { get; set; }
        public List<WiringStepForJob> _WiringStepsForJob { get; set; }
        public List<WiringTriggeringFeature> _WiringTriggeringFeatures { get; set; }
    }

    public class WiringStepForJob
    {
        public int WiringStepForJobID { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public int WiringStepID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public int WiringID { get; set; }
        [Required(ErrorMessage = "Please enter a {0} Date")]
        public DateTime Start { get; set; }
        [Required(ErrorMessage = "Please enter a {0} Date")]
        public DateTime Stop { get; set; }
        [Required(ErrorMessage = "Please enter a {0} Date")]
        public DateTime Elapsed { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public bool Complete { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public int Consecutivo { get; set; }


        //New boolean that is activated when a duplicate steps exists
        public bool Obsolete { get; set; }

        //Auxiliaries fields
        public int AuxWirerID { get; set; }
        public int AuxStationID { get; set; }

    }

    public class WiringReason1
    {
        public int WiringReason1ID { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }

        public List<WiringReason2> _WiringReason2s { get; set; }
    }

    public class WiringReason2
    {
        public int WiringReason2ID { get; set; }

        [Display(Name = "Reason 1")]
        public int WiringReason1ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }

        public List<WiringReason3> _WiringReason3s { get; set; }
    }

    public class WiringReason3
    {
        public int WiringReason3ID { get; set; }

        [NotMapped]
        [Display(Name = "Reason 1")]
        public int WiringReason1ID { get; set; }

        [Display(Name = "Reason 2")]
        public int WiringReason2ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }

        public List<WiringReason4> _WiringReason4s { get; set; }
    }

    public class WiringReason4
    {
        public int WiringReason4ID { get; set; }

        [NotMapped]
        [Display(Name = "Reason 1")]
        public int WiringReason1ID { get; set; }
        [NotMapped]
        [Display(Name = "Reason 2")]
        public int WiringReason2ID { get; set; }

        [Display(Name = "Reason 3")]
        public int WiringReason3ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }

        public List<WiringReason5> _WiringReason5s { get; set; }
    }

    public class WiringReason5
    {
        public int WiringReason5ID { get; set; }

        [NotMapped]
        [Display(Name = "Reason 1")]
        public int WiringReason1ID { get; set; }
        [NotMapped]
        [Display(Name = "Reason 2")]
        public int WiringReason2ID { get; set; }
        [NotMapped]
        [Display(Name = "Reason 3")]
        public int WiringReason3ID { get; set; }

        [Display(Name = "Reason 4")]
        public int WiringReason4ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }

        public List<WiringStop> _WiringStops { get; set; }
    }

    //New classes
    public class WiringOption
    {
        public int WiringOptionID { get; set; }
        public string Description { get; set; }
        public bool isDeleted { get; set; }
    }

    public class WiringFeatures
    {
        public int WiringFeaturesID { get; set; }
        public int WiringID { get; set; }

        public int WiringOptionID { get; set; }
        public int Quantity { get; set; }

    }

    public class WiringTriggeringFeature
    {
        public int WiringTriggeringFeatureID { get; set; }
        public int WiringOptionID { get; set; }
        public int WiringStepID { get; set; }

        public int Quantity { get; set; }
        public string Equality { get; set; }

    }

}
