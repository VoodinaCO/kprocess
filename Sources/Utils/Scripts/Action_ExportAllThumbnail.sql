IF EXISTS(SELECT 1 FROM sysobjects WHERE type = 'P' and name = 'Action_ExportAllThumbnail')
BEGIN
   DROP PROCEDURE Action_ExportAllThumbnail
END
GO
CREATE PROCEDURE [dbo].[Action_ExportAllThumbnail] (
   @ImageFolderPath NVARCHAR(1000)
   ,@Extension NVARCHAR(30)
   )
AS
BEGIN
	DECLARE @ActionId int;
	DECLARE Action_Cursor CURSOR FOR  
	SELECT [ActionId] FROM dbo.[Action] WHERE [Thumbnail] IS NOT NULL;
	OPEN Action_Cursor;  
	FETCH NEXT FROM Action_Cursor INTO @ActionId;  
	WHILE @@FETCH_STATUS = 0  
	    BEGIN
	        PRINT CONCAT('Export de la vignette de l''action : ',@ActionId);
	        EXECUTE dbo.[Action_ExportThumbnail] @ActionId, @ImageFolderPath, @Extension;
	        FETCH NEXT FROM Action_Cursor INTO @ActionId;  
	    END;  
	CLOSE Action_Cursor;
	DEALLOCATE Action_Cursor;
END
GO