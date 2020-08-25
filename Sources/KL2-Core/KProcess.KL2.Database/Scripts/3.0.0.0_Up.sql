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


IF NOT EXISTS(SELECT name FROM sys.sql_logins WHERE name = 'KL2User')
	BEGIN
		SELECT @sql = 'CREATE LOGIN [KL2User] WITH PASSWORD=''' + REPLACE(@UserPassword, '''', '''''') + ''', DEFAULT_DATABASE=[KProcess.KL2], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF'
		EXEC sp_executesql @sql;
	END

IF NOT EXISTS(SELECT name FROM sys.sql_logins WHERE name = 'KL2Admin')
	BEGIN
		SELECT @sql = 'CREATE LOGIN [KL2Admin] WITH PASSWORD=''' + REPLACE(@AdminPassword, '''', '''''') + ''', DEFAULT_DATABASE=[KProcess.KL2], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF'
		EXEC sp_executesql @sql;
		EXEC master..sp_addsrvrolemember @loginame = N'KL2Admin', @rolename = N'sysadmin'
	END

USE [KProcess.KL2]

IF NOT EXISTS(SELECT name FROM sys.sysusers WHERE name = 'KL2User')
	BEGIN
		CREATE USER [KL2User] FOR LOGIN [KL2User]
		EXEC sp_addrolemember N'db_datareader', N'KL2User'
		EXEC sp_addrolemember N'db_datawriter', N'KL2User'
	END

IF NOT EXISTS(SELECT name FROM sys.sysusers WHERE name = 'KL2Admin')
	BEGIN
		CREATE USER [KL2Admin] FOR LOGIN [KL2Admin]
		EXEC sp_addrolemember N'db_owner', N'KL2Admin'
	END
GO

EXEC AddColumnIfNotExists 'Solution', 'Who', '[nvarchar](50) NULL';
EXEC AddColumnIfNotExists 'Solution', 'When', '[date] NULL';
EXEC AddColumnIfNotExists 'Solution', 'P', '[decimal](5,2) NOT NULL DEFAULT(0)';
EXEC AddColumnIfNotExists 'Solution', 'D', '[decimal](5,2) NOT NULL DEFAULT(0)';
EXEC AddColumnIfNotExists 'Solution', 'C', '[decimal](5,2) NOT NULL DEFAULT(0)';
EXEC AddColumnIfNotExists 'Solution', 'A', '[decimal](5,2) NOT NULL DEFAULT(0)';
GO

ALTER TABLE [dbo].[Action] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[ActionReduced] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[ActionType] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[ActionValue] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Objective] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Project] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[ProjectReferential] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Ref1] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Ref1Action] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Ref2] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Ref2Action] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Ref3] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Ref3Action] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Ref4] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Ref4Action] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Ref5] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Ref5Action] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Ref6] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Ref6Action] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Ref7] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Ref7Action] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[RefActionCategory] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[RefResource] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Role] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Scenario] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[ScenarioNature] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[ScenarioState] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Solution] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[User] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[Video] ADD [RowVersion] [timestamp] NULL;
ALTER TABLE [dbo].[VideoNature] ADD [RowVersion] [timestamp] NULL;
GO

--Passing Workshop from 50 to 100
ALTER TABLE [dbo].[Project] ALTER COLUMN [Workshop] [nvarchar](100) NOT NULL;

--ADD ProjectDir
CREATE TABLE [dbo].[ProjectDir] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (50) NOT NULL,
    [ParentId]   INT           NULL,
	[RowVersion] TIMESTAMP     NULL,
    CONSTRAINT [PK_ProjectDir] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
ALTER TABLE [dbo].[Project] ADD [ProjectDirId] [int] NULL;
GO
ALTER TABLE [dbo].[ProjectDir]
    ADD CONSTRAINT [FK_ProjectDir_ProjectDir] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[ProjectDir] ([Id]);
GO
ALTER TABLE [dbo].[Project]
    ADD CONSTRAINT [FK_Project_ProjectDir] FOREIGN KEY ([ProjectDirId]) REFERENCES [dbo].[ProjectDir] ([Id]);
GO

--ADD Project Recursive
ALTER TABLE [dbo].[Project] ADD [ParentProjectId] [int] NULL;
ALTER TABLE [dbo].[Project]
	ADD  CONSTRAINT [FK_Project_Project] FOREIGN KEY([ParentProjectId]) REFERENCES [dbo].[Project] ([ProjectId]);
GO

