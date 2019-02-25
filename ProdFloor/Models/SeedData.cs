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
                        JobTypeID = 1,
                        Name = "ElmHydro"
                    },
                    new JobType
                    {
                        JobTypeID = 2,
                        Name = "M2000"
                    },
                    new JobType
                    {
                        JobTypeID = 3,
                        Name = "M3000"
                    },
                    new JobType
                    {
                        JobTypeID = 4,
                        Name = "M4000"
                    },
                    new JobType
                    {
                        JobTypeID = 5,
                        Name = "ElmTractio"
                    }
                    );
                context.SaveChanges();
            }

            if (!context.DoorOperators.Any())
            {
                context.DoorOperators.AddRange(
                    new DoorOperator
                    {
                        DoorOperatorID = 1,
                        Style = "Automatic",
                        Brand = "MCE",
                        Name = "SmarTraq Complete"
                    }, 
                    new DoorOperator
                    {
                        DoorOperatorID = 2,
                        Style = "Manual",
                        Brand = "Manual",
                        Name = "Manual"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 3,
                        Style = "Automatic",
                        Brand = "MCE",
                        Name = "SmarTraq Upgrade"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 4,
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOVFR I"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 5,
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOVFR II"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 6,
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOMVC MOHVC"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 7,
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOD(230V)"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 8,
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOD(115V)"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 9,
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MODHA"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 10,
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MODVC MODHVC"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 11,
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOA"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 12,
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOM MOH"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 13,
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOSVCL"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 14,
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOPM-P MOPM-PL"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 15,
                        Style = "Automatic",
                        Brand = "GAL",
                        Name = "MOCT MOCTA MODCT MOMCT MOHCT"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 16,
                        Style = "Automatic",
                        Brand = "MAC_Kone",
                        Name = "PM-SSC/104 Board"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 17,
                        Style = "Automatic",
                        Brand = "MAC_Kone",
                        Name = "AMD/KONE"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 18,
                        Style = "Automatic",
                        Brand = "MAC_Kone",
                        Name = "MAC (Old style)"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 19,
                        Style = "Automatic",
                        Brand = "TKE_Dover",
                        Name = "HD03M"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 20,
                        Style = "Automatic",
                        Brand = "TKE_Dover",
                        Name = "HD68/70/73/91"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 21,
                        Style = "Automatic",
                        Brand = "TKE_Dover",
                        Name = "HD98/85"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 22,
                        Style = "Automatic",
                        Brand = "TKE_Dover",
                        Name = "HDLM"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 23,
                        Style = "Automatic",
                        Brand = "Otis",
                        Name = "6970A-Resistance"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 24,
                        Style = "Automatic",
                        Brand = "Otis",
                        Name = "6970A-Reactance"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 25,
                        Style = "Automatic",
                        Brand = "Otis",
                        Name = "A7770A"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 26,
                        Style = "Automatic",
                        Brand = "Otis",
                        Name = "OVL"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 27,
                        Style = "Automatic",
                        Brand = "Otis",
                        Name = "7300"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 28,
                        Style = "Automatic",
                        Brand = "Otis",
                        Name = "7782AA"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 29,
                        Style = "Automatic",
                        Brand = "Otis",
                        Name = "iMotion 1 & 2"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 30,
                        Style = "Automatic",
                        Brand = "ECI",
                        Name = "895/1000"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 31,
                        Style = "Automatic",
                        Brand = "ECI",
                        Name = "VFE2500"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 32,
                        Style = "Automatic",
                        Brand = "ECI",
                        Name = "2000"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 33,
                        Style = "Automatic",
                        Brand = "Other",
                        Name = "IPC Encore"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 34,
                        Style = "Automatic",
                        Brand = "Other",
                        Name = "Delco"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 35,
                        Style = "Automatic",
                        Brand = "Other",
                        Name = "Atlantic/Vertisys Model"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 36,
                        Style = "Automatic",
                        Brand = "Other",
                        Name = "Mitsubishi LV1/4K"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 37,
                        Style = "Automatic",
                        Brand = "Other",
                        Name = "Schindler QKS 14 & 15"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 38,
                        Style = "Freight",
                        Brand = "Peelle",
                        Name = "Peelle"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 39,
                        Style = "Freight",
                        Brand = "Courion",
                        Name = "Courion"
                    },
                    new DoorOperator
                    {
                        DoorOperatorID = 40,
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
                        CountryID = 1,
                        Name = "USA",
                    },
                    new Country
                    {
                        CountryID = 2,
                        Name = "Canada"
                    }
                    );
                context.SaveChanges();
            }

           if (!context.States.Any())
            {
                context.States.AddRange(
                    new State
                    {
                        StateID = 1,
                        Name = "Alaska",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 2,
                        Name = "Alabama",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 3,
                        Name = "Arkansas",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 4,
                        Name = "Arizona",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 5,
                        Name = "California",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 6,
                        Name = "Colorado",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 7,
                        Name = "Connecticut",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 8,
                        Name = "District of Columbia",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 9,
                        Name = "Delaware",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 10,
                        Name = "Florida",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 11,
                        Name = "Georgia",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 12,
                        Name = "Hawaii",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 13,
                        Name = "Iowa",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 14,
                        Name = "Idaho",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 15,
                        Name = "Illinois",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 16,
                        Name = "Kansas",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 17,
                        Name = "Kentucky",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 18,
                        Name = "Louisiana",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 19,
                        Name = "Massachusetts",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 20,
                        Name = "Michigan",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 21,
                        Name = "Minnesota",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 22,
                        Name = "Missouri",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 23,
                        Name = "Mississippi",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 24,
                        Name = "Montana",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 25,
                        Name = "North Carolina",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 26,
                        Name = "North Dakota",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 27,
                        Name = "Nebraska",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 28,
                        Name = "New Hampshire",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 29,
                        Name = "New Jersey",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 30,
                        Name = "New Mexico",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 31,
                        Name = "Nevada",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 32,
                        Name = "New York",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 33,
                        Name = "Ohio",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 34,
                        Name = "Oklahoma",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 35,
                        Name = "Oregon",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 36,
                        Name = "Pennsylvania",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 37,
                        Name = "Puerto Rico",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 38,
                        Name = "Rhode Island",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 39,
                        Name = "South Carolina",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 40,
                        Name = "South Dakota",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 41,
                        Name = "Tennessee",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 42,
                        Name = "Texas",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 43,
                        Name = "Utah",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 44,
                        Name = "Vermont",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 45,
                        Name = "Virginia",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 46,
                        Name = "Washington",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 47,
                        Name = "West Virginia",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 48,
                        Name = "Wisconsin",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 49,
                        Name = "Wyoming",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 50,
                        Name = "Alberta",
                        CountryID = 2
                    },
                    new State
                    {
                        StateID = 51,
                        Name = "British Columbia",
                        CountryID = 2
                    },
                    new State
                    {
                        StateID = 52,
                        Name = "Manitoba",
                        CountryID = 2
                    },
                    new State
                    {
                        StateID = 53,
                        Name = "New Brunswick",
                        CountryID = 2
                    },
                    new State
                    {
                        StateID = 54,
                        Name = "Newfoundland and Labrador",
                        CountryID = 2
                    },
                    new State
                    {
                        StateID = 55,
                        Name = "Northwest Territories",
                        CountryID = 2
                    },
                    new State
                    {
                        StateID = 56,
                        Name = "Nova Scotia",
                        CountryID = 2
                    },
                    new State
                    {
                        StateID = 57,
                        Name = "Ontario",
                        CountryID = 2
                    },
                    new State
                    {
                        StateID = 58,
                        Name = "Prince Edward Island",
                        CountryID = 2
                    },
                    new State
                    {
                        StateID = 59,
                        Name = "Quebec",
                        CountryID = 2
                    },
                    new State
                    {
                        StateID = 60,
                        Name = "Saskatchewan",
                        CountryID = 2
                    },
                    new State
                    {
                        StateID = 61,
                        Name = "Bahamas",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 62,
                        Name = "Maryland",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 63,
                        Name = "Maine",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 64,
                        Name = "Cayman Islands",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 65,
                        Name = "Guam",
                        CountryID = 1
                    },
                    new State
                    {
                        StateID = 66,
                        Name = "Indiana",
                        CountryID = 1
                    }
                    );
                context.SaveChanges();
            }

            if (!context.FireCodes.Any())
            {
                context.FireCodes.AddRange(
                    new FireCode { FireCodeID = 1, Name = "ASME A17.1-1993" },
                    new FireCode { FireCodeID = 2, Name = "ASME A17.1b-1996" },
                    new FireCode { FireCodeID = 3, Name = "ASME A17.1b-1998" },
                    new FireCode { FireCodeID = 4, Name = "ASME A17.1-2000" },
                    new FireCode { FireCodeID = 5, Name = "ASME A17.1-2002" },
                    new FireCode { FireCodeID = 6, Name = "ASME A17.1-2003 w/NYC Appendix K" },
                    new FireCode { FireCodeID = 7, Name = "ASME A17.1-2004 w/CSA B44-04" },
                    new FireCode { FireCodeID = 8, Name = "ASME A17.1-2004 w/524 CMR 35.00" },
                    new FireCode { FireCodeID = 9, Name = "ASME A17.1-2004 w/CA TITLE 8, GROUP IV" },
                    new FireCode { FireCodeID = 10, Name = "ASME A17.1a-2005/CSA B44S1-05" },
                    new FireCode { FireCodeID = 11, Name = "ASME A17.1-2007/CSA B44-07" },
                    new FireCode { FireCodeID = 12, Name = "ASME A17.1a-2008/CSA B44a-08" },
                    new FireCode { FireCodeID = 13, Name = "ASME A17.1b-2009/CSA B44b-09" },
                    new FireCode { FireCodeID = 14, Name = "ASME A17.1-2010/CSA B44-10" },
                    new FireCode { FireCodeID = 15, Name = "ASME A17.1-2013/CSA B44-13" },
                    new FireCode { FireCodeID = 16, Name = "ASME A17.1-2016/CSA B44-16" },
                    new FireCode { FireCodeID = 17, Name = "ASME A17.1-1996 w/CA TITLE 8,GROUP III" },
                    new FireCode { FireCodeID = 18, Name = "ASME A17.1-2016 w/Chicago Title 14C-3" }
                    );
                context.SaveChanges();
            }

            if (!context.Cities.Any())
            {
                context.Cities.AddRange(
                    new City
                    {
                        Name = "Other",
                        StateID = 8,
                        FirecodeID = 14
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 12,
                        FirecodeID = 14
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 14,
                        FirecodeID = 14
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 17,
                        FirecodeID = 14
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 20,
                        FirecodeID = 14
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 21,
                        FirecodeID = 14
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 35,
                        FirecodeID = 14
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 37,
                        FirecodeID = 14
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 41,
                        FirecodeID = 14
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 53,
                        FirecodeID = 14
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 57,
                        FirecodeID = 14
                    },//------------------------
                    new City
                    {
                        Name = "Other",
                        StateID = 2,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 61,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 13,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 16,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 62,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 23,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 28,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 25,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 26,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 33,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 34,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 38,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 40,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 42,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 43,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 46,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 49,
                        FirecodeID = 16
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 54,
                        FirecodeID = 16
                    },//--------------------
                    new City
                    {
                        Name = "Other",
                        StateID = 1,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 6,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 7,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 10,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 11,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 15,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 18,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 63,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 31,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 27,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 30,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 32,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 39,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 44,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 45,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 47,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 48,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 50,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 56,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 58,
                        FirecodeID = 15
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 60,
                        FirecodeID = 15
                    },//----------------------
                    new City
                    {
                        Name = "Other",
                        StateID = 4,
                        FirecodeID = 11
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 3,
                        FirecodeID = 11
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 64,
                        FirecodeID = 11
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 65,
                        FirecodeID = 11
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 66,
                        FirecodeID = 11
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 51,
                        FirecodeID = 11
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 52,
                        FirecodeID = 11
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 55,
                        FirecodeID = 11
                    },
                    new City
                    {
                        Name = "Other",
                        StateID = 59,
                        FirecodeID = 11
                    }
                    );
                context.SaveChanges();
            }

            if (!context.LandingSystems.Any())
            {
                context.LandingSystems.AddRange(
                    new LandingSystem
                    {
                        LandingSystemID = 1,
                        Name = "LS-QUTE",
                        UsedIn = "M2000"
                    },
                    new LandingSystem
                    {
                        LandingSystemID = 2,
                        Name = "LS-EDGE",
                        UsedIn = "M2000"
                    },
                    new LandingSystem
                    {
                        LandingSystemID = 3,
                        Name = "LS-VANE",
                        UsedIn = "M2000"
                    },
                    new LandingSystem
                    {
                        LandingSystemID = 4,
                        Name = "LS-QUTE",
                        UsedIn = "Element Hydro"
                    },
                    new LandingSystem
                    {
                        LandingSystemID = 5,
                        Name = "LS-EDGE",
                        UsedIn = "Element Hydro"
                    },
                    new LandingSystem
                    {
                        LandingSystemID = 6,
                        Name = "LS-Rail",
                        UsedIn = "M4000"
                    },
                    new LandingSystem
                    {
                        LandingSystemID = 7,
                        Name = "LS-EDGE",
                        UsedIn = "M4000"
                    },
                    new LandingSystem
                    {
                        LandingSystemID = 8,
                        Name = "LS-Rail",
                        UsedIn = "Element Traction"
                    },
                    new LandingSystem
                    {
                        LandingSystemID = 9,
                        Name = "LS-EDGE",
                        UsedIn = "Element Traction"
                    }
                    );
                context.SaveChanges();
            }

        }
    }
}
