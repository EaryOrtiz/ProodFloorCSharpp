﻿using System;
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
        [Display(Name = "Job Label")]
        public string JobLabel { get; set; }
        [Display(Name = "Estacion")]
        public string Station { get; set; }

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

        public int JobTypeID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Stage { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
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
        public TimeSpan Elapsed { get; set; }
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

        public List<Reason2> _Reason2s { get; set; }
        public List<Stop> _Stops { get; set; }
    }

    public class Reason2
    {
        public int Reason2ID { get; set; }

        [Display(Name = "Reason 1")]
        [Required(ErrorMessage = "Please enter a {0}")]
        public int Reason1ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }

        public List<Reason3> _Reason3s { get; set; }
        public List<Stop> _Stops { get; set; }
    }

    public class Reason3
    {
        public int Reason3ID { get; set; }

        [NotMapped]
        [Display(Name = "Reason 1")]
        public int Reason1ID { get; set; }

        [Display(Name = "Reason 2")]
        [Required(ErrorMessage = "Please enter a {0}")]
        public int Reason2ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }

        public List<Reason4> _Reason4s { get; set; }
        public List<Stop> _Stops { get; set; }
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
        [Required(ErrorMessage = "Please enter a {0}")]
        public int Reason3ID { get; set; }
        [Required(ErrorMessage = "Please enter a {0}")]
        public string Description { get; set; }

        public List<Reason5> _Reason5s { get; set; }
        public List<Stop> _Stops { get; set; }
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
        [Required(ErrorMessage = "Please enter a {0}")]
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
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Please enter a Stop Date")]
        public DateTime StopDate { get; set; }
        public TimeSpan Elapsed { get; set; }
        public string Description { get; set; }
    }

}