--ADD Backup et Restore seulement en local
EXEC InsertOrUpdateResource 'VM_MainWindowView_BackupRestoreOnlyOnLocal', 'fr-FR', N'La sauvegarde et la restauration ne peuvent pas être effectuées en mode distant.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowView_BackupRestoreOnlyOnLocal', 'en-US', N'Backup and restore can not be performed in remote mode.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowView_BackupRestoreOnlyOnLocal', 'pt-BR', N'Backup e restauração não pode ser realizada remotamente.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowView_BackupRestoreOnlyOnLocal', 'es-ES', N'Copia de seguridad y restauración no se pueden realizar de forma remota.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowView_BackupRestoreOnlyOnLocal', 'de-DE', N'Backup und Wiederherstellung kann nicht aus der Ferne durchgeführt werden.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowView_BackupRestoreOnlyOnLocal', 'pl-PL', N'Tworzenie kopii zapasowych i przywracanie nie może być wykonywana zdalnie.', null;
GO

--ADD Lecture en boucle
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_LoopPlaying', 'fr-FR', N'En boucle', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_LoopPlaying', 'en-US', N'Loop', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_LoopPlaying', 'pt-BR', N'Loop', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_LoopPlaying', 'es-ES', N'En bucle', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_LoopPlaying', 'de-DE', N'Schleife', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_LoopPlaying', 'pl-PL', N'Pętla', null;
GO

--ADD Titre Dialog pour Accès concurrent
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Title', 'fr-FR', N'Erreur d''accès concurrents', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Title', 'en-US', N'Access concurrency error', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Title', 'pt-BR', N'Concorrentes acesso erro', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Title', 'es-ES', N'Error de competidores acceso', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Title', 'de-DE', N'Konkurrenten Zugang Fehler', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Title', 'pl-PL', N'Konkurentom dostępu błędu', null;
GO

--ADD Message Dialog pour Accès concurrent
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Message', 'fr-FR', N'Un ou des éléments ont été modifiés par un autre utilisateur.
Leurs valeurs vont être rechargées à partir de la base de données.', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Message', 'en-US', N'One or more items have been modified by another user.
Their values ​​will be reloaded from the database.', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Message', 'pt-BR', N'Um ou mais itens foram alterados por outro usuário.
Seus valores serão recarregados a partir do banco de dados.', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Message', 'es-ES', N'Uno o más elementos se han cambiado por otro usuario.
Sus valores se volverá a cargar desde la base de datos.', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Message', 'de-DE', N'Ein oder mehrere Elemente wurden von einem anderen Benutzer geändert.
Ihre Werte werden aus der Datenbank geladen werden.', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Message', 'pl-PL', N'Jeden lub więcej elementów zostały zmienione przez innego użytkownika.
Ich wartości będą ładowane z bazy danych.', null;
GO

--ADD Tooltip pour changer la vidéo
EXEC InsertOrUpdateResource 'Common_Context_ChangeVideoTooltip_Content', 'fr-FR', N'Cliquer pour changer la séquence vidéo associée à la tâche', null;
EXEC InsertOrUpdateResource 'Common_Context_ChangeVideoTooltip_Content', 'en-US', N'Click to change the video clip associated to the task', null;
EXEC InsertOrUpdateResource 'Common_Context_ChangeVideoTooltip_Content', 'pt-BR', N'Clique para mudar a sequência do vídeo associado à tarefa', null;
EXEC InsertOrUpdateResource 'Common_Context_ChangeVideoTooltip_Content', 'es-ES', N'Pulse para cambiar el clip de vídeo asociado a la tarea', null;
EXEC InsertOrUpdateResource 'Common_Context_ChangeVideoTooltip_Content', 'de-DE', N'Klicken Sie hier um die an die Aufgabe zugeordnete Videosequenz zu ändern', null;
EXEC InsertOrUpdateResource 'Common_Context_ChangeVideoTooltip_Content', 'pl-PL', N'Kliknij aby zmienić clip video skojarzony z zadaniem', null;
GO

--ADD Create folder
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateFolder', 'fr-FR', N'Créer un dossier', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateFolder', 'en-US', N'Create a folder', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateFolder', 'pt-BR', N'Criar uma pasta', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateFolder', 'es-ES', N'Crear una carpeta', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateFolder', 'de-DE', N'Ordner erstellen', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateFolder', 'pl-PL', N'Utwórz katalog', null;
GO

--ADD Remove folder
EXEC InsertOrUpdateResource 'View_PrepareProject_RemoveFolder', 'fr-FR', N'Supprimer un dossier (Suppr)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RemoveFolder', 'en-US', N'Delete a folder (Suppr)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RemoveFolder', 'pt-BR', N'Excluir uma pasta (Suppr)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RemoveFolder', 'es-ES', N'Borrar una carpeta (Suppr)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RemoveFolder', 'de-DE', N'Ordner löschen (Suppr)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RemoveFolder', 'pl-PL', N'Usuń katalog (Suppr)', null;
GO

