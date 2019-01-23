IF OBJECT_ID(N'__EFMigrationsHistory') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Cities] (
    [CityID] int NOT NULL IDENTITY,
    [Country] nvarchar(max) NULL,
    [CurrentFireCode] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [State] nvarchar(max) NULL,
    CONSTRAINT [PK_Cities] PRIMARY KEY ([CityID])
);

GO

CREATE TABLE [Countries] (
    [CountryID] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    CONSTRAINT [PK_Countries] PRIMARY KEY ([CountryID])
);

GO

CREATE TABLE [DoorOperators] (
    [DoorOperatorID] int NOT NULL IDENTITY,
    [Brand] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [Style] nvarchar(max) NULL,
    CONSTRAINT [PK_DoorOperators] PRIMARY KEY ([DoorOperatorID])
);

GO

CREATE TABLE [FireCodes] (
    [FireCodeID] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    CONSTRAINT [PK_FireCodes] PRIMARY KEY ([FireCodeID])
);

GO

CREATE TABLE [Jobs] (
    [JobID] int NOT NULL IDENTITY,
    [Contractor] nvarchar(max) NOT NULL,
    [Cust] nvarchar(max) NOT NULL,
    [JobNum] int NOT NULL,
    [JobState] nvarchar(max) NOT NULL,
    [JobType] nvarchar(max) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [PO] int NOT NULL,
    [SafetyCode] nvarchar(max) NOT NULL,
    [ShipDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Jobs] PRIMARY KEY ([JobID])
);

GO

CREATE TABLE [JobsExtensions] (
    [JobExtensionID] int NOT NULL IDENTITY,
    [AUXCOP] bit NOT NULL,
    [CartopDoorButtons] bit NOT NULL,
    [DoorBrand] nvarchar(max) NULL,
    [DoorGate] nvarchar(max) NULL,
    [DoorHoist] nvarchar(max) NULL,
    [DoorHold] bit NOT NULL,
    [DoorModel] nvarchar(max) NULL,
    [DoorStyle] nvarchar(max) NULL,
    [HeavyDoors] bit NOT NULL,
    [InfDetector] bit NOT NULL,
    [InputFrecuency] int NOT NULL,
    [InputPhase] int NOT NULL,
    [InputVoltage] int NOT NULL,
    [JobID] int NOT NULL,
    [JobTypeAdd] nvarchar(max) NULL,
    [JobTypeMain] nvarchar(max) NULL,
    [MechSafEdge] bit NOT NULL,
    [Nudging] bit NOT NULL,
    [NumOfStops] int NOT NULL,
    [SCOP] bit NOT NULL,
    [SHC] bit NOT NULL,
    [SHCRisers] int NOT NULL,
    CONSTRAINT [PK_JobsExtensions] PRIMARY KEY ([JobExtensionID])
);

GO

CREATE TABLE [JobTypes] (
    [JobTypeID] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_JobTypes] PRIMARY KEY ([JobTypeID])
);

GO

CREATE TABLE [LandingSystems] (
    [LandingSystemID] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [UsedIn] nvarchar(max) NULL,
    CONSTRAINT [PK_LandingSystems] PRIMARY KEY ([LandingSystemID])
);

GO

