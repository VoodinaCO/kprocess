SET NOCOUNT ON
GO

USE [master]
GO

DECLARE @UserPassword as varchar(max);
DECLARE @AdminPassword as varchar(max);

DECLARE @sql nvarchar(4000);


DECLARE @vb as Varbinary(max);

SET @vb = 0x010000009F0F8D00B7FA1EBE08FA559EB674374E40704DDCFC0E27606ABD000126CA846E;
SELECT @UserPassword = CONVERT(varchar(max), DECRYPTBYPASSPHRASE('2_4*Pu*v"Oz1DUYUGYEXEJ9`', @vb));

SET @vb = 0x0100000078A1DDFE493F60CF98CD51FC249235CCE9430A02E7D9F4B03EECCBC3F9DF1C93;
SELECT @AdminPassword = CONVERT(varchar(max), DECRYPTBYPASSPHRASE('2_4*Pu*v"Oz1DUYUGYEXEJ9`', @vb));


IF NOT EXISTS(SELECT name FROM sys.sql_logins WHERE name = 'KsmedUser')
	BEGIN
		SELECT @sql = 'CREATE LOGIN [KsmedUser] WITH PASSWORD=''' + REPLACE(@UserPassword, '''', '''''') + ''', DEFAULT_DATABASE=[KProcess.KL2], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF'
		EXEC sp_executesql @sql;
	END

IF NOT EXISTS(SELECT name FROM sys.sql_logins WHERE name = 'KsmedAdmin')
	BEGIN
		SELECT @sql = 'CREATE LOGIN [KsmedAdmin] WITH PASSWORD=''' + REPLACE(@AdminPassword, '''', '''''') + ''', DEFAULT_DATABASE=[KProcess.KL2], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF'
		EXEC sp_executesql @sql;
		EXEC master..sp_addsrvrolemember @loginame = N'KsmedAdmin', @rolename = N'sysadmin'
	END

USE [KProcess.KL2]

IF NOT EXISTS(SELECT name FROM sys.sysusers WHERE name = 'KsmedUser')
	BEGIN
		CREATE USER [KsmedUser] FOR LOGIN [KsmedUser]
		EXEC sp_addrolemember N'db_datareader', N'KsmedUser'
		EXEC sp_addrolemember N'db_datawriter', N'KsmedUser'
	END

IF NOT EXISTS(SELECT name FROM sys.sysusers WHERE name = 'KsmedAdmin')
	BEGIN
		CREATE USER [KsmedAdmin] FOR LOGIN [KsmedAdmin]
		EXEC sp_addrolemember N'db_owner', N'KsmedAdmin'
	END
GO

EXEC DropColumnWithConstraints 'Solution', 'Who';
EXEC DropColumnWithConstraints 'Solution', 'When';
EXEC DropColumnWithConstraints 'Solution', 'P';
EXEC DropColumnWithConstraints 'Solution', 'D';
EXEC DropColumnWithConstraints 'Solution', 'C';
EXEC DropColumnWithConstraints 'Solution', 'A';
GO

ALTER TABLE [dbo].[Action] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[ActionReduced] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[ActionType] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[ActionValue] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Objective] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Project] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[ProjectReferential] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Ref1] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Ref1Action] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Ref2] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Ref2Action] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Ref3] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Ref3Action] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Ref4] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Ref4Action] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Ref5] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Ref5Action] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Ref6] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Ref6Action] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Ref7] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Ref7Action] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[RefActionCategory] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[RefResource] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Role] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Scenario] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[ScenarioNature] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[ScenarioState] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Solution] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[User] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[Video] DROP COLUMN [RowVersion];
ALTER TABLE [dbo].[VideoNature] DROP COLUMN [RowVersion];
GO

--Repassing Workshop from 100 to 50
ALTER TABLE [dbo].[Project] ALTER COLUMN [Workshop] [nvarchar](50) NOT NULL;

ALTER TABLE [dbo].[Project] DROP CONSTRAINT [FK_Project_Project];
ALTER TABLE [dbo].[Project] DROP COLUMN [ParentProjectId];
GO

ALTER TABLE [dbo].[Project] DROP CONSTRAINT [FK_Project_ProjectDir];
ALTER TABLE [dbo].[Project] DROP COLUMN [ProjectDirId];
GO

ALTER TABLE [dbo].[ProjectDir] DROP CONSTRAINT [FK_ProjectDir_ProjectDir];
DROP TABLE [dbo].[ProjectDir];
GO

EXEC DeleteResource 'VM_MainWindowView_BackupRestoreOnlyOnLocal';
GO

EXEC DeleteResource 'View_AnalyzeBuild_LoopPlaying';
GO

EXEC DeleteResource 'Common_Context_DbConcurrency_Title';
GO

EXEC DeleteResource 'Common_Context_DbConcurrency_Message';
GO

EXEC DeleteResource 'Common_Context_ChangeVideoTooltip_Content';
GO

EXEC DeleteResource 'View_PrepareProject_CreateFolder';
GO

EXEC DeleteResource 'View_PrepareProject_RemoveFolder';
GO

EXEC DeleteResource 'View_PrepareProject_CantRemoveFolder';
GO

EXEC DeleteResource 'View_PrepareProject_AllProjects';
GO

EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'fr-FR', N'Uri', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'en-US', N'Uri', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'pt-BR', N'Uri', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'es-ES', N'Uri', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'de-DE', N'Uri', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'pl-PL', N'Źródło', null;
GO

EXEC DeleteResource 'View_PrepareProject_Empty';
GO
EXEC DeleteResource 'View_PrepareProject_FixedSI';
GO
EXEC DeleteResource 'View_PrepareProject_OneSC';
GO
EXEC DeleteResource 'View_PrepareProject_FixedSC';
GO
EXEC DeleteResource 'View_PrepareProject_OneSV';
GO
EXEC DeleteResource 'View_PrepareProject_FixedSV';
GO

EXEC DeleteResource 'View_MainWindow_Synchronize';
GO

EXEC DeleteResource 'View_AnalyzeBuild_Who';
GO
EXEC DeleteResource 'View_AnalyzeBuild_When';
GO

EXEC DeleteResource 'Common_Context_BindingValidationFailed';
GO