--ADD Can't remove folder
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveFolder', 'fr-FR', N'Impossible de supprimer un dossier qui contient un process avec des projets', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveFolder', 'en-US', N'The folder has to not contain a process with projects to be deleted', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveFolder', 'pt-BR', N'The folder has to not contain a process with projects to be deleted', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveFolder', 'es-ES', N'The folder has to not contain a process with projects to be deleted', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveFolder', 'de-DE', N'The folder has to not contain a process with projects to be deleted', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveFolder', 'pl-PL', N'The folder has to not contain a process with projects to be deleted', null;
GO

--ADD Can't remove folder
EXEC InsertOrUpdateResource 'View_PrepareProject_AllProjects', 'fr-FR', N'Tous les projets', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_AllProjects', 'en-US', N'All projects', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_AllProjects', 'pt-BR', N'Todos os projetos', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_AllProjects', 'es-ES', N'Todos los proyectos', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_AllProjects', 'de-DE', N'Alle projekte', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_AllProjects', 'pl-PL', N'Wszystkie projekty', null;
GO

--UPDATE Uri
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'fr-FR', N'Adresse du fichier attaché', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'en-US', N'Path to linked file', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'pt-BR', N'Local do arquivo vinculado', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'es-ES', N'Ruta de archivo adjunto', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'de-DE', N'Pfad zur angehängten Datei', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'pl-PL', N'Ścieżka do dołączonego pliku', null;
GO

--ADD View_PrepareProject_Empty
EXEC InsertOrUpdateResource 'View_PrepareProject_Empty', 'fr-FR', N'Projet vierge', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Empty', 'en-US', N'New project', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Empty', 'pt-BR', N'Novo projeto', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Empty', 'es-ES', N'Nuevo Proyecto', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Empty', 'de-DE', N'Neues Projekt', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Empty', 'pl-PL', N'Nowy projekt', null;
GO

--ADD View_PrepareProject_FixedSI
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSI', 'fr-FR', N'Scénario initial figé', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSI', 'en-US', N'Initial scenario frozen', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSI', 'pt-BR', N'Cenário inicial congelado', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSI', 'es-ES', N'Escenario inicial fijado', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSI', 'de-DE', N'Initiales Szenario eingefroren', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSI', 'pl-PL', N'Scenariusz początkowy zablokowany', null;
GO

--ADD View_PrepareProject_OneSC
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSC', 'fr-FR', N'Au moins un scénario cible', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSC', 'en-US', N'At least one target scenario', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSC', 'pt-BR', N'Pelo menos um cenário alvo', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSC', 'es-ES', N'Al menos un escenario objetivo', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSC', 'de-DE', N'Mindestens ein Zielszenario', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSC', 'pl-PL', N'Przynajmniej jeden scenariusz docelowy', null;
GO

--ADD View_PrepareProject_FixedSC
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSC', 'fr-FR', N'Au moins un scénario cible figé', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSC', 'en-US', N'At least one frozen target scenario', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSC', 'pt-BR', N'Pelo menos um cenário alvo congelado', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSC', 'es-ES', N'Al menos un escenario objetivo fijado', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSC', 'de-DE', N'Mindestens ein eingefrorenes Zielszenario', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSC', 'pl-PL', N'Przynajmniej jeden scenariusz docelowy zablokowany', null;
GO

--ADD View_PrepareProject_OneSV
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSV', 'fr-FR', N'Un scénario de validation', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSV', 'en-US', N'One validation scenario', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSV', 'pt-BR', N'Um cenário de validação', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSV', 'es-ES', N'Un escenario de validación', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSV', 'de-DE', N'Ein Validierungsszenario', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSV', 'pl-PL', N'Scenariusz zatwierdzony', null;
GO

--ADD View_PrepareProject_FixedSV
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSV', 'fr-FR', N'Scénario de validation figé', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSV', 'en-US', N'Validation scenario frozen', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSV', 'pt-BR', N'Cenário de validação congelado', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSV', 'es-ES', N'Escenario de validación fijado', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSV', 'de-DE', N'Validierungsszenario eingefroren', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSV', 'pl-PL', N'Zatwierdzony scenariusz zablokowany', null;
GO

