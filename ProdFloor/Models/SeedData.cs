using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace ProdFloor.Models
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices
            .GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            /*if (!context.Jobs.Any())
            {
                context.Jobs.AddRange(
                    new Job
                    {
                        Name = "BRENTWOOD CONDOS",
                        JobNum = 2017088571,
                        PO = 3398238,
                        Cust = "OTIS-MP",
                        JobCountry = "USA",
                        JobCity = "Austin",
                        JobState = "Texas",
                        SafetyCode = "ASME A17.1-2007/CSA B44-07",
                        JobType = "M2000",
                        Contractor = "Contractor 1",
                        EngID = 97,
                        Status = "Incomplete",
                        ShipDate = new DateTime(2017,12,28)
                    },
                    new Job
                    {
                        Name = "Job Name 2",
                        JobNum = 2017088362,
                        PO = 3398175,
                        Cust = "FAKE-2",
                        JobCountry = "USA",
                        JobCity = "Pasadena",
                        JobState = "California",
                        SafetyCode = "ASME A17.1-2004 w/CA TITLE 8,GROUP IV",
                        JobType = "M4000",
                        Contractor = "Contractor 2",
                        EngID = 99,
                        Status = "Incomplete",
                        ShipDate = new DateTime(2017, 12, 12)
                    },
                    new Job
                    {
                        Name = "PACIFIC PALISADES LAUSD CHARTER HIGH SCHOOL",
                        JobNum = 2017088536,
                        PO = 3397819,
                        Cust = "CAEE2999",
                        JobCountry = "USA",
                        JobCity = "Los Angeles",
                        JobState = "California",
                        SafetyCode = "ASME A17.1-2004 w/CA TITLE 8,GROUP IV ",
                        JobType = "ELEM",
                        Contractor = "Contractor 1",
                        EngID = 27,
                        Status = "Incomplete",
                        ShipDate = new DateTime(2017, 11, 13)
                    },
                    new Job
                    {
                        Name = "SMC ADMIN",
                        JobNum = 2017088535,
                        PO = 3397817,
                        Cust = "CAEE3000",
                        JobCountry = "USA",
                        JobCity = "Sacramento",
                        JobState = "California",
                        SafetyCode = "ASME A17.1-2004 w/CA TITLE 8,GROUP IV ",
                        JobType = "M2000",
                        Contractor = "Contractor 6",
                        EngID = 27,
                        Status = "Incomplete",
                        ShipDate = new DateTime(2018, 1, 26)
                    }
                    );
                context.SaveChanges();
            }*/

            if (!context.JobTypes.Any())
            {
                context.JobTypes.AddRange(
                    new JobType
                    {
                        Name = "Element Hydro"
                    },
                    new JobType
                    {
                        Name = "M2000"
                    },
                    new JobType
                    {
                        Name = "M3000"
                    },
                    new JobType
                    {
                        Name = "M4000"
                    },
                    new JobType
                    {
                        Name = "Element Traction"
                    }
                    );
                context.SaveChanges();
            }

            if (!context.DoorOperators.Any())
            {
                context.DoorOperators.AddRange(
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "MCE",
                        Name = "SmarTraq Complete"
                    }, 
                    new DoorOperator
                    {
                        Style = "Manual",
                        Brand = "Manual",
                        Name = "Manual"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "MCE",
                        Name = "SmarTraq Upgrade"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOVFR I"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOVFR II"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOMVC MOHVC"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOD(230V)"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOD(115V)"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MODHA"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MODVC MODHVC"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOA"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOM MOH"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOSVCL"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOPM-P MOPM-PL"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOCT MOCTA MODCT MOMCT MOHCT"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "MAC_Kone",
                        Name = "PM-SSC/104 Board"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "MAC_Kone",
                        Name = "AMD/KONE"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "MAC_Kone",
                        Name = "MAC (Old style)"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "TKE_Dover",
                        Name = "HD03M"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "TKE_Dover",
                        Name = "HD68/70/73/91"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "TKE_Dover",
                        Name = "HD98/85"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "TKE_Dover",
                        Name = "HDLM"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "Otis",
                        Name = "6970A-Resistance"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "Otis",
                        Name = "6970A-Reactance"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "Otis",
                        Name = "A7770A"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "Otis",
                        Name = "OVL"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "Otis",
                        Name = "7300"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "Otis",
                        Name = "7782AA"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "Otis",
                        Name = "iMotion 1 & 2"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "ECI",
                        Name = "895/1000"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "ECI",
                        Name = "VFE2500"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "ECI",
                        Name = "2000"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "Other",
                        Name = "IPC Encore"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "Other",
                        Name = "Delco"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "Other",
                        Name = "Atlantic/Vertisys Model"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "Other",
                        Name = "Mitsubishi LV1/4K"
                    },
                    new DoorOperator
                    {
                        Style = "Automatic",
                        Brand = "Other",
                        Name = "Schindler QKS 14 & 15"
                    },
                    new DoorOperator
                    {
                        Style = "Freight",
                        Brand = "Peelle",
                        Name = "Peelle"
                    },
                    new DoorOperator
                    {
                        Style = "Freight",
                        Brand = "Courion",
                        Name = "Courion"
                    },
                    new DoorOperator
                    {
                        Style = "Freight",
                        Brand = "EMS",
                        Name = "EMS"
                    });
                context.SaveChanges();
            }

            if (!context.Countries.Any())
            {
                context.Countries.AddRange(
                    new Country
                    {
                        Name = "USA",
                    },
                    new Country
                    {
                        Name = "Canada"
                    }
                    );
                context.SaveChanges();
            }

           /* if (!context.States.Any())
            {
                context.States.AddRange(
                    new State
                    {
                        Name = "Alaska",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Alabama",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Arkansas",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Arizona",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "California",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Colorado",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Connecticut",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "District of Columbia",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Delaware",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Florida",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Georgia",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Hawaii",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Iowa",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Idaho",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Illinois",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Kansas",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Kentucky",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Louisiana",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Massachusetts",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Michigan",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Minnesota",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Missouri",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Mississippi",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Montana",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "North Carolina",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "North Dakota",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Nebraska",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "New Hampshire",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "New Jersey",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "New Mexico",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Nevada",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "New York",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Ohio",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Oklahoma",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Oregon",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Pennsylvania",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Puerto Rico",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Rhode Island",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "South Carolina",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "South Dakota",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Tennessee",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Texas",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Utah",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Vermont",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Virginia",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Washington",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "West Virginia",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Wisconsin",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Wyoming",
                        Country = "USA"
                    },
                    new State
                    {
                        Name = "Alberta",
                        Country = "Canada"
                    },
                    new State
                    {
                        Name = "British Columbia",
                        Country = "Canada"
                    },
                    new State
                    {
                        Name = "Manitoba",
                        Country = "Canada"
                    },
                    new State
                    {
                        Name = "New Brunswick",
                        Country = "Canada"
                    },
                    new State
                    {
                        Name = "Newfoundland and Labrador",
                        Country = "Canada"
                    },
                    new State
                    {
                        Name = "Northwest Territories",
                        Country = "Canada"
                    },
                    new State
                    {
                        Name = "Nova Scotia",
                        Country = "Canada"
                    },
                    new State
                    {
                        Name = "Ontario",
                        Country = "Canada"
                    },
                    new State
                    {
                        Name = "Prince Edward Island",
                        Country = "Canada"
                    },
                    new State
                    {
                        Name = "Quebec",
                        Country = "Canada"
                    },
                    new State
                    {
                        Name = "Saskatchewan",
                        Country = "Canada" }
                    );
                context.SaveChanges();
            }

            if (!context.Cities.Any())
            {
                context.Cities.AddRange(
                    new City
                    {
                        Name = "Other",
                        Country = "USA",
                        State = "Alaska",
                        CurrentFireCode = "ASME A17.1-2010/CSA B44-10"
                    },
                    new City
                    {
                        Name = "Other",
                        Country = "USA",
                        State = "Ohio",
                        CurrentFireCode = "ASME A17.1-2010/CSA B44-10"
                    },
                    new City
                    {
                        Name = "Other",
                        Country = "USA",
                        State = "Arkansas",
                        CurrentFireCode = "ASME A17.1-2010/CSA B44-10"
                    },
                    new City
                    {
                        Name = "Other",
                        Country = "USA",
                        State = "Arizona",
                        CurrentFireCode = "ASME A17.1-2010/CSA B44-10"
                    },
                    new City
                    {
                        Name = "Other",
                        Country = "USA",
                        State = "California",
                        CurrentFireCode = "ASME A17.1-2010/CSA B44-10"
                    },
                    new City
                    {
                        Name = "Other",
                        Country = "USA",
                        State = "Colorado",
                        CurrentFireCode = "ASME A17.1-2010/CSA B44-10"
                    },
                    new City
                    {
                        Name = "Other",
                        Country = "Canada",
                        State = "Nova Scotia",
                        CurrentFireCode = "ASME A17.1-2010/CSA B44-10"
                    },
                    new City
                    {
                        Name = "Other",
                        Country = "Canada",
                        State = "Ontario",
                        CurrentFireCode = "ASME A17.1-2010/CSA B44-10"
                    },
                    new City
                    {
                        Name = "Other",
                        Country = "Canada",
                        State = "Prince Edward Island",
                        CurrentFireCode = "ASME A17.1-2010/CSA B44-10"
                    },
                    new City
                    {
                        Name = "Other",
                        Country = "Canada",
                        State = "Quebec",
                        CurrentFireCode = "ASME A17.1-2010/CSA B44-10"
                    },
                    new City
                    {
                        Name = "Other",
                        Country = "Canada",
                        State = "Saskatchewan",
                        CurrentFireCode = "ASME A17.1-2010/CSA B44-10"
                    }
                    );
                context.SaveChanges();
            }*/

            if (!context.LandingSystems.Any())
            {
                context.LandingSystems.AddRange(
                    new LandingSystem
                    {
                        Name = "LS-QUTE",
                        UsedIn = "M2000"
                    },
                    new LandingSystem
                    {
                        Name = "LS-EDGE",
                        UsedIn = "M2000"
                    },
                    new LandingSystem
                    {
                        Name = "LS-VANE",
                        UsedIn = "M2000"
                    },
                    new LandingSystem
                    {
                        Name = "LS-QUTE",
                        UsedIn = "Element Hydro"
                    },
                    new LandingSystem
                    {
                        Name = "LS-EDGE",
                        UsedIn = "Element Hydro"
                    },
                    new LandingSystem
                    {
                        Name = "LS-Rail",
                        UsedIn = "M4000"
                    },
                    new LandingSystem
                    {
                        Name = "LS-EDGE",
                        UsedIn = "M4000"
                    },
                    new LandingSystem
                    {
                        Name = "LS-Rail",
                        UsedIn = "Element Traction"
                    },
                    new LandingSystem
                    {
                        Name = "LS-EDGE",
                        UsedIn = "Element Traction"
                    }
                    );
                context.SaveChanges();
            }

            if (!context.FireCodes.Any())
            {
                context.FireCodes.AddRange(
                    new FireCode { Name = "ASME A17.1-1993" },
                    new FireCode { Name = "ASME A17.1b-1996" },
                    new FireCode { Name = "ASME A17.1b-1998" },
                    new FireCode { Name = "ASME A17.1-2000" },
                    new FireCode { Name = "ASME A17.1-2002" },
                    new FireCode { Name = "ASME A17.1-2003 w/NYC Appendix K" },
                    new FireCode { Name = "ASME A17.1-2004 w/CSA B44-04" },
                    new FireCode { Name = "ASME A17.1-2004 w/524 CMR 35.00" },
                    new FireCode { Name = "ASME A17.1-2004 w/CA TITLE 8, GROUP IV" },
                    new FireCode { Name = "ASME A17.1a-2005/CSA B44S1-05" },
                    new FireCode { Name = "ASME A17.1-2007/CSA B44-07" },
                    new FireCode { Name = "ASME A17.1a-2008/CSA B44a-08" },
                    new FireCode { Name = "ASME A17.1b-2009/CSA B44b-09" },
                    new FireCode { Name = "ASME A17.1-2010/CSA B44-10" },
                    new FireCode { Name = "ASME A17.1-2013/CSA B44-13" },
                    new FireCode { Name = "ASME A17.1-2016/CSA B44-16" }
                    );
                context.SaveChanges();
            }
        }
    }
}