CREATE TABLE [States] (
    [StateID] int NOT NULL IDENTITY,
    [Country] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    CONSTRAINT [PK_States] PRIMARY KEY ([StateID])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180124220344_Initial', N'2.0.1-rtm-125');

GO

CREATE TABLE [HydroSpecifics] (
    [HydroSpecificID] int NOT NULL IDENTITY,
    [Battery] bit NOT NULL,
    [BatteryBrand] nvarchar(max) NULL,
    [FLA] int NOT NULL,
    [HP] int NOT NULL,
    [JobID] int NOT NULL,
    [LOS] bit NOT NULL,
    [LifeJacket] bit NOT NULL,
    [MotorsNum] int NOT NULL,
    [OilCool] bit NOT NULL,
    [OilTank] bit NOT NULL,
    [PSS] bit NOT NULL,
    [Resync] bit NOT NULL,
    [Roped] bit NOT NULL,
    [SPH] int NOT NULL,
    [Starter] nvarchar(max) NULL,
    [VCI] bit NOT NULL,
    [ValveBrand] nvarchar(max) NULL,
    [ValveCoils] int NOT NULL,
    [ValveModel] nvarchar(max) NULL,
    [ValveNum] int NOT NULL,
    [ValveVoltage] int NOT NULL,
    CONSTRAINT [PK_HydroSpecifics] PRIMARY KEY ([HydroSpecificID])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180219165105_HydroSpecifics', N'2.0.1-rtm-125');

GO

CREATE TABLE [GenericFeaturesList] (
    [GenericFeaturesID] int NOT NULL IDENTITY,
    [Attendant] bit NOT NULL,
    [CallEnable] bit NOT NULL,
    [CarCallCodeSecurity] nvarchar(max) NULL,
    [CarToLobby] bit NOT NULL,
    [EMT] bit NOT NULL,
    [EP] bit NOT NULL,
    [EQ] bit NOT NULL,
    [FLO] bit NOT NULL,
    [FRON2] bit NOT NULL,
    [Hosp] bit NOT NULL,
    [IDS] bit NOT NULL,
    [IMon] bit NOT NULL,
    [INA] bit NOT NULL,
    [INCP] bit NOT NULL,
    [Ind] bit NOT NULL,
    [JobID] int NOT NULL,
    [LoadWeigher] bit NOT NULL,
    [MView] bit NOT NULL,
    [SpecialInstructions] nvarchar(max) NULL,
    [SwitchStyle] nvarchar(max) NULL,
    CONSTRAINT [PK_GenericFeaturesList] PRIMARY KEY ([GenericFeaturesID])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180302142723_GenericFeatures', N'2.0.1-rtm-125');

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'JobsExtensions') AND [c].[name] = N'DoorStyle');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [JobsExtensions] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [JobsExtensions] DROP COLUMN [DoorStyle];

GO

ALTER TABLE [Jobs] ADD [JobCity] nvarchar(max) NOT NULL DEFAULT N'';

GO

ALTER TABLE [Jobs] ADD [JobCountry] nvarchar(max) NOT NULL DEFAULT N'';

GO

ALTER TABLE [Jobs] ADD [LatestFinishDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.000';

GO

CREATE TABLE [Indicators] (
    [IndicatorID] int NOT NULL IDENTITY,
    [JobID] int NOT NULL,
    [Name] nvarchar(max) NULL,
    [Voltage] int NOT NULL,
    [VotageType] nvarchar(max) NULL,
    CONSTRAINT [PK_Indicators] PRIMARY KEY ([IndicatorID])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180312205521_Indicators', N'2.0.1-rtm-125');

GO

DROP TABLE [Indicators];

GO

CREATE TABLE [StatusIndicators] (
    [StatusIndicatorID] int NOT NULL IDENTITY,
    [JobID] int NOT NULL,
    [Name] nvarchar(max) NULL,
    [Voltage] int NOT NULL,
    [VoltageType] nvarchar(max) NULL,
    CONSTRAINT [PK_StatusIndicators] PRIMARY KEY ([StatusIndicatorID])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180312222926_Indicators1', N'2.0.1-rtm-125');

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180312223020_Indicators2', N'2.0.1-rtm-125');

GO

CREATE TABLE [Indicators] (
    [IndicatorID] int NOT NULL IDENTITY,
    [CarCallsType] nvarchar(max) NULL,
    [CarCallsVoltage] int NOT NULL,
    [CarCallsVoltageType] nvarchar(max) NULL,
    [CarLanterns] bit NOT NULL,
    [CarLanternsType] nvarchar(max) NULL,
    [CarLanternsVoltage] int NOT NULL,
    [CarLanternsVoltageType] nvarchar(max) NULL,
    [CarPI] bit NOT NULL,
    [CarPIDiscreteType] nvarchar(max) NULL,
    [CarPIDiscreteVoltage] nvarchar(max) NULL,
    [CarPIDiscreteVoltageType] nvarchar(max) NULL,
    [CarPIType] nvarchar(max) NULL,
    [HallCallsType] nvarchar(max) NULL,
    [HallCallsVoltage] int NOT NULL,
    [HallCallsVoltageType] nvarchar(max) NULL,
    [HallLanterns] bit NOT NULL,
    [HallLanternsType] nvarchar(max) NULL,
    [HallLanternsVoltage] int NOT NULL,
    [HallLanternsVoltageType] nvarchar(max) NULL,
    [HallPI] bit NOT NULL,
    [HallPIDiscreteType] nvarchar(max) NULL,
    [HallPIDiscreteVoltage] nvarchar(max) NULL,
    [HallPIDiscreteVoltageType] nvarchar(max) NULL,
    [HallPIType] nvarchar(max) NULL,
    [IndicatorsVoltage] int NOT NULL,
    [IndicatorsVoltageType] nvarchar(max) NULL,
    [JobID] int NOT NULL,
    [PassingFloor] bit NOT NULL,
    [PassingFloorDiscreteType] nvarchar(max) NULL,
    [PassingFloorDiscreteVoltage] nvarchar(max) NULL,
    [PassingFloorDiscreteVoltageType] nvarchar(max) NULL,
    [PassingFloorEnable] bit NOT NULL,
    [PassingFloorType] nvarchar(max) NULL,
    [VoiceAnnunciationPI] bit NOT NULL,
    [VoiceAnnunciationPIType] nvarchar(max) NULL,
    CONSTRAINT [PK_Indicators] PRIMARY KEY ([IndicatorID])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180314160510_Indicators3', N'2.0.1-rtm-125');

GO

DROP TABLE [StatusIndicators];

GO

CREATE TABLE [HoistWayDatas] (
    [HoistWayDataID] int NOT NULL IDENTITY,
    [Capacity] int NOT NULL,
    [DownSpeed] int NOT NULL,
    [FrontEightServed] bit NOT NULL,
    [FrontEleventhServed] bit NOT NULL,
    [FrontFifteenthServed] bit NOT NULL,
    [FrontFifthServed] bit NOT NULL,
    [FrontFirstServed] bit NOT NULL,
    [FrontFourteenthServed] bit NOT NULL,
    [FrontFourthServed] bit NOT NULL,
    [FrontNinthServed] bit NOT NULL,
    [FrontSecondServed] bit NOT NULL,
    [FrontSeventhServed] bit NOT NULL,
    [FrontSexthServed] bit NOT NULL,
    [FrontSixteenthServed] bit NOT NULL,
    [FrontTenthServed] bit NOT NULL,
    [FrontThirdServed] bit NOT NULL,
    [FrontThirteenthServed] bit NOT NULL,
    [FrontTwelvethServed] bit NOT NULL,
    [JobID] int NOT NULL,
    [LandingSystem] nvarchar(max) NULL,
    [RearEightServed] bit NOT NULL,
    [RearEleventhServed] bit NOT NULL,
    [RearFifteenthServed] bit NOT NULL,
    [RearFifthServed] bit NOT NULL,
    [RearFirstServed] bit NOT NULL,
    [RearFourteenthServed] bit NOT NULL,
    [RearFourthServed] bit NOT NULL,
    [RearNinthServed] bit NOT NULL,
    [RearSecondServed] bit NOT NULL,
    [RearSeventhServed] bit NOT NULL,
    [RearSexthServed] bit NOT NULL,
    [RearSixteenthServed] bit NOT NULL,
    [RearTenthServed] bit NOT NULL,
    [RearThirdServed] bit NOT NULL,
    [RearThirteenthServed] bit NOT NULL,
    [RearTwelvethServed] bit NOT NULL,
    [TotalTravel] int NOT NULL,
    [UpSpeed] int NOT NULL,
    CONSTRAINT [PK_HoistWayDatas] PRIMARY KEY ([HoistWayDataID])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180314195527_HWYData', N'2.0.1-rtm-125');

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'JobsExtensions') AND [c].[name] = N'JobTypeMain');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [JobsExtensions] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [JobsExtensions] ALTER COLUMN [JobTypeMain] nvarchar(max) NOT NULL;

GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'JobsExtensions') AND [c].[name] = N'JobTypeAdd');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [JobsExtensions] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [JobsExtensions] ALTER COLUMN [JobTypeAdd] nvarchar(max) NOT NULL;

GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'JobsExtensions') AND [c].[name] = N'DoorModel');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [JobsExtensions] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [JobsExtensions] ALTER COLUMN [DoorModel] nvarchar(max) NOT NULL;

GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'JobsExtensions') AND [c].[name] = N'DoorHoist');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [JobsExtensions] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [JobsExtensions] ALTER COLUMN [DoorHoist] nvarchar(max) NOT NULL;

GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'JobsExtensions') AND [c].[name] = N'DoorGate');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [JobsExtensions] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [JobsExtensions] ALTER COLUMN [DoorGate] nvarchar(max) NOT NULL;

GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'JobsExtensions') AND [c].[name] = N'DoorBrand');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [JobsExtensions] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [JobsExtensions] ALTER COLUMN [DoorBrand] nvarchar(max) NOT NULL;

GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'IndicatorsVoltageType');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [Indicators] ALTER COLUMN [IndicatorsVoltageType] nvarchar(max) NOT NULL;

GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'IndicatorsVoltage');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [Indicators] ALTER COLUMN [IndicatorsVoltage] nvarchar(max) NOT NULL;

GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'HallLanternsVoltage');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [Indicators] ALTER COLUMN [HallLanternsVoltage] nvarchar(max) NULL;

GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'HallCallsVoltageType');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [Indicators] ALTER COLUMN [HallCallsVoltageType] nvarchar(max) NOT NULL;

GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'HallCallsVoltage');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [Indicators] ALTER COLUMN [HallCallsVoltage] nvarchar(max) NOT NULL;