--ADD View_MainWindow_Synchronize
EXEC InsertOrUpdateResource 'View_MainWindow_Synchronize', 'fr-FR', N'Synchroniser', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Synchronize', 'en-US', N'Synchronize', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Synchronize', 'pt-BR', N'Sincronizar', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Synchronize', 'es-ES', N'Sincronizar', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Synchronize', 'de-DE', N'Synchronisieren', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Synchronize', 'pl-PL', N'Synchronizować', null;

--ADD View_AnalyzeBuild_Who
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Who', 'fr-FR', N'Qui', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Who', 'en-US', N'Who', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Who', 'pt-BR', N'Quem', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Who', 'es-ES', N'Quién', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Who', 'de-DE', N'Wer', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Who', 'pl-PL', N'Kto', null;
GO

--ADD View_AnalyzeBuild_When
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_When', 'fr-FR', N'Quand', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_When', 'en-US', N'When', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_When', 'pt-BR', N'Quando', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_When', 'es-ES', N'Cuándo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_When', 'de-DE', N'Wann', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_When', 'pl-PL', N'Kiedy', null;
GO

--ADD Message lors d'une erreur de conversion lors d'un binding
EXEC InsertOrUpdateResource 'Common_Context_BindingValidationFailed', 'fr-FR', N'La valeur ''{0}'' n''a pas pu être convertie', N'La valeur d''origine';
EXEC InsertOrUpdateResource 'Common_Context_BindingValidationFailed', 'en-US', N'Value ''{0}'' could not be converted', N'La valeur d''origine';
EXEC InsertOrUpdateResource 'Common_Context_BindingValidationFailed', 'pt-BR', N'O valor ''{0}'' não pôde ser convertido', N'La valeur d''origine';
EXEC InsertOrUpdateResource 'Common_Context_BindingValidationFailed', 'es-ES', N'El valor ''{0}'' no se pudó convertir', N'La valeur d''origine';
EXEC InsertOrUpdateResource 'Common_Context_BindingValidationFailed', 'de-DE', N'Der Wert ''{0}'' könnte nicht konvertiert werden', N'La valeur d''origine';
EXEC InsertOrUpdateResource 'Common_Context_BindingValidationFailed', 'pl-PL', N'Wartość ''{0}'' nie mogła zostać przekształcona', N'La valeur d''origine';
GO

--UPDATE Administrateur Chef de projet
EXEC InsertOrUpdateResource 'Role_ADM_LONG', 'fr-FR', N'Administrateur', null;
EXEC InsertOrUpdateResource 'Role_ADM_LONG', 'en-US', N'Administrator', null;
EXEC InsertOrUpdateResource 'Role_ADM_LONG', 'pt-BR', N'Administrador', null;
EXEC InsertOrUpdateResource 'Role_ADM_LONG', 'es-ES', N'Administrador', null;
EXEC InsertOrUpdateResource 'Role_ADM_LONG', 'de-DE', N'Administrator', null;
EXEC InsertOrUpdateResource 'Role_ADM_LONG', 'pl-PL', N'Administrator', null;
GO
EXEC InsertOrUpdateResource 'Role_ADM_SHORT', 'fr-FR', N'Administrateur', null;
EXEC InsertOrUpdateResource 'Role_ADM_SHORT', 'en-US', N'Administrator', null;
EXEC InsertOrUpdateResource 'Role_ADM_SHORT', 'pt-BR', N'Administrador', null;
EXEC InsertOrUpdateResource 'Role_ADM_SHORT', 'es-ES', N'Administrador', null;
EXEC InsertOrUpdateResource 'Role_ADM_SHORT', 'de-DE', N'Administrator', null;
EXEC InsertOrUpdateResource 'Role_ADM_SHORT', 'pl-PL', N'Administrator', null;
GO

--ADD View_PrepareScenarios_CantChangeStatut
EXEC InsertOrUpdateResource 'View_PrepareScenarios_CantChangeStatut', 'fr-FR', N'Défiger un scénario n’est pas autorisé tant qu''un scénario de Validation existe dans ce projet', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_CantChangeStatut', 'en-US', N'Unfreezing a scenario is not allowed as long a Validation scenario exists in this project', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_CantChangeStatut', 'pt-BR', N'Não é permitido descongelar um cenário enquanto o cenário de validação existir no projeto', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_CantChangeStatut', 'es-ES', N'No es posible liberar el escenario debido a que hay un escenario de validación existente en este proyecto', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_CantChangeStatut', 'de-DE', N'Wiederaktivieren eines Szenarios ist nicht erlaubt, solange ein Validierungsszenario in diesem Projekt existiert', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_CantChangeStatut', 'pl-PL', N'Odblokowanie scenariusza nie jest możliwe tak długo jsk w tym projekcie istnieje scenariusz zatwierdzający', null;
GO