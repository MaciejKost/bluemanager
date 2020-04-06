CREATE TABLE [Tools] (
                         [Id] int PRIMARY KEY IDENTITY(1, 1),
                         [ToolName] nvarchar(255),
                         [MacAddress] nvarchar(255)
)
GO

CREATE TABLE [Hubs] (
                        [Id] int PRIMARY KEY IDENTITY(1, 1),
                        [LocationName] nvarchar(255),
                        [IpAddress] nvarchar(255),
                        [IsActive] bool
)
GO

CREATE TABLE [ToolAtHubs] (
                              [Id] int PRIMARY KEY IDENTITY(1, 1),
                              [HubId] int,
                              [ToolId] int,
                              [BleName] nvarchar(255),
                              [Timestamp] datetime2,
                              [Strength] int
)
GO

CREATE TABLE [ToolBatteryReadouts] (
                                       [Id] int PRIMARY KEY IDENTITY(1, 1),
                                       [ToolId] int,
                                       [Timestamp] datetime2,
                                       [BatteryState] int
)
GO

ALTER TABLE [ToolAtHubs] ADD FOREIGN KEY ([HubId]) REFERENCES [Hubs] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [ToolAtHubs] ADD FOREIGN KEY ([ToolId]) REFERENCES [Tools] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [ToolBatteryReadouts] ADD FOREIGN KEY ([ToolId]) REFERENCES [Tools] ([Id]) ON DELETE CASCADE;
GO


---------------
---- VIEWS
---------------


CREATE VIEW ToolLastLocations AS
SELECT loc.ToolId, loc.HubId, loc.BleName, loc.Timestamp as LocationTimestamp, bat.BatteryState, bat.Timestamp as BatteryReadTimestamp
FROM (
         SELECT *, ROW_NUMBER() OVER (PARTITION BY p0.ToolId ORDER BY p0.Timestamp DESC) as row
         FROM ToolAtHubs p0
     ) AS loc
JOIN (
    SELECT *, ROW_NUMBER() OVER (PARTITION BY p1.ToolId ORDER BY p1.Timestamp DESC) as row
    FROM dbo.ToolBatteryReadouts p1
    ) as bat ON loc.ToolId = bat.ToolId
WHERE loc.row <= 1 AND bat.row <= 1