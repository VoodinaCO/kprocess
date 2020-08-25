ALTER TABLE [dbo].[Publication] ALTER COLUMN [ProjectId] INT NOT NULL;
GO
ALTER TABLE [dbo].[Publication] ALTER COLUMN [ProcessId] INT NOT NULL;
GO

EXEC AddColumnIfNotExists 'Procedure', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'Project', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'ProjectDir', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'Scenario', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'RefEquipment', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'RefOperator', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'RefActionCategory', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'Ref1', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'Ref2', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'Ref3', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'Ref4', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'Ref5', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'Ref6', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';
GO
EXEC AddColumnIfNotExists 'Ref7', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';
GO

EXEC AddColumnIfNotExists 'Anomaly', 'Origin', '[INT] NOT NULL DEFAULT((0))';
GO