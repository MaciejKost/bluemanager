CREATE TABLE [Tools] (
                         [Id] int PRIMARY KEY IDENTITY(1, 1),
                         [ToolName] nvarchar(255),
                         [MacAddress] nvarchar(255)
)
GO

CREATE TABLE [Hubs] (
                        [Id] int PRIMARY KEY IDENTITY(1, 1),
                        [LocationName] nvarchar(255),
                        [IpAddress] nvarchar(255)
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
                                       [ToolId] int,
                                       [Timestamp] datetime2,
                                       [BatteryState] int
)
GO

ALTER TABLE [ToolAtHubs] ADD FOREIGN KEY ([HubId]) REFERENCES [Hubs] ([Id])
GO

ALTER TABLE [ToolAtHubs] ADD FOREIGN KEY ([ToolId]) REFERENCES [Tools] ([Id])
GO

ALTER TABLE [ToolBatteryReadouts] ADD FOREIGN KEY ([ToolId]) REFERENCES [Tools] ([Id])
GO


---------------
---- VIEWS
---------------


CREATE VIEW ToolLastLocation AS
SELECT ToolId, HubId, BleName, Timestamp
FROM (
         SELECT *, ROW_NUMBER() OVER (PARTITION BY p0.ToolId ORDER BY p0.Timestamp DESC) as row
         FROM ToolAtHubs p0
     ) AS t
WHERE t.row <= 1