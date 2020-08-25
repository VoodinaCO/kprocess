EXEC AddColumnIfNotExists 'Project', 'Formation_Disposition', '[VARBINARY](MAX) NULL';
EXEC AddColumnIfNotExists 'Project', 'Inspection_Disposition', '[VARBINARY](MAX) NULL';
EXEC AddColumnIfNotExists 'Project', 'Audit_Disposition', '[VARBINARY](MAX) NULL';
GO