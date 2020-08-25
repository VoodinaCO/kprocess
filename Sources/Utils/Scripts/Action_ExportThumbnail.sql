IF EXISTS(SELECT 1 FROM sysobjects WHERE type = 'P' and name = 'Action_ExportThumbnail')
BEGIN
   DROP PROCEDURE Action_ExportThumbnail
END
GO
CREATE PROCEDURE [dbo].[Action_ExportThumbnail] (
   @ActionId INT
   ,@ImageFolderPath NVARCHAR(1000)
   ,@Extension NVARCHAR(30)
   )
AS
BEGIN
   DECLARE @ImageData VARBINARY (max);
   DECLARE @Path2OutFile NVARCHAR (2000);
   DECLARE @Obj INT
 
   SET NOCOUNT ON
 
   SELECT @ImageData = (
         SELECT convert (VARBINARY (max), Thumbnail, 1)
         FROM dbo.[Action]
         WHERE [ActionId] = @ActionId
         );
 
   SET @Path2OutFile = CONCAT (
         @ImageFolderPath
         ,'\'
         , @ActionId
         , @Extension
         );
    BEGIN TRY
     EXEC sp_OACreate 'ADODB.Stream' ,@Obj OUTPUT;
     EXEC sp_OASetProperty @Obj ,'Type',1;
     EXEC sp_OAMethod @Obj,'Open';
     EXEC sp_OAMethod @Obj,'Write', NULL, @ImageData;
     EXEC sp_OAMethod @Obj,'SaveToFile', NULL, @Path2OutFile, 2;
     EXEC sp_OAMethod @Obj,'Close';
     EXEC sp_OADestroy @Obj;
    END TRY
    
 BEGIN CATCH
  EXEC sp_OADestroy @Obj;
 END CATCH
 
   SET NOCOUNT OFF
END
GO