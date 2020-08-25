USE [master]
GO

ALTER DATABASE [KProcess.Ksmed] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

RESTORE DATABASE [KProcess.Ksmed] FROM DISK = N'\KProcess.Ksmed.bak' WITH  FILE = 1,  NOUNLOAD,  REPLACE,  STATS = 10;
GO

ALTER DATABASE [KProcess.Ksmed] SET MULTI_USER;
GO

EXEC [KProcess.Ksmed].dbo.sp_changedbowner @loginame = N'KsmedAdmin', @map = false
GO

USE [KProcess.Ksmed];
GO

DROP USER KsmedUser;
CREATE USER [KsmedUser] FOR LOGIN [KsmedUser]; 
EXEC sp_addrolemember N'db_datareader', N'KsmedUser'
EXEC sp_addrolemember N'db_datawriter', N'KsmedUser'
