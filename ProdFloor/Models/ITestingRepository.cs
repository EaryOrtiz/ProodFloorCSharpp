using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    interface ITestingRepository
    {
        IQueryable<TestJob> TestJobs { get; }
        IQueryable<TestFeature> TestFeatures { get; }
        IQueryable<Step> Steps { get; }
        IQueryable<TriggeringFeature> TriggeringFeatures { get; }
        IQueryable<StepsForJob> StepsForJobs { get; }
        IQueryable<Reason1> Reasons1 { get; }
        IQueryable<Reason2> Reasons2 { get; }
        IQueryable<Reason3> Reasons3 { get; }
        IQueryable<Reason4> Reasons4 { get; }
        IQueryable<Reason5> Reasons5 { get; }
        IQueryable<Stop> Stops { get; }

        void SaveTestJob(TestJob testJob);
        void SaveTestFeature(TestFeature testFeature);
        void SaveStep(Step step);
        void SaveTriggeringFeature(TriggeringFeature triggeringFeature);
        void SaveStepsForJob(StepsForJob stepsForJob);
        void SaveReason1(Reason1 reason1);
        void SaveReason2(Reason2 reason2);
        void SaveReason3(Reason3 reason3);
        void SaveReason4(Reason4 reason4);
        void SaveReason5(Reason5 reason5);
        void SaveStop(Stop stop);

        TestJob DeleteTestJob(int TestJobID);
        TestFeature DeleteTestFeature(int TestFeatureID);
        Step DeleteStep(int StepID);
        TriggeringFeature DeleteTriggeringFeature(int TriggeringFeatureID);
        StepsForJob DeleteStepsForJob(int StepsForJobID);
        Reason1 DeleteReason1(int Reason1ID);
        Reason2 DeleteReason2(int Reason2ID);
        Reason3 DeleteReason3(int Reason3ID);
        Reason4 DeleteReason4(int Reason4ID);
        Reason5 DeleteReason5(int Reason5ID);
        Stop DeleteStop(int StopID);
    }
}