GO

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'HallCallsType');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [Indicators] ALTER COLUMN [HallCallsType] nvarchar(max) NOT NULL;

GO

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'CarLanternsVoltage');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [Indicators] ALTER COLUMN [CarLanternsVoltage] nvarchar(max) NULL;

GO

DECLARE @var14 sysname;
SELECT @var14 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'CarCallsVoltageType');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var14 + '];');
ALTER TABLE [Indicators] ALTER COLUMN [CarCallsVoltageType] nvarchar(max) NOT NULL;

GO

DECLARE @var15 sysname;
SELECT @var15 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'CarCallsVoltage');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var15 + '];');
ALTER TABLE [Indicators] ALTER COLUMN [CarCallsVoltage] nvarchar(max) NOT NULL;

GO

DECLARE @var16 sysname;
SELECT @var16 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'CarCallsType');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var16 + '];');
ALTER TABLE [Indicators] ALTER COLUMN [CarCallsType] nvarchar(max) NOT NULL;

GO

DECLARE @var17 sysname;
SELECT @var17 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'HydroSpecifics') AND [c].[name] = N'ValveModel');
IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [HydroSpecifics] DROP CONSTRAINT [' + @var17 + '];');
ALTER TABLE [HydroSpecifics] ALTER COLUMN [ValveModel] nvarchar(max) NOT NULL;

GO

DECLARE @var18 sysname;
SELECT @var18 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'HydroSpecifics') AND [c].[name] = N'ValveBrand');
IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [HydroSpecifics] DROP CONSTRAINT [' + @var18 + '];');
ALTER TABLE [HydroSpecifics] ALTER COLUMN [ValveBrand] nvarchar(max) NOT NULL;

GO

DECLARE @var19 sysname;
SELECT @var19 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'HydroSpecifics') AND [c].[name] = N'Starter');
IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [HydroSpecifics] DROP CONSTRAINT [' + @var19 + '];');
ALTER TABLE [HydroSpecifics] ALTER COLUMN [Starter] nvarchar(max) NOT NULL;

GO

DECLARE @var20 sysname;
SELECT @var20 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'HoistWayDatas') AND [c].[name] = N'LandingSystem');
IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [HoistWayDatas] DROP CONSTRAINT [' + @var20 + '];');
ALTER TABLE [HoistWayDatas] ALTER COLUMN [LandingSystem] nvarchar(max) NOT NULL;

GO

ALTER TABLE [GenericFeaturesList] ADD [CRO] bit NOT NULL DEFAULT 0;

GO

ALTER TABLE [GenericFeaturesList] ADD [CarCallRead] bit NOT NULL DEFAULT 0;

GO

ALTER TABLE [GenericFeaturesList] ADD [CarKey] bit NOT NULL DEFAULT 0;

GO

ALTER TABLE [GenericFeaturesList] ADD [HCRO] bit NOT NULL DEFAULT 0;

GO

ALTER TABLE [GenericFeaturesList] ADD [HallCallRead] bit NOT NULL DEFAULT 0;

GO

ALTER TABLE [GenericFeaturesList] ADD [HallKey] bit NOT NULL DEFAULT 0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180314220555_NamesAndReq', N'2.0.1-rtm-125');

GO

ALTER TABLE [Jobs] ADD [EngID] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [Jobs] ADD [Status] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180409204605_UserId', N'2.0.1-rtm-125');

GO

