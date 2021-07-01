using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFloor.Models
{
    public interface IWiringRepository
    {
        //Wiring
        IQueryable<Wiring> Wirings { get; }
        IQueryable<WirersInvolved> WirersInvolveds { get; }
        IQueryable<WiringStop> WiringStops { get; }
        IQueryable<WiringStep> WiringSteps { get; }
        IQueryable<WiringStepForJob> WiringStepsForJobs { get; }
        IQueryable<WiringReason1> WiringReasons1 { get; }
        IQueryable<WiringReason2> WiringReasons2 { get; }
        IQueryable<WiringReason3> WiringReasons3 { get; }
        IQueryable<WiringReason4> WiringReasons4 { get; }
        IQueryable<WiringReason5> WiringReasons5 { get; }
        IQueryable<WiringOption> WiringOptions { get; }
        IQueryable<WiringFeatures> WiringFeatures { get; }
        IQueryable<WiringTriggeringFeature> WiringTriggeringFeatures { get; }

        void SaveWiring(Wiring wiring);
        void SaveWirersInvolved(WirersInvolved wirersInvolved);
        void SaveWiringStop(WiringStop wiringStop);
        void SaveWiringStep(WiringStep wiringStep);
        void SaveWiringStepForJob(WiringStepForJob wiringStepForJob);
        void SaveWiringReason1(WiringReason1 wiringReason1);
        void SaveWiringReason2(WiringReason2 wiringReason2);
        void SaveWiringReason3(WiringReason3 wiringReason3);
        void SaveWiringReason4(WiringReason4 wiringReason4);
        void SaveWiringReason5(WiringReason5 wiringReason5);
        void SaveWiringOption(WiringOption wiringOption);
        void SaveWiringFeatures(WiringFeatures wiringFeatures);
        void SaveWiringTriggeringFeature(WiringTriggeringFeature wiringTriggeringFeature);

        Wiring DeleteWiring(int WiringID);
        WirersInvolved DeleteWirersInvolved(int wirersInvolvedID);
        WiringStop DeleteWiringStop(int WiringStopID);
        WiringStep DeleteWiringStep(int wiringStepID);
        WiringStepForJob DeleteWiringStepForJob(int wiringStepForJobID);
        WiringReason1 DeleteWiringReason1(int wiringReason1ID);
        WiringReason2 DeleteWiringReason2(int wiringReason2ID);
        WiringReason3 DeleteWiringReason3(int wiringReason3ID);
        WiringReason4 DeleteWiringReason4(int wiringReason4ID);
        WiringReason5 DeleteWiringReason5(int wiringReason5ID);
        WiringOption DeleteWiringOption(int wiringOptionID);
        WiringFeatures DeleteWiringFeatures(int wiringFeaturesID);
        WiringTriggeringFeature DeleteWiringTriggeringFeature(int wiringTriggeringFeatureID);

        //PXP
        IQueryable<WiringPXP> WiringPXPs { get; }
        IQueryable<PXPError> PXPErrors { get; }
        IQueryable<PXPReason> PXPReasons { get; }
        IQueryable<WirersPXPInvolved> WirersPXPInvolveds { get; }

        void SaveWiringPXP(WiringPXP wiringPXP);
        void SavePXPError(PXPError pXPError);
        void SavePXPReason(PXPReason pXPReason);
        void SaveWirersPXPInvolved(WirersPXPInvolved wirersPXPInvolved);

        WiringPXP DeleteWiringPXP(int WiringPXPID);
        PXPError DeletePXPError(int PXPErrorID);
        PXPReason DeletePXPReason(int PXPReasonID);
        WirersPXPInvolved DeleteWirersPXPInvolved(int WirersPXPInvolvedID);
    }
}
