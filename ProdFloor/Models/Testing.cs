using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int UserID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public int Status { get; set; }
    }

    public class TestFeature
    {
        public int TestFeatureID { get; set; }

        public int TestJobID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public bool Overlay { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public bool Group { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public bool PC { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public bool BrakeCoilVoltageMoreThan10 { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public bool MBrake { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public bool EMCO { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public bool R6 { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public bool Local { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public bool ShortFloor { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public bool Custom { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public bool MRL { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public bool CTL2 { get; set; }
    }

    public class Step
    {
        public int StepID { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public string Stage { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public DateTime ExpectedTime { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public int Order { get; set; }
        public bool Overlay { get; set; }
        public bool Group { get; set; }
        public bool PC { get; set; }
        public bool BrakeCoilVoltageMoreThan10 { get; set; }
        public bool MBrake { get; set; }
        public bool EMCO { get; set; }
        public bool R6 { get; set; }
        public bool Local { get; set; }
        public bool ShortFloor { get; set; }
        public bool Custom { get; set; }
        public bool MRL { get; set; }
        public bool CTL2 { get; set; }
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
    }

    public class Reason1
    {
        public int Reason1ID { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }
    }

    public class Reason2
    {
        public int Reason2ID { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public int Reason1ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }
    }

    public class Reason3
    {
        public int Reason3ID { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public int Reason2ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }
    }

    public class Reason4
    {
        public int Reason4ID { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public int Reason3ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }
    }

    public class Reason5
    {
        public int Reason5ID { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public int Reason4ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }
    }

    public class Stop
    {
        public int StopID { get; set; }

        [Required(ErrorMessage = "Please enter a {0}")]
        public int TestJobID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public int Reason1ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public int Reason2ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public int Reason3ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public int Reason4ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public int Reason5ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0} Date")]
        public DateTime Start { get; set; }
        [Required(ErrorMessage = "Please enter a Stop Date")]
        public DateTime _Stop { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }
    }

}
