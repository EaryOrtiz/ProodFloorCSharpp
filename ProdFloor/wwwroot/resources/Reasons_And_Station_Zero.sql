use ProdFloor;
/*---------------------Reasons 0----------------------------------------------------------*/
SET IDENTITY_INSERT Reasons1 ON;
INSERT INTO Reasons1(Reason1ID,Description) VALUES (0,'-');
SET IDENTITY_INSERT Reasons1 OFF;

SET IDENTITY_INSERT Reasons2 ON;
INSERT INTO Reasons2(Reason2ID,Reason1ID,Description)  VALUES (0,0,'-');
SET IDENTITY_INSERT Reasons2 OFF;

SET IDENTITY_INSERT Reasons3 ON;
INSERT INTO Reasons3(Reason3ID,Reason2ID,Description) VALUES (0,0,'-');
SET IDENTITY_INSERT Reasons3 OFF;

SET IDENTITY_INSERT Reasons4 ON;
INSERT INTO Reasons4(Reason4ID,Reason3ID,Description) VALUES (0,0,'-');
SET IDENTITY_INSERT Reasons4 OFF;

SET IDENTITY_INSERT Reasons5 ON;
INSERT INTO Reasons5(Reason5ID,Reason4ID,Description) VALUES (0,0,'-');
SET IDENTITY_INSERT Reasons5 OFF;

/*-----------------------SpecialReasons-------------------------------------------------------*/
SET IDENTITY_INSERT Reasons1 ON;
INSERT INTO Reasons1(Reason1ID,Description) VALUES (980,'Reassignment');
SET IDENTITY_INSERT Reasons1 OFF;

SET IDENTITY_INSERT Reasons2 ON;
INSERT INTO Reasons2(Reason2ID,Reason1ID,Description)  VALUES (980,980,'N/A');
SET IDENTITY_INSERT Reasons2 OFF;

SET IDENTITY_INSERT Reasons3 ON;
INSERT INTO Reasons3(Reason3ID,Reason2ID,Description) VALUES (980,980,'N/A');
SET IDENTITY_INSERT Reasons3 OFF;

SET IDENTITY_INSERT Reasons4 ON;
INSERT INTO Reasons4(Reason4ID,Reason3ID,Description) VALUES (980,980,'N/A');
SET IDENTITY_INSERT Reasons4 OFF;

SET IDENTITY_INSERT Reasons5 ON;
INSERT INTO Reasons5(Reason5ID,Reason4ID,Description) VALUES (980,980,'N/A');
SET IDENTITY_INSERT Reasons5 OFF;
/*-------------------------------------------------------------------------------*/
SET IDENTITY_INSERT Reasons1 ON;
INSERT INTO Reasons1(Reason1ID,Description) VALUES (981,'Shift End');
SET IDENTITY_INSERT Reasons1 OFF;

SET IDENTITY_INSERT Reasons2 ON;
INSERT INTO Reasons2(Reason2ID,Reason1ID,Description)  VALUES (981,981,'N/A');
SET IDENTITY_INSERT Reasons2 OFF;

SET IDENTITY_INSERT Reasons3 ON;
INSERT INTO Reasons3(Reason3ID,Reason2ID,Description) VALUES (981,981,'N/A');
SET IDENTITY_INSERT Reasons3 OFF;

SET IDENTITY_INSERT Reasons4 ON;
INSERT INTO Reasons4(Reason4ID,Reason3ID,Description) VALUES (981,981,'N/A');
SET IDENTITY_INSERT Reasons4 OFF;

SET IDENTITY_INSERT Reasons5 ON;
INSERT INTO Reasons5(Reason5ID,Reason4ID,Description) VALUES (981,981,'N/A');
SET IDENTITY_INSERT Reasons5 OFF;
/*-------------------------------------------------------------------------------*/
SET IDENTITY_INSERT Reasons1 ON;
INSERT INTO Reasons1(Reason1ID,Description) VALUES (982,'Returned from complete');
SET IDENTITY_INSERT Reasons1 OFF;

SET IDENTITY_INSERT Reasons2 ON;
INSERT INTO Reasons2(Reason2ID,Reason1ID,Description)  VALUES (982,982,'N/A');
SET IDENTITY_INSERT Reasons2 OFF;

SET IDENTITY_INSERT Reasons3 ON;
INSERT INTO Reasons3(Reason3ID,Reason2ID,Description) VALUES (982,982,'N/A');
SET IDENTITY_INSERT Reasons3 OFF;

SET IDENTITY_INSERT Reasons4 ON;
INSERT INTO Reasons4(Reason4ID,Reason3ID,Description) VALUES (982,982,'N/A');
SET IDENTITY_INSERT Reasons4 OFF;

SET IDENTITY_INSERT Reasons5 ON;
INSERT INTO Reasons5(Reason5ID,Reason4ID,Description) VALUES (982,982,'N/A');
SET IDENTITY_INSERT Reasons5 OFF;
/*---------------------Station 0----------------------------------------------------------*/
SET IDENTITY_INSERT Stations ON;
INSERT INTO Stations(StationID,JobTypeID,Label) VALUES (0,1,'-');
SET IDENTITY_INSERT Stations OFF;
/*-------------------------------------WiringOption 0-----------------------------*/
SET IDENTITY_INSERT WiringOptions ON;
INSERT INTO WiringOptions (WiringOptionID,Description,isDeleted) VALUES (0,'Select a Option', 0);
SET IDENTITY_INSERT WiringOptions OFF;