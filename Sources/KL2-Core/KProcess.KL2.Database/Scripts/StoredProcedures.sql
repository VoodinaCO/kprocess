USE [KProcess.KL2]
GO

IF EXISTS(SELECT 1 FROM sysobjects WHERE type = 'P' and name = 'InsertOrUpdateDatabaseVersion')
BEGIN
   DROP PROCEDURE InsertOrUpdateDatabaseVersion
END
GO
CREATE PROC InsertOrUpdateDatabaseVersion
  @version nvarchar(30)
AS
BEGIN

	IF NOT EXISTS (SELECT 1 FROM SYS.EXTENDED_PROPERTIES WHERE class_desc='DATABASE' AND name='KL_Version')
		EXEC sys.sp_addextendedproperty @name = N'KL_Version', @value = @version;
	ELSE
		EXEC sys.sp_updateextendedproperty @name = N'KL_Version', @value = @version;
END
GO

IF EXISTS(SELECT 1 FROM sysobjects WHERE type = 'P' and name = 'GetDatabaseVersion')
BEGIN
   DROP PROCEDURE GetDatabaseVersion
END
GO
CREATE PROC GetDatabaseVersion
AS
BEGIN

	SELECT CONVERT(nvarchar(30),[Value]) AS 'Version' FROM SYS.EXTENDED_PROPERTIES WHERE class_desc='DATABASE' AND name='KL_Version';

END
GO

IF EXISTS(SELECT 1 FROM sysobjects WHERE type = 'P' and name = 'InsertOrUpdateResource')
BEGIN
   DROP PROCEDURE InsertOrUpdateResource
END
GO
CREATE PROC InsertOrUpdateResource
  @key nvarchar(100),
  @language nchar(5),
  @value nvarchar(500),
  @comment nvarchar(500)
AS
BEGIN

	DECLARE @id int;
	SELECT @id = [ResourceId] FROM [dbo].[AppResourceKey] WHERE [ResourceKey] = @key;

	IF @id IS NULL
	BEGIN
		INSERT INTO [dbo].[AppResourceKey] ([ResourceKey]) VALUES (@key);
		SET @id = @@IDENTITY;
	END

	IF (SELECT COUNT(*) FROM [dbo].[AppResourceValue] WHERE [ResourceId] = @id AND [LanguageCode] = @language) > 0
		UPDATE [dbo].[AppResourceValue] SET 
			[Value] = @value,
			[Comment] = @comment,
			[CreatedByUserId] = 1,
			[CreationDate] = GETDATE(),
			[ModifiedByUserId] = 1,
			[LastModificationDate] = GETDATE()
			WHERE [ResourceId] = @id AND [LanguageCode] = @language;
	ELSE
		INSERT INTO [dbo].[AppResourceValue]
				   ([ResourceId]
				   ,[LanguageCode]
				   ,[Value]
				   ,[Comment]
				   ,[CreatedByUserId]
				   ,[CreationDate]
				   ,[ModifiedByUserId]
				   ,[LastModificationDate])
			 VALUES
				   (@id
				   ,@language
				   ,@value
				   ,@comment
				   ,1
				   ,GETDATE()
				   ,1
				   ,GETDATE());

	RETURN @id;

END
GO

IF EXISTS(SELECT 1 FROM sysobjects WHERE type = 'P' and name = 'DeleteResource')
BEGIN
   DROP PROCEDURE DeleteResource
END
GO
CREATE PROC DeleteResource
  @key nvarchar(100)
AS
BEGIN

	DECLARE @id int;
	SELECT @id = [ResourceId] FROM [dbo].[AppResourceKey] WHERE [ResourceKey] = @key;

	IF @id IS NOT NULL
	BEGIN
		DELETE FROM [dbo].[AppResourceValue] WHERE [ResourceId] = @id;
		DELETE FROM [dbo].[AppResourceKey] WHERE [ResourceId] = @id;
	END
END
GO

IF EXISTS(SELECT 1 FROM sysobjects WHERE type = 'P' and name = 'DropColumnWithConstraints')
BEGIN
   DROP PROCEDURE DropColumnWithConstraints
END
GO
CREATE PROC DropColumnWithConstraints
  @tableName nvarchar(100),
  @columnName nvarchar(100)
AS
BEGIN

	DECLARE @columnId int;
	SELECT @columnId = column_id FROM sys.columns
	WHERE NAME = @columnName
	AND object_id = OBJECT_ID(@tableName);

	DECLARE @constraintName nvarchar(200);
	SELECT @constraintName = name FROM sys.default_constraints
	WHERE parent_object_id = OBJECT_ID(@tableName)
	AND parent_column_id = @columnId;

	IF @constraintName IS NOT NULL
		EXEC('ALTER TABLE [dbo].[' + @tableName + '] DROP CONSTRAINT [' + @constraintName + ']');
	IF @columnId IS NOT NULL
		EXEC('ALTER TABLE [dbo].[' + @tableName + '] DROP COLUMN [' + @columnName + ']');
END
GO

IF EXISTS(SELECT 1 FROM sysobjects WHERE type = 'P' and name = 'AddColumnIfNotExists')
BEGIN
   DROP PROCEDURE AddColumnIfNotExists
END
GO
CREATE PROC AddColumnIfNotExists
  @tableName nvarchar(100),
  @columnName nvarchar(100),
  @params nvarchar(MAX)
AS
BEGIN

	DECLARE @columnId int;
	SELECT @columnId = column_id FROM sys.columns
	WHERE NAME = @columnName
	AND object_id = OBJECT_ID(@tableName);

	IF @columnId IS NULL
		EXEC('ALTER TABLE [dbo].[' + @tableName + '] ADD [' + @columnName + '] ' + @params);
END
GO