DECLARE @var21 sysname;
SELECT @var21 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'CarLanternsVoltage');
IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var21 + '];');
ALTER TABLE [Indicators] DROP COLUMN [CarLanternsVoltage];

GO

DECLARE @var22 sysname;
SELECT @var22 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'CarLanternsVoltageType');
IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var22 + '];');
ALTER TABLE [Indicators] DROP COLUMN [CarLanternsVoltageType];

GO

DECLARE @var23 sysname;
SELECT @var23 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'CarPIDiscreteVoltage');
IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var23 + '];');
ALTER TABLE [Indicators] DROP COLUMN [CarPIDiscreteVoltage];

GO

DECLARE @var24 sysname;
SELECT @var24 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'CarPIDiscreteVoltageType');
IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var24 + '];');
ALTER TABLE [Indicators] DROP COLUMN [CarPIDiscreteVoltageType];

GO

DECLARE @var25 sysname;
SELECT @var25 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'HallLanternsVoltage');
IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var25 + '];');
ALTER TABLE [Indicators] DROP COLUMN [HallLanternsVoltage];

GO

DECLARE @var26 sysname;
SELECT @var26 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'HallLanternsVoltageType');
IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var26 + '];');
ALTER TABLE [Indicators] DROP COLUMN [HallLanternsVoltageType];

GO

DECLARE @var27 sysname;
SELECT @var27 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'HallPIDiscreteVoltage');
IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var27 + '];');
ALTER TABLE [Indicators] DROP COLUMN [HallPIDiscreteVoltage];

GO

DECLARE @var28 sysname;
SELECT @var28 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Indicators') AND [c].[name] = N'HallPIDiscreteVoltageType');
IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [Indicators] DROP CONSTRAINT [' + @var28 + '];');
ALTER TABLE [Indicators] DROP COLUMN [HallPIDiscreteVoltageType];

GO

EXEC sp_rename N'Indicators.PassingFloorDiscreteVoltageType', N'HallLanternsStyle', N'COLUMN';

GO

EXEC sp_rename N'Indicators.PassingFloorDiscreteVoltage', N'CarLanternsStyle', N'COLUMN';

GO

EXEC sp_rename N'GenericFeaturesList.MView', N'TopAccess', N'COLUMN';

GO

EXEC sp_rename N'GenericFeaturesList.IMon', N'Roped', N'COLUMN';

GO

EXEC sp_rename N'GenericFeaturesList.IDS', N'EPVoltage', N'COLUMN';

GO

ALTER TABLE [GenericFeaturesList] ADD [BottomAccess] bit NOT NULL DEFAULT 0;

GO

ALTER TABLE [GenericFeaturesList] ADD [BottomAccessLocation] nvarchar(max) NULL;

GO

ALTER TABLE [GenericFeaturesList] ADD [CTINSPST] bit NOT NULL DEFAULT 0;

GO

ALTER TABLE [GenericFeaturesList] ADD [EPCarsNumber] nvarchar(max) NULL;

GO

ALTER TABLE [GenericFeaturesList] ADD [EPContact] nvarchar(max) NULL;

GO

ALTER TABLE [GenericFeaturesList] ADD [EPOtherCars] bit NOT NULL DEFAULT 0;

GO

ALTER TABLE [GenericFeaturesList] ADD [EPSelect] nvarchar(max) NULL;

GO

ALTER TABLE [GenericFeaturesList] ADD [GovModel] nvarchar(max) NULL;

GO

ALTER TABLE [GenericFeaturesList] ADD [INCPButtons] nvarchar(max) NULL;

GO

ALTER TABLE [GenericFeaturesList] ADD [Monitoring] nvarchar(max) NULL;

GO

ALTER TABLE [GenericFeaturesList] ADD [PTI] nvarchar(max) NULL;

GO

ALTER TABLE [GenericFeaturesList] ADD [TopAccessLocation] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180521204336_MeetingUpdate', N'2.0.1-rtm-125');

GO

DECLARE @var29 sysname;
SELECT @var29 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'GenericFeaturesList') AND [c].[name] = N'PTI');
IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [GenericFeaturesList] DROP CONSTRAINT [' + @var29 + '];');
ALTER TABLE [GenericFeaturesList] ALTER COLUMN [PTI] bit NOT NULL;

GO

