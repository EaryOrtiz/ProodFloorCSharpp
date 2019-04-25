using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    public class Country_audit
    {
        public int ID { get; set; }
        public int AffectedItemID { get; set; }
        public string Name { get; set; }
        public string Statement { get; set; }
        public DateTime Date { get; set; }
    }

    public class State_audit
    {
        public int ID { get; set; }
        public int AffectedItemID { get; set; }
        public string Name { get; set; }
        public string Statement { get; set; }
        public DateTime Date { get; set; }
    }

    public class City_audit
    {
        public int ID { get; set; }
        public int AffectedItemID { get; set; }
        public string Name { get; set; }
        public string Statement { get; set; }
        public DateTime Date { get; set; }
    }

    public class FireCode_audit
    {
        public int ID { get; set; }
        public int AffectedItemID { get; set; }
        public string Name { get; set; }
        public string Statement { get; set; }
        public DateTime Date { get; set; }
    }

    public class LandingSystem_audit
    {
        public int ID { get; set; }
        public int AffectedItemID { get; set; }
        public string Name { get; set; }
        public string Statement { get; set; }
        public DateTime Date { get; set; }
    }

    public class DoorOperator_audit
    {
        public int ID { get; set; }
        public int AffectedItemID { get; set; }
        public string Name { get; set; }
        public string Statement { get; set; }
        public DateTime Date { get; set; }
    }

    public class JobType_audit
    {
        public int ID { get; set; }
        public int AffectedItemID { get; set; }
        public string Name { get; set; }
        public string Statement { get; set; }
        public DateTime Date { get; set; }
    }

    public class Starter_audit
    {
        public int ID { get; set; }
        public int AffectedItemID { get; set; }
        public string Statement { get; set; }
        public DateTime Date { get; set; }
    }

    public class Overload_audit
    {
        public int ID { get; set; }
        public int AffectedItemID { get; set; }
        public string Statement { get; set; }
        public DateTime Date { get; set; }
    }

    public class Slowdown_audit
    {
        public int ID { get; set; }
        public int AffectedItemID { get; set; }
        public string Statement { get; set; }
        public DateTime Date { get; set; }
    }

    public class WireTypeSize_audit
    {
        public int ID { get; set; }
        public int AffectedItemID { get; set; }
        public string Statement { get; set; }
        public DateTime Date { get; set; }
    }

}
