using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    public class TestJob
    {
        public int TestJobID { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public int JobID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public int TechnicianID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Status { get; set; }
        public int SinglePO { get; set; }
        [Display(Name = "Job Label")]
        public string JobLabel { get; set; }

        //Total time
        public DateTime StartDate { get; set; }
        public DateTime CompletedDate { get; set; }

        //New field for station
        [Display(Name = "Station")]
        public int StationID { get; set; }

        public List<Stop> _Stops { get; set; }
        public List<StepsForJob> _StepsForJobs { get; set; }
        public TestFeature _TestFeature { get; set; }
        
    }

    public class TestFeature
    {
        public int TestFeatureID { get; set; }

        public int TestJobID { get; set; }
        public bool Overlay { get; set; }
        public bool Group { get; set; }
        [Display(Name = "PC de Cliente")]
        public bool PC { get; set; }
        [Display(Name = "Brake Coil Voltage > 10")]
        public bool BrakeCoilVoltageMoreThan10 { get; set; }
        [Display(Name = "EMBrake Module")]
        public bool EMBrake { get; set; }
        [Display(Name = "EMCO Board")]
        public bool EMCO { get; set; }
        [Display(Name = "R6 Regen Unit")]
        public bool R6 { get; set; }
        public bool Local { get; set; }
        [Display(Name = "Short Floor")]
        public bool ShortFloor { get; set; }
        public bool Custom { get; set; }
        public bool MRL { get; set; }
        public bool CTL2 { get; set; }
        [Display(Name = "Tarjeta CPI Incluida")]
        public bool TrajetaCPI { get; set; }
        [Display(Name = "Door Control en Cartop")]
        public bool Cartop { get; set; }
    }

    public class Step
    {
        public int StepID { get; set; }

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
        
        public List<TriggeringFeature> _TriggeringFeatures { get; set; }
        public List<StepsForJob> _StepsForJob { get; set; }
    }

    public class TriggeringFeature
    {
        public int TriggeringFeatureID { get; set; }
        public int StepID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class StepsForJob
    {
        public int StepsForJobID { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public int StepID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public int TestJobID { get; set; }
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
        public int AuxTechnicianID { get; set; }
        public int AuxStationID { get; set; }

    }

    public class Reason1
    {
        public int Reason1ID { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }

        public List<Reason2> _Reason2s { get; set; }
    }

    public class Reason2
    {
        public int Reason2ID { get; set; }

        [Display(Name = "Reason 1")]
        public int Reason1ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }

        public List<Reason3> _Reason3s { get; set; }
    }

    public class Reason3
    {
        public int Reason3ID { get; set; }

        [NotMapped]
        [Display(Name = "Reason 1")]
        public int Reason1ID { get; set; }

        [Display(Name = "Reason 2")]
        public int Reason2ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }

        public List<Reason4> _Reason4s { get; set; }
    }

    public class Reason4
    {
        public int Reason4ID { get; set; }

        [NotMapped]
        [Display(Name = "Reason 1")]
        public int Reason1ID { get; set; }
        [NotMapped]
        [Display(Name = "Reason 2")]
        public int Reason2ID { get; set; }

        [Display(Name = "Reason 3")]
        public int Reason3ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }

        public List<Reason5> _Reason5s { get; set; }
    }

    public class Reason5
    {
        public int Reason5ID { get; set; }

        [NotMapped]
        [Display(Name = "Reason 1")]
        public int Reason1ID { get; set; }
        [NotMapped]
        [Display(Name = "Reason 2")]
        public int Reason2ID { get; set; }
        [NotMapped]
        [Display(Name = "Reason 3")]
        public int Reason3ID { get; set; }

        [Display(Name = "Reason 4")]
        public int Reason4ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }

        public List<Stop> _Stops { get; set; }
    }

    public class Stop
    {
        public int StopID { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public int TestJobID { get; set; }
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
        public int AuxTechnicianID { get; set; }
        public int AuxStationID { get; set; }
    }

    public class Station
    {
        public int StationID { get; set; }
        public string Label { get; set; }
        [Display(Name = "Job Type")]
        public int JobTypeID { get; set; }
    }

    [NotMapped]
    public class TestStats
    {
        public string JobNumer { get; set; }
        public string TechName { get; set; }
        public string Stage { get; set; }
        public double Efficiency { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string Station { get; set; }
        public DateTime TTC { get; set; }

        public string Color { get; set; }
        public string TextColor { get; set; }
    }

}