DECLARE @var30 sysname;
SELECT @var30 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'GenericFeaturesList') AND [c].[name] = N'EPSelect');
IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [GenericFeaturesList] DROP CONSTRAINT [' + @var30 + '];');
ALTER TABLE [GenericFeaturesList] ALTER COLUMN [EPSelect] bit NOT NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180521205334_MeetingUpdate1', N'2.0.1-rtm-125');

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190121214000_MyTest2', N'2.0.1-rtm-125');

GO

CREATE UNIQUE INDEX [IX_JobsExtensions_JobID] ON [JobsExtensions] ([JobID]);

GO

CREATE UNIQUE INDEX [IX_Indicators_JobID] ON [Indicators] ([JobID]);

GO

CREATE UNIQUE INDEX [IX_HydroSpecifics_JobID] ON [HydroSpecifics] ([JobID]);

GO

CREATE UNIQUE INDEX [IX_HoistWayDatas_JobID] ON [HoistWayDatas] ([JobID]);

GO

CREATE UNIQUE INDEX [IX_GenericFeaturesList_JobID] ON [GenericFeaturesList] ([JobID]);

GO

ALTER TABLE [GenericFeaturesList] ADD CONSTRAINT [FK_GenericFeaturesList_Jobs_JobID] FOREIGN KEY ([JobID]) REFERENCES [Jobs] ([JobID]) ON DELETE CASCADE;

GO

ALTER TABLE [HoistWayDatas] ADD CONSTRAINT [FK_HoistWayDatas_Jobs_JobID] FOREIGN KEY ([JobID]) REFERENCES [Jobs] ([JobID]) ON DELETE CASCADE;

GO

ALTER TABLE [HydroSpecifics] ADD CONSTRAINT [FK_HydroSpecifics_Jobs_JobID] FOREIGN KEY ([JobID]) REFERENCES [Jobs] ([JobID]) ON DELETE CASCADE;

GO

ALTER TABLE [Indicators] ADD CONSTRAINT [FK_Indicators_Jobs_JobID] FOREIGN KEY ([JobID]) REFERENCES [Jobs] ([JobID]) ON DELETE CASCADE;

GO

ALTER TABLE [JobsExtensions] ADD CONSTRAINT [FK_JobsExtensions_Jobs_JobID] FOREIGN KEY ([JobID]) REFERENCES [Jobs] ([JobID]) ON DELETE CASCADE;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190122133803_RelacionesJob', N'2.0.1-rtm-125');

GO

ALTER TABLE [States] ADD [CountryID] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [JobsExtensions] ADD [DoorOperatorID] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [Jobs] ADD [CityID] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [Jobs] ADD [FireCodeID] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [Jobs] ADD [JobTypeID] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [HoistWayDatas] ADD [LandingSystemID] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [Cities] ADD [FirecodeID] int NOT NULL DEFAULT 0;

GO

ALTER TABLE [Cities] ADD [StateID] int NOT NULL DEFAULT 0;

GO

CREATE INDEX [IX_States_CountryID] ON [States] ([CountryID]);

GO

CREATE INDEX [IX_JobsExtensions_DoorOperatorID] ON [JobsExtensions] ([DoorOperatorID]);

GO

CREATE INDEX [IX_Jobs_CityID] ON [Jobs] ([CityID]);

GO

CREATE INDEX [IX_Jobs_FireCodeID] ON [Jobs] ([FireCodeID]);

GO

CREATE INDEX [IX_Jobs_JobTypeID] ON [Jobs] ([JobTypeID]);

GO

CREATE INDEX [IX_HoistWayDatas_LandingSystemID] ON [HoistWayDatas] ([LandingSystemID]);

GO

CREATE INDEX [IX_Cities_FirecodeID] ON [Cities] ([FirecodeID]);

GO

CREATE INDEX [IX_Cities_StateID] ON [Cities] ([StateID]);

GO

ALTER TABLE [Cities] ADD CONSTRAINT [FK_Cities_FireCodes_FirecodeID] FOREIGN KEY ([FirecodeID]) REFERENCES [FireCodes] ([FireCodeID]) ON DELETE CASCADE;

GO

ALTER TABLE [Cities] ADD CONSTRAINT [FK_Cities_States_StateID] FOREIGN KEY ([StateID]) REFERENCES [States] ([StateID]) ON DELETE CASCADE;

GO

ALTER TABLE [HoistWayDatas] ADD CONSTRAINT [FK_HoistWayDatas_LandingSystems_LandingSystemID] FOREIGN KEY ([LandingSystemID]) REFERENCES [LandingSystems] ([LandingSystemID]) ON DELETE CASCADE;

