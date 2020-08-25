SET NOCOUNT ON
GO

USE [KProcess.KL2]

DROP USER [KL2User];
DROP USER [KL2Admin];

CREATE USER [KL2User] FOR LOGIN [KL2User];
EXEC sp_addrolemember N'db_datareader', N'KL2User';
EXEC sp_addrolemember N'db_datawriter', N'KL2User';

CREATE USER [KL2Admin] FOR LOGIN [KL2Admin];
EXEC sp_addrolemember N'db_owner', N'KL2Admin';
GO