GO

ALTER TABLE [Jobs] ADD CONSTRAINT [FK_Jobs_Cities_CityID] FOREIGN KEY ([CityID]) REFERENCES [Cities] ([CityID]) ON DELETE CASCADE;

GO

ALTER TABLE [Jobs] ADD CONSTRAINT [FK_Jobs_FireCodes_FireCodeID] FOREIGN KEY ([FireCodeID]) REFERENCES [FireCodes] ([FireCodeID]);

GO

ALTER TABLE [Jobs] ADD CONSTRAINT [FK_Jobs_JobTypes_JobTypeID] FOREIGN KEY ([JobTypeID]) REFERENCES [JobTypes] ([JobTypeID]) ON DELETE CASCADE;

GO

ALTER TABLE [JobsExtensions] ADD CONSTRAINT [FK_JobsExtensions_DoorOperators_DoorOperatorID] FOREIGN KEY ([DoorOperatorID]) REFERENCES [DoorOperators] ([DoorOperatorID]) ON DELETE CASCADE;

GO

ALTER TABLE [States] ADD CONSTRAINT [FK_States_Countries_CountryID] FOREIGN KEY ([CountryID]) REFERENCES [Countries] ([CountryID]) ON DELETE CASCADE;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190122142752_RelacionesExtra', N'2.0.1-rtm-125');

GO

DECLARE @var31 sysname;
SELECT @var31 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'JobsExtensions') AND [c].[name] = N'DoorBrand');
IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [JobsExtensions] DROP CONSTRAINT [' + @var31 + '];');
ALTER TABLE [JobsExtensions] DROP COLUMN [DoorBrand];

GO

DECLARE @var32 sysname;
SELECT @var32 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'JobsExtensions') AND [c].[name] = N'DoorModel');
IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [JobsExtensions] DROP CONSTRAINT [' + @var32 + '];');
ALTER TABLE [JobsExtensions] DROP COLUMN [DoorModel];

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190122151852_DoorInJobChanges', N'2.0.1-rtm-125');

GO

DECLARE @var33 sysname;
SELECT @var33 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'States') AND [c].[name] = N'Country');
IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [States] DROP CONSTRAINT [' + @var33 + '];');
ALTER TABLE [States] DROP COLUMN [Country];

GO

DECLARE @var34 sysname;
SELECT @var34 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Jobs') AND [c].[name] = N'JobCity');
IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [Jobs] DROP CONSTRAINT [' + @var34 + '];');
ALTER TABLE [Jobs] DROP COLUMN [JobCity];

GO

DECLARE @var35 sysname;
SELECT @var35 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Jobs') AND [c].[name] = N'JobType');
IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [Jobs] DROP CONSTRAINT [' + @var35 + '];');
ALTER TABLE [Jobs] DROP COLUMN [JobType];

GO

DECLARE @var36 sysname;
SELECT @var36 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Jobs') AND [c].[name] = N'SafetyCode');
IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [Jobs] DROP CONSTRAINT [' + @var36 + '];');
ALTER TABLE [Jobs] DROP COLUMN [SafetyCode];

GO

DECLARE @var37 sysname;
SELECT @var37 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'HoistWayDatas') AND [c].[name] = N'LandingSystem');
IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [HoistWayDatas] DROP CONSTRAINT [' + @var37 + '];');
ALTER TABLE [HoistWayDatas] DROP COLUMN [LandingSystem];

GO

DECLARE @var38 sysname;
SELECT @var38 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Cities') AND [c].[name] = N'Country');
IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [Cities] DROP CONSTRAINT [' + @var38 + '];');
ALTER TABLE [Cities] DROP COLUMN [Country];

GO

DECLARE @var39 sysname;
SELECT @var39 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Cities') AND [c].[name] = N'CurrentFireCode');
IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [Cities] DROP CONSTRAINT [' + @var39 + '];');
ALTER TABLE [Cities] DROP COLUMN [CurrentFireCode];

GO

DECLARE @var40 sysname;
SELECT @var40 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'Cities') AND [c].[name] = N'State');
IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [Cities] DROP CONSTRAINT [' + @var40 + '];');
ALTER TABLE [Cities] DROP COLUMN [State];

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190122222610_ChangesDbExtra01222019', N'2.0.1-rtm-125');

GO

