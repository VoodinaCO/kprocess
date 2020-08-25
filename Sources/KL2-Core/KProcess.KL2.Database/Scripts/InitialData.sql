SET NOCOUNT ON
GO

USE [KProcess.KL2]
GO

-- Langues par défaut
/*-------------------------------------------------------*/
/* Langues */
/* Page pour les valeurs : http://msdn.microsoft.com/en-us/goglobal/bb896001.aspx */
/*-------------------------------------------------------*/

INSERT INTO [Language] VALUES ('en-US', 'English (United States)', 'The_United_States');
INSERT INTO [Language] VALUES ('fr-FR', 'Français (France)', 'France');
INSERT INTO [Language] VALUES ('es-ES', 'Español (España)', 'Spain');
INSERT INTO [Language] VALUES ('pt-BR', 'Português (Brasil)', 'Brazil');
INSERT INTO [Language] VALUES ('de-DE', 'Deutsch (Deutschland)', 'Germany');
INSERT INTO [Language] VALUES ('pl-PL', 'Polski (Polska)', 'Poland');
GO


-- Utilisateur(s) par défaut
/* Contient utilisateurs par défaut */
USE [master]
GO

DECLARE @UserPassword as varchar(max);
DECLARE @AdminPassword as varchar(max);

DECLARE @AdminHash as binary(20);
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
	END

EXEC master..sp_addsrvrolemember @loginame = N'KL2Admin', @rolename = N'sysadmin'

USE [KProcess.KL2]

CREATE USER [KL2User] FOR LOGIN [KL2User]
EXEC sp_addrolemember N'db_datareader', N'KL2User'
EXEC sp_addrolemember N'db_datawriter', N'KL2User'

CREATE USER [KL2Admin] FOR LOGIN [KL2Admin]
EXEC sp_addrolemember N'db_owner', N'KL2Admin'

SET @AdminHash = HashBytes('SHA1', 'admin');
INSERT INTO [dbo].[User] ([DefaultLanguageCode], [Username], [Password], [Firstname], [Name], [Email], [PhoneNumber], [CreatedByUserId],[CreationDate],[ModifiedByUserId],[LastModificationDate], [IsDeleted])  VALUES
	('en-US', 'admin', @AdminHash, 'Administrator', '', '', '', null, GETDATE(), null, GETDATE(), 0);
GO

-- Ressources de l'appli
EXEC InsertOrUpdateResource 'ActionType_E_Long', 'fr-FR', N'à Externaliser', null;
EXEC InsertOrUpdateResource 'ActionType_E_Long', 'en-US', N'To move Ext.', null;
EXEC InsertOrUpdateResource 'ActionType_E_Long', 'pt-BR', N'Para mover Ext.', null;
EXEC InsertOrUpdateResource 'ActionType_E_Long', 'es-ES', N'Externalizar', null;
EXEC InsertOrUpdateResource 'ActionType_E_Long', 'de-DE', N'Extern', null;
EXEC InsertOrUpdateResource 'ActionType_E_Long', 'pl-PL', N'Przenieść do zew', null;
GO
EXEC InsertOrUpdateResource 'ActionType_E_Short', 'fr-FR', N'E', null;
EXEC InsertOrUpdateResource 'ActionType_E_Short', 'en-US', N'E', null;
EXEC InsertOrUpdateResource 'ActionType_E_Short', 'pt-BR', N'E', null;
EXEC InsertOrUpdateResource 'ActionType_E_Short', 'es-ES', N'E', null;
EXEC InsertOrUpdateResource 'ActionType_E_Short', 'de-DE', N'E', null;
EXEC InsertOrUpdateResource 'ActionType_E_Short', 'pl-PL', N'Z', null;
GO
EXEC InsertOrUpdateResource 'ActionType_I_Long', 'fr-FR', N'Interne', null;
EXEC InsertOrUpdateResource 'ActionType_I_Long', 'en-US', N'To keep Int.', null;
EXEC InsertOrUpdateResource 'ActionType_I_Long', 'pt-BR', N'Para manter Int.', null;
EXEC InsertOrUpdateResource 'ActionType_I_Long', 'es-ES', N'Interno', null;
EXEC InsertOrUpdateResource 'ActionType_I_Long', 'de-DE', N'Intern', null;
EXEC InsertOrUpdateResource 'ActionType_I_Long', 'pl-PL', N'Zostawić wew', null;
GO
EXEC InsertOrUpdateResource 'ActionType_I_Short', 'fr-FR', N'I', null;
EXEC InsertOrUpdateResource 'ActionType_I_Short', 'en-US', N'I', null;
EXEC InsertOrUpdateResource 'ActionType_I_Short', 'pt-BR', N'I', null;
EXEC InsertOrUpdateResource 'ActionType_I_Short', 'es-ES', N'I', null;
EXEC InsertOrUpdateResource 'ActionType_I_Short', 'de-DE', N'I', null;
EXEC InsertOrUpdateResource 'ActionType_I_Short', 'pl-PL', N'W', null;
GO
EXEC InsertOrUpdateResource 'ActionType_S_Long', 'fr-FR', N'à Supprimer', null;
EXEC InsertOrUpdateResource 'ActionType_S_Long', 'en-US', N'To Delete', null;
EXEC InsertOrUpdateResource 'ActionType_S_Long', 'pt-BR', N'Para eXcluir', null;
EXEC InsertOrUpdateResource 'ActionType_S_Long', 'es-ES', N'Borrar', null;
EXEC InsertOrUpdateResource 'ActionType_S_Long', 'de-DE', N'entFernen', null;
EXEC InsertOrUpdateResource 'ActionType_S_Long', 'pl-PL', N'Do usunięcia', null;
GO
EXEC InsertOrUpdateResource 'ActionType_S_Short', 'fr-FR', N'S', null;
EXEC InsertOrUpdateResource 'ActionType_S_Short', 'en-US', N'D', null;
EXEC InsertOrUpdateResource 'ActionType_S_Short', 'pt-BR', N'X', null;
EXEC InsertOrUpdateResource 'ActionType_S_Short', 'es-ES', N'B', null;
EXEC InsertOrUpdateResource 'ActionType_S_Short', 'de-DE', N'F', null;
EXEC InsertOrUpdateResource 'ActionType_S_Short', 'pl-PL', N'U', null;
GO
EXEC InsertOrUpdateResource 'ActionValue_BNVA_Long', 'fr-FR', N'Business Non Valeur Ajoutée', null;
EXEC InsertOrUpdateResource 'ActionValue_BNVA_Long', 'en-US', N'Business Non Value Added', null;
EXEC InsertOrUpdateResource 'ActionValue_BNVA_Long', 'pt-BR', N'Business Non Value Added', null;
EXEC InsertOrUpdateResource 'ActionValue_BNVA_Long', 'es-ES', N'Business Non Value Added', null;
EXEC InsertOrUpdateResource 'ActionValue_BNVA_Long', 'de-DE', N'geschäftlich nicht-wertschöpfend', null;
EXEC InsertOrUpdateResource 'ActionValue_BNVA_Long', 'pl-PL', N'Business Non Value Added', null;
GO
EXEC InsertOrUpdateResource 'ActionValue_BNVA_Short', 'fr-FR', N'BNVA', null;
EXEC InsertOrUpdateResource 'ActionValue_BNVA_Short', 'en-US', N'BNVA', null;
EXEC InsertOrUpdateResource 'ActionValue_BNVA_Short', 'pt-BR', N'BNVA', null;
EXEC InsertOrUpdateResource 'ActionValue_BNVA_Short', 'es-ES', N'BNVA', null;
EXEC InsertOrUpdateResource 'ActionValue_BNVA_Short', 'de-DE', N'BNVA', null;
EXEC InsertOrUpdateResource 'ActionValue_BNVA_Short', 'pl-PL', N'BNVA', null;
GO
EXEC InsertOrUpdateResource 'ActionValue_NVA_Long', 'fr-FR', N'Non Valeur Ajoutée', null;
EXEC InsertOrUpdateResource 'ActionValue_NVA_Long', 'en-US', N'Non Value Added', null;
EXEC InsertOrUpdateResource 'ActionValue_NVA_Long', 'pt-BR', N'Non Value Added', null;
EXEC InsertOrUpdateResource 'ActionValue_NVA_Long', 'es-ES', N'Non Value Added', null;
EXEC InsertOrUpdateResource 'ActionValue_NVA_Long', 'de-DE', N'nicht-wertschöpfend', null;
EXEC InsertOrUpdateResource 'ActionValue_NVA_Long', 'pl-PL', N'Non Value Added', null;
GO
EXEC InsertOrUpdateResource 'ActionValue_NVA_Short', 'fr-FR', N'NVA', null;
EXEC InsertOrUpdateResource 'ActionValue_NVA_Short', 'en-US', N'NVA', null;
EXEC InsertOrUpdateResource 'ActionValue_NVA_Short', 'pt-BR', N'NVA', null;
EXEC InsertOrUpdateResource 'ActionValue_NVA_Short', 'es-ES', N'NVA', null;
EXEC InsertOrUpdateResource 'ActionValue_NVA_Short', 'de-DE', N'NVA', null;
EXEC InsertOrUpdateResource 'ActionValue_NVA_Short', 'pl-PL', N'NVA', null;
GO
EXEC InsertOrUpdateResource 'ActionValue_VA_Long', 'fr-FR', N'Valeur Ajoutée', null;
EXEC InsertOrUpdateResource 'ActionValue_VA_Long', 'en-US', N'Value Added', null;
EXEC InsertOrUpdateResource 'ActionValue_VA_Long', 'pt-BR', N'Value Added', null;
EXEC InsertOrUpdateResource 'ActionValue_VA_Long', 'es-ES', N'Value Added', null;
EXEC InsertOrUpdateResource 'ActionValue_VA_Long', 'de-DE', N'wertschöpfend', null;
EXEC InsertOrUpdateResource 'ActionValue_VA_Long', 'pl-PL', N'Value Added', null;
GO
EXEC InsertOrUpdateResource 'ActionValue_VA_Short', 'fr-FR', N'VA', null;
EXEC InsertOrUpdateResource 'ActionValue_VA_Short', 'en-US', N'VA', null;
EXEC InsertOrUpdateResource 'ActionValue_VA_Short', 'pt-BR', N'VA', null;
EXEC InsertOrUpdateResource 'ActionValue_VA_Short', 'es-ES', N'VA', null;
EXEC InsertOrUpdateResource 'ActionValue_VA_Short', 'de-DE', N'VA', null;
EXEC InsertOrUpdateResource 'ActionValue_VA_Short', 'pl-PL', N'VA', null;
GO
EXEC InsertOrUpdateResource 'Business_AnalyzeService_InitialScenarioLabel', 'fr-FR', N'Scénario Initial', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_InitialScenarioLabel', 'en-US', N'Initial scenario', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_InitialScenarioLabel', 'pt-BR', N'Cenário inicial', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_InitialScenarioLabel', 'es-ES', N'Escenario inicial', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_InitialScenarioLabel', 'de-DE', N'Ausgangs-Szenario', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_InitialScenarioLabel', 'pl-PL', N'Scenariusz początkowy', null;
GO
EXEC InsertOrUpdateResource 'Business_AnalyzeService_TargetScenarioLabel', 'fr-FR', N'Scénario cible', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_TargetScenarioLabel', 'en-US', N'Target scenario', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_TargetScenarioLabel', 'pt-BR', N'Cenário de objetivo', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_TargetScenarioLabel', 'es-ES', N'Escenario objetivo', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_TargetScenarioLabel', 'de-DE', N'Ziel-Szenario', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_TargetScenarioLabel', 'pl-PL', N'Scenariusz docelowy', null;
GO
EXEC InsertOrUpdateResource 'Business_AnalyzeService_ValidationScenarioLabel', 'fr-FR', N'Scénario de validation', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_ValidationScenarioLabel', 'en-US', N'Validation scenario', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_ValidationScenarioLabel', 'pt-BR', N'Cenário de validação', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_ValidationScenarioLabel', 'es-ES', N'Escenario de validación', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_ValidationScenarioLabel', 'de-DE', N'Validierungs-Szenario', null;
EXEC InsertOrUpdateResource 'Business_AnalyzeService_ValidationScenarioLabel', 'pl-PL', N'Scenariusz zatwierdzony', null;
GO
EXEC InsertOrUpdateResource 'Business_EmptySolution', 'fr-FR', N'-', null;
EXEC InsertOrUpdateResource 'Business_EmptySolution', 'en-US', N'-', null;
EXEC InsertOrUpdateResource 'Business_EmptySolution', 'pt-BR', N'-', null;
EXEC InsertOrUpdateResource 'Business_EmptySolution', 'es-ES', N'-', null;
EXEC InsertOrUpdateResource 'Business_EmptySolution', 'de-DE', N'-', null;
EXEC InsertOrUpdateResource 'Business_EmptySolution', 'pl-PL', N'-', null;
GO
EXEC InsertOrUpdateResource 'Business_PrepareService_DefaultCustomFieldLabel1', 'fr-FR', N'Remarques', null;
EXEC InsertOrUpdateResource 'Business_PrepareService_DefaultCustomFieldLabel1', 'en-US', N'Comments', null;
EXEC InsertOrUpdateResource 'Business_PrepareService_DefaultCustomFieldLabel1', 'pt-BR', N'Comentários', null;
EXEC InsertOrUpdateResource 'Business_PrepareService_DefaultCustomFieldLabel1', 'es-ES', N'Notas', null;
EXEC InsertOrUpdateResource 'Business_PrepareService_DefaultCustomFieldLabel1', 'de-DE', N'Hinweise', null;
EXEC InsertOrUpdateResource 'Business_PrepareService_DefaultCustomFieldLabel1', 'pl-PL', N'Komentarze', null;
GO
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromInitialProject', 'fr-FR', N'Ce projet fait suite au scénario "{1}" du projet "{0}"', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromInitialProject', 'en-US', N'This project follows "{1}" scenario of the "{0}" project.', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromInitialProject', 'pt-BR', N'Este projeto segue o cenário "{1}" do projeto "{0}".', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromInitialProject', 'es-ES', N'Este proyecto es consecuente al scenario "{1}" del proyecto "{0}".', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromInitialProject', 'de-DE', N'Dieses Projekt stammt aus dem Szenario "{1}" des Projektes "{0}".', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromInitialProject', 'pl-PL', N'Ten projekt  śledzi "{1}" scenariusz z projektu "{0}"', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
GO
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromRealizedProject', 'fr-FR', N'Ce projet fait suite au scénario de validation du projet "{0}"', N'{0} est le nom de l''ancien projet';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromRealizedProject', 'en-US', N'This project follows the Validation scenario of the "{0}" project.', N'{0} est le nom de l''ancien projet';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromRealizedProject', 'pt-BR', N'Este projeto segue o cenário de validação do projeto "{0}".', N'{0} est le nom de l''ancien projet';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromRealizedProject', 'es-ES', N'Este proyecto es consecuente al scenario de validación del proyecto "{0}".', N'{0} est le nom de l''ancien projet';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromRealizedProject', 'de-DE', N'Dieses Projekt stammt aus dem Validierungs-Szenario des Projektes "{0}".', N'{0} est le nom de l''ancien projet';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromRealizedProject', 'pl-PL', N'Ten projekt śledzi scenariusz zatwierdzony z projektu "{0}"', N'{0} est le nom de l''ancien projet';
GO
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromTargetProject', 'fr-FR', N'Ce projet fait suite au scénario "{1}" du projet "{0}"', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromTargetProject', 'en-US', N'This project follows "{1}" scenario of the "{0}" project.', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromTargetProject', 'pt-BR', N'Este projeto segue o cenário "{1}" do projeto "{0}".', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromTargetProject', 'es-ES', N'Este proyecto es consecuente al scenario "{1}" del proyecto "{0}".', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromTargetProject', 'de-DE', N'Dieses Projekt stammt aus dem Szenario "{1}" des Projektes "{0}".', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetDescriptionFromTargetProject', 'pl-PL', N'Ten projekt  śledzi "{1}" scenariusz z projektu "{0}"', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
GO
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromInitialProject', 'fr-FR', N'{0}/{1} suite', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromInitialProject', 'en-US', N'{0}/{1} continued', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromInitialProject', 'pt-BR', N'{0}/{1} continuar', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromInitialProject', 'es-ES', N'{0}/{1} continuación', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromInitialProject', 'de-DE', N'{0}/{1} weiterführen', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromInitialProject', 'pl-PL', N'{0}/{1} kontynuuj', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
GO
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromRealizedProject', 'fr-FR', N'{0} suite', N'{0} est le nom de l''ancien projet';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromRealizedProject', 'en-US', N'{0} continued', N'{0} est le nom de l''ancien projet';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromRealizedProject', 'pt-BR', N'{0} continuar', N'{0} est le nom de l''ancien projet';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromRealizedProject', 'es-ES', N'{0} continuación', N'{0} est le nom de l''ancien projet';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromRealizedProject', 'de-DE', N'{0} weiterführen', N'{0} est le nom de l''ancien projet';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromRealizedProject', 'pl-PL', N'{0} kontynuuj', N'{0} est le nom de l''ancien projet';
GO
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromTargetProject', 'fr-FR', N'{0}/{1} suite', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromTargetProject', 'en-US', N'{0}/{1} continued', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromTargetProject', 'pt-BR', N'{0}/{1} continuar', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromTargetProject', 'es-ES', N'{0}/{1} continuación', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromTargetProject', 'de-DE', N'{0}/{1} weiterführen', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
EXEC InsertOrUpdateResource 'Business_PrepareService_NewProjetLabelFromTargetProject', 'pl-PL', N'{0}/{1} kontynuuj', N'{0} est le nom de l''ancien projet, {1} est le nom de l''ancien scénario';
GO
EXEC InsertOrUpdateResource 'Common_Analyze', 'fr-FR', N'Analyser', null;
EXEC InsertOrUpdateResource 'Common_Analyze', 'en-US', N'Analyze', null;
EXEC InsertOrUpdateResource 'Common_Analyze', 'pt-BR', N'Analisar', null;
EXEC InsertOrUpdateResource 'Common_Analyze', 'es-ES', N'Analizar', null;
EXEC InsertOrUpdateResource 'Common_Analyze', 'de-DE', N'Analysieren', null;
EXEC InsertOrUpdateResource 'Common_Analyze', 'pl-PL', N'Analiza', null;
GO
EXEC InsertOrUpdateResource 'Common_Analyze_Acquire', 'fr-FR', N'Décomposition', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Acquire', 'en-US', N'Breaking down', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Acquire', 'pt-BR', N'Decomposição', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Acquire', 'es-ES', N'Descomposición', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Acquire', 'de-DE', N'Aufschlüsseln', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Acquire', 'pl-PL', N'Podział ', null;
GO
EXEC InsertOrUpdateResource 'Common_Analyze_Build', 'fr-FR', N'Construction', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Build', 'en-US', N'Creation', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Build', 'pt-BR', N'Criação', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Build', 'es-ES', N'Construcción', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Build', 'de-DE', N'Erstellen', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Build', 'pl-PL', N'Tworzenie', null;
GO
EXEC InsertOrUpdateResource 'Common_Analyze_Compare', 'fr-FR', N'Comparaison', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Compare', 'en-US', N'Comparison', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Compare', 'pt-BR', N'Comparação', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Compare', 'es-ES', N'Comparación', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Compare', 'de-DE', N'Vergleich', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Compare', 'pl-PL', N'Porównanie', null;
GO
EXEC InsertOrUpdateResource 'Common_Analyze_Restore', 'fr-FR', N'Synthèse', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Restore', 'en-US', N'Sum up', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Restore', 'pt-BR', N'Resumo', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Restore', 'es-ES', N'Síntesis', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Restore', 'de-DE', N'Zusammenfassung', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Restore', 'pl-PL', N'Sumowanie', null;
GO
EXEC InsertOrUpdateResource 'Common_Analyze_Simulate', 'fr-FR', N'Optimisation', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Simulate', 'en-US', N'Optimization', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Simulate', 'pt-BR', N'Otimização', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Simulate', 'es-ES', N'Optimización', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Simulate', 'de-DE', N'Optimierung', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Simulate', 'pl-PL', N'Optymalizacja', null;
GO
EXEC InsertOrUpdateResource 'Common_Analyze_Validate', 'fr-FR', N'Valider', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Validate', 'en-US', N'Accept', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Validate', 'pt-BR', N'Aceitar', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Validate', 'es-ES', N'Validar', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Validate', 'de-DE', N'Bestätigen', null;
EXEC InsertOrUpdateResource 'Common_Analyze_Validate', 'pl-PL', N'Zaakceptuj', null;
GO
EXEC InsertOrUpdateResource 'Common_Cancel', 'fr-FR', N'Annuler', null;
EXEC InsertOrUpdateResource 'Common_Cancel', 'en-US', N'Cancel', null;
EXEC InsertOrUpdateResource 'Common_Cancel', 'pt-BR', N'Cancelar', null;
EXEC InsertOrUpdateResource 'Common_Cancel', 'es-ES', N'Cancelar', null;
EXEC InsertOrUpdateResource 'Common_Cancel', 'de-DE', N'Abbrechen', null;
EXEC InsertOrUpdateResource 'Common_Cancel', 'pl-PL', N'Anuluj', null;
GO
EXEC InsertOrUpdateResource 'Common_Capitalize', 'fr-FR', N'Pérenniser', null;
EXEC InsertOrUpdateResource 'Common_Capitalize', 'en-US', N'Sustain', null;
EXEC InsertOrUpdateResource 'Common_Capitalize', 'pt-BR', N'Sustentar', null;
EXEC InsertOrUpdateResource 'Common_Capitalize', 'es-ES', N'Mantener', null;
EXEC InsertOrUpdateResource 'Common_Capitalize', 'de-DE', N'Nachhaltigkeit', null;
EXEC InsertOrUpdateResource 'Common_Capitalize', 'pl-PL', N'Utrzymaj', null;
GO
EXEC InsertOrUpdateResource 'Common_Close', 'fr-FR', N'Fermer', null;
EXEC InsertOrUpdateResource 'Common_Close', 'en-US', N'Close', null;
EXEC InsertOrUpdateResource 'Common_Close', 'pt-BR', N'Fechar', null;
EXEC InsertOrUpdateResource 'Common_Close', 'es-ES', N'Cerrar', null;
EXEC InsertOrUpdateResource 'Common_Close', 'de-DE', N'Schliessen', null;
EXEC InsertOrUpdateResource 'Common_Close', 'pl-PL', N'Zamknij', null;
GO
EXEC InsertOrUpdateResource 'Common_Confirm', 'fr-FR', N'Confirmation', null;
EXEC InsertOrUpdateResource 'Common_Confirm', 'en-US', N'Confirmation', null;
EXEC InsertOrUpdateResource 'Common_Confirm', 'pt-BR', N'Confirmação', null;
EXEC InsertOrUpdateResource 'Common_Confirm', 'es-ES', N'Confirmación', null;
EXEC InsertOrUpdateResource 'Common_Confirm', 'de-DE', N'Bestätigung', null;
EXEC InsertOrUpdateResource 'Common_Confirm', 'pl-PL', N'Potwierdzenie', null;
GO
EXEC InsertOrUpdateResource 'Common_Confirmation', 'fr-FR', N'Confirmation', null;
EXEC InsertOrUpdateResource 'Common_Confirmation', 'en-US', N'Confirmation', null;
EXEC InsertOrUpdateResource 'Common_Confirmation', 'pt-BR', N'Confirmação', null;
EXEC InsertOrUpdateResource 'Common_Confirmation', 'es-ES', N'Confirmación', null;
EXEC InsertOrUpdateResource 'Common_Confirmation', 'de-DE', N'Bestätigung', null;
EXEC InsertOrUpdateResource 'Common_Confirmation', 'pl-PL', N'Potwierdzenie', null;
GO
EXEC InsertOrUpdateResource 'Common_Create', 'fr-FR', N'Créer', null;
EXEC InsertOrUpdateResource 'Common_Create', 'en-US', N'Create', null;
EXEC InsertOrUpdateResource 'Common_Create', 'pt-BR', N'Criar', null;
EXEC InsertOrUpdateResource 'Common_Create', 'es-ES', N'Crear', null;
EXEC InsertOrUpdateResource 'Common_Create', 'de-DE', N'Erstellen', null;
EXEC InsertOrUpdateResource 'Common_Create', 'pl-PL', N'Utwórz', null;
GO
EXEC InsertOrUpdateResource 'Common_Delete', 'fr-FR', N'Supprimer', null;
EXEC InsertOrUpdateResource 'Common_Delete', 'en-US', N'Delete', null;
EXEC InsertOrUpdateResource 'Common_Delete', 'pt-BR', N'Excluir', null;
EXEC InsertOrUpdateResource 'Common_Delete', 'es-ES', N'Eliminar', null;
EXEC InsertOrUpdateResource 'Common_Delete', 'de-DE', N'Entfernen', null;
EXEC InsertOrUpdateResource 'Common_Delete', 'pl-PL', N'Usuń', null;
GO
EXEC InsertOrUpdateResource 'Common_Error', 'fr-FR', N'Erreur', null;
EXEC InsertOrUpdateResource 'Common_Error', 'en-US', N'Error', null;
EXEC InsertOrUpdateResource 'Common_Error', 'pt-BR', N'Erro', null;
EXEC InsertOrUpdateResource 'Common_Error', 'es-ES', N'Error', null;
EXEC InsertOrUpdateResource 'Common_Error', 'de-DE', N'Fehler', null;
EXEC InsertOrUpdateResource 'Common_Error', 'pl-PL', N'Błąd', null;
GO
EXEC InsertOrUpdateResource 'Common_Error_AlreadyOpenMessage', 'fr-FR', N'Impossible de modifier le fichier. Merci de vérifiez que ce fichier n''est pas déjà utilisé.', null;
EXEC InsertOrUpdateResource 'Common_Error_AlreadyOpenMessage', 'en-US', N'Impossible to modify the file. Please check if the file has not been used', null;
EXEC InsertOrUpdateResource 'Common_Error_AlreadyOpenMessage', 'pt-BR', N'Impossível modificar o arquivo. Por favor, verifique se o arquivo não está em uso.', null;
EXEC InsertOrUpdateResource 'Common_Error_AlreadyOpenMessage', 'es-ES', N'Imposible de modificar el archivo. Verificar que el archivo no ha sido utilisado', null;
EXEC InsertOrUpdateResource 'Common_Error_AlreadyOpenMessage', 'de-DE', N'Datei kann nicht abgeändert werden. Bitte prüfen Sie, ob die Datei bereits geöffnet ist.', null;
EXEC InsertOrUpdateResource 'Common_Error_AlreadyOpenMessage', 'pl-PL', N'Niemożliwa modyfikacja pliku. Sprawdź czy plik nie jest używany', null;
GO
EXEC InsertOrUpdateResource 'Common_Error_GenericMessage', 'fr-FR', N'Une erreur non prévue a eu lieu', null;
EXEC InsertOrUpdateResource 'Common_Error_GenericMessage', 'en-US', N'An unexpected error occurred', null;
EXEC InsertOrUpdateResource 'Common_Error_GenericMessage', 'pt-BR', N'Ocorreu um erro não previsto', null;
EXEC InsertOrUpdateResource 'Common_Error_GenericMessage', 'es-ES', N'Un error inesperado ha ocurido', null;
EXEC InsertOrUpdateResource 'Common_Error_GenericMessage', 'de-DE', N'Ein unerwarteter Fehler ist aufgetreten', null;
EXEC InsertOrUpdateResource 'Common_Error_GenericMessage', 'pl-PL', N'Pojawił się niespodziewany błąd', null;
GO
EXEC InsertOrUpdateResource 'Common_Error_OutOfMemoryException', 'fr-FR', N'Une erreur a eu lieu. Pour résoudre cette erreur, veuillez fermer, sauvegarder vos travaux et redémarrer.', null;
EXEC InsertOrUpdateResource 'Common_Error_OutOfMemoryException', 'en-US', N'An error occurred. To resolve this error, please close, save your works and restart.', null;
EXEC InsertOrUpdateResource 'Common_Error_OutOfMemoryException', 'pt-BR', N'An error occurred. To resolve this error, please close, save your works and restart.', null;
EXEC InsertOrUpdateResource 'Common_Error_OutOfMemoryException', 'es-ES', N'An error occurred. To resolve this error, please close, save your works and restart.', null;
EXEC InsertOrUpdateResource 'Common_Error_OutOfMemoryException', 'de-DE', N'An error occurred. To resolve this error, please close, save your works and restart.', null;
EXEC InsertOrUpdateResource 'Common_Error_OutOfMemoryException', 'pl-PL', N'An error occurred. To resolve this error, please close, save your works and restart.', null;
GO
EXEC InsertOrUpdateResource 'Common_InsufficantRights', 'fr-FR', N'Vous n''avez pas assez de droits pour effectuer cette action.', null;
EXEC InsertOrUpdateResource 'Common_InsufficantRights', 'en-US', N'You do not have enough rights for selected action.', null;
EXEC InsertOrUpdateResource 'Common_InsufficantRights', 'pt-BR', N'Você não tem direitos suficientes para a ação selecionada.', null;
EXEC InsertOrUpdateResource 'Common_InsufficantRights', 'es-ES', N'Usted no tiene suficientes derechos para realizar esta acción.', null;
EXEC InsertOrUpdateResource 'Common_InsufficantRights', 'de-DE', N'Sie haben nicht genug Rechte, um diese Aktion auszuführen.', null;
EXEC InsertOrUpdateResource 'Common_InsufficantRights', 'pl-PL', N'Nie masz uprawnień do wykonania tej akcji', null;
GO
EXEC InsertOrUpdateResource 'Common_Message_SureToDelete', 'fr-FR', N'Êtes vous sûr(e) de vouloir supprimer cet élément ?', null;
EXEC InsertOrUpdateResource 'Common_Message_SureToDelete', 'en-US', N'Are you sure you want to delete this item?', null;
EXEC InsertOrUpdateResource 'Common_Message_SureToDelete', 'pt-BR', N'Tem certeza de que deseja excluir este item?', null;
EXEC InsertOrUpdateResource 'Common_Message_SureToDelete', 'es-ES', N'¿Está seguro de que desea eliminar este elemento?', null;
EXEC InsertOrUpdateResource 'Common_Message_SureToDelete', 'de-DE', N'Sind Sie sicher, dass Sie dieses Element wirklich löschen wollen?', null;
EXEC InsertOrUpdateResource 'Common_Message_SureToDelete', 'pl-PL', N'Czy na pewno usunąć?', null;
GO
EXEC InsertOrUpdateResource 'Common_Message_WantToSave', 'fr-FR', N'Voulez-vous enregistrer vos modifications sur cet écran ?
Toute modification non sauvegardée sera perdue', null;
EXEC InsertOrUpdateResource 'Common_Message_WantToSave', 'en-US', N'Do you want to save your changes?
Any changes will be lost', null;
EXEC InsertOrUpdateResource 'Common_Message_WantToSave', 'pt-BR', N'Gostaria de salvar suas modificações?
Qualquer alteração será perdida ', null;
EXEC InsertOrUpdateResource 'Common_Message_WantToSave', 'es-ES', N'Quiere guardar sus cambios? Todo cambio sera perdido', null;
EXEC InsertOrUpdateResource 'Common_Message_WantToSave', 'de-DE', N'Möchten Sie die Änderungen abspeichern ?
Sonst sind alle Änderungen verloren.', null;
EXEC InsertOrUpdateResource 'Common_Message_WantToSave', 'pl-PL', N'Czy chcesz zachowac zmiany?
Wszystkie zmiany będą utracone', null;
GO
EXEC InsertOrUpdateResource 'Common_Modify', 'fr-FR', N'Modifier', null;
EXEC InsertOrUpdateResource 'Common_Modify', 'en-US', N'Change', null;
EXEC InsertOrUpdateResource 'Common_Modify', 'pt-BR', N'Mudança', null;
EXEC InsertOrUpdateResource 'Common_Modify', 'es-ES', N'Cambiar', null;
EXEC InsertOrUpdateResource 'Common_Modify', 'de-DE', N'Ändern', null;
EXEC InsertOrUpdateResource 'Common_Modify', 'pl-PL', N'Zmień', null;
GO
EXEC InsertOrUpdateResource 'Common_No', 'fr-FR', N'Non', null;
EXEC InsertOrUpdateResource 'Common_No', 'en-US', N'No', null;
EXEC InsertOrUpdateResource 'Common_No', 'pt-BR', N'Não', null;
EXEC InsertOrUpdateResource 'Common_No', 'es-ES', N'No', null;
EXEC InsertOrUpdateResource 'Common_No', 'de-DE', N'Nein', null;
EXEC InsertOrUpdateResource 'Common_No', 'pl-PL', N'Nie', null;
GO
EXEC InsertOrUpdateResource 'Common_OK', 'fr-FR', N'OK', null;
EXEC InsertOrUpdateResource 'Common_OK', 'en-US', N'OK', null;
EXEC InsertOrUpdateResource 'Common_OK', 'pt-BR', N'OK', null;
EXEC InsertOrUpdateResource 'Common_OK', 'es-ES', N'OK', null;
EXEC InsertOrUpdateResource 'Common_OK', 'de-DE', N'OK', null;
EXEC InsertOrUpdateResource 'Common_OK', 'pl-PL', N'OK', null;
GO
EXEC InsertOrUpdateResource 'Common_Open', 'fr-FR', N'Ouvrir', null;
EXEC InsertOrUpdateResource 'Common_Open', 'en-US', N'Open', null;
EXEC InsertOrUpdateResource 'Common_Open', 'pt-BR', N'Aberto', null;
EXEC InsertOrUpdateResource 'Common_Open', 'es-ES', N'Abierto', null;
EXEC InsertOrUpdateResource 'Common_Open', 'de-DE', N'Öffnen', null;
EXEC InsertOrUpdateResource 'Common_Open', 'pl-PL', N'Otwórz', null;
GO
EXEC InsertOrUpdateResource 'Common_OverrideHighlight_OriginalValueFrom', 'fr-FR', N'Valeur d''origine dans', null;
EXEC InsertOrUpdateResource 'Common_OverrideHighlight_OriginalValueFrom', 'en-US', N'Original value in', null;
EXEC InsertOrUpdateResource 'Common_OverrideHighlight_OriginalValueFrom', 'pt-BR', N'Valor original  in', null;
EXEC InsertOrUpdateResource 'Common_OverrideHighlight_OriginalValueFrom', 'es-ES', N'Valor original en', null;
EXEC InsertOrUpdateResource 'Common_OverrideHighlight_OriginalValueFrom', 'de-DE', N'Ausgangswert in', null;
EXEC InsertOrUpdateResource 'Common_OverrideHighlight_OriginalValueFrom', 'pl-PL', N'Orginalna wartość w', null;
GO
EXEC InsertOrUpdateResource 'Common_Prepare', 'fr-FR', N'Préparer', null;
EXEC InsertOrUpdateResource 'Common_Prepare', 'en-US', N'Prepare', null;
EXEC InsertOrUpdateResource 'Common_Prepare', 'pt-BR', N'Preparar', null;
EXEC InsertOrUpdateResource 'Common_Prepare', 'es-ES', N'Preparar', null;
EXEC InsertOrUpdateResource 'Common_Prepare', 'de-DE', N'Vorbereiten', null;
EXEC InsertOrUpdateResource 'Common_Prepare', 'pl-PL', N'Przygotuj', null;
GO
EXEC InsertOrUpdateResource 'Common_Prepare_Members', 'fr-FR', N'Membres', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Members', 'en-US', N'Members', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Members', 'pt-BR', N'Membros', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Members', 'es-ES', N'Miembros', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Members', 'de-DE', N'Mitglied', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Members', 'pl-PL', N'Członkowie', null;
GO
EXEC InsertOrUpdateResource 'Common_Prepare_Project', 'fr-FR', N'Projet', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Project', 'en-US', N'Project', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Project', 'pt-BR', N'Projeto', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Project', 'es-ES', N'Proyecto', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Project', 'de-DE', N'Projekt', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Project', 'pl-PL', N'Projekt', null;
GO
EXEC InsertOrUpdateResource 'Common_Prepare_Referentials', 'fr-FR', N'Référentiels', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Referentials', 'en-US', N'Referentials', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Referentials', 'pt-BR', N'Referenciais', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Referentials', 'es-ES', N'Referencias', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Referentials', 'de-DE', N'Referenzen', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Referentials', 'pl-PL', N'Odniesienia', null;
GO
EXEC InsertOrUpdateResource 'Common_Prepare_Resources', 'fr-FR', N'Ressource', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Resources', 'en-US', N'Resource', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Resources', 'pt-BR', N'Recurso', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Resources', 'es-ES', N'Recurso', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Resources', 'de-DE', N'Ressource', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Resources', 'pl-PL', N'Zasoby', null;
GO
EXEC InsertOrUpdateResource 'Common_Prepare_Scenarios', 'fr-FR', N'Scénarios', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Scenarios', 'en-US', N'Scenarios', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Scenarios', 'pt-BR', N'Cenários', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Scenarios', 'es-ES', N'Escenarios', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Scenarios', 'de-DE', N'Szenarien', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Scenarios', 'pl-PL', N'Scenariusze', null;
GO
EXEC InsertOrUpdateResource 'Common_Prepare_Videos', 'fr-FR', N'Vidéos', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Videos', 'en-US', N'Videos', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Videos', 'pt-BR', N'Vídeos', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Videos', 'es-ES', N'Videos', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Videos', 'de-DE', N'Videos', null;
EXEC InsertOrUpdateResource 'Common_Prepare_Videos', 'pl-PL', N'Filmy', null;
GO
EXEC InsertOrUpdateResource 'Common_ProductName', 'fr-FR', N'KL2®', null;
EXEC InsertOrUpdateResource 'Common_ProductName', 'en-US', N'KL2®', null;
EXEC InsertOrUpdateResource 'Common_ProductName', 'pt-BR', N'KL2®', null;
EXEC InsertOrUpdateResource 'Common_ProductName', 'es-ES', N'KL2®', null;
EXEC InsertOrUpdateResource 'Common_ProductName', 'de-DE', N'KL2®', null;
EXEC InsertOrUpdateResource 'Common_ProductName', 'pl-PL', N'KL2®', null;
GO
EXEC InsertOrUpdateResource 'Common_Project', 'fr-FR', N'Projet', null;
EXEC InsertOrUpdateResource 'Common_Project', 'en-US', N'Project', null;
EXEC InsertOrUpdateResource 'Common_Project', 'pt-BR', N'Projeto', null;
EXEC InsertOrUpdateResource 'Common_Project', 'es-ES', N'Proyecto', null;
EXEC InsertOrUpdateResource 'Common_Project', 'de-DE', N'Projekt', null;
EXEC InsertOrUpdateResource 'Common_Project', 'pl-PL', N'Projekt', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_Category', 'fr-FR', N'Catégorie', null;
EXEC InsertOrUpdateResource 'Common_Referential_Category', 'en-US', N'Category', null;
EXEC InsertOrUpdateResource 'Common_Referential_Category', 'pt-BR', N'Categoria', null;
EXEC InsertOrUpdateResource 'Common_Referential_Category', 'es-ES', N'Categoría', null;
EXEC InsertOrUpdateResource 'Common_Referential_Category', 'de-DE', N'Kategorie', null;
EXEC InsertOrUpdateResource 'Common_Referential_Category', 'pl-PL', N'Kategorie', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_Skill', 'fr-FR', N'Compétence', null;
EXEC InsertOrUpdateResource 'Common_Referential_Skill', 'en-US', N'Skill', null;
EXEC InsertOrUpdateResource 'Common_Referential_Skill', 'pt-BR', N'Skill', null;
EXEC InsertOrUpdateResource 'Common_Referential_Skill', 'es-ES', N'Skill', null;
EXEC InsertOrUpdateResource 'Common_Referential_Skill', 'de-DE', N'Skill', null;
EXEC InsertOrUpdateResource 'Common_Referential_Skill', 'pl-PL', N'Skill', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_Consumable', 'fr-FR', N'Consommable', null;
EXEC InsertOrUpdateResource 'Common_Referential_Consumable', 'en-US', N'Consumable', null;
EXEC InsertOrUpdateResource 'Common_Referential_Consumable', 'pt-BR', N'Consumíveis', null;
EXEC InsertOrUpdateResource 'Common_Referential_Consumable', 'es-ES', N'Consumible', null;
EXEC InsertOrUpdateResource 'Common_Referential_Consumable', 'de-DE', N'Verbrauchsmaterial', null;
EXEC InsertOrUpdateResource 'Common_Referential_Consumable', 'pl-PL', N'Eksploatacyjne', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_Document', 'fr-FR', N'Document', null;
EXEC InsertOrUpdateResource 'Common_Referential_Document', 'en-US', N'Document', null;
EXEC InsertOrUpdateResource 'Common_Referential_Document', 'pt-BR', N'Documento', null;
EXEC InsertOrUpdateResource 'Common_Referential_Document', 'es-ES', N'Documento', null;
EXEC InsertOrUpdateResource 'Common_Referential_Document', 'de-DE', N'Dokument', null;
EXEC InsertOrUpdateResource 'Common_Referential_Document', 'pl-PL', N'Dokument', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_Equipment', 'fr-FR', N'Equipement', null;
EXEC InsertOrUpdateResource 'Common_Referential_Equipment', 'en-US', N'Equipment', null;
EXEC InsertOrUpdateResource 'Common_Referential_Equipment', 'pt-BR', N'Equipamento', null;
EXEC InsertOrUpdateResource 'Common_Referential_Equipment', 'es-ES', N'Equipo', null;
EXEC InsertOrUpdateResource 'Common_Referential_Equipment', 'de-DE', N'Ausrüstung', null;
EXEC InsertOrUpdateResource 'Common_Referential_Equipment', 'pl-PL', N'Sprzęt', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_Operator', 'fr-FR', N'Opérateur', null;
EXEC InsertOrUpdateResource 'Common_Referential_Operator', 'en-US', N'Operator', null;
EXEC InsertOrUpdateResource 'Common_Referential_Operator', 'pt-BR', N'Operador', null;
EXEC InsertOrUpdateResource 'Common_Referential_Operator', 'es-ES', N'Operador', null;
EXEC InsertOrUpdateResource 'Common_Referential_Operator', 'de-DE', N'Operator', null;
EXEC InsertOrUpdateResource 'Common_Referential_Operator', 'pl-PL', N'Operator', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_Place', 'fr-FR', N'Lieu', null;
EXEC InsertOrUpdateResource 'Common_Referential_Place', 'en-US', N'Location', null;
EXEC InsertOrUpdateResource 'Common_Referential_Place', 'pt-BR', N'Localização', null;
EXEC InsertOrUpdateResource 'Common_Referential_Place', 'es-ES', N'Lugar', null;
EXEC InsertOrUpdateResource 'Common_Referential_Place', 'de-DE', N'Ort', null;
EXEC InsertOrUpdateResource 'Common_Referential_Place', 'pl-PL', N'Lokalizacja', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_R1', 'fr-FR', N'Réf. 1', null;
EXEC InsertOrUpdateResource 'Common_Referential_R1', 'en-US', N'Ref.1', null;
EXEC InsertOrUpdateResource 'Common_Referential_R1', 'pt-BR', N'Ref.1', null;
EXEC InsertOrUpdateResource 'Common_Referential_R1', 'es-ES', N'Ref.1', null;
EXEC InsertOrUpdateResource 'Common_Referential_R1', 'de-DE', N'Ab.1', null;
EXEC InsertOrUpdateResource 'Common_Referential_R1', 'pl-PL', N'Odnośnik 1', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_R2', 'fr-FR', N'Réf. 2', null;
EXEC InsertOrUpdateResource 'Common_Referential_R2', 'en-US', N'Ref.2', null;
EXEC InsertOrUpdateResource 'Common_Referential_R2', 'pt-BR', N'Ref.2', null;
EXEC InsertOrUpdateResource 'Common_Referential_R2', 'es-ES', N'Ref.2', null;
EXEC InsertOrUpdateResource 'Common_Referential_R2', 'de-DE', N'Ab.2', null;
EXEC InsertOrUpdateResource 'Common_Referential_R2', 'pl-PL', N'Odnośnik 2', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_R3', 'fr-FR', N'Réf. 3', null;
EXEC InsertOrUpdateResource 'Common_Referential_R3', 'en-US', N'Ref.3', null;
EXEC InsertOrUpdateResource 'Common_Referential_R3', 'pt-BR', N'Ref.3', null;
EXEC InsertOrUpdateResource 'Common_Referential_R3', 'es-ES', N'Ref.3', null;
EXEC InsertOrUpdateResource 'Common_Referential_R3', 'de-DE', N'Ab.3', null;
EXEC InsertOrUpdateResource 'Common_Referential_R3', 'pl-PL', N'Odnośnik 3', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_R4', 'fr-FR', N'Réf. 4', null;
EXEC InsertOrUpdateResource 'Common_Referential_R4', 'en-US', N'Ref.4', null;
EXEC InsertOrUpdateResource 'Common_Referential_R4', 'pt-BR', N'Ref.4', null;
EXEC InsertOrUpdateResource 'Common_Referential_R4', 'es-ES', N'Ref.4', null;
EXEC InsertOrUpdateResource 'Common_Referential_R4', 'de-DE', N'Ab.4', null;
EXEC InsertOrUpdateResource 'Common_Referential_R4', 'pl-PL', N'Odnośnik 4', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_R5', 'fr-FR', N'Réf. 5', null;
EXEC InsertOrUpdateResource 'Common_Referential_R5', 'en-US', N'Ref.5', null;
EXEC InsertOrUpdateResource 'Common_Referential_R5', 'pt-BR', N'Ref.5', null;
EXEC InsertOrUpdateResource 'Common_Referential_R5', 'es-ES', N'Ref.5', null;
EXEC InsertOrUpdateResource 'Common_Referential_R5', 'de-DE', N'Ab.5', null;
EXEC InsertOrUpdateResource 'Common_Referential_R5', 'pl-PL', N'Odnośnik 5', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_R6', 'fr-FR', N'Réf. 6', null;
EXEC InsertOrUpdateResource 'Common_Referential_R6', 'en-US', N'Ref.6', null;
EXEC InsertOrUpdateResource 'Common_Referential_R6', 'pt-BR', N'Ref.6', null;
EXEC InsertOrUpdateResource 'Common_Referential_R6', 'es-ES', N'Ref.6', null;
EXEC InsertOrUpdateResource 'Common_Referential_R6', 'de-DE', N'Ab.6', null;
EXEC InsertOrUpdateResource 'Common_Referential_R6', 'pl-PL', N'Odnośnik 6', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_R7', 'fr-FR', N'Réf. 7', null;
EXEC InsertOrUpdateResource 'Common_Referential_R7', 'en-US', N'Ref.7', null;
EXEC InsertOrUpdateResource 'Common_Referential_R7', 'pt-BR', N'Ref.7', null;
EXEC InsertOrUpdateResource 'Common_Referential_R7', 'es-ES', N'Ref.7', null;
EXEC InsertOrUpdateResource 'Common_Referential_R7', 'de-DE', N'Ab.7', null;
EXEC InsertOrUpdateResource 'Common_Referential_R7', 'pl-PL', N'Odnośnik 7', null;
GO
EXEC InsertOrUpdateResource 'Common_Referential_Tool', 'fr-FR', N'Outil', null;
EXEC InsertOrUpdateResource 'Common_Referential_Tool', 'en-US', N'Tool', null;
EXEC InsertOrUpdateResource 'Common_Referential_Tool', 'pt-BR', N'Ferramenta', null;
EXEC InsertOrUpdateResource 'Common_Referential_Tool', 'es-ES', N'Herramienta', null;
EXEC InsertOrUpdateResource 'Common_Referential_Tool', 'de-DE', N'Werkzeug', null;
EXEC InsertOrUpdateResource 'Common_Referential_Tool', 'pl-PL', N'Narzędzie', null;
GO
EXEC InsertOrUpdateResource 'Common_Referentials_QuantityDescription', 'fr-FR', N'{0} x {1}', N'{0} est la quantité, {1} est le référentiel';
EXEC InsertOrUpdateResource 'Common_Referentials_QuantityDescription', 'en-US', N'{0} x {1}', N'{0} est la quantité, {1} est le référentiel';
EXEC InsertOrUpdateResource 'Common_Referentials_QuantityDescription', 'pt-BR', N'{0} x {1}', N'{0} est la quantité, {1} est le référentiel';
EXEC InsertOrUpdateResource 'Common_Referentials_QuantityDescription', 'es-ES', N'{0} x {1}', N'{0} est la quantité, {1} est le référentiel';
EXEC InsertOrUpdateResource 'Common_Referentials_QuantityDescription', 'de-DE', N'{0} x {1}', N'{0} est la quantité, {1} est le référentiel';
EXEC InsertOrUpdateResource 'Common_Referentials_QuantityDescription', 'pl-PL', N'{0} x {1}', N'{0} est la quantité, {1} est le référentiel';
GO
EXEC InsertOrUpdateResource 'Common_Save', 'fr-FR', N'Enregistrer', null;
EXEC InsertOrUpdateResource 'Common_Save', 'en-US', N'Save', null;
EXEC InsertOrUpdateResource 'Common_Save', 'pt-BR', N'Salvar', null;
EXEC InsertOrUpdateResource 'Common_Save', 'es-ES', N'Grabar', null;
EXEC InsertOrUpdateResource 'Common_Save', 'de-DE', N'Speichern', null;
EXEC InsertOrUpdateResource 'Common_Save', 'pl-PL', N'Zapisz', null;
GO
EXEC InsertOrUpdateResource 'Common_Track', 'fr-FR', N'Suivre', null;
EXEC InsertOrUpdateResource 'Common_Track', 'en-US', N'Follow', null;
EXEC InsertOrUpdateResource 'Common_Track', 'pt-BR', N'Seguir', null;
EXEC InsertOrUpdateResource 'Common_Track', 'es-ES', N'Seguir', null;
EXEC InsertOrUpdateResource 'Common_Track', 'de-DE', N'Folgen', null;
EXEC InsertOrUpdateResource 'Common_Track', 'pl-PL', N'Śledź', null;
GO
EXEC InsertOrUpdateResource 'Common_UserFullName', 'fr-FR', N'{1} {0}', N'{0} est le prénom, {1} le nom';
EXEC InsertOrUpdateResource 'Common_UserFullName', 'en-US', N'{1} {0}', N'{0} est le prénom, {1} le nom';
EXEC InsertOrUpdateResource 'Common_UserFullName', 'pt-BR', N'{1} {0}', N'{0} est le prénom, {1} le nom';
EXEC InsertOrUpdateResource 'Common_UserFullName', 'es-ES', N'{1} {0}', N'{0} est le prénom, {1} le nom';
EXEC InsertOrUpdateResource 'Common_UserFullName', 'de-DE', N'{1} {0}', N'{0} est le prénom, {1} le nom';
EXEC InsertOrUpdateResource 'Common_UserFullName', 'pl-PL', N'{1} {0}', N'{0} est le prénom, {1} le nom';
GO
EXEC InsertOrUpdateResource 'Common_Validate', 'fr-FR', N'Valider', null;
EXEC InsertOrUpdateResource 'Common_Validate', 'en-US', N'Validate', null;
EXEC InsertOrUpdateResource 'Common_Validate', 'pt-BR', N'Validar', null;
EXEC InsertOrUpdateResource 'Common_Validate', 'es-ES', N'Validar', null;
EXEC InsertOrUpdateResource 'Common_Validate', 'de-DE', N'Validieren', null;
EXEC InsertOrUpdateResource 'Common_Validate', 'pl-PL', N'Zatwierdź', null;
GO
EXEC InsertOrUpdateResource 'Common_Validate_Acquire', 'fr-FR', N'Décomposition', null;
EXEC InsertOrUpdateResource 'Common_Validate_Acquire', 'en-US', N'Breaking down', null;
EXEC InsertOrUpdateResource 'Common_Validate_Acquire', 'pt-BR', N'Decomposição', null;
EXEC InsertOrUpdateResource 'Common_Validate_Acquire', 'es-ES', N'Descomposición', null;
EXEC InsertOrUpdateResource 'Common_Validate_Acquire', 'de-DE', N'Aufschlüsseln', null;
EXEC InsertOrUpdateResource 'Common_Validate_Acquire', 'pl-PL', N'Podziel', null;
GO
EXEC InsertOrUpdateResource 'Common_Validate_Build', 'fr-FR', N'Construction', null;
EXEC InsertOrUpdateResource 'Common_Validate_Build', 'en-US', N'Creation', null;
EXEC InsertOrUpdateResource 'Common_Validate_Build', 'pt-BR', N'Criação', null;
EXEC InsertOrUpdateResource 'Common_Validate_Build', 'es-ES', N'Construcción', null;
EXEC InsertOrUpdateResource 'Common_Validate_Build', 'de-DE', N'Erzeugung', null;
EXEC InsertOrUpdateResource 'Common_Validate_Build', 'pl-PL', N'Tworzenie', null;
GO
EXEC InsertOrUpdateResource 'Common_Validate_Compare', 'fr-FR', N'Comparaison', null;
EXEC InsertOrUpdateResource 'Common_Validate_Compare', 'en-US', N'Comparison', null;
EXEC InsertOrUpdateResource 'Common_Validate_Compare', 'pt-BR', N'Comparação', null;
EXEC InsertOrUpdateResource 'Common_Validate_Compare', 'es-ES', N'Comparación', null;
EXEC InsertOrUpdateResource 'Common_Validate_Compare', 'de-DE', N'Vergleich', null;
EXEC InsertOrUpdateResource 'Common_Validate_Compare', 'pl-PL', N'Porównanie', null;
GO
EXEC InsertOrUpdateResource 'Common_Validate_Restore', 'fr-FR', N'Synthèse', null;
EXEC InsertOrUpdateResource 'Common_Validate_Restore', 'en-US', N'Sum up', null;
EXEC InsertOrUpdateResource 'Common_Validate_Restore', 'pt-BR', N'Resumo', null;
EXEC InsertOrUpdateResource 'Common_Validate_Restore', 'es-ES', N'Síntesis', null;
EXEC InsertOrUpdateResource 'Common_Validate_Restore', 'de-DE', N'Zussammenfassung', null;
EXEC InsertOrUpdateResource 'Common_Validate_Restore', 'pl-PL', N'Sumowanie', null;
GO
EXEC InsertOrUpdateResource 'Common_Validate_Simulate', 'fr-FR', N'Construction', null;
EXEC InsertOrUpdateResource 'Common_Validate_Simulate', 'en-US', N'Creation', null;
EXEC InsertOrUpdateResource 'Common_Validate_Simulate', 'pt-BR', N'Criação', null;
EXEC InsertOrUpdateResource 'Common_Validate_Simulate', 'es-ES', N'Construcción', null;
EXEC InsertOrUpdateResource 'Common_Validate_Simulate', 'de-DE', N'Erstellung', null;
EXEC InsertOrUpdateResource 'Common_Validate_Simulate', 'pl-PL', N'Tworzenie', null;
GO
EXEC InsertOrUpdateResource 'Common_Next', 'fr-FR', N'Suivant', null;
EXEC InsertOrUpdateResource 'Common_Next', 'en-US', N'Next', null;
EXEC InsertOrUpdateResource 'Common_Next', 'pt-BR', N'Next', null;
EXEC InsertOrUpdateResource 'Common_Next', 'es-ES', N'Next', null;
EXEC InsertOrUpdateResource 'Common_Next', 'de-DE', N'Next', null;
EXEC InsertOrUpdateResource 'Common_Next', 'pl-PL', N'Next', null;
GO
EXEC InsertOrUpdateResource 'Common_Previous', 'fr-FR', N'Précédent', null;
EXEC InsertOrUpdateResource 'Common_Previous', 'en-US', N'Previous', null;
EXEC InsertOrUpdateResource 'Common_Previous', 'pt-BR', N'Previous', null;
EXEC InsertOrUpdateResource 'Common_Previous', 'es-ES', N'Previous', null;
EXEC InsertOrUpdateResource 'Common_Previous', 'de-DE', N'Previous', null;
EXEC InsertOrUpdateResource 'Common_Previous', 'pl-PL', N'Previous', null;
GO
EXEC InsertOrUpdateResource 'Common_Finish', 'fr-FR', N'Terminer', null;
EXEC InsertOrUpdateResource 'Common_Finish', 'en-US', N'Finish', null;
EXEC InsertOrUpdateResource 'Common_Finish', 'pt-BR', N'Finish', null;
EXEC InsertOrUpdateResource 'Common_Finish', 'es-ES', N'Finish', null;
EXEC InsertOrUpdateResource 'Common_Finish', 'de-DE', N'Finish', null;
EXEC InsertOrUpdateResource 'Common_Finish', 'pl-PL', N'Finish', null;
GO
EXEC InsertOrUpdateResource 'Common_Publish', 'fr-FR', N'Publier', null;
EXEC InsertOrUpdateResource 'Common_Publish', 'en-US', N'Publish', null;
EXEC InsertOrUpdateResource 'Common_Publish', 'pt-BR', N'Publish', null;
EXEC InsertOrUpdateResource 'Common_Publish', 'es-ES', N'Publish', null;
EXEC InsertOrUpdateResource 'Common_Publish', 'de-DE', N'Publish', null;
EXEC InsertOrUpdateResource 'Common_Publish', 'pl-PL', N'Publish', null;
GO
EXEC InsertOrUpdateResource 'Common_Publish_Summary', 'fr-FR', N'Récapitulatif', null;
EXEC InsertOrUpdateResource 'Common_Publish_Summary', 'en-US', N'Summary', null;
EXEC InsertOrUpdateResource 'Common_Publish_Summary', 'pt-BR', N'Summary', null;
EXEC InsertOrUpdateResource 'Common_Publish_Summary', 'es-ES', N'Summary', null;
EXEC InsertOrUpdateResource 'Common_Publish_Summary', 'de-DE', N'Summary', null;
EXEC InsertOrUpdateResource 'Common_Publish_Summary', 'pl-PL', N'Summary', null;
GO
EXEC InsertOrUpdateResource 'Common_Publish_Scenario', 'fr-FR', N'Scénarios', null;
EXEC InsertOrUpdateResource 'Common_Publish_Scenario', 'en-US', N'Scenarios', null;
EXEC InsertOrUpdateResource 'Common_Publish_Scenario', 'pt-BR', N'Scenarios', null;
EXEC InsertOrUpdateResource 'Common_Publish_Scenario', 'es-ES', N'Scenarios', null;
EXEC InsertOrUpdateResource 'Common_Publish_Scenario', 'de-DE', N'Scenarios', null;
EXEC InsertOrUpdateResource 'Common_Publish_Scenario', 'pl-PL', N'Scenarios', null;
GO
EXEC InsertOrUpdateResource 'Common_Publish_Format', 'fr-FR', N'Mise en forme', null;
EXEC InsertOrUpdateResource 'Common_Publish_Format', 'en-US', N'Format', null;
EXEC InsertOrUpdateResource 'Common_Publish_Format', 'pt-BR', N'Format', null;
EXEC InsertOrUpdateResource 'Common_Publish_Format', 'es-ES', N'Format', null;
EXEC InsertOrUpdateResource 'Common_Publish_Format', 'de-DE', N'Format', null;
EXEC InsertOrUpdateResource 'Common_Publish_Format', 'pl-PL', N'Format', null;
GO
EXEC InsertOrUpdateResource 'Common_Publish_Videos', 'fr-FR', N'Vidéos', null;
EXEC InsertOrUpdateResource 'Common_Publish_Videos', 'en-US', N'Videos', null;
EXEC InsertOrUpdateResource 'Common_Publish_Videos', 'pt-BR', N'Videos', null;
EXEC InsertOrUpdateResource 'Common_Publish_Videos', 'es-ES', N'Videos', null;
EXEC InsertOrUpdateResource 'Common_Publish_Videos', 'de-DE', N'Videos', null;
EXEC InsertOrUpdateResource 'Common_Publish_Videos', 'pl-PL', N'Videos', null;
GO
EXEC InsertOrUpdateResource 'Common_PublishMode_Training', 'fr-FR', N'Formation', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Training', 'en-US', N'Training', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Training', 'pt-BR', N'Training', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Training', 'es-ES', N'Training', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Training', 'de-DE', N'Training', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Training', 'pl-PL', N'Training', null;
GO
EXEC InsertOrUpdateResource 'Common_PublishMode_Audit', 'fr-FR', N'Audit', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Audit', 'en-US', N'Audit', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Audit', 'pt-BR', N'Audit', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Audit', 'es-ES', N'Audit', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Audit', 'de-DE', N'Audit', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Audit', 'pl-PL', N'Audit', null;
GO
EXEC InsertOrUpdateResource 'Common_PublishMode_Inspection', 'fr-FR', N'Inspection', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Inspection', 'en-US', N'Inspection', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Inspection', 'pt-BR', N'Inspection', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Inspection', 'es-ES', N'Inspection', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Inspection', 'de-DE', N'Inspection', null;
EXEC InsertOrUpdateResource 'Common_PublishMode_Inspection', 'pl-PL', N'Inspection', null;
GO
EXEC InsertOrUpdateResource 'Common_ResourceView_Intern', 'fr-FR', N'Interne', null;
EXEC InsertOrUpdateResource 'Common_ResourceView_Intern', 'en-US', N'Internal', null;
EXEC InsertOrUpdateResource 'Common_ResourceView_Intern', 'pt-BR', N'Internal', null;
EXEC InsertOrUpdateResource 'Common_ResourceView_Intern', 'es-ES', N'Internal', null;
EXEC InsertOrUpdateResource 'Common_ResourceView_Intern', 'de-DE', N'Internal', null;
EXEC InsertOrUpdateResource 'Common_ResourceView_Intern', 'pl-PL', N'Internal', null;
GO
EXEC InsertOrUpdateResource 'Common_ResourceView_Extern', 'fr-FR', N'Externe', null;
EXEC InsertOrUpdateResource 'Common_ResourceView_Extern', 'en-US', N'External', null;
EXEC InsertOrUpdateResource 'Common_ResourceView_Extern', 'pt-BR', N'External', null;
EXEC InsertOrUpdateResource 'Common_ResourceView_Extern', 'es-ES', N'External', null;
EXEC InsertOrUpdateResource 'Common_ResourceView_Extern', 'de-DE', N'External', null;
EXEC InsertOrUpdateResource 'Common_ResourceView_Extern', 'pl-PL', N'External', null;
GO
EXEC InsertOrUpdateResource 'Common_ValidationErrors', 'fr-FR', N'Erreurs de validation', null;
EXEC InsertOrUpdateResource 'Common_ValidationErrors', 'en-US', N'Validation errors', null;
EXEC InsertOrUpdateResource 'Common_ValidationErrors', 'pt-BR', N'Erros de validação', null;
EXEC InsertOrUpdateResource 'Common_ValidationErrors', 'es-ES', N'Errores de validación', null;
EXEC InsertOrUpdateResource 'Common_ValidationErrors', 'de-DE', N'Validierungsfehler', null;
EXEC InsertOrUpdateResource 'Common_ValidationErrors', 'pl-PL', N'Zatwierdź błędy', null;
GO
EXEC InsertOrUpdateResource 'Common_Workshop', 'fr-FR', N'Périmètre', null;
EXEC InsertOrUpdateResource 'Common_Workshop', 'en-US', N'Scope', null;
EXEC InsertOrUpdateResource 'Common_Workshop', 'pt-BR', N'Escopo', null;
EXEC InsertOrUpdateResource 'Common_Workshop', 'es-ES', N'Perímetro', null;
EXEC InsertOrUpdateResource 'Common_Workshop', 'de-DE', N'Geltungsbereich', null;
EXEC InsertOrUpdateResource 'Common_Workshop', 'pl-PL', N'Zakres', null;
GO
EXEC InsertOrUpdateResource 'Common_FurtherInformation', 'fr-FR', N'Informations complémentaires', null;
EXEC InsertOrUpdateResource 'Common_FurtherInformation', 'en-US', N'Further information', null;
EXEC InsertOrUpdateResource 'Common_FurtherInformation', 'pt-BR', N'Further information', null;
EXEC InsertOrUpdateResource 'Common_FurtherInformation', 'es-ES', N'Further information', null;
EXEC InsertOrUpdateResource 'Common_FurtherInformation', 'de-DE', N'Further information', null;
EXEC InsertOrUpdateResource 'Common_FurtherInformation', 'pl-PL', N'Further information', null;
GO
EXEC InsertOrUpdateResource 'Common_Yes', 'fr-FR', N'Oui', null;
EXEC InsertOrUpdateResource 'Common_Yes', 'en-US', N'Yes', null;
EXEC InsertOrUpdateResource 'Common_Yes', 'pt-BR', N'Sim', null;
EXEC InsertOrUpdateResource 'Common_Yes', 'es-ES', N'Sí', null;
EXEC InsertOrUpdateResource 'Common_Yes', 'de-DE', N'Ja', null;
EXEC InsertOrUpdateResource 'Common_Yes', 'pl-PL', N'Tak', null;
GO
EXEC InsertOrUpdateResource 'Common_NotAFileOfFileType', 'fr-FR', N'Ce fichier n''est pas de type {0}.', null;
EXEC InsertOrUpdateResource 'Common_NotAFileOfFileType', 'en-US', N'This file is not of {0} type.', null;
EXEC InsertOrUpdateResource 'Common_NotAFileOfFileType', 'pt-BR', N'This file is not of {0} type.', null;
EXEC InsertOrUpdateResource 'Common_NotAFileOfFileType', 'es-ES', N'This file is not of {0} type.', null;
EXEC InsertOrUpdateResource 'Common_NotAFileOfFileType', 'de-DE', N'This file is not of {0} type.', null;
EXEC InsertOrUpdateResource 'Common_NotAFileOfFileType', 'pl-PL', N'This file is not of {0} type.', null;
GO
EXEC InsertOrUpdateResource 'Common_FileType_Backup', 'fr-FR', N'sauvegarde', null;
EXEC InsertOrUpdateResource 'Common_FileType_Backup', 'en-US', N'backup', null;
EXEC InsertOrUpdateResource 'Common_FileType_Backup', 'pt-BR', N'backup', null;
EXEC InsertOrUpdateResource 'Common_FileType_Backup', 'es-ES', N'backup', null;
EXEC InsertOrUpdateResource 'Common_FileType_Backup', 'de-DE', N'backup', null;
EXEC InsertOrUpdateResource 'Common_FileType_Backup', 'pl-PL', N'backup', null;
GO
EXEC InsertOrUpdateResource 'Common_FileType_Picture', 'fr-FR', N'image', null;
EXEC InsertOrUpdateResource 'Common_FileType_Picture', 'en-US', N'image', null;
EXEC InsertOrUpdateResource 'Common_FileType_Picture', 'pt-BR', N'image', null;
EXEC InsertOrUpdateResource 'Common_FileType_Picture', 'es-ES', N'image', null;
EXEC InsertOrUpdateResource 'Common_FileType_Picture', 'de-DE', N'image', null;
EXEC InsertOrUpdateResource 'Common_FileType_Picture', 'pl-PL', N'image', null;
GO
EXEC InsertOrUpdateResource 'Common_FileType_Video', 'fr-FR', N'vidéo', null;
EXEC InsertOrUpdateResource 'Common_FileType_Video', 'en-US', N'video', null;
EXEC InsertOrUpdateResource 'Common_FileType_Video', 'pt-BR', N'video', null;
EXEC InsertOrUpdateResource 'Common_FileType_Video', 'es-ES', N'video', null;
EXEC InsertOrUpdateResource 'Common_FileType_Video', 'de-DE', N'video', null;
EXEC InsertOrUpdateResource 'Common_FileType_Video', 'pl-PL', N'video', null;
GO
EXEC InsertOrUpdateResource 'Core_Buttons_Merge', 'fr-FR', N'Fusionner', null;
EXEC InsertOrUpdateResource 'Core_Buttons_Merge', 'en-US', N'Merge', null;
EXEC InsertOrUpdateResource 'Core_Buttons_Merge', 'pt-BR', N'Unir', null;
EXEC InsertOrUpdateResource 'Core_Buttons_Merge', 'es-ES', N'Fusionar', null;
EXEC InsertOrUpdateResource 'Core_Buttons_Merge', 'de-DE', N'Zusammenführen', null;
EXEC InsertOrUpdateResource 'Core_Buttons_Merge', 'pl-PL', N'Połącz', null;
GO
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_AditionalInformation', 'fr-FR', N'Informations complémentaires', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_AditionalInformation', 'en-US', N'Additional comments', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_AditionalInformation', 'pt-BR', N'Informações adicionais', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_AditionalInformation', 'es-ES', N'Información adicional', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_AditionalInformation', 'de-DE', N'Weitere Informationen', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_AditionalInformation', 'pl-PL', N'Dodatkowe komentarze', null;
GO
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Company', 'fr-FR', N'Société', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Company', 'en-US', N'Company', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Company', 'pt-BR', N'Empresa', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Company', 'es-ES', N'Empresa', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Company', 'de-DE', N'Firma', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Company', 'pl-PL', N'Firma', null;
GO
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Email', 'fr-FR', N'E-mail', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Email', 'en-US', N'Email', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Email', 'pt-BR', N'E-mail', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Email', 'es-ES', N'E-mail', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Email', 'de-DE', N'E-Mail', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Email', 'pl-PL', N'Email', null;
GO
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Report', 'fr-FR', N'Rapport d''erreur', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Report', 'en-US', N'Error reporting', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Report', 'pt-BR', N'Relatório de erros', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Report', 'es-ES', N'Informar del error', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Report', 'de-DE', N'Fehler melden', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Report', 'pl-PL', N'Raport błędów', null;
GO
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_SendReport', 'fr-FR', N'Envoyer le rapport', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_SendReport', 'en-US', N'Send the report', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_SendReport', 'pt-BR', N'Enviar o relatório', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_SendReport', 'es-ES', N'Enviar el informe', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_SendReport', 'de-DE', N'Bericht senden', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_SendReport', 'pl-PL', N'Wyślij raport', null;
GO
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Username', 'fr-FR', N'Nom', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Username', 'en-US', N'Name', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Username', 'pt-BR', N'Nome', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Username', 'es-ES', N'Nombre', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Username', 'de-DE', N'Name', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_ErrorDialog_Username', 'pl-PL', N'Nazwisko', null;
GO
EXEC InsertOrUpdateResource 'Core_Dialogs_MessageDialog_ErrorReportSendFail', 'fr-FR', N'L''envoi de l''erreur par email a échoué.
Contactez votre administrateur système pour valider la configuration SMTP et votre connexion internet.', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_MessageDialog_ErrorReportSendFail', 'en-US', N'Unable to send the error report by email.
Contact your IT support to validate the SMTP configuration and your Internet connexion.', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_MessageDialog_ErrorReportSendFail', 'pt-BR', N'Não foi possível enviar o relatório de erro por e-mail.
Entre em contato com a area de TI para validar a configuração SMTP e sua conexão à Internet.', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_MessageDialog_ErrorReportSendFail', 'es-ES', N'No fue posible enviar el informe del error por email.
Póngase en contacto con el administrador del sistema para validar la configuración SMTP y la conexión a Internet. "', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_MessageDialog_ErrorReportSendFail', 'de-DE', N'Senden des Fehlerreports via E-Mail ist fehlgeschlagen.
Kontaktieren Sie Ihren Systemadministrator, um die SMTP-Konfiguration und Ihre Internetverbindung zu überprüfen.', null;
EXEC InsertOrUpdateResource 'Core_Dialogs_MessageDialog_ErrorReportSendFail', 'pl-PL', N'Unable to send the error report by email.
Contact your IT support to validate the SMTP configuration and your Internet connexion.', null;
GO
EXEC InsertOrUpdateResource 'Core_Gantt_DeleteLink', 'fr-FR', N'Supprimer le lien', null;
EXEC InsertOrUpdateResource 'Core_Gantt_DeleteLink', 'en-US', N'Delete the link', null;
EXEC InsertOrUpdateResource 'Core_Gantt_DeleteLink', 'pt-BR', N'Excluir o link', null;
EXEC InsertOrUpdateResource 'Core_Gantt_DeleteLink', 'es-ES', N'Quitar vínculo', null;
EXEC InsertOrUpdateResource 'Core_Gantt_DeleteLink', 'de-DE', N'Link löschen', null;
EXEC InsertOrUpdateResource 'Core_Gantt_DeleteLink', 'pl-PL', N'Usuń link', null;
GO
EXEC InsertOrUpdateResource 'DataGrid_ExcelExport', 'fr-FR', N'Exporter vers Excel', null;
EXEC InsertOrUpdateResource 'DataGrid_ExcelExport', 'en-US', N'Export to Excel', null;
EXEC InsertOrUpdateResource 'DataGrid_ExcelExport', 'pt-BR', N'Exportar para Excel', null;
EXEC InsertOrUpdateResource 'DataGrid_ExcelExport', 'es-ES', N'Exportación hacia Excel', null;
EXEC InsertOrUpdateResource 'DataGrid_ExcelExport', 'de-DE', N'Export nach Excel', null;
EXEC InsertOrUpdateResource 'DataGrid_ExcelExport', 'pl-PL', N'Eksport do excela', null;
GO
EXEC InsertOrUpdateResource 'DataGrid_ExcelExportSheetName', 'fr-FR', N'Export', null;
EXEC InsertOrUpdateResource 'DataGrid_ExcelExportSheetName', 'en-US', N'Export', null;
EXEC InsertOrUpdateResource 'DataGrid_ExcelExportSheetName', 'pt-BR', N'Exportação', null;
EXEC InsertOrUpdateResource 'DataGrid_ExcelExportSheetName', 'es-ES', N'Exportación', null;
EXEC InsertOrUpdateResource 'DataGrid_ExcelExportSheetName', 'de-DE', N'Export', null;
EXEC InsertOrUpdateResource 'DataGrid_ExcelExportSheetName', 'pl-PL', N'Eksport do excela', null;
GO
EXEC InsertOrUpdateResource 'ExportDialog_Browse', 'fr-FR', N'...', null;
EXEC InsertOrUpdateResource 'ExportDialog_Browse', 'en-US', N'...', null;
EXEC InsertOrUpdateResource 'ExportDialog_Browse', 'pt-BR', N'...', null;
EXEC InsertOrUpdateResource 'ExportDialog_Browse', 'es-ES', N'...', null;
EXEC InsertOrUpdateResource 'ExportDialog_Browse', 'de-DE', N'...', null;
EXEC InsertOrUpdateResource 'ExportDialog_Browse', 'pl-PL', N'…', null;
GO
EXEC InsertOrUpdateResource 'ExportDialog_Filename', 'fr-FR', N'Fichier :', null;
EXEC InsertOrUpdateResource 'ExportDialog_Filename', 'en-US', N'File:', null;
EXEC InsertOrUpdateResource 'ExportDialog_Filename', 'pt-BR', N'Arquivo:', null;
EXEC InsertOrUpdateResource 'ExportDialog_Filename', 'es-ES', N'Archivo:', null;
EXEC InsertOrUpdateResource 'ExportDialog_Filename', 'de-DE', N'Datei:', null;
EXEC InsertOrUpdateResource 'ExportDialog_Filename', 'pl-PL', N'Plik:', null;
GO
EXEC InsertOrUpdateResource 'ExportDialog_OpenWhenCreated', 'fr-FR', N'Ouvrir le fichier une fois créé', null;
EXEC InsertOrUpdateResource 'ExportDialog_OpenWhenCreated', 'en-US', N'Open the file once created', null;
EXEC InsertOrUpdateResource 'ExportDialog_OpenWhenCreated', 'pt-BR', N'Abra o arquivo depois de criado', null;
EXEC InsertOrUpdateResource 'ExportDialog_OpenWhenCreated', 'es-ES', N'Abra el archivo una vez creado', null;
EXEC InsertOrUpdateResource 'ExportDialog_OpenWhenCreated', 'de-DE', N'Datei nach Erstellung öffnen', null;
EXEC InsertOrUpdateResource 'ExportDialog_OpenWhenCreated', 'pl-PL', N'Otwórz utworzony plik', null;
GO
EXEC InsertOrUpdateResource 'ExportDialog_PleaseSetAFile', 'fr-FR', N'Veuillez spécifier un fichier', null;
EXEC InsertOrUpdateResource 'ExportDialog_PleaseSetAFile', 'en-US', N'Please specify a file', null;
EXEC InsertOrUpdateResource 'ExportDialog_PleaseSetAFile', 'pt-BR', N'Por favor, especifique um arquivo', null;
EXEC InsertOrUpdateResource 'ExportDialog_PleaseSetAFile', 'es-ES', N'Por favor, especifique un archivo', null;
EXEC InsertOrUpdateResource 'ExportDialog_PleaseSetAFile', 'de-DE', N'Bitte Datei auswählen', null;
EXEC InsertOrUpdateResource 'ExportDialog_PleaseSetAFile', 'pl-PL', N'Określ plik', null;
GO
EXEC InsertOrUpdateResource 'ExportDialog_Video', 'fr-FR', N'Vidéo :', null;
EXEC InsertOrUpdateResource 'ExportDialog_Video', 'en-US', N'Video:', null;
EXEC InsertOrUpdateResource 'ExportDialog_Video', 'pt-BR', N'Vídeo:', null;
EXEC InsertOrUpdateResource 'ExportDialog_Video', 'es-ES', N'Video:', null;
EXEC InsertOrUpdateResource 'ExportDialog_Video', 'de-DE', N'Video:', null;
EXEC InsertOrUpdateResource 'ExportDialog_Video', 'pl-PL', N'Film:', null;
GO
EXEC InsertOrUpdateResource 'ExportDialog_VideoFolder', 'fr-FR', N'Dossier vidéos :', null;
EXEC InsertOrUpdateResource 'ExportDialog_VideoFolder', 'en-US', N'Video folder:', null;
EXEC InsertOrUpdateResource 'ExportDialog_VideoFolder', 'pt-BR', N'Pasta de vídeos:', null;
EXEC InsertOrUpdateResource 'ExportDialog_VideoFolder', 'es-ES', N'Archivo de vídeo:', null;
EXEC InsertOrUpdateResource 'ExportDialog_VideoFolder', 'de-DE', N'Video-Verzeichnis:', null;
EXEC InsertOrUpdateResource 'ExportDialog_VideoFolder', 'pl-PL', N'Folder filmu:', null;
GO
EXEC InsertOrUpdateResource 'FileDescription_AllFiles', 'fr-FR', N'Tous les fichiers', null;
EXEC InsertOrUpdateResource 'FileDescription_AllFiles', 'en-US', N'All files', null;
EXEC InsertOrUpdateResource 'FileDescription_AllFiles', 'pt-BR', N'Todos os arquivos', null;
EXEC InsertOrUpdateResource 'FileDescription_AllFiles', 'es-ES', N'Todos los archivos', null;
EXEC InsertOrUpdateResource 'FileDescription_AllFiles', 'de-DE', N'Alle Dateien', null;
EXEC InsertOrUpdateResource 'FileDescription_AllFiles', 'pl-PL', N'Wszystkie pliki', null;
GO
EXEC InsertOrUpdateResource 'FileDescription_AllImageFiles', 'fr-FR', N'Tous les fichiers image', null;
EXEC InsertOrUpdateResource 'FileDescription_AllImageFiles', 'en-US', N'All picture files', null;
EXEC InsertOrUpdateResource 'FileDescription_AllImageFiles', 'pt-BR', N'Todos os arquivos', null;
EXEC InsertOrUpdateResource 'FileDescription_AllImageFiles', 'es-ES', N'Todos los archivos', null;
EXEC InsertOrUpdateResource 'FileDescription_AllImageFiles', 'de-DE', N'Alle Bild-Dateien', null;
EXEC InsertOrUpdateResource 'FileDescription_AllImageFiles', 'pl-PL', N'Wszystkie pliki z obrazkami', null;
GO
EXEC InsertOrUpdateResource 'FileDescription_AllSqlBackupFiles', 'fr-FR', N'Tous les fichiers de sauvegarde SQL', null;
EXEC InsertOrUpdateResource 'FileDescription_AllSqlBackupFiles', 'en-US', N'All the SQL backup files', null;
EXEC InsertOrUpdateResource 'FileDescription_AllSqlBackupFiles', 'pt-BR', N'Todos os arquivos SQL de backup ', null;
EXEC InsertOrUpdateResource 'FileDescription_AllSqlBackupFiles', 'es-ES', N'Todos los archivos de SQL backup', null;
EXEC InsertOrUpdateResource 'FileDescription_AllSqlBackupFiles', 'de-DE', N'Alle Sicherungskopien SQL ', null;
EXEC InsertOrUpdateResource 'FileDescription_AllSqlBackupFiles', 'pl-PL', N'Wszystkie pliki zapasowe SQL', null;
GO
EXEC InsertOrUpdateResource 'FileDescription_AllVideoFiles', 'fr-FR', N'Tous les fichiers vidéos', null;
EXEC InsertOrUpdateResource 'FileDescription_AllVideoFiles', 'en-US', N'All video files', null;
EXEC InsertOrUpdateResource 'FileDescription_AllVideoFiles', 'pt-BR', N'Todos os arquivos de vídeo', null;
EXEC InsertOrUpdateResource 'FileDescription_AllVideoFiles', 'es-ES', N'Todos los archivos de vídeo', null;
EXEC InsertOrUpdateResource 'FileDescription_AllVideoFiles', 'de-DE', N'Alle Video-Dateien', null;
EXEC InsertOrUpdateResource 'FileDescription_AllVideoFiles', 'pl-PL', N'Wszystkie pliki z filmami', null;
GO
EXEC InsertOrUpdateResource 'FileDescription_BMP', 'fr-FR', N'BMP', null;
EXEC InsertOrUpdateResource 'FileDescription_BMP', 'en-US', N'BMP', null;
EXEC InsertOrUpdateResource 'FileDescription_BMP', 'pt-BR', N'BMP', null;
EXEC InsertOrUpdateResource 'FileDescription_BMP', 'es-ES', N'BMP', null;
EXEC InsertOrUpdateResource 'FileDescription_BMP', 'de-DE', N'BMP', null;
EXEC InsertOrUpdateResource 'FileDescription_BMP', 'pl-PL', N'BMP', null;
GO
EXEC InsertOrUpdateResource 'FileDescription_JPEG', 'fr-FR', N'JPEG', null;
EXEC InsertOrUpdateResource 'FileDescription_JPEG', 'en-US', N'JPEG', null;
EXEC InsertOrUpdateResource 'FileDescription_JPEG', 'pt-BR', N'JPEG', null;
EXEC InsertOrUpdateResource 'FileDescription_JPEG', 'es-ES', N'JPEG', null;
EXEC InsertOrUpdateResource 'FileDescription_JPEG', 'de-DE', N'JPEG', null;
EXEC InsertOrUpdateResource 'FileDescription_JPEG', 'pl-PL', N'JPEG', null;
GO
EXEC InsertOrUpdateResource 'FileDescription_PNG', 'fr-FR', N'PNG', null;
EXEC InsertOrUpdateResource 'FileDescription_PNG', 'en-US', N'PNG', null;
EXEC InsertOrUpdateResource 'FileDescription_PNG', 'pt-BR', N'PNG', null;
EXEC InsertOrUpdateResource 'FileDescription_PNG', 'es-ES', N'PNG', null;
EXEC InsertOrUpdateResource 'FileDescription_PNG', 'de-DE', N'PNG', null;
EXEC InsertOrUpdateResource 'FileDescription_PNG', 'pl-PL', N'PNG', null;
GO
EXEC InsertOrUpdateResource 'HelpUserManualFileName', 'fr-FR', N'notice utilisateur.pdf', null;
EXEC InsertOrUpdateResource 'HelpUserManualFileName', 'en-US', N'user manual.pdf', null;
EXEC InsertOrUpdateResource 'HelpUserManualFileName', 'pt-BR', N'user manual.pdf', null;
EXEC InsertOrUpdateResource 'HelpUserManualFileName', 'es-ES', N'user manual.pdf', null;
EXEC InsertOrUpdateResource 'HelpUserManualFileName', 'de-DE', N'user manual.pdf', null;
EXEC InsertOrUpdateResource 'HelpUserManualFileName', 'pl-PL', N'user manual.pdf', null;
GO
EXEC InsertOrUpdateResource 'Key_Delete', 'fr-FR', N'Suppr', null;
EXEC InsertOrUpdateResource 'Key_Delete', 'en-US', N'Del', null;
EXEC InsertOrUpdateResource 'Key_Delete', 'pt-BR', N'Del', null;
EXEC InsertOrUpdateResource 'Key_Delete', 'es-ES', N'Borrar', null;
EXEC InsertOrUpdateResource 'Key_Delete', 'de-DE', N'Lösch', null;
EXEC InsertOrUpdateResource 'Key_Delete', 'pl-PL', N'Usuń', null;
GO
EXEC InsertOrUpdateResource 'Key_F2', 'fr-FR', N'F2', null;
EXEC InsertOrUpdateResource 'Key_F2', 'en-US', N'F2', null;
EXEC InsertOrUpdateResource 'Key_F2', 'pt-BR', N'F2', null;
EXEC InsertOrUpdateResource 'Key_F2', 'es-ES', N'F2', null;
EXEC InsertOrUpdateResource 'Key_F2', 'de-DE', N'F2', null;
EXEC InsertOrUpdateResource 'Key_F2', 'pl-PL', N'F2', null;
GO
EXEC InsertOrUpdateResource 'Key_F3', 'fr-FR', N'F3', null;
EXEC InsertOrUpdateResource 'Key_F3', 'en-US', N'F3', null;
EXEC InsertOrUpdateResource 'Key_F3', 'pt-BR', N'F3', null;
EXEC InsertOrUpdateResource 'Key_F3', 'es-ES', N'F3', null;
EXEC InsertOrUpdateResource 'Key_F3', 'de-DE', N'F3', null;
EXEC InsertOrUpdateResource 'Key_F3', 'pl-PL', N'F3', null;
GO
EXEC InsertOrUpdateResource 'Key_F4', 'fr-FR', N'F4', null;
EXEC InsertOrUpdateResource 'Key_F4', 'en-US', N'F4', null;
EXEC InsertOrUpdateResource 'Key_F4', 'pt-BR', N'F4', null;
EXEC InsertOrUpdateResource 'Key_F4', 'es-ES', N'F4', null;
EXEC InsertOrUpdateResource 'Key_F4', 'de-DE', N'F4', null;
EXEC InsertOrUpdateResource 'Key_F4', 'pl-PL', N'F4', null;
GO
EXEC InsertOrUpdateResource 'Key_F5', 'fr-FR', N'F5', null;
EXEC InsertOrUpdateResource 'Key_F5', 'en-US', N'F5', null;
EXEC InsertOrUpdateResource 'Key_F5', 'pt-BR', N'F5', null;
EXEC InsertOrUpdateResource 'Key_F5', 'es-ES', N'F5', null;
EXEC InsertOrUpdateResource 'Key_F5', 'de-DE', N'F5', null;
EXEC InsertOrUpdateResource 'Key_F5', 'pl-PL', N'F5', null;
GO
EXEC InsertOrUpdateResource 'Key_F8', 'fr-FR', N'F8', null;
EXEC InsertOrUpdateResource 'Key_F8', 'en-US', N'F8', null;
EXEC InsertOrUpdateResource 'Key_F8', 'pt-BR', N'F8', null;
EXEC InsertOrUpdateResource 'Key_F8', 'es-ES', N'F8', null;
EXEC InsertOrUpdateResource 'Key_F8', 'de-DE', N'F8', null;
EXEC InsertOrUpdateResource 'Key_F8', 'pl-PL', N'F8', null;
GO
EXEC InsertOrUpdateResource 'Key_F9', 'fr-FR', N'F9', null;
EXEC InsertOrUpdateResource 'Key_F9', 'en-US', N'F9', null;
EXEC InsertOrUpdateResource 'Key_F9', 'pt-BR', N'F9', null;
EXEC InsertOrUpdateResource 'Key_F9', 'es-ES', N'F9', null;
EXEC InsertOrUpdateResource 'Key_F9', 'de-DE', N'F9', null;
EXEC InsertOrUpdateResource 'Key_F9', 'pl-PL', N'F9', null;
GO
EXEC InsertOrUpdateResource 'KMediaPlayer_FastBackward', 'fr-FR', N'Ralentir', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_FastBackward', 'en-US', N'Slow down', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_FastBackward', 'pt-BR', N'Desacelerar', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_FastBackward', 'es-ES', N'Lento', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_FastBackward', 'de-DE', N'Verlangsamen', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_FastBackward', 'pl-PL', N'Zwolnij', null;
GO
EXEC InsertOrUpdateResource 'KMediaPlayer_FastForward', 'fr-FR', N'Accélérer', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_FastForward', 'en-US', N'Speed up', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_FastForward', 'pt-BR', N'Acelerar', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_FastForward', 'es-ES', N'Acelerar', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_FastForward', 'de-DE', N'Beschleunigen', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_FastForward', 'pl-PL', N'Przyspiesz', null;
GO
EXEC InsertOrUpdateResource 'KMediaPlayer_LinkMarkers', 'fr-FR', N'Lier les marqueurs', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_LinkMarkers', 'en-US', N'Link markers', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_LinkMarkers', 'pt-BR', N'Marcadores de ligação', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_LinkMarkers', 'es-ES', N'Unir los marcadores', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_LinkMarkers', 'de-DE', N'Marker verlinken', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_LinkMarkers', 'pl-PL', N'Oznaczenie linków', null;
GO
EXEC InsertOrUpdateResource 'KMediaPlayer_Pause', 'fr-FR', N'Pause', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Pause', 'en-US', N'Pause', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Pause', 'pt-BR', N'Pausa', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Pause', 'es-ES', N'Pausa', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Pause', 'de-DE', N'Pause', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Pause', 'pl-PL', N'Pauza', null;
GO
EXEC InsertOrUpdateResource 'KMediaPlayer_Play', 'fr-FR', N'Lecture', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Play', 'en-US', N'Play', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Play', 'pt-BR', N'Leitura', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Play', 'es-ES', N'Lectura', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Play', 'de-DE', N'Play', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Play', 'pl-PL', N'Odtwórz', null;
GO
EXEC InsertOrUpdateResource 'KMediaPlayer_PlayPause', 'fr-FR', N'Lecture / Pause', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_PlayPause', 'en-US', N'Play / Pause', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_PlayPause', 'pt-BR', N'Leitura / Pausa', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_PlayPause', 'es-ES', N'Reproducir / Pausa', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_PlayPause', 'de-DE', N'Play / Pause', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_PlayPause', 'pl-PL', N'Odtwórz/Pauza', null;
GO
EXEC InsertOrUpdateResource 'KMediaPlayer_ResetSpeed', 'fr-FR', N'Vitesse normale', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_ResetSpeed', 'en-US', N'Normal speed', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_ResetSpeed', 'pt-BR', N'Velocidade normal', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_ResetSpeed', 'es-ES', N'Velocidad normal', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_ResetSpeed', 'de-DE', N'Normale Geschwindigkeit', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_ResetSpeed', 'pl-PL', N'Normalna prędkość', null;
GO
EXEC InsertOrUpdateResource 'KMediaPlayer_StepBackward', 'fr-FR', N'Pas en arrière', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_StepBackward', 'en-US', N'Step backward', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_StepBackward', 'pt-BR', N'Passo para trás', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_StepBackward', 'es-ES', N'Atrás', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_StepBackward', 'de-DE', N'Schritt zurück', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_StepBackward', 'pl-PL', N'Krok wstecz', null;
GO
EXEC InsertOrUpdateResource 'KMediaPlayer_StepForward', 'fr-FR', N'Pas en avant', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_StepForward', 'en-US', N'Step forward', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_StepForward', 'pt-BR', N'Passo para frente', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_StepForward', 'es-ES', N'Adelante', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_StepForward', 'de-DE', N'Schritt nach vorn', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_StepForward', 'pl-PL', N'Krok do przodu', null;
GO
EXEC InsertOrUpdateResource 'KMediaPlayer_Stop', 'fr-FR', N'Changer le fichier vidéo', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Stop', 'en-US', N'Change video file', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Stop', 'pt-BR', N'Mudar arquivo de vídeo', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Stop', 'es-ES', N'Cambio de vídeo', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Stop', 'de-DE', N'Video ändern', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Stop', 'pl-PL', N'Zmień plik filmowy', null;
GO
EXEC InsertOrUpdateResource 'KMediaPlayer_Thumbnail', 'fr-FR', N'Extraire une image de la vidéo ou importer une image.', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Thumbnail', 'en-US', N'Extract a picture from the video clip or import a picture.', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Thumbnail', 'pt-BR', N'Capturar uma imagem do vídeo ou importar uma imagem', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Thumbnail', 'es-ES', N'Extraer una foto desde el video o importar una imagén.', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Thumbnail', 'de-DE', N'Ein Bild vom Video aufnehmen oder ein Bild importieren.', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Thumbnail', 'pl-PL', N'Wytnij zdjęcie z folmu lub importuj obrazek', null;
GO
EXEC InsertOrUpdateResource 'KMediaPlayer_Thumbnail_Remove', 'fr-FR', N'Supprimer la vignette actuelle', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Thumbnail_Remove', 'en-US', N'Delete the current thumbnail', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Thumbnail_Remove', 'pt-BR', N'Excluir esta miniatura', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Thumbnail_Remove', 'es-ES', N'Eliminar esta miniatura', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Thumbnail_Remove', 'de-DE', N'Miniaturbild löschen', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_Thumbnail_Remove', 'pl-PL', N'Usuń wszystkie podręczne szkice', null;
GO
EXEC InsertOrUpdateResource 'SubProcess_Thumbnail', 'fr-FR', N'Importer une image.', null;
EXEC InsertOrUpdateResource 'SubProcess_Thumbnail', 'en-US', N'Import a picture.', null;
EXEC InsertOrUpdateResource 'SubProcess_Thumbnail', 'pt-BR', N'Importar uma imagem', null;
EXEC InsertOrUpdateResource 'SubProcess_Thumbnail', 'es-ES', N'Importar una imagén.', null;
EXEC InsertOrUpdateResource 'SubProcess_Thumbnail', 'de-DE', N'Ein Bild importieren.', null;
EXEC InsertOrUpdateResource 'SubProcess_Thumbnail', 'pl-PL', N'Importuj obrazek', null;
GO
EXEC InsertOrUpdateResource 'SubProcess_Thumbnail_Remove', 'fr-FR', N'Supprimer la vignette actuelle', null;
EXEC InsertOrUpdateResource 'SubProcess_Thumbnail_Remove', 'en-US', N'Delete the current thumbnail', null;
EXEC InsertOrUpdateResource 'SubProcess_Thumbnail_Remove', 'pt-BR', N'Excluir esta miniatura', null;
EXEC InsertOrUpdateResource 'SubProcess_Thumbnail_Remove', 'es-ES', N'Eliminar esta miniatura', null;
EXEC InsertOrUpdateResource 'SubProcess_Thumbnail_Remove', 'de-DE', N'Miniaturbild löschen', null;
EXEC InsertOrUpdateResource 'SubProcess_Thumbnail_Remove', 'pl-PL', N'Usuń wszystkie podręczne szkice', null;
GO
EXEC InsertOrUpdateResource 'KMediaPlayer_UnlinkMarkers', 'fr-FR', N'Délier les marqueurs', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_UnlinkMarkers', 'en-US', N'Unlink markers', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_UnlinkMarkers', 'pt-BR', N'Marcadores desvinculados', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_UnlinkMarkers', 'es-ES', N'Desatar los marcadores', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_UnlinkMarkers', 'de-DE', N'Lösen der Markierungen', null;
EXEC InsertOrUpdateResource 'KMediaPlayer_UnlinkMarkers', 'pl-PL', N'Odznacz oznaczenia', null;
GO
EXEC InsertOrUpdateResource 'ListFilter_Date', 'fr-FR', N'Date', null;
EXEC InsertOrUpdateResource 'ListFilter_Date', 'en-US', N'Date', null;
EXEC InsertOrUpdateResource 'ListFilter_Date', 'pt-BR', N'Data', null;
EXEC InsertOrUpdateResource 'ListFilter_Date', 'es-ES', N'Fecha', null;
EXEC InsertOrUpdateResource 'ListFilter_Date', 'de-DE', N'Datum', null;
EXEC InsertOrUpdateResource 'ListFilter_Date', 'pl-PL', N'Data', null;
GO
EXEC InsertOrUpdateResource 'ListFilter_Label', 'fr-FR', N'Libellé', null;
EXEC InsertOrUpdateResource 'ListFilter_Label', 'en-US', N'Label', null;
EXEC InsertOrUpdateResource 'ListFilter_Label', 'pt-BR', N'Rótulo', null;
EXEC InsertOrUpdateResource 'ListFilter_Label', 'es-ES', N'Etiqueta', null;
EXEC InsertOrUpdateResource 'ListFilter_Label', 'de-DE', N'Beschriftung', null;
EXEC InsertOrUpdateResource 'ListFilter_Label', 'pl-PL', N'Etykieta', null;
GO
EXEC InsertOrUpdateResource 'ListFilter_SortBy', 'fr-FR', N'Trier par :', null;
EXEC InsertOrUpdateResource 'ListFilter_SortBy', 'en-US', N'Sort by:', null;
EXEC InsertOrUpdateResource 'ListFilter_SortBy', 'pt-BR', N'Classificar por:', null;
EXEC InsertOrUpdateResource 'ListFilter_SortBy', 'es-ES', N'Ordenar por:', null;
EXEC InsertOrUpdateResource 'ListFilter_SortBy', 'de-DE', N'Sortieren nach:', null;
EXEC InsertOrUpdateResource 'ListFilter_SortBy', 'pl-PL', N'Sortuj według:', null;
GO
EXEC InsertOrUpdateResource 'Menu_Admin_Activation', 'fr-FR', N'Activation', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Activation', 'en-US', N'Activation', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Activation', 'pt-BR', N'Ativação', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Activation', 'es-ES', N'Activación', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Activation', 'de-DE', N'Aktivierung', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Activation', 'pl-PL', N'Aktywacja', null;
GO
EXEC InsertOrUpdateResource 'Menu_Admin_Activation_Activation', 'fr-FR', N'Activation', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Activation_Activation', 'en-US', N'Activation', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Activation_Activation', 'pt-BR', N'Ativação', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Activation_Activation', 'es-ES', N'Activación', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Activation_Activation', 'de-DE', N'Aktivierung', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Activation_Activation', 'pl-PL', N'Aktywacja', null;
GO
EXEC InsertOrUpdateResource 'Menu_Admin_BackupRestore', 'fr-FR', N'Backup/Restore', null;
EXEC InsertOrUpdateResource 'Menu_Admin_BackupRestore', 'en-US', N'Backup/Restore', null;
EXEC InsertOrUpdateResource 'Menu_Admin_BackupRestore', 'pt-BR', N'Backup/Restore', null;
EXEC InsertOrUpdateResource 'Menu_Admin_BackupRestore', 'es-ES', N'Backup/Restore', null;
EXEC InsertOrUpdateResource 'Menu_Admin_BackupRestore', 'de-DE', N'Backup/Restore', null;
EXEC InsertOrUpdateResource 'Menu_Admin_BackupRestore', 'pl-PL', N'Backup/Restore', null;
GO
EXEC InsertOrUpdateResource 'Menu_Admin_Referentials', 'fr-FR', N'Référentiels', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Referentials', 'en-US', N'Referentials', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Referentials', 'pt-BR', N'Referenciais', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Referentials', 'es-ES', N'Referencias', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Referentials', 'de-DE', N'Referenzen', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Referentials', 'pl-PL', N'Odnośniki', null;
GO
EXEC InsertOrUpdateResource 'Menu_Admin_Users', 'fr-FR', N'Directory', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Users', 'en-US', N'Directory', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Users', 'pt-BR', N'Directory', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Users', 'es-ES', N'Directory', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Users', 'de-DE', N'Directory', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Users', 'pl-PL', N'Directory', null;
GO
EXEC InsertOrUpdateResource 'Menu_Admin_Users_Users', 'fr-FR', N'Directory', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Users_Users', 'en-US', N'Directory', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Users_Users', 'pt-BR', N'Directory', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Users_Users', 'es-ES', N'Directory', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Users_Users', 'de-DE', N'Directory', null;
EXEC InsertOrUpdateResource 'Menu_Admin_Users_Users', 'pl-PL', N'Directory', null;
GO
EXEC InsertOrUpdateResource 'Menu_Extensions', 'fr-FR', N'Extensions', null;
EXEC InsertOrUpdateResource 'Menu_Extensions', 'en-US', N'Extensions', null;
EXEC InsertOrUpdateResource 'Menu_Extensions', 'pt-BR', N'Extensões', null;
EXEC InsertOrUpdateResource 'Menu_Extensions', 'es-ES', N'Extenciones', null;
EXEC InsertOrUpdateResource 'Menu_Extensions', 'de-DE', N'Erweiterungen', null;
EXEC InsertOrUpdateResource 'Menu_Extensions', 'pl-PL', N'Utrzymanie', null;
GO
EXEC InsertOrUpdateResource 'Menu_Extensions_Configuration', 'fr-FR', N'Configuration', null;
EXEC InsertOrUpdateResource 'Menu_Extensions_Configuration', 'en-US', N'Settings', null;
EXEC InsertOrUpdateResource 'Menu_Extensions_Configuration', 'pt-BR', N'Configuração', null;
EXEC InsertOrUpdateResource 'Menu_Extensions_Configuration', 'es-ES', N'Configuración', null;
EXEC InsertOrUpdateResource 'Menu_Extensions_Configuration', 'de-DE', N'Einstellungen', null;
EXEC InsertOrUpdateResource 'Menu_Extensions_Configuration', 'pl-PL', N'Ustawienia', null;
GO
EXEC InsertOrUpdateResource 'Obj_Prod_Long', 'fr-FR', N'Productivité / flexibilité', null;
EXEC InsertOrUpdateResource 'Obj_Prod_Long', 'en-US', N'Productivity / flexibility', null;
EXEC InsertOrUpdateResource 'Obj_Prod_Long', 'pt-BR', N'Produtividade / flexibilidade', null;
EXEC InsertOrUpdateResource 'Obj_Prod_Long', 'es-ES', N'Productividad / flexibilidad', null;
EXEC InsertOrUpdateResource 'Obj_Prod_Long', 'de-DE', N'Produktivität / Flexibilität', null;
EXEC InsertOrUpdateResource 'Obj_Prod_Long', 'pl-PL', N'Wydajność produkcyjna/Elastyczność', null;
GO
EXEC InsertOrUpdateResource 'Obj_Prod_Short', 'fr-FR', N'Productivité / flexibilité', null;
EXEC InsertOrUpdateResource 'Obj_Prod_Short', 'en-US', N'Productivity / flexibility', null;
EXEC InsertOrUpdateResource 'Obj_Prod_Short', 'pt-BR', N'Produtividade / flexibilidade', null;
EXEC InsertOrUpdateResource 'Obj_Prod_Short', 'es-ES', N'Productividad / flexibilidad', null;
EXEC InsertOrUpdateResource 'Obj_Prod_Short', 'de-DE', N'Produktivität / Flexibilität', null;
EXEC InsertOrUpdateResource 'Obj_Prod_Short', 'pl-PL', N'Wydajność produkcyjna/Elastyczność', null;
GO
EXEC InsertOrUpdateResource 'Obj_Std_Long', 'fr-FR', N'Standardisation / Bonnes pratiques', null;
EXEC InsertOrUpdateResource 'Obj_Std_Long', 'en-US', N'Standardization / best practices', null;
EXEC InsertOrUpdateResource 'Obj_Std_Long', 'pt-BR', N'Padronização / melhores práticas', null;
EXEC InsertOrUpdateResource 'Obj_Std_Long', 'es-ES', N'Estandarización / Buenas Prácticas', null;
EXEC InsertOrUpdateResource 'Obj_Std_Long', 'de-DE', N'Standardisierung / Best Practices', null;
EXEC InsertOrUpdateResource 'Obj_Std_Long', 'pl-PL', N'Standaryzacja/najlepsze praktyki', null;
GO
EXEC InsertOrUpdateResource 'Obj_Std_Short', 'fr-FR', N'Standardisation / Bonnes pratiques', null;
EXEC InsertOrUpdateResource 'Obj_Std_Short', 'en-US', N'Standardization / best practices', null;
EXEC InsertOrUpdateResource 'Obj_Std_Short', 'pt-BR', N'Padronização / melhores práticas', null;
EXEC InsertOrUpdateResource 'Obj_Std_Short', 'es-ES', N'Estandarización / Buenas Prácticas', null;
EXEC InsertOrUpdateResource 'Obj_Std_Short', 'de-DE', N'Standardisierung / Best Practices', null;
EXEC InsertOrUpdateResource 'Obj_Std_Short', 'pl-PL', N'Standaryzacja/najlepsze praktyki', null;
GO
EXEC InsertOrUpdateResource 'Obj_VSM_Long', 'fr-FR', N'Visite / Audit', null;
EXEC InsertOrUpdateResource 'Obj_VSM_Long', 'en-US', N'Visit / Audit', null;
EXEC InsertOrUpdateResource 'Obj_VSM_Long', 'pt-BR', N'Visita / Auditoria', null;
EXEC InsertOrUpdateResource 'Obj_VSM_Long', 'es-ES', N'Visita / Estudio', null;
EXEC InsertOrUpdateResource 'Obj_VSM_Long', 'de-DE', N'Besuch / Audit', null;
EXEC InsertOrUpdateResource 'Obj_VSM_Long', 'pl-PL', N'Wizyta/Audyt', null;
GO
EXEC InsertOrUpdateResource 'Obj_VSM_Short', 'fr-FR', N'Visite / Audit', null;
EXEC InsertOrUpdateResource 'Obj_VSM_Short', 'en-US', N'Visit / Audit', null;
EXEC InsertOrUpdateResource 'Obj_VSM_Short', 'pt-BR', N'Visita / Auditoria', null;
EXEC InsertOrUpdateResource 'Obj_VSM_Short', 'es-ES', N'Visita / Estudio', null;
EXEC InsertOrUpdateResource 'Obj_VSM_Short', 'de-DE', N'Besuch / Audit', null;
EXEC InsertOrUpdateResource 'Obj_VSM_Short', 'pl-PL', N'Wizyta/Audyt', null;
GO
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
EXEC InsertOrUpdateResource 'Role_ANA_LONG', 'fr-FR', N'Analyste', null;
EXEC InsertOrUpdateResource 'Role_ANA_LONG', 'en-US', N'Analyst', null;
EXEC InsertOrUpdateResource 'Role_ANA_LONG', 'pt-BR', N'Analista', null;
EXEC InsertOrUpdateResource 'Role_ANA_LONG', 'es-ES', N'Analista', null;
EXEC InsertOrUpdateResource 'Role_ANA_LONG', 'de-DE', N'Analytiker', null;
EXEC InsertOrUpdateResource 'Role_ANA_LONG', 'pl-PL', N'Analityk', null;
GO
EXEC InsertOrUpdateResource 'Role_ANA_SHORT', 'fr-FR', N'Analyste', null;
EXEC InsertOrUpdateResource 'Role_ANA_SHORT', 'en-US', N'Analyst', null;
EXEC InsertOrUpdateResource 'Role_ANA_SHORT', 'pt-BR', N'Analista', null;
EXEC InsertOrUpdateResource 'Role_ANA_SHORT', 'es-ES', N'Analista', null;
EXEC InsertOrUpdateResource 'Role_ANA_SHORT', 'de-DE', N'Analytiker', null;
EXEC InsertOrUpdateResource 'Role_ANA_SHORT', 'pl-PL', N'Analityk', null;
GO
EXEC InsertOrUpdateResource 'Role_CON_LONG', 'fr-FR', N'Contributeur', null;
EXEC InsertOrUpdateResource 'Role_CON_LONG', 'en-US', N'Contributor', null;
EXEC InsertOrUpdateResource 'Role_CON_LONG', 'pt-BR', N'Contribuinte', null;
EXEC InsertOrUpdateResource 'Role_CON_LONG', 'es-ES', N'Contribuyente', null;
EXEC InsertOrUpdateResource 'Role_CON_LONG', 'de-DE', N'Beitragender', null;
EXEC InsertOrUpdateResource 'Role_CON_LONG', 'pl-PL', N'Osoba wspierająca', null;
GO
EXEC InsertOrUpdateResource 'Role_CON_SHORT', 'fr-FR', N'Contributeur', null;
EXEC InsertOrUpdateResource 'Role_CON_SHORT', 'en-US', N'Contributor', null;
EXEC InsertOrUpdateResource 'Role_CON_SHORT', 'pt-BR', N'Contribuinte', null;
EXEC InsertOrUpdateResource 'Role_CON_SHORT', 'es-ES', N'Contribuyente', null;
EXEC InsertOrUpdateResource 'Role_CON_SHORT', 'de-DE', N'Beitragender', null;
EXEC InsertOrUpdateResource 'Role_CON_SHORT', 'pl-PL', N'Osoba wspierająca', null;
GO
EXEC InsertOrUpdateResource 'Role_TEC_LONG', 'fr-FR', N'Technicien', null;
EXEC InsertOrUpdateResource 'Role_TEC_LONG', 'en-US', N'Technician', null;
EXEC InsertOrUpdateResource 'Role_TEC_LONG', 'pt-BR', N'Technician', null;
EXEC InsertOrUpdateResource 'Role_TEC_LONG', 'es-ES', N'Technician', null;
EXEC InsertOrUpdateResource 'Role_TEC_LONG', 'de-DE', N'Technician', null;
EXEC InsertOrUpdateResource 'Role_TEC_LONG', 'pl-PL', N'Technician', null;
GO
EXEC InsertOrUpdateResource 'Role_TEC_SHORT', 'fr-FR', N'Technicien', null;
EXEC InsertOrUpdateResource 'Role_TEC_SHORT', 'en-US', N'Technician', null;
EXEC InsertOrUpdateResource 'Role_TEC_SHORT', 'pt-BR', N'Technician', null;
EXEC InsertOrUpdateResource 'Role_TEC_SHORT', 'es-ES', N'Technician', null;
EXEC InsertOrUpdateResource 'Role_TEC_SHORT', 'de-DE', N'Technician', null;
EXEC InsertOrUpdateResource 'Role_TEC_SHORT', 'pl-PL', N'Technician', null;
GO
EXEC InsertOrUpdateResource 'Role_SUP_LONG', 'fr-FR', N'Superviseur', null;
EXEC InsertOrUpdateResource 'Role_SUP_LONG', 'en-US', N'Supervisor', null;
EXEC InsertOrUpdateResource 'Role_SUP_LONG', 'pt-BR', N'Supervisor', null;
EXEC InsertOrUpdateResource 'Role_SUP_LONG', 'es-ES', N'Supervisor', null;
EXEC InsertOrUpdateResource 'Role_SUP_LONG', 'de-DE', N'Supervisor', null;
EXEC InsertOrUpdateResource 'Role_SUP_LONG', 'pl-PL', N'Supervisor', null;
GO
EXEC InsertOrUpdateResource 'Role_SUP_SHORT', 'fr-FR', N'Superviseur', null;
EXEC InsertOrUpdateResource 'Role_SUP_SHORT', 'en-US', N'Supervisor', null;
EXEC InsertOrUpdateResource 'Role_SUP_SHORT', 'pt-BR', N'Supervisor', null;
EXEC InsertOrUpdateResource 'Role_SUP_SHORT', 'es-ES', N'Supervisor', null;
EXEC InsertOrUpdateResource 'Role_SUP_SHORT', 'de-DE', N'Supervisor', null;
EXEC InsertOrUpdateResource 'Role_SUP_SHORT', 'pl-PL', N'Supervisor', null;
GO
EXEC InsertOrUpdateResource 'Role_OPE_LONG', 'fr-FR', N'Opérateur', null;
EXEC InsertOrUpdateResource 'Role_OPE_LONG', 'en-US', N'Operator', null;
EXEC InsertOrUpdateResource 'Role_OPE_LONG', 'pt-BR', N'Operator', null;
EXEC InsertOrUpdateResource 'Role_OPE_LONG', 'es-ES', N'Operator', null;
EXEC InsertOrUpdateResource 'Role_OPE_LONG', 'de-DE', N'Operator', null;
EXEC InsertOrUpdateResource 'Role_OPE_LONG', 'pl-PL', N'Operator', null;
GO
EXEC InsertOrUpdateResource 'Role_OPE_SHORT', 'fr-FR', N'Opérateur', null;
EXEC InsertOrUpdateResource 'Role_OPE_SHORT', 'en-US', N'Operator', null;
EXEC InsertOrUpdateResource 'Role_OPE_SHORT', 'pt-BR', N'Operator', null;
EXEC InsertOrUpdateResource 'Role_OPE_SHORT', 'es-ES', N'Operator', null;
EXEC InsertOrUpdateResource 'Role_OPE_SHORT', 'de-DE', N'Operator', null;
EXEC InsertOrUpdateResource 'Role_OPE_SHORT', 'pl-PL', N'Operator', null;
GO
EXEC InsertOrUpdateResource 'Role_DOC_LONG', 'fr-FR', N'Documentaliste', null;
EXEC InsertOrUpdateResource 'Role_DOC_LONG', 'en-US', N'Documentalist', null;
EXEC InsertOrUpdateResource 'Role_DOC_LONG', 'pt-BR', N'Documentalist', null;
EXEC InsertOrUpdateResource 'Role_DOC_LONG', 'es-ES', N'Documentalist', null;
EXEC InsertOrUpdateResource 'Role_DOC_LONG', 'de-DE', N'Documentalist', null;
EXEC InsertOrUpdateResource 'Role_DOC_LONG', 'pl-PL', N'Documentalist', null;
GO
EXEC InsertOrUpdateResource 'Role_DOC_SHORT', 'fr-FR', N'Documentaliste', null;
EXEC InsertOrUpdateResource 'Role_DOC_SHORT', 'en-US', N'Documentalist', null;
EXEC InsertOrUpdateResource 'Role_DOC_SHORT', 'pt-BR', N'Documentalist', null;
EXEC InsertOrUpdateResource 'Role_DOC_SHORT', 'es-ES', N'Documentalist', null;
EXEC InsertOrUpdateResource 'Role_DOC_SHORT', 'de-DE', N'Documentalist', null;
EXEC InsertOrUpdateResource 'Role_DOC_SHORT', 'pl-PL', N'Documentalist', null;
GO
EXEC InsertOrUpdateResource 'Role_TRA_LONG', 'fr-FR', N'Formateur', null;
EXEC InsertOrUpdateResource 'Role_TRA_LONG', 'en-US', N'Trainer', null;
EXEC InsertOrUpdateResource 'Role_TRA_LONG', 'pt-BR', N'Instrutor', null;
EXEC InsertOrUpdateResource 'Role_TRA_LONG', 'es-ES', N'Formador', null;
EXEC InsertOrUpdateResource 'Role_TRA_LONG', 'de-DE', N'Trainer', null;
EXEC InsertOrUpdateResource 'Role_TRA_LONG', 'pl-PL', N'Trener', null;
GO
EXEC InsertOrUpdateResource 'Role_TRA_SHORT', 'fr-FR', N'Formateur', null;
EXEC InsertOrUpdateResource 'Role_TRA_SHORT', 'en-US', N'Trainer', null;
EXEC InsertOrUpdateResource 'Role_TRA_SHORT', 'pt-BR', N'Instrutor', null;
EXEC InsertOrUpdateResource 'Role_TRA_SHORT', 'es-ES', N'Formador', null;
EXEC InsertOrUpdateResource 'Role_TRA_SHORT', 'de-DE', N'Trainer', null;
EXEC InsertOrUpdateResource 'Role_TRA_SHORT', 'pl-PL', N'Trener', null;
GO
EXEC InsertOrUpdateResource 'Role_EVA_LONG', 'fr-FR', N'Evaluateur', null;
EXEC InsertOrUpdateResource 'Role_EVA_LONG', 'en-US', N'Evaluator', null;
EXEC InsertOrUpdateResource 'Role_EVA_LONG', 'pt-BR', N'Evaluator', null;
EXEC InsertOrUpdateResource 'Role_EVA_LONG', 'es-ES', N'Evaluator', null;
EXEC InsertOrUpdateResource 'Role_EVA_LONG', 'de-DE', N'Evaluator', null;
EXEC InsertOrUpdateResource 'Role_EVA_LONG', 'pl-PL', N'Evaluator', null;
GO
EXEC InsertOrUpdateResource 'Role_EVA_SHORT', 'fr-FR', N'Evaluateur', null;
EXEC InsertOrUpdateResource 'Role_EVA_SHORT', 'en-US', N'Evaluator', null;
EXEC InsertOrUpdateResource 'Role_EVA_SHORT', 'pt-BR', N'Evaluator', null;
EXEC InsertOrUpdateResource 'Role_EVA_SHORT', 'es-ES', N'Evaluator', null;
EXEC InsertOrUpdateResource 'Role_EVA_SHORT', 'de-DE', N'Evaluator', null;
EXEC InsertOrUpdateResource 'Role_EVA_SHORT', 'pl-PL', N'Evaluator', null;
GO
EXEC InsertOrUpdateResource 'ScenarioState_BRO_Long', 'fr-FR', N'Non figé', null;
EXEC InsertOrUpdateResource 'ScenarioState_BRO_Long', 'en-US', N'Non-frozen', null;
EXEC InsertOrUpdateResource 'ScenarioState_BRO_Long', 'pt-BR', N'Não-congelados', null;
EXEC InsertOrUpdateResource 'ScenarioState_BRO_Long', 'es-ES', N'No fijado', null;
EXEC InsertOrUpdateResource 'ScenarioState_BRO_Long', 'de-DE', N'Nicht schreibgeschützt', null;
EXEC InsertOrUpdateResource 'ScenarioState_BRO_Long', 'pl-PL', N'Nie zamrażaj', null;
GO
EXEC InsertOrUpdateResource 'ScenarioState_BRO_Short', 'fr-FR', N'Non figé', null;
EXEC InsertOrUpdateResource 'ScenarioState_BRO_Short', 'en-US', N'Non-frozen', null;
EXEC InsertOrUpdateResource 'ScenarioState_BRO_Short', 'pt-BR', N'Não-congelados', null;
EXEC InsertOrUpdateResource 'ScenarioState_BRO_Short', 'es-ES', N'No fijado', null;
EXEC InsertOrUpdateResource 'ScenarioState_BRO_Short', 'de-DE', N'Nicht schreibgeschützt', null;
EXEC InsertOrUpdateResource 'ScenarioState_BRO_Short', 'pl-PL', N'Nie zamrażaj', null;
GO
EXEC InsertOrUpdateResource 'ScenarioState_CIB_Long', 'fr-FR', N'Cible', null;
EXEC InsertOrUpdateResource 'ScenarioState_CIB_Long', 'en-US', N'Target', null;
EXEC InsertOrUpdateResource 'ScenarioState_CIB_Long', 'pt-BR', N'Alvo', null;
EXEC InsertOrUpdateResource 'ScenarioState_CIB_Long', 'es-ES', N'Objetivo', null;
EXEC InsertOrUpdateResource 'ScenarioState_CIB_Long', 'de-DE', N'Ziel', null;
EXEC InsertOrUpdateResource 'ScenarioState_CIB_Long', 'pl-PL', N'Cel', null;
GO
EXEC InsertOrUpdateResource 'ScenarioState_CIB_Short', 'fr-FR', N'Cible', null;
EXEC InsertOrUpdateResource 'ScenarioState_CIB_Short', 'en-US', N'Target', null;
EXEC InsertOrUpdateResource 'ScenarioState_CIB_Short', 'pt-BR', N'Alvo', null;
EXEC InsertOrUpdateResource 'ScenarioState_CIB_Short', 'es-ES', N'Objetivo', null;
EXEC InsertOrUpdateResource 'ScenarioState_CIB_Short', 'de-DE', N'Ziel', null;
EXEC InsertOrUpdateResource 'ScenarioState_CIB_Short', 'pl-PL', N'Cel', null;
GO
EXEC InsertOrUpdateResource 'ScenarioState_INI_Long', 'fr-FR', N'Initial', null;
EXEC InsertOrUpdateResource 'ScenarioState_INI_Long', 'en-US', N'Initial', null;
EXEC InsertOrUpdateResource 'ScenarioState_INI_Long', 'pt-BR', N'Inicial', null;
EXEC InsertOrUpdateResource 'ScenarioState_INI_Long', 'es-ES', N'Inicial', null;
EXEC InsertOrUpdateResource 'ScenarioState_INI_Long', 'de-DE', N'Initial', null;
EXEC InsertOrUpdateResource 'ScenarioState_INI_Long', 'pl-PL', N'Początkowy', null;
GO
EXEC InsertOrUpdateResource 'ScenarioState_INI_Short', 'fr-FR', N'Initial', null;
EXEC InsertOrUpdateResource 'ScenarioState_INI_Short', 'en-US', N'Initial', null;
EXEC InsertOrUpdateResource 'ScenarioState_INI_Short', 'pt-BR', N'Inicial', null;
EXEC InsertOrUpdateResource 'ScenarioState_INI_Short', 'es-ES', N'Inicial', null;
EXEC InsertOrUpdateResource 'ScenarioState_INI_Short', 'de-DE', N'Initial', null;
EXEC InsertOrUpdateResource 'ScenarioState_INI_Short', 'pl-PL', N'Początkowy', null;
GO
EXEC InsertOrUpdateResource 'ScenarioState_INT_Long', 'fr-FR', N'Intermédiaire', null;
EXEC InsertOrUpdateResource 'ScenarioState_INT_Long', 'en-US', N'Intermediate', null;
EXEC InsertOrUpdateResource 'ScenarioState_INT_Long', 'pt-BR', N'Intermediário', null;
EXEC InsertOrUpdateResource 'ScenarioState_INT_Long', 'es-ES', N'Intermedio', null;
EXEC InsertOrUpdateResource 'ScenarioState_INT_Long', 'de-DE', N'Zwischen-', null;
EXEC InsertOrUpdateResource 'ScenarioState_INT_Long', 'pl-PL', N'Pośredni', null;
GO
EXEC InsertOrUpdateResource 'ScenarioState_INT_Short', 'fr-FR', N'Intermédiaire', null;
EXEC InsertOrUpdateResource 'ScenarioState_INT_Short', 'en-US', N'Intermediate', null;
EXEC InsertOrUpdateResource 'ScenarioState_INT_Short', 'pt-BR', N'Intermediário', null;
EXEC InsertOrUpdateResource 'ScenarioState_INT_Short', 'es-ES', N'Intermedio', null;
EXEC InsertOrUpdateResource 'ScenarioState_INT_Short', 'de-DE', N'Zwischen-', null;
EXEC InsertOrUpdateResource 'ScenarioState_INT_Short', 'pl-PL', N'Pośredni', null;
GO
EXEC InsertOrUpdateResource 'ScenarioState_REA_Long', 'fr-FR', N'Validé', null;
EXEC InsertOrUpdateResource 'ScenarioState_REA_Long', 'en-US', N'Validated', null;
EXEC InsertOrUpdateResource 'ScenarioState_REA_Long', 'pt-BR', N'Validado', null;
EXEC InsertOrUpdateResource 'ScenarioState_REA_Long', 'es-ES', N'Validado', null;
EXEC InsertOrUpdateResource 'ScenarioState_REA_Long', 'de-DE', N'Validiert', null;
EXEC InsertOrUpdateResource 'ScenarioState_REA_Long', 'pl-PL', N'Zatwierdzony', null;
GO
EXEC InsertOrUpdateResource 'ScenarioState_REA_Short', 'fr-FR', N'Validé', null;
EXEC InsertOrUpdateResource 'ScenarioState_REA_Short', 'en-US', N'Validated', null;
EXEC InsertOrUpdateResource 'ScenarioState_REA_Short', 'pt-BR', N'Validado', null;
EXEC InsertOrUpdateResource 'ScenarioState_REA_Short', 'es-ES', N'Validado', null;
EXEC InsertOrUpdateResource 'ScenarioState_REA_Short', 'de-DE', N'Validiert', null;
EXEC InsertOrUpdateResource 'ScenarioState_REA_Short', 'pl-PL', N'Zatwierdzony', null;
GO
EXEC InsertOrUpdateResource 'ScenarioState_VAL_Long', 'fr-FR', N'Figé', null;
EXEC InsertOrUpdateResource 'ScenarioState_VAL_Long', 'en-US', N'Frozen', null;
EXEC InsertOrUpdateResource 'ScenarioState_VAL_Long', 'pt-BR', N'Congelado', null;
EXEC InsertOrUpdateResource 'ScenarioState_VAL_Long', 'es-ES', N'Fijado', null;
EXEC InsertOrUpdateResource 'ScenarioState_VAL_Long', 'de-DE', N'Schreibgeschützt', null;
EXEC InsertOrUpdateResource 'ScenarioState_VAL_Long', 'pl-PL', N'Zamrożony', null;
GO
EXEC InsertOrUpdateResource 'ScenarioState_VAL_Short', 'fr-FR', N'Figé', null;
EXEC InsertOrUpdateResource 'ScenarioState_VAL_Short', 'en-US', N'Frozen', null;
EXEC InsertOrUpdateResource 'ScenarioState_VAL_Short', 'pt-BR', N'Congelado', null;
EXEC InsertOrUpdateResource 'ScenarioState_VAL_Short', 'es-ES', N'Fijado', null;
EXEC InsertOrUpdateResource 'ScenarioState_VAL_Short', 'de-DE', N'Schreibgeschützt', null;
EXEC InsertOrUpdateResource 'ScenarioState_VAL_Short', 'pl-PL', N'Zamrożony', null;
GO
EXEC InsertOrUpdateResource 'Spinner_DefaultWaitMessage', 'fr-FR', N'Merci de patienter', null;
EXEC InsertOrUpdateResource 'Spinner_DefaultWaitMessage', 'en-US', N'Please wait', null;
EXEC InsertOrUpdateResource 'Spinner_DefaultWaitMessage', 'pt-BR', N'Espere por favor', null;
EXEC InsertOrUpdateResource 'Spinner_DefaultWaitMessage', 'es-ES', N'Espere por favor', null;
EXEC InsertOrUpdateResource 'Spinner_DefaultWaitMessage', 'de-DE', N'Bitte warten', null;
EXEC InsertOrUpdateResource 'Spinner_DefaultWaitMessage', 'pl-PL', N'Proszę czekać', null;
GO
EXEC InsertOrUpdateResource 'Style_Window_Maximize', 'fr-FR', N'Maximizer', null;
EXEC InsertOrUpdateResource 'Style_Window_Maximize', 'en-US', N'Maximize', null;
EXEC InsertOrUpdateResource 'Style_Window_Maximize', 'pt-BR', N'Maximizar', null;
EXEC InsertOrUpdateResource 'Style_Window_Maximize', 'es-ES', N'Maximizar', null;
EXEC InsertOrUpdateResource 'Style_Window_Maximize', 'de-DE', N'Maximieren', null;
EXEC InsertOrUpdateResource 'Style_Window_Maximize', 'pl-PL', N'Powiększ ', null;
GO
EXEC InsertOrUpdateResource 'Style_Window_Minimize', 'fr-FR', N'Minimiser', null;
EXEC InsertOrUpdateResource 'Style_Window_Minimize', 'en-US', N'Minimize', null;
EXEC InsertOrUpdateResource 'Style_Window_Minimize', 'pt-BR', N'Minimizar', null;
EXEC InsertOrUpdateResource 'Style_Window_Minimize', 'es-ES', N'Minimizar', null;
EXEC InsertOrUpdateResource 'Style_Window_Minimize', 'de-DE', N'Minimieren', null;
EXEC InsertOrUpdateResource 'Style_Window_Minimize', 'pl-PL', N'Pomniejsz', null;
GO
EXEC InsertOrUpdateResource 'Style_Window_Quit', 'fr-FR', N'Quitter', null;
EXEC InsertOrUpdateResource 'Style_Window_Quit', 'en-US', N'Quit', null;
EXEC InsertOrUpdateResource 'Style_Window_Quit', 'pt-BR', N'Sair', null;
EXEC InsertOrUpdateResource 'Style_Window_Quit', 'es-ES', N'Cerrar', null;
EXEC InsertOrUpdateResource 'Style_Window_Quit', 'de-DE', N'Verlassen', null;
EXEC InsertOrUpdateResource 'Style_Window_Quit', 'pl-PL', N'Wyjscie', null;
GO
EXEC InsertOrUpdateResource 'Style_Window_Restore', 'fr-FR', N'Restaurer', null;
EXEC InsertOrUpdateResource 'Style_Window_Restore', 'en-US', N'Restore', null;
EXEC InsertOrUpdateResource 'Style_Window_Restore', 'pt-BR', N'Restaurar', null;
EXEC InsertOrUpdateResource 'Style_Window_Restore', 'es-ES', N'Restaurar', null;
EXEC InsertOrUpdateResource 'Style_Window_Restore', 'de-DE', N'Wiederherstellen', null;
EXEC InsertOrUpdateResource 'Style_Window_Restore', 'pl-PL', N'Przywróc', null;
GO
EXEC InsertOrUpdateResource 'Validation_ActionCategory_Value_Required', 'fr-FR', N'La valorisation de la catégorie est requise.', null;
EXEC InsertOrUpdateResource 'Validation_ActionCategory_Value_Required', 'en-US', N'Category value is required.', null;
EXEC InsertOrUpdateResource 'Validation_ActionCategory_Value_Required', 'pt-BR', N'O valor da categoria é necessário.', null;
EXEC InsertOrUpdateResource 'Validation_ActionCategory_Value_Required', 'es-ES', N'Se requiere el valor de la categoria.', null;
EXEC InsertOrUpdateResource 'Validation_ActionCategory_Value_Required', 'de-DE', N'Kategorie-Wert ist erforderlich.', null;
EXEC InsertOrUpdateResource 'Validation_ActionCategory_Value_Required', 'pl-PL', N'Wartość kategorii jest wymagana', null;
GO
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue_StringLength', 'fr-FR', N'La valeur du champ libre ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue_StringLength', 'en-US', N'Free field value cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue_StringLength', 'pt-BR', N'O valor do campo livre não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue_StringLength', 'es-ES', N'El valor del campo libre no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue_StringLength', 'de-DE', N'Der Wert des freien Feld darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue_StringLength', 'pl-PL', N'Wolne pole nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue2_StringLength', 'fr-FR', N'La valeur du champ libre ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue2_StringLength', 'en-US', N'Free field value cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue2_StringLength', 'pt-BR', N'O valor do campo livre não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue2_StringLength', 'es-ES', N'El valor del campo libre no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue2_StringLength', 'de-DE', N'Der Wert des freien Feld darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue2_StringLength', 'pl-PL', N'Wolne pole nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue3_StringLength', 'fr-FR', N'La valeur du champ libre ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue3_StringLength', 'en-US', N'Free field value cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue3_StringLength', 'pt-BR', N'O valor do campo livre não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue3_StringLength', 'es-ES', N'El valor del campo libre no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue3_StringLength', 'de-DE', N'Der Wert des freien Feld darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue3_StringLength', 'pl-PL', N'Wolne pole nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue4_StringLength', 'fr-FR', N'La valeur du champ libre ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue4_StringLength', 'en-US', N'Free field value cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue4_StringLength', 'pt-BR', N'O valor do campo livre não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue4_StringLength', 'es-ES', N'El valor del campo libre no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue4_StringLength', 'de-DE', N'Der Wert des freien Feld darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_CustomTextValue4_StringLength', 'pl-PL', N'Wolne pole nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_KAction_DifferenceReason_StringLength', 'fr-FR', N'Le texte de Cause écart ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_DifferenceReason_StringLength', 'en-US', N'Gap reason text can not exceed {0} prints.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_DifferenceReason_StringLength', 'pt-BR', N'O texto da causa da diferença não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_DifferenceReason_StringLength', 'es-ES', N'El texto Causa desvio no puede exceder de  {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_DifferenceReason_StringLength', 'de-DE', N'Der Text der Ursachen darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_DifferenceReason_StringLength', 'pl-PL', N'Tekst dotyczący przyczyny luki nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_KAction_Label_StringLength', 'fr-FR', N'Le libellé ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_Label_StringLength', 'en-US', N'Label may not exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_Label_StringLength', 'pt-BR', N'O rótulo não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_Label_StringLength', 'es-ES', N'La etiqueta no podrá exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_Label_StringLength', 'de-DE', N'Die Beschreibung darf aus nicht mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_Label_StringLength', 'pl-PL', N'Etykieta nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_KAction_StartFinishDuration_Incoherent', 'fr-FR', N'Les informations de temps (début, durée, fin) sont incohérentes.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_StartFinishDuration_Incoherent', 'en-US', N'Task time information (start, duration, finish) are inconsistent.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_StartFinishDuration_Incoherent', 'pt-BR', N'Informações de tempo (início, duração, fim) da tarefa são inconsistentes.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_StartFinishDuration_Incoherent', 'es-ES', N'La información del tiempo (inicio, duración, final), son incoherentes.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_StartFinishDuration_Incoherent', 'de-DE', N'Die Zeitinformation (Beginn, Dauer, Ende) stimmen nicht überein.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_StartFinishDuration_Incoherent', 'pl-PL', N'Informacja nt. czasu zadań (start, trwanie, stop) jest niekonsekwenty.', null;
GO
EXEC InsertOrUpdateResource 'Validation_KAction_LinkedProcess_Incoherent', 'fr-FR', N'Le process lié ne peut être vide.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_LinkedProcess_Incoherent', 'en-US', N'Linked process may not be null.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_LinkedProcess_Incoherent', 'pt-BR', N'Linked process may not be null.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_LinkedProcess_Incoherent', 'es-ES', N'Linked process may not be null.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_LinkedProcess_Incoherent', 'de-DE', N'Linked process may not be null.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_LinkedProcess_Incoherent', 'pl-PL', N'Linked process may not be null.', null;
GO
EXEC InsertOrUpdateResource 'Validation_KAction_StartFinishDuration_Required', 'fr-FR', N'Les informations de temps (début, durée, fin) de la tâche sont requises.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_StartFinishDuration_Required', 'en-US', N'Task time information (start, duration, finish) are required.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_StartFinishDuration_Required', 'pt-BR', N'As informações de tempo (início, duração, fim) da tarefa são necessárias.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_StartFinishDuration_Required', 'es-ES', N'Se requieren las informaciones de tiempo (inicio, duración y final) de la tarea.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_StartFinishDuration_Required', 'de-DE', N'Die Zeitinformation (Beginn, Dauer, Ende) der Aufgabe ist erforderlich.', null;
EXEC InsertOrUpdateResource 'Validation_KAction_StartFinishDuration_Required', 'pl-PL', N'Informacja nt. czasu zadań (start, trwanie, stop) jest wymagany.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Member_Roles_MustHaveAtLeastOneRole', 'fr-FR', N'Le membre doit posséder au moins un rôle.', null;
EXEC InsertOrUpdateResource 'Validation_Member_Roles_MustHaveAtLeastOneRole', 'en-US', N'The member must have at least one role.', null;
EXEC InsertOrUpdateResource 'Validation_Member_Roles_MustHaveAtLeastOneRole', 'pt-BR', N'O membro deve ter pelo menos uma função.', null;
EXEC InsertOrUpdateResource 'Validation_Member_Roles_MustHaveAtLeastOneRole', 'es-ES', N'El miembro debe tener por lo menos una función.', null;
EXEC InsertOrUpdateResource 'Validation_Member_Roles_MustHaveAtLeastOneRole', 'de-DE', N'Mitglieder müssen mindestens einer Funktion zugeordnet sein.', null;
EXEC InsertOrUpdateResource 'Validation_Member_Roles_MustHaveAtLeastOneRole', 'pl-PL', N'Członek musi mieć co najmniej jedną rolę.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel_StringLength', 'fr-FR', N'Le libellé du champ libre numérique ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel_StringLength', 'en-US', N'Free numeric field label cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel_StringLength', 'pt-BR', N'O rótulo do campo numérico livre não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel_StringLength', 'es-ES', N'La etiqueta del campo libre numerico no puede exceder {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel_StringLength', 'de-DE', N'Die Beschreibung eines freien Zahlenfeldes darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel_StringLength', 'pl-PL', N'Wolne pole numeryczne etykiety nie może przekraczać {0} znaków', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel2_StringLength', 'fr-FR', N'Le libellé du champ libre numérique ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel2_StringLength', 'en-US', N'Free numeric field label cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel2_StringLength', 'pt-BR', N'O rótulo do campo numérico livre não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel2_StringLength', 'es-ES', N'La etiqueta del campo libre numerico no puede exceder {0} caracteres', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel2_StringLength', 'de-DE', N'Die Beschreibung eines freien Zahlenfeldes darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel2_StringLength', 'pl-PL', N'Wolne pole numeryczne etykiety nie może przekraczać {0} znaków', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel3_StringLength', 'fr-FR', N'Le libellé du champ libre numérique ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel3_StringLength', 'en-US', N'Free numeric field label cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel3_StringLength', 'pt-BR', N'O rótulo do campo numérico livre não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel3_StringLength', 'es-ES', N'La etiqueta del campo libre numerico no puede exceder {0} caracteres', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel3_StringLength', 'de-DE', N'Die Beschreibung eines freien Zahlenfeldes darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel3_StringLength', 'pl-PL', N'Wolne pole numeryczne etykiety nie może przekraczać {0} znaków', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel4_StringLength', 'fr-FR', N'Le libellé du champ libre numérique ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel4_StringLength', 'en-US', N'Free numeric field label cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel4_StringLength', 'pt-BR', N'O rótulo do campo numérico livre não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel4_StringLength', 'es-ES', N'La etiqueta del campo libre numerico no puede exceder {0} caracteres', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel4_StringLength', 'de-DE', N'Die Beschreibung eines freien Zahlenfeldes darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomNumericLabel4_StringLength', 'pl-PL', N'Wolne pole numeryczne etykiety nie może przekraczać {0} znaków', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel_StringLength', 'fr-FR', N'Le libellé du champ libre texte ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel_StringLength', 'en-US', N'Free text field label cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel_StringLength', 'pt-BR', N'O rótulo do campo de texto livre não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel_StringLength', 'es-ES', N'La etiqueta de un texto libre no podrá exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel_StringLength', 'de-DE', N'Die Beschreibung eines freien Textfeldes darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel_StringLength', 'pl-PL', N'Wolne pole tekstowe etykiety nie może przekraczać {0} znaków', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel2_StringLength', 'fr-FR', N'Le libellé du champ libre texte ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel2_StringLength', 'en-US', N'Free text field label cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel2_StringLength', 'pt-BR', N'O rótulo do campo de texto livre não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel2_StringLength', 'es-ES', N'La etiqueta de un texto libre no podrá exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel2_StringLength', 'de-DE', N'Die Beschreibung eines freien Textfeldes darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel2_StringLength', 'pl-PL', N'Wolne pole tekstowe etykiety nie może przekraczać {0} znaków', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel3_StringLength', 'fr-FR', N'Le libellé du champ libre texte ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel3_StringLength', 'en-US', N'Free text field label cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel3_StringLength', 'pt-BR', N'O rótulo do campo de texto livre não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel3_StringLength', 'es-ES', N'La etiqueta de un texto libre no podrá exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel3_StringLength', 'de-DE', N'Die Beschreibung eines freien Textfeldes darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel3_StringLength', 'pl-PL', N'Wolne pole tekstowe etykiety nie może przekraczać {0} znaków', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel4_StringLength', 'fr-FR', N'Le libellé du champ libre texte ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel4_StringLength', 'en-US', N'Free text field label cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel4_StringLength', 'pt-BR', N'O rótulo do campo de texto livre não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel4_StringLength', 'es-ES', N'La etiqueta de un texto libre no podrá exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel4_StringLength', 'de-DE', N'Die Beschreibung eines freien Textfeldes darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Project_CustomTextLabel4_StringLength', 'pl-PL', N'Wolne pole tekstowe etykiety nie może przekraczać {0} znaków', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_Description_StringLength', 'fr-FR', N'La description des enjeux ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Description_StringLength', 'en-US', N'The challenge description cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Description_StringLength', 'pt-BR', N'A descrição do desafio não pode ser superior a {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Description_StringLength', 'es-ES', N'La descripción de los objetivos no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Description_StringLength', 'de-DE', N'Die Beschreibung der Probleme kann nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Description_StringLength', 'pl-PL', N'Opis wyzwania nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_Label_Required', 'fr-FR', N'Le nom du projet est requis.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Label_Required', 'en-US', N'Project name is required.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Label_Required', 'pt-BR', N'O nome do projeto é necessário.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Label_Required', 'es-ES', N'El nombre del proyecto es necesario.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Label_Required', 'de-DE', N'Der Name des Projekts ist erforderlich.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Label_Required', 'pl-PL', N'Nazwa projketu jest wymagana.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_Label_StringLength', 'fr-FR', N'Le libellé ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Label_StringLength', 'en-US', N'Label may not exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Label_StringLength', 'pt-BR', N'O rótulo não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Label_StringLength', 'es-ES', N'La etiqueta no podrá exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Label_StringLength', 'de-DE', N'Die Beschreibung darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Label_StringLength', 'pl-PL', N'Etykieta nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_Objective_Required', 'fr-FR', N'L''objectif est requis.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Objective_Required', 'en-US', N'Objective is required.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Objective_Required', 'pt-BR', N'O objectivo é necessário.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Objective_Required', 'es-ES', N'El objetivo es necesario.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Objective_Required', 'de-DE', N'Der Zweck ist  erforderlich.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Objective_Required', 'pl-PL', N'Cel jest wymagany', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_OtherObjectiveLabel_StringLength', 'fr-FR', N'L''objectif ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Project_OtherObjectiveLabel_StringLength', 'en-US', N'The objective must not exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Project_OtherObjectiveLabel_StringLength', 'pt-BR', N'O objetivo não pode ser superior a {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Project_OtherObjectiveLabel_StringLength', 'es-ES', N'El objetivo no podrá exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Project_OtherObjectiveLabel_StringLength', 'de-DE', N'Der Zweck darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Project_OtherObjectiveLabel_StringLength', 'pl-PL', N'Cel nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_TimeScale_Required', 'fr-FR', N'La précision est requise.', null;
EXEC InsertOrUpdateResource 'Validation_Project_TimeScale_Required', 'en-US', N'Accuracy is required.', null;
EXEC InsertOrUpdateResource 'Validation_Project_TimeScale_Required', 'pt-BR', N'A precisão é necessária.', null;
EXEC InsertOrUpdateResource 'Validation_Project_TimeScale_Required', 'es-ES', N'La precisión es necesaria.', null;
EXEC InsertOrUpdateResource 'Validation_Project_TimeScale_Required', 'de-DE', N'Die Genauigkeit ist erforderlich.', null;
EXEC InsertOrUpdateResource 'Validation_Project_TimeScale_Required', 'pl-PL', N'Dokładnośc jest wymagana.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_Workshop_Required', 'fr-FR', N'Le périmètre est requis.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Workshop_Required', 'en-US', N'Scope is required.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Workshop_Required', 'pt-BR', N'O escopo é necessário.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Workshop_Required', 'es-ES', N'El perímetro es necesario.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Workshop_Required', 'de-DE', N'Der Umfang ist erforderlich.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Workshop_Required', 'pl-PL', N'Zakres jest wymagany.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Process_Label_Required', 'fr-FR', N'Le label est requis.', null;
EXEC InsertOrUpdateResource 'Validation_Process_Label_Required', 'en-US', N'Label is required.', null;
EXEC InsertOrUpdateResource 'Validation_Process_Label_Required', 'pt-BR', N'Label is required.', null;
EXEC InsertOrUpdateResource 'Validation_Process_Label_Required', 'es-ES', N'Label is required.', null;
EXEC InsertOrUpdateResource 'Validation_Process_Label_Required', 'de-DE', N'Label is required.', null;
EXEC InsertOrUpdateResource 'Validation_Process_Label_Required', 'pl-PL', N'Label is required.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Team_Name_Required', 'fr-FR', N'Le nom est requis.', null;
EXEC InsertOrUpdateResource 'Validation_Team_Name_Required', 'en-US', N'Name is required.', null;
EXEC InsertOrUpdateResource 'Validation_Team_Name_Required', 'pt-BR', N'Name is required.', null;
EXEC InsertOrUpdateResource 'Validation_Team_Name_Required', 'es-ES', N'Name is required.', null;
EXEC InsertOrUpdateResource 'Validation_Team_Name_Required', 'de-DE', N'Name is required.', null;
EXEC InsertOrUpdateResource 'Validation_Team_Name_Required', 'pl-PL', N'Name is required.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_StartDate_Required', 'fr-FR', N'La date de début est requise.', null;
EXEC InsertOrUpdateResource 'Validation_Project_StartDate_Required', 'en-US', N'Start date is required.', null;
EXEC InsertOrUpdateResource 'Validation_Project_StartDate_Required', 'pt-BR', N'Start date is required.', null;
EXEC InsertOrUpdateResource 'Validation_Project_StartDate_Required', 'es-ES', N'Start date is required.', null;
EXEC InsertOrUpdateResource 'Validation_Project_StartDate_Required', 'de-DE', N'Start date is required.', null;
EXEC InsertOrUpdateResource 'Validation_Project_StartDate_Required', 'pl-PL', N'Start date is required.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Project_Workshop_StringLength', 'fr-FR', N'Le périmètre ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Workshop_StringLength', 'en-US', N'The scope cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Workshop_StringLength', 'pt-BR', N'O escopo não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Workshop_StringLength', 'es-ES', N'El perimetro no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Workshop_StringLength', 'de-DE', N'Der Umfang darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Project_Workshop_StringLength', 'pl-PL', N'Zakres nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Referential_Color_Syntax', 'fr-FR', N'La couleur est invalide.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Color_Syntax', 'en-US', N'Color is invalid.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Color_Syntax', 'pt-BR', N'A cor não é válida.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Color_Syntax', 'es-ES', N'El color no es válido.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Color_Syntax', 'de-DE', N'Die Farbe ist ungültig.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Color_Syntax', 'pl-PL', N'Kolor jest błędny.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Referential_Description_StringLength', 'fr-FR', N'La description du référentiel ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Description_StringLength', 'en-US', N'The referential description cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Description_StringLength', 'pt-BR', N'A descrição do referencia não pode ser superior a {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Description_StringLength', 'es-ES', N'La descripción de la referencia no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Description_StringLength', 'de-DE', N'Die Beschreibung der Referenzen darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Description_StringLength', 'pl-PL', N'Opis alternatywy nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Referential_Label_Required', 'fr-FR', N'Le libellé est requis.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Label_Required', 'en-US', N'Label is required.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Label_Required', 'pt-BR', N'O rótulo é necessário.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Label_Required', 'es-ES', N'La etiqueta es necesaria.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Label_Required', 'de-DE', N'Die Beschreibung ist erforderlich.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Label_Required', 'pl-PL', N'Etykieta jest wymagana.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Referential_Label_StringLength', 'fr-FR', N'Le libellé ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Label_StringLength', 'en-US', N'Label may not exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Label_StringLength', 'pt-BR', N'O rótulo não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Label_StringLength', 'es-ES', N'La etiqueta no podrá exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Label_StringLength', 'de-DE', N'Die Beschreibung darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Label_StringLength', 'pl-PL', N'Etykieta nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Referential_Projet_Required', 'fr-FR', N'Le projet est requis.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Projet_Required', 'en-US', N'Project is required', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Projet_Required', 'pt-BR', N'O projeto é necessário.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Projet_Required', 'es-ES', N'El proyecto es necesario.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Projet_Required', 'de-DE', N'Das Projekt ist erforderlich,', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Projet_Required', 'pl-PL', N'Projekt jest wymagany.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Referential_Uri_StringLength', 'fr-FR', N'Le chemin ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Uri_StringLength', 'en-US', N'The path cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Uri_StringLength', 'pt-BR', N'O caminho não pode ser superior a {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Uri_StringLength', 'es-ES', N'La ruta no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Uri_StringLength', 'de-DE', N'Der Pfad darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Referential_Uri_StringLength', 'pl-PL', N'Ścieżka nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Resource_PaceRating_Range', 'fr-FR', N'Le jugement d''allure doit être compris en {1} et {2}.', null;
EXEC InsertOrUpdateResource 'Validation_Resource_PaceRating_Range', 'en-US', N'Pace Rate must be included in {1} and {2}.', null;
EXEC InsertOrUpdateResource 'Validation_Resource_PaceRating_Range', 'pt-BR', N'A taxa de ritmo deve ser entre {1} e {2}.', null;
EXEC InsertOrUpdateResource 'Validation_Resource_PaceRating_Range', 'es-ES', N'El coeficiente de velocidad debe encontrarse entre {1} y {2}.', null;
EXEC InsertOrUpdateResource 'Validation_Resource_PaceRating_Range', 'de-DE', N'Die Geschwindigkeit muss in {1} und {2} stehen.', null;
EXEC InsertOrUpdateResource 'Validation_Resource_PaceRating_Range', 'pl-PL', N'Współczynnik tempa musi być włączony w {1} i {2}.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Resource_Type_Required', 'fr-FR', N'Le type de la ressource est requis.', null;
EXEC InsertOrUpdateResource 'Validation_Resource_Type_Required', 'en-US', N'Resource type is required', null;
EXEC InsertOrUpdateResource 'Validation_Resource_Type_Required', 'pt-BR', N'O tipo de recurso é necessário.', null;
EXEC InsertOrUpdateResource 'Validation_Resource_Type_Required', 'es-ES', N'El tipo de recurso es necesario.', null;
EXEC InsertOrUpdateResource 'Validation_Resource_Type_Required', 'de-DE', N'Die Art der Ressource ist erforderlich', null;
EXEC InsertOrUpdateResource 'Validation_Resource_Type_Required', 'pl-PL', N'Rodzaj zasobów jest wymagany', null;
GO
EXEC InsertOrUpdateResource 'Validation_Scenario_Description_StringLength', 'fr-FR', N'La description ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Description_StringLength', 'en-US', N'The description cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Description_StringLength', 'pt-BR', N'A descrição não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Description_StringLength', 'es-ES', N'La descripción no podrá exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Description_StringLength', 'de-DE', N'Die Beschreibung darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Description_StringLength', 'pl-PL', N'Opis nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Scenario_Label_StringLength', 'fr-FR', N'Le libellé ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Label_StringLength', 'en-US', N'Label may not exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Label_StringLength', 'pt-BR', N'O rótulo não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Label_StringLength', 'es-ES', N'La etiqueta no podrá exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Label_StringLength', 'de-DE', N'Die Beschreibung darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Label_StringLength', 'pl-PL', N'Etykieta nie powinna przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Scenario_Name_Required', 'fr-FR', N'Le nom du scénario est requis.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Name_Required', 'en-US', N'Scenario name is required.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Name_Required', 'pt-BR', N'O nome do cenário é necessário.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Name_Required', 'es-ES', N'El nombre del escenario se requiere.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Name_Required', 'de-DE', N'Der Name des Szenarios ist erforderlich.', null;
EXEC InsertOrUpdateResource 'Validation_Scenario_Name_Required', 'pl-PL', N'Nazwa scenariusza jest wymagana.', null;
GO
EXEC InsertOrUpdateResource 'Validation_User_Email_StringLength', 'fr-FR', N'L''email ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_User_Email_StringLength', 'en-US', N'Email cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_User_Email_StringLength', 'pt-BR', N'O e-mail não pode ser superior a {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_User_Email_StringLength', 'es-ES', N'El correo electrónico no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_User_Email_StringLength', 'de-DE', N'Die E-Mail darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_User_Email_StringLength', 'pl-PL', N'Email nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_User_Firstname_StringLength', 'fr-FR', N'Le prénom ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_User_Firstname_StringLength', 'en-US', N'The First Name cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_User_Firstname_StringLength', 'pt-BR', N'O nome não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_User_Firstname_StringLength', 'es-ES', N'El Nombre no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_User_Firstname_StringLength', 'de-DE', N'Der Vorname darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_User_Firstname_StringLength', 'pl-PL', N'Imię nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_User_Name_StringLength', 'fr-FR', N'Le nom ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_User_Name_StringLength', 'en-US', N'The Last Name cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_User_Name_StringLength', 'pt-BR', N'O sobrenome não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_User_Name_StringLength', 'es-ES', N'El Apellido no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_User_Name_StringLength', 'de-DE', N'Der Nachname darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_User_Name_StringLength', 'pl-PL', N'Nazwisko nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_User_NewPassword_Required', 'fr-FR', N'Le mot de passe est requis.', null;
EXEC InsertOrUpdateResource 'Validation_User_NewPassword_Required', 'en-US', N'Password is required.', null;
EXEC InsertOrUpdateResource 'Validation_User_NewPassword_Required', 'pt-BR', N'A Password é necessária.', null;
EXEC InsertOrUpdateResource 'Validation_User_NewPassword_Required', 'es-ES', N'La contraseña es necesaria.', null;
EXEC InsertOrUpdateResource 'Validation_User_NewPassword_Required', 'de-DE', N'Das Passwort ist erforderlich.', null;
EXEC InsertOrUpdateResource 'Validation_User_NewPassword_Required', 'pl-PL', N'Hasło jest wymagane.', null;
GO
EXEC InsertOrUpdateResource 'Validation_User_NewPasswords_DontMatch', 'fr-FR', N'Les mots de passe ne correspondent pas.', null;
EXEC InsertOrUpdateResource 'Validation_User_NewPasswords_DontMatch', 'en-US', N'Passwords do not match.', null;
EXEC InsertOrUpdateResource 'Validation_User_NewPasswords_DontMatch', 'pt-BR', N'As Passwords não coincidem.', null;
EXEC InsertOrUpdateResource 'Validation_User_NewPasswords_DontMatch', 'es-ES', N'Las contraseñas no coinciden.', null;
EXEC InsertOrUpdateResource 'Validation_User_NewPasswords_DontMatch', 'de-DE', N'Die Passwörter stimmen nicht überein.', null;
EXEC InsertOrUpdateResource 'Validation_User_NewPasswords_DontMatch', 'pl-PL', N'Błędne hasło.', null;
GO
EXEC InsertOrUpdateResource 'Validation_User_PhoneNumber_StringLength', 'fr-FR', N'Le numéro de téléphone ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_User_PhoneNumber_StringLength', 'en-US', N'The phone number cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_User_PhoneNumber_StringLength', 'pt-BR', N'O número de telefone não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_User_PhoneNumber_StringLength', 'es-ES', N'El número de teléfono no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_User_PhoneNumber_StringLength', 'de-DE', N'Die Rufnummer darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_User_PhoneNumber_StringLength', 'pl-PL', N'Numer telefonu nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_User_Roles_MustHaveAtLeastOneRole', 'fr-FR', N'L''utilisateur doit posséder au moins un rôle.', null;
EXEC InsertOrUpdateResource 'Validation_User_Roles_MustHaveAtLeastOneRole', 'en-US', N'The user must have at least one role.', null;
EXEC InsertOrUpdateResource 'Validation_User_Roles_MustHaveAtLeastOneRole', 'pt-BR', N'O usuário deve ter pelo menos uma função.', null;
EXEC InsertOrUpdateResource 'Validation_User_Roles_MustHaveAtLeastOneRole', 'es-ES', N'El usuario debe tener al menos una función.', null;
EXEC InsertOrUpdateResource 'Validation_User_Roles_MustHaveAtLeastOneRole', 'de-DE', N'Der Benutzer muss über mindestens eine Rolle verfügen.', null;
EXEC InsertOrUpdateResource 'Validation_User_Roles_MustHaveAtLeastOneRole', 'pl-PL', N'Użytkownik musi mieć co najmniej jedną rolę.', null;
GO
EXEC InsertOrUpdateResource 'Validation_User_Username_Required', 'fr-FR', N'L''identifiant de l''utilisateur est requis.', null;
EXEC InsertOrUpdateResource 'Validation_User_Username_Required', 'en-US', N'User ID is required.', null;
EXEC InsertOrUpdateResource 'Validation_User_Username_Required', 'pt-BR', N'O ID do usuário é necessário.', null;
EXEC InsertOrUpdateResource 'Validation_User_Username_Required', 'es-ES', N'El ID del usuario es requerido.', null;
EXEC InsertOrUpdateResource 'Validation_User_Username_Required', 'de-DE', N'Die Benutzer-ID ist erforderlich.', null;
EXEC InsertOrUpdateResource 'Validation_User_Username_Required', 'pl-PL', N'ID użytkownika jest wymagane.', null;
GO
EXEC InsertOrUpdateResource 'Validation_User_Username_StringLength', 'fr-FR', N'Le nom d''utilisateur ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_User_Username_StringLength', 'en-US', N'The user name cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_User_Username_StringLength', 'pt-BR', N'O nome de usuário não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_User_Username_StringLength', 'es-ES', N'El nombre del usuario no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_User_Username_StringLength', 'de-DE', N'Der Benutzername darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_User_Username_StringLength', 'pl-PL', N'Nazwa użytkownika nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Video_Description_StringLength', 'fr-FR', N'La description ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Description_StringLength', 'en-US', N'The description cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Description_StringLength', 'pt-BR', N'A descrição não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Description_StringLength', 'es-ES', N'La descripción no podrá exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Description_StringLength', 'de-DE', N'Die Beschreibung darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Description_StringLength', 'pl-PL', N'Opis nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Video_FilePath_Required', 'fr-FR', N'Le chemin vers le fichier vidéo est requis.', null;
EXEC InsertOrUpdateResource 'Validation_Video_FilePath_Required', 'en-US', N'Path to the video file is required.', null;
EXEC InsertOrUpdateResource 'Validation_Video_FilePath_Required', 'pt-BR', N'Caminho para o arquivo de vídeo é necessário.', null;
EXEC InsertOrUpdateResource 'Validation_Video_FilePath_Required', 'es-ES', N'La ruta hacia archivo de vídeo es necesario.', null;
EXEC InsertOrUpdateResource 'Validation_Video_FilePath_Required', 'de-DE', N'Pfad der Video-Datei wird benötigt.', null;
EXEC InsertOrUpdateResource 'Validation_Video_FilePath_Required', 'pl-PL', N'Ścieżka do filmu jest wymagana.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Video_FilePath_StringLength', 'fr-FR', N'Le chemin ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Video_FilePath_StringLength', 'en-US', N'The path cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Video_FilePath_StringLength', 'pt-BR', N'O caminho não pode ser superior a {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Video_FilePath_StringLength', 'es-ES', N'La ruta no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Video_FilePath_StringLength', 'de-DE', N'Der Pfad kann nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Video_FilePath_StringLength', 'pl-PL', N'Ścieżka nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Video_Format_StringLength', 'fr-FR', N'Le format ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Format_StringLength', 'en-US', N'The format must not exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Format_StringLength', 'pt-BR', N'O formato não deve exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Format_StringLength', 'es-ES', N'El formato no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Format_StringLength', 'de-DE', N'Das Format darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Format_StringLength', 'pl-PL', N'Format nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Video_Name_Required', 'fr-FR', N'Le nom de la vidéo est requis.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Name_Required', 'en-US', N'Video name is required.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Name_Required', 'pt-BR', N'O nome do vídeo é necessário.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Name_Required', 'es-ES', N'El nombre del video es necesario.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Name_Required', 'de-DE', N'Der Name des Videos wird benötigt.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Name_Required', 'pl-PL', N'Nazwa filmu jest wymagana.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Video_Name_StringLength', 'fr-FR', N'Le nom ne peut excéder {0} caractères.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Name_StringLength', 'en-US', N'The name cannot exceed {0} characters.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Name_StringLength', 'pt-BR', N'O nome não pode exceder {0} dígitos.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Name_StringLength', 'es-ES', N'El nombre no puede exceder de {0} caracteres.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Name_StringLength', 'de-DE', N'Der Name darf nicht aus mehr als {0} Zeichen bestehen.', null;
EXEC InsertOrUpdateResource 'Validation_Video_Name_StringLength', 'pl-PL', N'Nazwa nie może przekraczać {0} znaków.', null;
GO
EXEC InsertOrUpdateResource 'Validation_Video_Nature_Required', 'fr-FR', N'La vidéo est-elle liée à une ressource dédiée (opérateur ou équipement) ?', null;
EXEC InsertOrUpdateResource 'Validation_Video_Nature_Required', 'en-US', N'Is the video linked to a dedicated resource (operator or equipment)?', null;
EXEC InsertOrUpdateResource 'Validation_Video_Nature_Required', 'pt-BR', N'O vídeo está ligado a um recurso dedicado (operador ou equipamento)?', null;
EXEC InsertOrUpdateResource 'Validation_Video_Nature_Required', 'es-ES', N'¿Está el video relacionado con un recurso específico (operador o equipo) ?', null;
EXEC InsertOrUpdateResource 'Validation_Video_Nature_Required', 'de-DE', N'Ist  das Video mit einer fest zugeordneten Ressource verbunden? (ein Operator oder eine Ausrüstung)', null;
EXEC InsertOrUpdateResource 'Validation_Video_Nature_Required', 'pl-PL', N'Czy film jest połaczony z dedykowanym zasobem (operator lub sprzęt)?', null;
GO
EXEC InsertOrUpdateResource 'View_AboutView_AllRightsReserved', 'fr-FR', N'Tous droits réservés', null;
EXEC InsertOrUpdateResource 'View_AboutView_AllRightsReserved', 'en-US', N'All rights reserved', null;
EXEC InsertOrUpdateResource 'View_AboutView_AllRightsReserved', 'pt-BR', N'Todos os direitos reservados', null;
EXEC InsertOrUpdateResource 'View_AboutView_AllRightsReserved', 'es-ES', N'Reservados todos los derechos', null;
EXEC InsertOrUpdateResource 'View_AboutView_AllRightsReserved', 'de-DE', N'Alle Rechte vorbehalten', null;
EXEC InsertOrUpdateResource 'View_AboutView_AllRightsReserved', 'pl-PL', N'Wszelkie prawa zastrzeżone', null;
GO
EXEC InsertOrUpdateResource 'View_AboutView_Contact', 'fr-FR', N'Contact :', null;
EXEC InsertOrUpdateResource 'View_AboutView_Contact', 'en-US', N'Contact:', null;
EXEC InsertOrUpdateResource 'View_AboutView_Contact', 'pt-BR', N'Contato:', null;
EXEC InsertOrUpdateResource 'View_AboutView_Contact', 'es-ES', N'Contacto:', null;
EXEC InsertOrUpdateResource 'View_AboutView_Contact', 'de-DE', N'Kontakt:', null;
EXEC InsertOrUpdateResource 'View_AboutView_Contact', 'pl-PL', N'Kontakt:', null;
GO
EXEC InsertOrUpdateResource 'View_AboutView_IDDN', 'fr-FR', N'Numéro IDDN :', null;
EXEC InsertOrUpdateResource 'View_AboutView_IDDN', 'en-US', N'IDDN number:', null;
EXEC InsertOrUpdateResource 'View_AboutView_IDDN', 'pt-BR', N'IDDN number:', null;
EXEC InsertOrUpdateResource 'View_AboutView_IDDN', 'es-ES', N'IDDN number:', null;
EXEC InsertOrUpdateResource 'View_AboutView_IDDN', 'de-DE', N'IDDN number:', null;
EXEC InsertOrUpdateResource 'View_AboutView_IDDN', 'pl-PL', N'IDDN numer:', null;
GO
EXEC InsertOrUpdateResource 'View_AboutView_Title', 'fr-FR', N'A propos', null;
EXEC InsertOrUpdateResource 'View_AboutView_Title', 'en-US', N'About', null;
EXEC InsertOrUpdateResource 'View_AboutView_Title', 'pt-BR', N'A respeito', null;
EXEC InsertOrUpdateResource 'View_AboutView_Title', 'es-ES', N'Sobre', null;
EXEC InsertOrUpdateResource 'View_AboutView_Title', 'de-DE', N'Über', null;
EXEC InsertOrUpdateResource 'View_AboutView_Title', 'pl-PL', N'Info', null;
GO
EXEC InsertOrUpdateResource 'View_AboutView_Version', 'fr-FR', N'Version :', null;
EXEC InsertOrUpdateResource 'View_AboutView_Version', 'en-US', N'Version:', null;
EXEC InsertOrUpdateResource 'View_AboutView_Version', 'pt-BR', N'Versão:', null;
EXEC InsertOrUpdateResource 'View_AboutView_Version', 'es-ES', N'Versión:', null;
EXEC InsertOrUpdateResource 'View_AboutView_Version', 'de-DE', N'Version:', null;
EXEC InsertOrUpdateResource 'View_AboutView_Version', 'pl-PL', N'Wersja:', null;
GO
EXEC InsertOrUpdateResource 'View_AboutView_Website', 'fr-FR', N'Site web :', null;
EXEC InsertOrUpdateResource 'View_AboutView_Website', 'en-US', N'Web site:', null;
EXEC InsertOrUpdateResource 'View_AboutView_Website', 'pt-BR', N'Web site:', null;
EXEC InsertOrUpdateResource 'View_AboutView_Website', 'es-ES', N'Sitio Web:', null;
EXEC InsertOrUpdateResource 'View_AboutView_Website', 'de-DE', N'Website:', null;
EXEC InsertOrUpdateResource 'View_AboutView_Website', 'pl-PL', N'Stona internetowa:', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_ActivationExplanation', 'fr-FR', N'Pour activer le produit, merci d''envoyer les informations ci-dessus par mail à {0}.
Pour vous aider lors de cet envoi, vous pouvez utiliser le bouton "Envoyer par e-mail" ci-dessous.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ActivationExplanation', 'en-US', N'To activate the product, please send the information above by email to {0}.
To help you in this shipment, you can use the "Send by e-mail" button below.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ActivationExplanation', 'pt-BR', N'Para ativar o produto, por favor envie as informações acima por e-mail para {0}.
Para ajudá-lo nesse envio, você pode usar o "Enviar por e-mail " abaixo.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ActivationExplanation', 'es-ES', N'Para activar el producto, por favor enviar la información arriba mencionada por correo electrónico a {0}.
Para ayudarles a enviar este correo, puede utilizar el "Enviar por E-mail" de abajo.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ActivationExplanation', 'de-DE', N'Um das Produkt zu aktivieren, senden Sie bitte die oben genannten Informationen per E-Mail an {0}.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ActivationExplanation', 'pl-PL', N'W celu aktywacji producktu, prześlij powyższą wiadomość emailem do {0}. W celu ułatwienia wysyłki, możesz użyć przycisku poniżej "Wyślij emailem". ', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_Company', 'fr-FR', N'Société', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Company', 'en-US', N'Company', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Company', 'pt-BR', N'Empresa', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Company', 'es-ES', N'Empresa', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Company', 'de-DE', N'Firma', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Company', 'pl-PL', N'Fima', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_Email', 'fr-FR', N'Email', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Email', 'en-US', N'Email', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Email', 'pt-BR', N'E-mail', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Email', 'es-ES', N'E-mail', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Email', 'de-DE', N'E-Mail', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Email', 'pl-PL', N'Email', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_ImportKey', 'fr-FR', N'Importer une clé', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ImportKey', 'en-US', N'Import a key', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ImportKey', 'pt-BR', N'Importar uma chave', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ImportKey', 'es-ES', N'Importación de una clave', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ImportKey', 'de-DE', N'Importieren einer Schlüsselrolle', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ImportKey', 'pl-PL', N'Importuj klucz', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_ImportKeyExplanation', 'fr-FR', N'Une fois que vous avez reçu le fichier de licence, envoyé par K-process, importez la clé à l''aide du bouton ci-dessous.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ImportKeyExplanation', 'en-US', N'Once you have received the license file, sent by K-process, import the key by using the button below.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ImportKeyExplanation', 'pt-BR', N'Depois de ter recebido o arquivo de licença, enviada por K-processo, importar a chave, usando o botão abaixo.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ImportKeyExplanation', 'es-ES', N'Una vez que haya recibido el archivo de licencia enviado por K-process, importar la clave utilizando el botón de abajo.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ImportKeyExplanation', 'de-DE', N'Nachdem Sie die Lizenzdatei von K-Prozess gesendet wurde, importieren Sie den Schlüssel über den Button unten.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_ImportKeyExplanation', 'pl-PL', N'Kiedy otrzymasz plik licencyjny, wysłany przez K-process, impotuj klucz przez użycie przycisku poniżej', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_KeyClipboard', 'fr-FR', N'Copier dans le presse papier', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyClipboard', 'en-US', N'Copy to Clipboard', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyClipboard', 'pt-BR', N'Copiar para o Clipboard', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyClipboard', 'es-ES', N'Copiar en el portapapeles', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyClipboard', 'de-DE', N'In die Zwischenablage kopieren', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyClipboard', 'pl-PL', N'Kopiuj do Clipboard', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmail', 'fr-FR', N'Envoyer par e-mail', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmail', 'en-US', N'Send by e-mail', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmail', 'pt-BR', N'Enviar por e-mail', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmail', 'es-ES', N'Enviar por E-mail', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmail', 'de-DE', N'Senden per E-Mail', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmail', 'pl-PL', N'Wyślij emailem', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmailBody', 'fr-FR', N'Nom :
{0}

Société :
{1}

Email :
{2}

Identifiant machine :
{3}

Informations logiciel :
  Licence : {4}
  Version : {5}

Informations système :
{6}', N'{0} est le nom, {1} la société, {2} l''email, {3} l''identifiant machine, {4} la license, {5} la version, {6} les informations système';
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmailBody', 'en-US', N'Name :
{0}

Company :
{1}

Email :
{2}

ID machine :
{3}

Software information:
  License : {4}
  Version : {5}

System information:
{6}', N'{0} est le nom, {1} la société, {2} l''email, {3} l''identifiant machine, {4} la license, {5} la version, {6} les informations système';
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmailBody', 'pt-BR', N'Nome:
{0}

Empresa:
{1}

E-mail:
{2}

ID da máquina:
{3}

Software information:
  License : {4}
  Version : {5}

System information:
{6}', N'{0} est le nom, {1} la société, {2} l''email, {3} l''identifiant machine, {4} la license, {5} la version, {6} les informations système';
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmailBody', 'es-ES', N'Nombre:
{0}

Empresa:
{1}

Email:
{2}

Identificador de la máquina:
{3}

Software information:
  License : {4}
  Version : {5}

System information:
{6}', N'{0} est le nom, {1} la société, {2} l''email, {3} l''identifiant machine, {4} la license, {5} la version, {6} les informations système';
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmailBody', 'de-DE', N'Name :
{0}

Firma :
{1}

Email :
{2}

ID Maschine :
{3}

Software Information:
  Lizenz : {4}
  Version : {5}

System Information:
{6}', N'{0} est le nom, {1} la société, {2} l''email, {3} l''identifiant machine, {4} la license, {5} la version, {6} les informations système';
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmailBody', 'pl-PL', N'Nazwisko :
{0}

Firma :
{1}

Email :
{2}

Numer identyfikacyjny :
{3}

Informacje o oprogramowaniu :
 Licencja : {4}
  Wersja : {5}

Informacja o systemie :
{6}', N'{0} est le nom, {1} la société, {2} l''email, {3} l''identifiant machine, {4} la license, {5} la version, {6} les informations système';
GO
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmailSubject', 'fr-FR', N'Activation de licence', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmailSubject', 'en-US', N'License activation', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmailSubject', 'pt-BR', N'Ativação de licença', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmailSubject', 'es-ES', N'Activación de la licencia', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmailSubject', 'de-DE', N'Lizenzaktivierung', null;
EXEC InsertOrUpdateResource 'View_ActivationView_KeyEmailSubject', 'pl-PL', N'Aktywacja licencji', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_MachineHash', 'fr-FR', N'Identifiant machine', null;
EXEC InsertOrUpdateResource 'View_ActivationView_MachineHash', 'en-US', N'ID machine', null;
EXEC InsertOrUpdateResource 'View_ActivationView_MachineHash', 'pt-BR', N'ID da máquina', null;
EXEC InsertOrUpdateResource 'View_ActivationView_MachineHash', 'es-ES', N'Identificador de la máquina', null;
EXEC InsertOrUpdateResource 'View_ActivationView_MachineHash', 'de-DE', N'ID Maschine', null;
EXEC InsertOrUpdateResource 'View_ActivationView_MachineHash', 'pl-PL', N'Numer identyfikacyjny', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_Message_BodySentToClipboard', 'fr-FR', N'Le contenu à envoyer a été copié dans le presse-papier.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_BodySentToClipboard', 'en-US', N'The content to send has been copied to the Clipboard.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_BodySentToClipboard', 'pt-BR', N'O conteúdo para enviar foi copiado na área de transferência.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_BodySentToClipboard', 'es-ES', N'El contenido que se envía se ha copiado en el portapapeles.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_BodySentToClipboard', 'de-DE', N'Der Sendeinhalt wurde in die Zwischenablage kopiert.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_BodySentToClipboard', 'pl-PL', N'Treść do wysłania została skopiowana do Clipboard.', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_Message_ErrorWhileActivating', 'fr-FR', N'Une erreur est survenue lors de l''activation de la clé.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_ErrorWhileActivating', 'en-US', N'An error occurred during activation of the key.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_ErrorWhileActivating', 'pt-BR', N'Ocorreu um erro durante a ativação da chave.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_ErrorWhileActivating', 'es-ES', N'Se produjo un error durante la activación de la clave.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_ErrorWhileActivating', 'de-DE', N'Ein Fehler ist während der Aktivierung des Schlüssels aufgetreten.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_ErrorWhileActivating', 'pl-PL', N'Podczas aktywacji klucza pojawił się błąd.', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_Message_ErrorWhileOpeningKeyFile', 'fr-FR', N'Une erreur est survenue lors de l''ouverture du fichier.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_ErrorWhileOpeningKeyFile', 'en-US', N'An error occurred while opening the file.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_ErrorWhileOpeningKeyFile', 'pt-BR', N'Ocorreu um erro ao abrir o arquivo.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_ErrorWhileOpeningKeyFile', 'es-ES', N'Se produjo un error al abrir el archivo.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_ErrorWhileOpeningKeyFile', 'de-DE', N'Fehler beim Öffnen der Datei.', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Message_ErrorWhileOpeningKeyFile', 'pl-PL', N'Podczas otwierania pliku pojawił się błąd.', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_Name', 'fr-FR', N'Nom', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Name', 'en-US', N'Name', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Name', 'pt-BR', N'Nome', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Name', 'es-ES', N'Nombre', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Name', 'de-DE', N'Name', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Name', 'pl-PL', N'Nazwisko', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Expired', 'fr-FR', N'Licence expirée', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Expired', 'en-US', N'License expired', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Expired', 'pt-BR', N'Licença expirada', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Expired', 'es-ES', N'Licencia vencida', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Expired', 'de-DE', N'Abgelaufene Lizenz', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Expired', 'pl-PL', N'Licencja wygasła', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Invalid', 'fr-FR', N'Licence invalide', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Invalid', 'en-US', N'License invalid', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Invalid', 'pt-BR', N'Licença inválida', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Invalid', 'es-ES', N'Licencia no válida', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Invalid', 'de-DE', N'Ungültige Lizenz', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Invalid', 'pl-PL', N'Licencja wadliwa', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Licenced', 'fr-FR', N'Licence activée', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Licenced', 'en-US', N'License enabled', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Licenced', 'pt-BR', N'Licença habilitada', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Licenced', 'es-ES', N'Licencia activada', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Licenced', 'de-DE', N'Aktivierte Lizenz', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Licenced', 'pl-PL', N'Licencja potwierdzona', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_Status_NoLicence', 'fr-FR', N'Aucune licence', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_NoLicence', 'en-US', N'No license', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_NoLicence', 'pt-BR', N'Nenhuma licença', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_NoLicence', 'es-ES', N'Sin licencia', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_NoLicence', 'de-DE', N'Keine Lizenz', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Status_NoLicence', 'pl-PL', N'Brak licencji', null;
GO
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Trial', 'fr-FR', N'Licence activée : {0} jours restants', N'{0} contient le nombre de jours restants';
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Trial', 'en-US', N'License enabled: {0} days remaining', N'{0} contient le nombre de jours restants';
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Trial', 'pt-BR', N'Licença habilitada: {0} dias restantes', N'{0} contient le nombre de jours restants';
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Trial', 'es-ES', N'Licencia activada: {0} días restantes', N'{0} contient le nombre de jours restants';
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Trial', 'de-DE', N'Lizenz aktivieren: {0} Tage verbleibend', N'{0} contient le nombre de jours restants';
EXEC InsertOrUpdateResource 'View_ActivationView_Status_Trial', 'pl-PL', N'Licencja potweirdzona : {0} dni przypomnienie', N'{0} contient le nombre de jours restants';
GO
EXEC InsertOrUpdateResource 'View_ActivationView_Title', 'fr-FR', N'Activation', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Title', 'en-US', N'Activation', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Title', 'pt-BR', N'Ativação', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Title', 'es-ES', N'Activación', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Title', 'de-DE', N'Aktivierung', null;
EXEC InsertOrUpdateResource 'View_ActivationView_Title', 'pl-PL', N'Aktywacja', null;
GO
EXEC InsertOrUpdateResource 'View_AdminReferentials_Color', 'fr-FR', N'Couleur', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Color', 'en-US', N'Color', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Color', 'pt-BR', N'Cor', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Color', 'es-ES', N'Color', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Color', 'de-DE', N'Farbe', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Color', 'pl-PL', N'Kolor jest błędny.', null;
GO
EXEC InsertOrUpdateResource 'View_AdminReferentials_Label', 'fr-FR', N'Libellé', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Label', 'en-US', N'Label', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Label', 'pt-BR', N'Rótulo', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Label', 'es-ES', N'Etiqueta', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Label', 'de-DE', N'Beschreibung', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Label', 'pl-PL', N'Etykieta', null;
GO
EXEC InsertOrUpdateResource 'View_AdminReferentials_Resources', 'fr-FR', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Resources', 'en-US', N'Resource', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Resources', 'pt-BR', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Resources', 'es-ES', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Resources', 'de-DE', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Resources', 'pl-PL', N'Zasoby', null;
GO
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'fr-FR', N'Adresse du fichier attaché', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'en-US', N'Path to linked file', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'pt-BR', N'Local do arquivo vinculado', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'es-ES', N'Ruta de archivo adjunto', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'de-DE', N'Pfad zur angehängten Datei', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri', 'pl-PL', N'Ścieżka do dołączonego pliku', null;
GO
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Browse', 'fr-FR', N'...', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Browse', 'en-US', N'…', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Browse', 'pt-BR', N'…', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Browse', 'es-ES', N'…', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Browse', 'de-DE', N'…', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Browse', 'pl-PL', N'…', null;
GO
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Browse_Caption', 'fr-FR', N'Sélectionnez un fichier', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Browse_Caption', 'en-US', N'Select a file', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Browse_Caption', 'pt-BR', N'Selecione um arquivo', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Browse_Caption', 'es-ES', N'Seleccione un archivo', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Browse_Caption', 'de-DE', N'Wählen Sie eine Datei', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Browse_Caption', 'pl-PL', N'Wybierz plik', null;
GO
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Open', 'fr-FR', N'Ouvrir', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Open', 'en-US', N'Open', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Open', 'pt-BR', N'Aberto', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Open', 'es-ES', N'Abierto', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Open', 'de-DE', N'Öffnen', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Open', 'pl-PL', N'Otwórz ', null;
GO
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Open_Error', 'fr-FR', N'Une erreur est survenue lors de l''ouverture du fichier.', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Open_Error', 'en-US', N'An error occurred while opening the file.', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Open_Error', 'pt-BR', N'Ocorreu um erro ao abrir o arquivo.', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Open_Error', 'es-ES', N'Se produjo un error al abrir el archivo.', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Open_Error', 'de-DE', N'Fehler beim Öffnen der Datei.', null;
EXEC InsertOrUpdateResource 'View_AdminReferentials_Uri_Open_Error', 'pl-PL', N'Podczas otwierania pliku pojawił się błąd.', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddAsChild', 'fr-FR', N'Créer une sous- tâche de celle sélectionnée', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddAsChild', 'en-US', N'Create a sub task of the selected task', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddAsChild', 'pt-BR', N'Criar uma sub-tarefa daquela selecionada', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddAsChild', 'es-ES', N'Crear sub-tarea de los seleccionados', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddAsChild', 'de-DE', N'Erzeugen einer Teilaufgabe der gewählten Aufgabe', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddAsChild', 'pl-PL', N'Utwórz podzadania dla wskazanego zadania', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddSameLevel', 'fr-FR', N'Créer une nouvelle tâche au niveau actuel', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddSameLevel', 'en-US', N'Create a new task at the current level', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddSameLevel', 'pt-BR', N'Criar uma nova tarefa no mesmo nível', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddSameLevel', 'es-ES', N'Crear una nueva tarea en el mismo nivel', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddSameLevel', 'de-DE', N'Erstellen Sie eine neue Aufgabe auf der gleichen Ebene', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddSameLevel', 'pl-PL', N'Utwórz nowe zadanie na obecnym poziomie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddAsProcess', 'fr-FR', N'Créer une tâche liée à un process', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddAsProcess', 'en-US', N'Create a task linked to a process', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddAsProcess', 'pt-BR', N'Create a task linked to a process', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddAsProcess', 'es-ES', N'Create a task linked to a process', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddAsProcess', 'de-DE', N'Create a task linked to a process', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_AddAsProcess', 'pl-PL', N'Create a task linked to a process', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Category', 'fr-FR', N'Catégorie', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Category', 'en-US', N'Category', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Category', 'pt-BR', N'Categoria', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Category', 'es-ES', N'Categoría', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Category', 'de-DE', N'Kategorie', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Category', 'pl-PL', N'Kategoria', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Category', 'fr-FR', N'Catégorie', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Category', 'en-US', N'Category', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Category', 'pt-BR', N'Categoria', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Category', 'es-ES', N'Categoría', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Category', 'de-DE', N'Kategorie', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Category', 'pl-PL', N'Kategoria', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Consumable', 'fr-FR', N'Consommable', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Consumable', 'en-US', N'Consumable', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Consumable', 'pt-BR', N'Consumíveis', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Consumable', 'es-ES', N'Consumible', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Consumable', 'de-DE', N'Verbrauchsmaterial', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Consumable', 'pl-PL', N'Eksploatacyjne', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Document', 'fr-FR', N'Document', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Document', 'en-US', N'Document', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Document', 'pt-BR', N'Documento', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Document', 'es-ES', N'Documento', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Document', 'de-DE', N'Dokument', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Document', 'pl-PL', N'Dokument', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Duration', 'fr-FR', N'Durée', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Duration', 'en-US', N'Duration', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Duration', 'pt-BR', N'Duração', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Duration', 'es-ES', N'Duración', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Duration', 'de-DE', N'Dauer', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Duration', 'pl-PL', N'Czas trwania', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Improvement', 'fr-FR', N'Amélioration', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Improvement', 'en-US', N'Improvement', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Improvement', 'pt-BR', N'Melhoria', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Improvement', 'es-ES', N'Mejora', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Improvement', 'de-DE', N'Verbesserung', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Improvement', 'pl-PL', N'Usprawnienie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_IsRandom', 'fr-FR', N'Aléa', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_IsRandom', 'en-US', N'Issue', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_IsRandom', 'pt-BR', N'Questão', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_IsRandom', 'es-ES', N'Asunto', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_IsRandom', 'de-DE', N'Gefahr', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_IsRandom', 'pl-PL', N'Kwestia', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Place', 'fr-FR', N'Lieu', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Place', 'en-US', N'Location', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Place', 'pt-BR', N'Localização', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Place', 'es-ES', N'Lugar', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Place', 'de-DE', N'Ort', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Place', 'pl-PL', N'Lokalizacja', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Predecessors', 'fr-FR', N'Prédécesseurs', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Predecessors', 'en-US', N'Predecessors', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Predecessors', 'pt-BR', N'Antecessores', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Predecessors', 'es-ES', N'Antecesores', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Predecessors', 'de-DE', N'Vorgänger', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Predecessors', 'pl-PL', N'Poprzednik', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Resource', 'fr-FR', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Resource', 'en-US', N'Resource', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Resource', 'pt-BR', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Resource', 'es-ES', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Resource', 'de-DE', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Resource', 'pl-PL', N'Zasoby', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Task', 'fr-FR', N'Tâche', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Task', 'en-US', N'Task', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Task', 'pt-BR', N'Tarefa', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Task', 'es-ES', N'Tarea', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Task', 'de-DE', N'Aufgabe', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Task', 'pl-PL', N'Zadanie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Tool', 'fr-FR', N'Outil', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Tool', 'en-US', N'Tool', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Tool', 'pt-BR', N'Ferramenta', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Tool', 'es-ES', N'Herramienta', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Tool', 'de-DE', N'Werkzeug', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Tool', 'pl-PL', N'Narzędzie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Video', 'fr-FR', N'Vidéo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Video', 'en-US', N'Video', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Video', 'pt-BR', N'Vídeo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Video', 'es-ES', N'Vídeo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Video', 'de-DE', N'Video', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_Video', 'pl-PL', N'Film', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_WBS', 'fr-FR', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_WBS', 'en-US', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_WBS', 'pt-BR', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_WBS', 'es-ES', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_WBS', 'de-DE', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Column_WBS', 'pl-PL', N'Lp', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Consumable', 'fr-FR', N'Consommable', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Consumable', 'en-US', N'Consumable', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Consumable', 'pt-BR', N'Consumíveis', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Consumable', 'es-ES', N'Consumible', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Consumable', 'de-DE', N'Verbrauchsmaterial', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Consumable', 'pl-PL', N'Eksploatacyjne', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Document', 'fr-FR', N'Document', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Document', 'en-US', N'Document', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Document', 'pt-BR', N'Documento', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Document', 'es-ES', N'Documento', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Document', 'de-DE', N'Dokument', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Document', 'pl-PL', N'Dokument', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Duration', 'fr-FR', N'Durée', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Duration', 'en-US', N'Duration', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Duration', 'pt-BR', N'Duração', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Duration', 'es-ES', N'Duración', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Duration', 'de-DE', N'Dauer', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Duration', 'pl-PL', N'Czas trwania', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Equipment', 'fr-FR', N'Equipement', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Equipment', 'en-US', N'Equipment', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Equipment', 'pt-BR', N'Equipamento', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Equipment', 'es-ES', N'Equipo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Equipment', 'de-DE', N'Ausrüstung', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Equipment', 'pl-PL', N'Sprzęt', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Finish', 'fr-FR', N'Fin', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Finish', 'en-US', N'Finish', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Finish', 'pt-BR', N'Fim', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Finish', 'es-ES', N'Final', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Finish', 'de-DE', N'Ende', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Finish', 'pl-PL', N'Koniec', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_IsRandom', 'fr-FR', N'Aléa', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_IsRandom', 'en-US', N'Issue', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_IsRandom', 'pt-BR', N'Questão', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_IsRandom', 'es-ES', N'Asunto', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_IsRandom', 'de-DE', N'Gefahr', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_IsRandom', 'pl-PL', N'Kwestia', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_IsKeyTask', 'fr-FR', N'Est une tâche importante', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_IsKeyTask', 'en-US', N'Is key task', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_IsKeyTask', 'pt-BR', N'Is key task', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_IsKeyTask', 'es-ES', N'Is key task', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_IsKeyTask', 'de-DE', N'Is key task', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_IsKeyTask', 'pl-PL', N'Is key task', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Label', 'fr-FR', N'Tâche', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Label', 'en-US', N'Task', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Label', 'pt-BR', N'Tarefa', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Label', 'es-ES', N'Tarea', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Label', 'de-DE', N'Aufgabe', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Label', 'pl-PL', N'Zadanie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_NewReferential', 'fr-FR', N'(Nouveau)', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_NewReferential', 'en-US', N'(New)', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_NewReferential', 'pt-BR', N'(New)', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_NewReferential', 'es-ES', N'(Nuevo)', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_NewReferential', 'de-DE', N'(Neu)', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_NewReferential', 'pl-PL', N'(Nowy)', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_NoVideo', 'fr-FR', N'Sans vidéo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_NoVideo', 'en-US', N'Without video', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_NoVideo', 'pt-BR', N'Sem vídeo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_NoVideo', 'es-ES', N'No hay vídeo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_NoVideo', 'de-DE', N'ohne Video', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_NoVideo', 'pl-PL', N'Bez filmu', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_OnTheFlyInput', 'fr-FR', N'Auto-pause', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_OnTheFlyInput', 'en-US', N'Auto-pause', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_OnTheFlyInput', 'pt-BR', N'Auto-pausa', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_OnTheFlyInput', 'es-ES', N'Auto-pausa', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_OnTheFlyInput', 'de-DE', N'Auto-Pause', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_OnTheFlyInput', 'pl-PL', N'Auto-pauza', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Operator', 'fr-FR', N'Opérateur', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Operator', 'en-US', N'Operator', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Operator', 'pt-BR', N'Operador', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Operator', 'es-ES', N'Operador', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Operator', 'de-DE', N'Operator', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Operator', 'pl-PL', N'Operator', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Place', 'fr-FR', N'Lieu', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Place', 'en-US', N'Location', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Place', 'pt-BR', N'Localização', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Place', 'es-ES', N'Lugar', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Place', 'de-DE', N'Ort', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Place', 'pl-PL', N'Lokalizacja', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Start', 'fr-FR', N'Début', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Start', 'en-US', N'Start', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Start', 'pt-BR', N'Inicio', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Start', 'es-ES', N'Comienzo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Start', 'de-DE', N'Anfang', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Start', 'pl-PL', N'Start', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Tool', 'fr-FR', N'Outil', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Tool', 'en-US', N'Tool', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Tool', 'pt-BR', N'Ferramenta', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Tool', 'es-ES', N'Herramienta', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Tool', 'de-DE', N'Werkzeug', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_Tool', 'pl-PL', N'Narzędzie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_WBS', 'fr-FR', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_WBS', 'en-US', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_WBS', 'pt-BR', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_WBS', 'es-ES', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_WBS', 'de-DE', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeAcquire_WBS', 'pl-PL', N'Lp', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action', 'fr-FR', N'Tâche', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action', 'en-US', N'Task', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action', 'pt-BR', N'Tarefa', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action', 'es-ES', N'Tarea', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action', 'de-DE', N'Aufgabe', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action', 'pl-PL', N'Zadanie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_ActionValue', 'fr-FR', N'Valorisation', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_ActionValue', 'en-US', N'Value', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_ActionValue', 'pt-BR', N'Valor', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_ActionValue', 'es-ES', N'Valor', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_ActionValue', 'de-DE', N'Entwicklung', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_ActionValue', 'pl-PL', N'Wartość ', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Category', 'fr-FR', N'Catégorie', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Category', 'en-US', N'Category', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Category', 'pt-BR', N'Categoria', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Category', 'es-ES', N'Categoría', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Category', 'de-DE', N'Kategorie', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Category', 'pl-PL', N'Kategoria', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Consumable', 'fr-FR', N'Consommable', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Consumable', 'en-US', N'Consumable', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Consumable', 'pt-BR', N'Consumível', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Consumable', 'es-ES', N'Consumible', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Consumable', 'de-DE', N'Verbrauchsmaterial', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Consumable', 'pl-PL', N'Eksploatacyjne', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Details', 'fr-FR', N'Détails', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Details', 'en-US', N'Details', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Details', 'pt-BR', N'Detalhes', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Details', 'es-ES', N'Detalles', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Details', 'de-DE', N'Details', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Details', 'pl-PL', N'Detale', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Document', 'fr-FR', N'Document', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Document', 'en-US', N'Document', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Document', 'pt-BR', N'Documento', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Document', 'es-ES', N'Documento', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Document', 'de-DE', N'Dokument', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Document', 'pl-PL', N'Dokument', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Duration', 'fr-FR', N'Durée', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Duration', 'en-US', N'Duration', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Duration', 'pt-BR', N'Duração', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Duration', 'es-ES', N'Duración', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Duration', 'de-DE', N'Dauer', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Duration', 'pl-PL', N'Czas trwania', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Finish', 'fr-FR', N'Fin', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Finish', 'en-US', N'Finish', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Finish', 'pt-BR', N'Fim', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Finish', 'es-ES', N'Final', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Finish', 'de-DE', N'Ende', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Finish', 'pl-PL', N'Koniec', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_IsRandom', 'fr-FR', N'Aléa', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_IsRandom', 'en-US', N'Issue', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_IsRandom', 'pt-BR', N'Questão', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_IsRandom', 'es-ES', N'Peligro', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_IsRandom', 'de-DE', N'Gefahr', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_IsRandom', 'pl-PL', N'Kwestia', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Label', 'fr-FR', N'Libellé', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Label', 'en-US', N'Label', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Label', 'pt-BR', N'Rótulo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Label', 'es-ES', N'Etiqueta', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Label', 'de-DE', N'Beschreibung', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Label', 'pl-PL', N'Etykieta', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Place', 'fr-FR', N'Lieu', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Place', 'en-US', N'Location', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Place', 'pt-BR', N'Localização', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Place', 'es-ES', N'Lugar', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Place', 'de-DE', N'Ort', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Place', 'pl-PL', N'Lokalo', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Resource', 'fr-FR', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Resource', 'en-US', N'Resource', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Resource', 'pt-BR', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Resource', 'es-ES', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Resource', 'de-DE', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Resource', 'pl-PL', N'Zasoby', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Start', 'fr-FR', N'Début', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Start', 'en-US', N'Start', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Start', 'pt-BR', N'Inicio', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Start', 'es-ES', N'Comienzo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Start', 'de-DE', N'Anfang', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Start', 'pl-PL', N'Start', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Tool', 'fr-FR', N'Outil', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Tool', 'en-US', N'Tool', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Tool', 'pt-BR', N'Ferramenta', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Tool', 'es-ES', N'Herramienta', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Tool', 'de-DE', N'Werkzeug', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_Tool', 'pl-PL', N'Narzędzie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_WBS', 'fr-FR', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_WBS', 'en-US', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_WBS', 'pt-BR', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_WBS', 'es-ES', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_WBS', 'de-DE', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Action_WBS', 'pl-PL', N'Lp', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Approved', 'fr-FR', N'OK', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Approved', 'en-US', N'OK', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Approved', 'pt-BR', N'OK', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Approved', 'es-ES', N'OK', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Approved', 'de-DE', N'OK', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Approved', 'pl-PL', N'OK', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Difficulty', 'fr-FR', N'Difficulté', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Difficulty', 'en-US', N'Difficulty', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Difficulty', 'pt-BR', N'Dificuldade', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Difficulty', 'es-ES', N'Dificultad', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Difficulty', 'de-DE', N'Schwierigkeit', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Difficulty', 'pl-PL', N'Trudność', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Externalized', 'fr-FR', N'Externalisée', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Externalized', 'en-US', N'Externalized', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Externalized', 'pt-BR', N'Externalizado', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Externalized', 'es-ES', N'Externalizado', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Externalized', 'de-DE', N'Extern', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Externalized', 'pl-PL', N'Zewnętrzne', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Internalized', 'fr-FR', N'Interne', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Internalized', 'en-US', N'Internal', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Internalized', 'pt-BR', N'Interno', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Internalized', 'es-ES', N'Interno', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Internalized', 'de-DE', N'Intern', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Internalized', 'pl-PL', N'Wewnętrzne', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Investment', 'fr-FR', N'Investissement', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Investment', 'en-US', N'Investment', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Investment', 'pt-BR', N'Investimento', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Investment', 'es-ES', N'Inversión', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Investment', 'de-DE', N'Investition', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Investment', 'pl-PL', N'Inwestycja', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_NotApproved', 'fr-FR', N'NOK', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_NotApproved', 'en-US', N'NOK', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_NotApproved', 'pt-BR', N'NOK', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_NotApproved', 'es-ES', N'NOK', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_NotApproved', 'de-DE', N'NOK', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_NotApproved', 'pl-PL', N'NOK', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_PercentageReduction', 'fr-FR', N'% Réduction', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_PercentageReduction', 'en-US', N'% Reduction', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_PercentageReduction', 'pt-BR', N'% de redução', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_PercentageReduction', 'es-ES', N'% De reducción', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_PercentageReduction', 'de-DE', N'% Reduzierung', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_PercentageReduction', 'pl-PL', N'% Redukcji', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Solution', 'fr-FR', N'Solution', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Solution', 'en-US', N'Solution', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Solution', 'pt-BR', N'Solução', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Solution', 'es-ES', N'Solución', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Solution', 'de-DE', N'Lösung', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ActionImproved_Solution', 'pl-PL', N'Rozwiązanie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_All', 'fr-FR', N'Tous', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_All', 'en-US', N'All', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_All', 'pt-BR', N'Todos', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_All', 'es-ES', N'Todos', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_All', 'de-DE', N'Alle', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_All', 'pl-PL', N'Wszystkie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improve', 'fr-FR', N'Améliorer', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improve', 'en-US', N'Improve', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improve', 'pt-BR', N'Melhorar', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improve', 'es-ES', N'Mejorar', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improve', 'de-DE', N'Verbessern', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improve', 'pl-PL', N'Usprawniać', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improvement', 'fr-FR', N'Amélioration', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improvement', 'en-US', N'Improvement', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improvement', 'pt-BR', N'Melhoria', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improvement', 'es-ES', N'Mejora', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improvement', 'de-DE', N'Verbesserung', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improvement', 'pl-PL', N'Usprawnienie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improvement_Past', 'fr-FR', N'I/E', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improvement_Past', 'en-US', N'I/E', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improvement_Past', 'pt-BR', N'I/E', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improvement_Past', 'es-ES', N'I/E', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improvement_Past', 'de-DE', N'I/E', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Improvement_Past', 'pl-PL', N'Wew/Zew', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayAction', 'fr-FR', N'Lire la tâche', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayAction', 'en-US', N'Read task sequence', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayAction', 'pt-BR', N'Ler a tarefa', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayAction', 'es-ES', N'Leer la secuencia de tareas', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayAction', 'de-DE', N'Lesen Sie die Aufgabe', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayAction', 'pl-PL', N'Przeczytaj kolejność zadań', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayAction_Short', 'fr-FR', N'Tâche', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayAction_Short', 'en-US', N'Task', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayAction_Short', 'pt-BR', N'Tarefa', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayAction_Short', 'es-ES', N'Tarea', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayAction_Short', 'de-DE', N'Aufgabe', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayAction_Short', 'pl-PL', N'Zadanie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayCriticalPath', 'fr-FR', N'Lire le chemin critique', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayCriticalPath', 'en-US', N'Read critical path video', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayCriticalPath', 'pt-BR', N'Ler o caminho crítico', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayCriticalPath', 'es-ES', N'Lea la ruta crítica', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayCriticalPath', 'de-DE', N'Laden Sie den kritischen Pfad', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayCriticalPath', 'pl-PL', N'Przeczytaj ścieżkę krytyczną', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayCriticalPath_Short', 'fr-FR', N'Chemin Critique', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayCriticalPath_Short', 'en-US', N'Critical Path', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayCriticalPath_Short', 'pt-BR', N'Caminho crítico', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayCriticalPath_Short', 'es-ES', N'Ruta Crítica', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayCriticalPath_Short', 'de-DE', N'Kritischer Pfad', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_PlayCriticalPath_Short', 'pl-PL', N'Ścieżka krytyczna', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Referentials', 'fr-FR', N'Référentiel', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Referentials', 'en-US', N'Referential', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Referentials', 'pt-BR', N'Referenciais', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Referentials', 'es-ES', N'Referencia', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Referentials', 'de-DE', N'Referenzen', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Referentials', 'pl-PL', N'Odniesienie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Resource', 'fr-FR', N'Lire la ressource', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Resource', 'en-US', N'Read resource video', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Resource', 'pt-BR', N'Ler a vídeo de recursos', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Resource', 'es-ES', N'Lea el Recurso', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Resource', 'de-DE', N'Laden Sie die Video Ressource', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Resource', 'pl-PL', N'Przeczytaj zasoby', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Resource_Short', 'fr-FR', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Resource_Short', 'en-US', N'Resource', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Resource_Short', 'pt-BR', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Resource_Short', 'es-ES', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Resource_Short', 'de-DE', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Resource_Short', 'pl-PL', N'Zasoby', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Create', 'fr-FR', N'Créer un nouveau scénario', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Create', 'en-US', N'Create a new scenario', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Create', 'pt-BR', N'Criar um novo cenário', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Create', 'es-ES', N'Crear un nuevo escenario', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Create', 'de-DE', N'Erstellen Sie ein neues Szenario', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Create', 'pl-PL', N'Utwórz nowy scenariusz', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Description', 'fr-FR', N'Description', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Description', 'en-US', N'Description', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Description', 'pt-BR', N'Descrição', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Description', 'es-ES', N'Descripción', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Description', 'de-DE', N'Beschreibung', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Description', 'pl-PL', N'Opis', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Label', 'fr-FR', N'Libellé', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Label', 'en-US', N'Label', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Label', 'pt-BR', N'Rótulo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Label', 'es-ES', N'Etiqueta', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Label', 'de-DE', N'Beschreibung', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_Label', 'pl-PL', N'Etykieta', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_OriginalScenario', 'fr-FR', N'Scénario d''origine', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_OriginalScenario', 'en-US', N'Original scenario', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_OriginalScenario', 'pt-BR', N'Cenário inicial', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_OriginalScenario', 'es-ES', N'Guión Original', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_OriginalScenario', 'de-DE', N'Original-Szenario', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_OriginalScenario', 'pl-PL', N'Scenariusz oryginalny', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_State', 'fr-FR', N'Etat', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_State', 'en-US', N'State', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_State', 'pt-BR', N'Estado', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_State', 'es-ES', N'Estado', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_State', 'de-DE', N'Zustand', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Scenario_State', 'pl-PL', N'Stan', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_TotalDuration', 'fr-FR', N'Durée totale du processus : {0}', N'{0} contient la durée';
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_TotalDuration', 'en-US', N'Total process duration: {0}', N'{0} contient la durée';
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_TotalDuration', 'pt-BR', N'Duração total do processo: {0}', N'{0} contient la durée';
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_TotalDuration', 'es-ES', N'Duracion total del procedimiento: {0}', N'{0} contient la durée';
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_TotalDuration', 'de-DE', N'Insgesamte Prozessdauer: {0}', N'{0} contient la durée';
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_TotalDuration', 'pl-PL', N'Całkowity czas trwania : {0}', N'{0} contient la durée';
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ViewSolutions', 'fr-FR', N'Solutions', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ViewSolutions', 'en-US', N'Solutions', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ViewSolutions', 'pt-BR', N'Soluções', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ViewSolutions', 'es-ES', N'Soluciones', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ViewSolutions', 'de-DE', N'Lösungen', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_ViewSolutions', 'pl-PL', N'Rozwiązania', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ChangeVideo', 'fr-FR', N'Changer de fichier vidéo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ChangeVideo', 'en-US', N'Change video file', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ChangeVideo', 'pt-BR', N'Mudar arquivo de vídeo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ChangeVideo', 'es-ES', N'Cambiar el archivo video', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ChangeVideo', 'de-DE', N'Anderes Video auswählen', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ChangeVideo', 'pl-PL', N'Znień plik z filmem', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_CollapseAll', 'fr-FR', N'Tout réduire', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_CollapseAll', 'en-US', N'Collapse all', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_CollapseAll', 'pt-BR', N'Reduzir tudo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_CollapseAll', 'es-ES', N'Reducir todo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_CollapseAll', 'de-DE', N'Alles minimieren', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_CollapseAll', 'pl-PL', N'Zwiń wszystko', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ExpandAll', 'fr-FR', N'Tout étendre', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ExpandAll', 'en-US', N'Extend all', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ExpandAll', 'pt-BR', N'Estender tudo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ExpandAll', 'es-ES', N'Expandir todo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ExpandAll', 'de-DE', N'Alles erweitern', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ExpandAll', 'pl-PL', N'Rozwiń wszystko', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterI', 'fr-FR', N'I', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterI', 'en-US', N'I', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterI', 'pt-BR', N'I', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterI', 'es-ES', N'I', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterI', 'de-DE', N'I', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterI', 'pl-PL', N'W', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterIE', 'fr-FR', N'I/E', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterIE', 'en-US', N'I/E', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterIE', 'pt-BR', N'I / E', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterIE', 'es-ES', N'I / E', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterIE', 'de-DE', N'I / E', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterIE', 'pl-PL', N'W/Z', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterIES', 'fr-FR', N'I/E/S', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterIES', 'en-US', N'I/E/D', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterIES', 'pt-BR', N'I / E / X', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterIES', 'es-ES', N'I / E / B', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterIES', 'de-DE', N'I / E / F', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_FilterIES', 'pl-PL', N'W/Z/U', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Group', 'fr-FR', N'Grouper', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Group', 'en-US', N'Group', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Group', 'pt-BR', N'Agrupar', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Group', 'es-ES', N'Agrupar', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Group', 'de-DE', N'gruppieren', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Group', 'pl-PL', N'Grupuj', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Indent', 'fr-FR', N'Not used', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Indent', 'en-US', N'Not used', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Indent', 'pt-BR', N'Not used', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Indent', 'es-ES', N'Not used', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Indent', 'de-DE', N'Not used', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Indent', 'pl-PL', N'Nieużywane', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_MoveDown', 'fr-FR', N'Descendre', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_MoveDown', 'en-US', N'Down', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_MoveDown', 'pt-BR', N'Descer', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_MoveDown', 'es-ES', N'Bajar', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_MoveDown', 'de-DE', N'runter', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_MoveDown', 'pl-PL', N'Na dół', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_MoveUp', 'fr-FR', N'Monter', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_MoveUp', 'en-US', N'Up', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_MoveUp', 'pt-BR', N'Subir', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_MoveUp', 'es-ES', N'Subir', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_MoveUp', 'de-DE', N'hoch', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_MoveUp', 'pl-PL', N'Do góry', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ShowPredecessorsLinks', 'fr-FR', N'Afficher/masquer les liens entre les tâches', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ShowPredecessorsLinks', 'en-US', N'Display/hide links between tasks', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ShowPredecessorsLinks', 'pt-BR', N'Exibir / ocultar links entre tarefas', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ShowPredecessorsLinks', 'es-ES', N'Mostrar/ocultar las conexiones entre las tareas', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ShowPredecessorsLinks', 'de-DE', N'Die Verbindungen zwischen Aufgaben ein-/ausblenden', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ShowPredecessorsLinks', 'pl-PL', N'Wyświetl/ukryj połączenia między zadaniami', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Ungroup', 'fr-FR', N'Dégrouper', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Ungroup', 'en-US', N'Ungroup', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Ungroup', 'pt-BR', N'Desagrupar', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Ungroup', 'es-ES', N'Desagregar', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Ungroup', 'de-DE', N'Nicht gruppieren', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Ungroup', 'pl-PL', N'Rozgrupuj', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Unindent', 'fr-FR', N'Not used', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Unindent', 'en-US', N'Not used', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Unindent', 'pt-BR', N'Not used', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Unindent', 'es-ES', N'Not used', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Unindent', 'de-DE', N'Not used', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Unindent', 'pl-PL', N'Nieużywane', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewBy', 'fr-FR', N'Vue par :', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewBy', 'en-US', N'View by:', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewBy', 'pt-BR', N'Visto por:', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewBy', 'es-ES', N'Visto por:', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewBy', 'de-DE', N'Anzeigen über:', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewBy', 'pl-PL', N'Widok przez:', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByCategory', 'fr-FR', N'Catégorie', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByCategory', 'en-US', N'Category', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByCategory', 'pt-BR', N'Categoria', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByCategory', 'es-ES', N'Categoría', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByCategory', 'de-DE', N'Kategorie', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByCategory', 'pl-PL', N'Kategoria', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByConsumable', 'fr-FR', N'Consommable', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByConsumable', 'en-US', N'Consumable', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByConsumable', 'pt-BR', N'Consumíveis', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByConsumable', 'es-ES', N'Consumible', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByConsumable', 'de-DE', N'Verbrauchsmaterial', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByConsumable', 'pl-PL', N'Eksploatacyjne', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByDocument', 'fr-FR', N'Document', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByDocument', 'en-US', N'Document', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByDocument', 'pt-BR', N'Documento', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByDocument', 'es-ES', N'Documento', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByDocument', 'de-DE', N'Dokument', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByDocument', 'pl-PL', N'Dokumet', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByPlace', 'fr-FR', N'Lieu', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByPlace', 'en-US', N'Location', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByPlace', 'pt-BR', N'Localização', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByPlace', 'es-ES', N'Lugar', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByPlace', 'de-DE', N'Ort', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByPlace', 'pl-PL', N'Lokalizacja', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByResource', 'fr-FR', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByResource', 'en-US', N'Resource', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByResource', 'pt-BR', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByResource', 'es-ES', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByResource', 'de-DE', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByResource', 'pl-PL', N'Zasoby', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByTool', 'fr-FR', N'Outil', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByTool', 'en-US', N'Tool', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByTool', 'pt-BR', N'Ferramenta', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByTool', 'es-ES', N'Herramienta', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByTool', 'de-DE', N'Werkzeug', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByTool', 'pl-PL', N'Narzędzie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByWBS', 'fr-FR', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByWBS', 'en-US', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByWBS', 'pt-BR', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByWBS', 'es-ES', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByWBS', 'de-DE', N'ID', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ViewByWBS', 'pl-PL', N'Lp', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoX', 'fr-FR', N'Zoom auto horizontal', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoX', 'en-US', N'Horizontal auto zoom', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoX', 'pt-BR', N'Auto zoom horizontal', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoX', 'es-ES', N'Zoom auto horizontal', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoX', 'de-DE', N'Waagerechter automatischer Zoom', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoX', 'pl-PL', N'Poziome auto powiększenie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoXReverse', 'fr-FR', N'Zoom vertical initial', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoXReverse', 'en-US', N'Initial vertical zoom', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoXReverse', 'pt-BR', N'Zoom vertical inicial', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoXReverse', 'es-ES', N'Zoom vertical initial', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoXReverse', 'de-DE', N'Initialer senkrechter Zoom', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoXReverse', 'pl-PL', N'Początkowe pionowe powiększenie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoXY', 'fr-FR', N'Zoom auto horizontal et vertical', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoXY', 'en-US', N'Horizontal and vertical auto zoom', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoXY', 'pt-BR', N'Auto zoom vertical e horizonal', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoXY', 'es-ES', N'Zoom auto horizontal y vertical', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoXY', 'de-DE', N'Waagerechter und senkrechter automatischer Zoom', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoXY', 'pl-PL', N'Poziome i pionowe auto powiększenie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoY', 'fr-FR', N'Zoom auto vertical', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoY', 'en-US', N'Vertical auto zoom', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoY', 'pt-BR', N'Auto zoom vertical', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoY', 'es-ES', N'Zoom auto vertical', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoY', 'de-DE', N'Senkrechter automatischer Zoom', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_ZoomAutoY', 'pl-PL', N'Pionowe auto powiększenie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_AdditionalScenarios', 'fr-FR', N'Scénarios supplémentaires :', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_AdditionalScenarios', 'en-US', N'Additional scenarios:', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_AdditionalScenarios', 'pt-BR', N'Cenários adicionais:', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_AdditionalScenarios', 'es-ES', N'Escenarios adicionales:', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_AdditionalScenarios', 'de-DE', N'Zusätzliche Szenarien:', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_AdditionalScenarios', 'pl-PL', N'Dodatkowe scenariusze:', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Consumables_Title', 'fr-FR', N'Consommables', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Consumables_Title', 'en-US', N'Consumables', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Consumables_Title', 'pt-BR', N'Consumíveis', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Consumables_Title', 'es-ES', N'Consumibles', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Consumables_Title', 'de-DE', N'Verbrauchsmaterial', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Consumables_Title', 'pl-PL', N'Eksploatacyjne', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Documents_Title', 'fr-FR', N'Documents', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Documents_Title', 'en-US', N'Documents', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Documents_Title', 'pt-BR', N'Documentos', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Documents_Title', 'es-ES', N'Documentos', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Documents_Title', 'de-DE', N'Unterlagen', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Documents_Title', 'pl-PL', N'Dokumenty', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_ExportToExcel', 'fr-FR', N'Exporter vers Excel', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_ExportToExcel', 'en-US', N'Export to Excel', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_ExportToExcel', 'pt-BR', N'Exportação para Excel', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_ExportToExcel', 'es-ES', N'Exportación a Excel', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_ExportToExcel', 'de-DE', N'Export nach Excel', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_ExportToExcel', 'pl-PL', N'Eksportuj do excela', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Places_Title', 'fr-FR', N'Lieux', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Places_Title', 'en-US', N'Locations', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Places_Title', 'pt-BR', N'Locais', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Places_Title', 'es-ES', N'Lugares', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Places_Title', 'de-DE', N'Orte', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Places_Title', 'pl-PL', N'Lokalizacje', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Scenarios', 'fr-FR', N'Scénarios', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Scenarios', 'en-US', N'Scenarios', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Scenarios', 'pt-BR', N'Cenários', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Scenarios', 'es-ES', N'Escenarios', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Scenarios', 'de-DE', N'Szenarien', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Scenarios', 'pl-PL', N'Scenariusze', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Solution_Title', 'fr-FR', N'Solutions', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Solution_Title', 'en-US', N'Solutions', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Solution_Title', 'pt-BR', N'Soluções', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Solution_Title', 'es-ES', N'Soluciones', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Solution_Title', 'de-DE', N'Lösungen', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Solution_Title', 'pl-PL', N'Rozwiązanie', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Tools_Title', 'fr-FR', N'Outils', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Tools_Title', 'en-US', N'Tools', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Tools_Title', 'pt-BR', N'Ferramentas', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Tools_Title', 'es-ES', N'Herramientas', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Tools_Title', 'de-DE', N'Werkzeuge', null;
EXEC InsertOrUpdateResource 'View_AnalyzeRestitution_Tools_Title', 'pl-PL', N'Narzędzie', null;
GO
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionType', 'fr-FR', N'Type par défaut', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionType', 'en-US', N'Default type', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionType', 'pt-BR', N'Tipo padrão', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionType', 'es-ES', N'Por defecto de tipo', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionType', 'de-DE', N'Standard-Typ', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionType', 'pl-PL', N'Rodzaj czynności', null;
GO
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionType_None', 'fr-FR', N'(Aucun)', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionType_None', 'en-US', N'(None)', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionType_None', 'pt-BR', N'(Nenhum)', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionType_None', 'es-ES', N'(Ninguno)', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionType_None', 'de-DE', N'(Keine)', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionType_None', 'pl-PL', N'(Żadny)', null;
GO
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionValue', 'fr-FR', N'Valorisation', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionValue', 'en-US', N'Value', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionValue', 'pt-BR', N'Valor', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionValue', 'es-ES', N'Valorizacion', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionValue', 'de-DE', N'Wert', null;
EXEC InsertOrUpdateResource 'View_AppActionCategories_ActionValue', 'pl-PL', N'Wartość', null;
GO
EXEC InsertOrUpdateResource 'View_DefaultResource_None', 'fr-FR', N'(Aucune)', null;
EXEC InsertOrUpdateResource 'View_DefaultResource_None', 'en-US', N'(None)', null;
EXEC InsertOrUpdateResource 'View_DefaultResource_None', 'pt-BR', N'(Nenhum)', null;
EXEC InsertOrUpdateResource 'View_DefaultResource_None', 'es-ES', N'(Ninguno)', null;
EXEC InsertOrUpdateResource 'View_DefaultResource_None', 'de-DE', N'(Keine)', null;
EXEC InsertOrUpdateResource 'View_DefaultResource_None', 'pl-PL', N'(Żadny)', null;
GO
EXEC InsertOrUpdateResource 'View_ApplicationMembers_ConfirmNewPassword', 'fr-FR', N'Confirmez le nouveau mot de passe', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_ConfirmNewPassword', 'en-US', N'Confirm the new password', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_ConfirmNewPassword', 'pt-BR', N'Confirme a nova senha', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_ConfirmNewPassword', 'es-ES', N'Confirmar nueva contraseña', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_ConfirmNewPassword', 'de-DE', N'Neues Passwort bestätigen', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_ConfirmNewPassword', 'pl-PL', N'Potwierdź nowe hasło', null;
GO
EXEC InsertOrUpdateResource 'View_ApplicationMembers_DefaultLanguage', 'fr-FR', N'Langue par défaut', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_DefaultLanguage', 'en-US', N'Default language', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_DefaultLanguage', 'pt-BR', N'Idioma padrão', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_DefaultLanguage', 'es-ES', N'Lenguaje por defecto', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_DefaultLanguage', 'de-DE', N'Standard-Sprache', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_DefaultLanguage', 'pl-PL', N'Język naruszenia', null;
GO
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Details', 'fr-FR', N'Détails', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Details', 'en-US', N'Details', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Details', 'pt-BR', N'Detalhes', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Details', 'es-ES', N'Detalles', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Details', 'de-DE', N'Details', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Details', 'pl-PL', N'Detale', null;
GO
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Email', 'fr-FR', N'Adresse e-mail', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Email', 'en-US', N'E-mail address', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Email', 'pt-BR', N'E-mail', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Email', 'es-ES', N'E-mail', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Email', 'de-DE', N'E-Mail', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Email', 'pl-PL', N'Adres email', null;
GO
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Firstname', 'fr-FR', N'Prénom', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Firstname', 'en-US', N'First name', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Firstname', 'pt-BR', N'Nome', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Firstname', 'es-ES', N'Nombre', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Firstname', 'de-DE', N'Vorname', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Firstname', 'pl-PL', N'Imię', null;
GO
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Name', 'fr-FR', N'Nom', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Name', 'en-US', N'Last name', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Name', 'pt-BR', N'Sobrenome', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Name', 'es-ES', N'Apellido', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Name', 'de-DE', N'Nachname', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Name', 'pl-PL', N'Nazwisko', null;
GO
EXEC InsertOrUpdateResource 'View_ApplicationMembers_NewPassword', 'fr-FR', N'Nouveau mot de passe', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_NewPassword', 'en-US', N'New password', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_NewPassword', 'pt-BR', N'Nova senha', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_NewPassword', 'es-ES', N'Nueva contraseña', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_NewPassword', 'de-DE', N'Neues Passwort', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_NewPassword', 'pl-PL', N'Nowe hasło', null;
GO
EXEC InsertOrUpdateResource 'View_ApplicationMembers_PhoneNumber', 'fr-FR', N'Numéro de téléphone', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_PhoneNumber', 'en-US', N'Phone number', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_PhoneNumber', 'pt-BR', N'Número de telefone', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_PhoneNumber', 'es-ES', N'Número de teléfono', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_PhoneNumber', 'de-DE', N'Telefonnummer', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_PhoneNumber', 'pl-PL', N'Numer telefonu', null;
GO
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Roles', 'fr-FR', N'Rôles', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Roles', 'en-US', N'Roles', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Roles', 'pt-BR', N'Papéis', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Roles', 'es-ES', N'Roles', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Roles', 'de-DE', N'Funktion', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Roles', 'pl-PL', N'Role', null;
GO
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Username', 'fr-FR', N'Identifiant', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Username', 'en-US', N'ID', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Username', 'pt-BR', N'ID', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Username', 'es-ES', N'ID', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Username', 'de-DE', N'Login', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Username', 'pl-PL', N'Lp', null;
GO
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Users', 'fr-FR', N'Utilisateurs', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Users', 'en-US', N'Users', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Users', 'pt-BR', N'Usuários', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Users', 'es-ES', N'Usuarios', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Users', 'de-DE', N'Benutzer', null;
EXEC InsertOrUpdateResource 'View_ApplicationMembers_Users', 'pl-PL', N'Użytkownicy', null;
GO
EXEC InsertOrUpdateResource 'View_ApplicationUsers_Add_ToolTip', 'fr-FR', N'Ajouter un utilisateur', null;
EXEC InsertOrUpdateResource 'View_ApplicationUsers_Add_ToolTip', 'en-US', N'Add a user', null;
EXEC InsertOrUpdateResource 'View_ApplicationUsers_Add_ToolTip', 'pt-BR', N'Adicionar um usuário', null;
EXEC InsertOrUpdateResource 'View_ApplicationUsers_Add_ToolTip', 'es-ES', N'Agregar un usuario', null;
EXEC InsertOrUpdateResource 'View_ApplicationUsers_Add_ToolTip', 'de-DE', N'Benutzer hinzufügen', null;
EXEC InsertOrUpdateResource 'View_ApplicationUsers_Add_ToolTip', 'pl-PL', N'Dodaj nowego użtkownika', null;
GO
EXEC InsertOrUpdateResource 'View_ApplicationUsers_Remove_ToolTip', 'fr-FR', N'Supprimer l''utilisateur sélectionné', null;
EXEC InsertOrUpdateResource 'View_ApplicationUsers_Remove_ToolTip', 'en-US', N'Delete the selected user', null;
EXEC InsertOrUpdateResource 'View_ApplicationUsers_Remove_ToolTip', 'pt-BR', N'Apagar o usuário selecionado', null;
EXEC InsertOrUpdateResource 'View_ApplicationUsers_Remove_ToolTip', 'es-ES', N'Eliminar el usuario seleccionado', null;
EXEC InsertOrUpdateResource 'View_ApplicationUsers_Remove_ToolTip', 'de-DE', N'Löschen des ausgewählten Benutzer', null;
EXEC InsertOrUpdateResource 'View_ApplicationUsers_Remove_ToolTip', 'pl-PL', N'Usuń wybranego użytkownika', null;
GO
EXEC InsertOrUpdateResource 'View_AppReferentials_Remove_Tooltip', 'fr-FR', N'Supprimer l''élément sélectionné', null;
EXEC InsertOrUpdateResource 'View_AppReferentials_Remove_Tooltip', 'en-US', N'Delete the selected item', null;
EXEC InsertOrUpdateResource 'View_AppReferentials_Remove_Tooltip', 'pt-BR', N'Excluir o item selecionados', null;
EXEC InsertOrUpdateResource 'View_AppReferentials_Remove_Tooltip', 'es-ES', N'Eliminar el elemento seleccionado', null;
EXEC InsertOrUpdateResource 'View_AppReferentials_Remove_Tooltip', 'de-DE', N'Löschen Sie das ausgewählte Element', null;
EXEC InsertOrUpdateResource 'View_AppReferentials_Remove_Tooltip', 'pl-PL', N'Usuń wybraną pozycję', null;
GO
EXEC InsertOrUpdateResource 'View_AppResources_PaceRating', 'fr-FR', N'Jugement d''allure', null;
EXEC InsertOrUpdateResource 'View_AppResources_PaceRating', 'en-US', N'Pace Rate', null;
EXEC InsertOrUpdateResource 'View_AppResources_PaceRating', 'pt-BR', N'Taxa de ritmo', null;
EXEC InsertOrUpdateResource 'View_AppResources_PaceRating', 'es-ES', N'Coeficiente de velocidad', null;
EXEC InsertOrUpdateResource 'View_AppResources_PaceRating', 'de-DE', N'Geschwindigkeitsfaktor', null;
EXEC InsertOrUpdateResource 'View_AppResources_PaceRating', 'pl-PL', N'Wsółczynnik tempa', null;
GO
EXEC InsertOrUpdateResource 'View_Authentication_Domain', 'fr-FR', N'Domaine', null;
EXEC InsertOrUpdateResource 'View_Authentication_Domain', 'en-US', N'Domain', null;
EXEC InsertOrUpdateResource 'View_Authentication_Domain', 'pt-BR', N'Domínio', null;
EXEC InsertOrUpdateResource 'View_Authentication_Domain', 'es-ES', N'Campo', null;
EXEC InsertOrUpdateResource 'View_Authentication_Domain', 'de-DE', N'Bereich', null;
EXEC InsertOrUpdateResource 'View_Authentication_Domain', 'pl-PL', N'Dziedzina', null;
GO
EXEC InsertOrUpdateResource 'View_Authentication_Language', 'fr-FR', N'Langue', null;
EXEC InsertOrUpdateResource 'View_Authentication_Language', 'en-US', N'Language', null;
EXEC InsertOrUpdateResource 'View_Authentication_Language', 'pt-BR', N'Lingua', null;
EXEC InsertOrUpdateResource 'View_Authentication_Language', 'es-ES', N'Lengua', null;
EXEC InsertOrUpdateResource 'View_Authentication_Language', 'de-DE', N'Sprache', null;
EXEC InsertOrUpdateResource 'View_Authentication_Language', 'pl-PL', N'Język ', null;
GO
EXEC InsertOrUpdateResource 'View_Authentication_Password', 'fr-FR', N'Mot de passe', null;
EXEC InsertOrUpdateResource 'View_Authentication_Password', 'en-US', N'Password', null;
EXEC InsertOrUpdateResource 'View_Authentication_Password', 'pt-BR', N'Senha', null;
EXEC InsertOrUpdateResource 'View_Authentication_Password', 'es-ES', N'Contraseña', null;
EXEC InsertOrUpdateResource 'View_Authentication_Password', 'de-DE', N'Kennwort', null;
EXEC InsertOrUpdateResource 'View_Authentication_Password', 'pl-PL', N'Hasło ', null;
GO
EXEC InsertOrUpdateResource 'View_Authentication_Username', 'fr-FR', N'Identifiant', null;
EXEC InsertOrUpdateResource 'View_Authentication_Username', 'en-US', N'ID', null;
EXEC InsertOrUpdateResource 'View_Authentication_Username', 'pt-BR', N'ID', null;
EXEC InsertOrUpdateResource 'View_Authentication_Username', 'es-ES', N'ID', null;
EXEC InsertOrUpdateResource 'View_Authentication_Username', 'de-DE', N'Login', null;
EXEC InsertOrUpdateResource 'View_Authentication_Username', 'pl-PL', N'Lp', null;
GO
EXEC InsertOrUpdateResource 'View_BackupRestore_BackupExplanation', 'fr-FR', N'Le backup de la base de données consiste à stocker dans un fichier l''état dans lequel se trouve l''application.', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_BackupExplanation', 'en-US', N'Backup will store in a file the current status of the application', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_BackupExplanation', 'pt-BR', N'Backup irá armazenar em um arquivo de status atual do aplicativo  ', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_BackupExplanation', 'es-ES', N'Backup consiste en guardar en un archivo al estado actual de la aplicación', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_BackupExplanation', 'de-DE', N'Backup speichert in einer Unterdatei den aktuellent Status des Programms', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_BackupExplanation', 'pl-PL', N'Zapsowa kopia będzie przecowywana w pliku z bieżącym statusem aplikacji', null;
GO
EXEC InsertOrUpdateResource 'View_BackupRestore_ConfirmRestore', 'fr-FR', N'Tous les changement effectués sur la base actuelle seront perdus.
Etes vous sûr de vouloir la remplacer par celle sélectionnée ?', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_ConfirmRestore', 'en-US', N'All the changes in the current data base will be lost.
Do you really want to restore the selected database?', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_ConfirmRestore', 'pt-BR', N'Todas as alterações na base de dados atual serão perdidos.
Deseja realmente restaurar a base de dados selecionada?', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_ConfirmRestore', 'es-ES', N'Todos los cambios de esta base de datos seran perdidos. Quiere restaurar la base de datos selecionada?', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_ConfirmRestore', 'de-DE', N'Alle Änderungen an den derzeitigen Daten werden verloren. 
Möchten Sie diese wirklich durch die Neuen ersetzen ?', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_ConfirmRestore', 'pl-PL', N'Wszystkie zmiany zostaną utracone.
Czy na pewno chcesz przywrócić wybraną bazę?', null;
GO
EXEC InsertOrUpdateResource 'View_BackupRestore_ConfirmRestoreTitle', 'fr-FR', N'Restauration de base de données en cours …', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_ConfirmRestoreTitle', 'en-US', N'Restoring in progress', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_ConfirmRestoreTitle', 'pt-BR', N'Restoring in progress', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_ConfirmRestoreTitle', 'es-ES', N'Restoring in progress', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_ConfirmRestoreTitle', 'de-DE', N'Restoring in progress', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_ConfirmRestoreTitle', 'pl-PL', N'Restoring in progress', null;
GO
EXEC InsertOrUpdateResource 'View_BackupRestore_Restart', 'fr-FR', N'KL² est sur le point de redémarrer. 
Cliquer sur OK lorsque vous être prêt à redémarrer l''application.', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_Restart', 'en-US', N'KL² is about to restart.
Click OK when you are ready to restart the application', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_Restart', 'pt-BR', N'KL² será reiniciado. 
Clique OK quando estiver pronto para reiniciar o aplicativo.', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_Restart', 'es-ES', N'KL² esta en proceso de re-iniciación
Click OK para re-iniciar la aplicación', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_Restart', 'de-DE', N'KL² wird neu gestartet.
Klicken Sie auf OK wenn Sie bereit sind neuzustarten.', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_Restart', 'pl-PL', N'KL² jest gotowe do przywrócenia.
Kliknij OK kiedy do rozpoczęcia aplikacji', null;
GO
EXEC InsertOrUpdateResource 'View_BackupRestore_RestartTitle', 'fr-FR', N'Redémarrage de l''application...', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_RestartTitle', 'en-US', N'Restarting in progress', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_RestartTitle', 'pt-BR', N'Restarting in progress', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_RestartTitle', 'es-ES', N'Restarting in progress', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_RestartTitle', 'de-DE', N'Restarting in progress', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_RestartTitle', 'pl-PL', N'Restarting in progress', null;
GO
EXEC InsertOrUpdateResource 'View_BackupRestore_RestoreExplanation', 'fr-FR', N'La restauration permet de restituer l''état de l''application dans lequel il a été sauvegardé.
L''état actuel est perdu.
Un redémarage de l''application vous sera demandé à l''issue de la restauration.', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_RestoreExplanation', 'en-US', N'Restore will allow you to  go back to any backup saved before.
The current database will be lost.
At the end of the restore process, the application will be re-started', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_RestoreExplanation', 'pt-BR', N'Restaurar permite voltar a qualquer backup salvo anteriormente.
A base de dados atual será perdida.
Ao final do processo de restauração, o aplicativo será reniciado.', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_RestoreExplanation', 'es-ES', N'Restaurar permite regresar a cuaquier backup guardado previamente
La base de dato actual sera perdida
Al final del proceso de restauración, la aplicación sera re-iniciada', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_RestoreExplanation', 'de-DE', N'Restore ermöglicht den vorherig abgespeicherten Zustand wieder herzustellen.
Die aktuellen Daten gehen verloren.
Am Ende des Restore-Prozesses ist ein Neustart erforderlich.', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_RestoreExplanation', 'pl-PL', N'Rozpoczęcie pozwoli na powrót do każdej zapisanej wersji zapasowej.
Obecna baza danych zostanie skasowana.
Na końcu procesu otwierania, aplikacja zostanie uruchomiona ponownie.', null;
GO
EXEC InsertOrUpdateResource 'View_BackupRestore_SelectABackup', 'fr-FR', N'Entrer un nom pour le fichier de backup', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_SelectABackup', 'en-US', N'Enter a name for the backup file', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_SelectABackup', 'pt-BR', N'Digite o nome para o arquivo de backup', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_SelectABackup', 'es-ES', N'Introduzca un nombre del archivo', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_SelectABackup', 'de-DE', N'Geben Sie bitte einen Namen für den Backupordner ein', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_SelectABackup', 'pl-PL', N'Nazwij plik zapasowy', null;
GO
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialProject', 'fr-FR', N'Ajouter un nouvel élément lié à un "Projet"', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialProject', 'en-US', N'Add a new item linked to a "Project"', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialProject', 'pt-BR', N'Adicionar um novo "Projeto" item', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialProject', 'es-ES', N'Añadir un nuevo elemento "Proyecto"', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialProject', 'de-DE', N'Fügen Sie ein neues "Project" Element ein', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialProject', 'pl-PL', N'Dodaj nową sprawę do "Projektu"', null;
GO
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialProcess', 'fr-FR', N'Ajouter un nouvel élément lié à un "Process"', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialProcess', 'en-US', N'Add a new item linked to a "Process"', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialProcess', 'pt-BR', N'Add a new item linked to a "Process"', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialProcess', 'es-ES', N'Add a new item linked to a "Process"', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialProcess', 'de-DE', N'Add a new item linked to a "Process"', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialProcess', 'pl-PL', N'Add a new item linked to a "Process"', null;
GO
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialStandard', 'fr-FR', N'Ajouter un nouvel élément "Standard"', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialStandard', 'en-US', N'Add a new item as a "Standard"', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialStandard', 'pt-BR', N'Adicionar um novo "Padrão" item', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialStandard', 'es-ES', N'Añadir un nuevo elemento "Standard"', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialStandard', 'de-DE', N'Fügen Sie ein neues "Standard" Element ein', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_AddReferentialStandard', 'pl-PL', N'Dodaj nową sprawę jako "Standard"', null;
GO
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Description', 'fr-FR', N'Description', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Description', 'en-US', N'Description', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Description', 'pt-BR', N'Descrição', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Description', 'es-ES', N'Descripción', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Description', 'de-DE', N'Beschreibung', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Description', 'pl-PL', N'Opis', null;
GO
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Project', 'fr-FR', N'Projet', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Project', 'en-US', N'Project', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Project', 'pt-BR', N'Projeto', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Project', 'es-ES', N'Proyecto', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Project', 'de-DE', N'Projekt', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Project', 'pl-PL', N'Pomoc', null;
GO
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Process', 'fr-FR', N'Process', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Process', 'en-US', N'Process', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Process', 'pt-BR', N'Process', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Process', 'es-ES', N'Process', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Process', 'de-DE', N'Process', null;
EXEC InsertOrUpdateResource 'View_Common_AppReferentials_Process', 'pl-PL', N'Process', null;
GO
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric1', 'fr-FR', N'(champ libre numérique 1)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric1', 'en-US', N'(Free numeric field 1)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric1', 'pt-BR', N'(Campo numérico livre 1)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric1', 'es-ES', N'(Campo libre numerico 1)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric1', 'de-DE', N'(Freies Zahlenfeld 1)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric1', 'pl-PL', N'(Wolne pole numeryczne 1)', null;
GO
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric2', 'fr-FR', N'(champ libre numérique 2)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric2', 'en-US', N'(Free numeric field 2)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric2', 'pt-BR', N'(Campo numérico livre 2)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric2', 'es-ES', N'(Campo libre numerico 2)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric2', 'de-DE', N'(Freies Zahlenfeld 2)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric2', 'pl-PL', N'(Wolne pole numeryczne 2)', null;
GO
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric3', 'fr-FR', N'(champ libre numérique 3)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric3', 'en-US', N'(Free numeric field 3)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric3', 'pt-BR', N'(Campo numérico livre 3)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric3', 'es-ES', N'(Campo libre numerico 3)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric3', 'de-DE', N'(Freies Zahlenfeld 3)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric3', 'pl-PL', N'(Wolne pole numeryczne 3)', null;
GO
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric4', 'fr-FR', N'(champ libre numérique 4)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric4', 'en-US', N'(Free numeric field 4)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric4', 'pt-BR', N'(Campo numérico livre 4)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric4', 'es-ES', N'(Campo libre numerico 4)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric4', 'de-DE', N'(Freies Zahlenfeld 4)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Numeric4', 'pl-PL', N'(Wolne pole numeryczne 4)', null;
GO
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text1', 'fr-FR', N'(champ libre texte 1)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text1', 'en-US', N'(Free text field 1)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text1', 'pt-BR', N'(Campo de texto livre 1)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text1', 'es-ES', N'(Campo libre texto 1)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text1', 'de-DE', N'(Freies Textfeld 1)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text1', 'pl-PL', N'(Wolne pole tekstowe 1)', null;
GO
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text2', 'fr-FR', N'(champ libre texte 2)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text2', 'en-US', N'(Free text field 2)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text2', 'pt-BR', N'(Campo de texto livre 2)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text2', 'es-ES', N'(Campo libre texto 2)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text2', 'de-DE', N'(Freies Textfeld 2)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text2', 'pl-PL', N'(Wolne pole tekstowe 2)', null;
GO
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text3', 'fr-FR', N'(champ libre texte 3)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text3', 'en-US', N'(Free text field 3)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text3', 'pt-BR', N'(Campo de texto livre 3)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text3', 'es-ES', N'(Campo libre texto 3)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text3', 'de-DE', N'(Freies Textfeld 3)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text3', 'pl-PL', N'(Wolne pole tekstowe 3)', null;
GO
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text4', 'fr-FR', N'(champ libre texte 4)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text4', 'en-US', N'(Free text field 4)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text4', 'pt-BR', N'(Campo de texto livre 4)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text4', 'es-ES', N'(Campo libre texto 4)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text4', 'de-DE', N'(Freies Textfeld 4)', null;
EXEC InsertOrUpdateResource 'View_Common_CustomFieldDefaultLabel_Text4', 'pl-PL', N'(Wolne pole tekstowe 4)', null;
GO
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_IsEnabled', 'fr-FR', N'Activé', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_IsEnabled', 'en-US', N'Activated', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_IsEnabled', 'pt-BR', N'Ativo', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_IsEnabled', 'es-ES', N'Activado', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_IsEnabled', 'de-DE', N'Aktivierung', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_IsEnabled', 'pl-PL', N'Aktywne', null;
GO
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_VersionIncompatibility', 'fr-FR', N'Version non compatible.', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_VersionIncompatibility', 'en-US', N'Your version is not compatible.', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_VersionIncompatibility', 'pt-BR', N'Versão não compatível.', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_VersionIncompatibility', 'es-ES', N'Version no compatible.', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_VersionIncompatibility', 'de-DE', N'Die Version ist nicht kompatibel.', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_VersionIncompatibility', 'pl-PL', N'Ta wersja nie jest kompatybilna.', null;
GO
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_VersionIncompatibilityDescription', 'fr-FR', N'La version de l''application n''est pas assez récente pour pouvoir gérer cette extension.', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_VersionIncompatibilityDescription', 'en-US', N'Your version is too old to support this extension.', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_VersionIncompatibilityDescription', 'pt-BR', N'Sua versão é muito antiga para suportar esta extensão.', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_VersionIncompatibilityDescription', 'es-ES', N'La version de la aplication no es suficientemente reciente para poder ejecutar esta extensión.', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_VersionIncompatibilityDescription', 'de-DE', N'Die Anwendung Version ist nicht aktuell genug, um diese Erweiterung zu verwalten.', null;
EXEC InsertOrUpdateResource 'View_ExtensionsConfigurationView_VersionIncompatibilityDescription', 'pl-PL', N'Ta wersja jest za stara żeby obsługiwać ten obszar.', null;
GO
EXEC InsertOrUpdateResource 'View_MainWindow_About', 'fr-FR', N'A propos', null;
EXEC InsertOrUpdateResource 'View_MainWindow_About', 'en-US', N'About', null;
EXEC InsertOrUpdateResource 'View_MainWindow_About', 'pt-BR', N'A respeito', null;
EXEC InsertOrUpdateResource 'View_MainWindow_About', 'es-ES', N'Sobre', null;
EXEC InsertOrUpdateResource 'View_MainWindow_About', 'de-DE', N'Über', null;
EXEC InsertOrUpdateResource 'View_MainWindow_About', 'pl-PL', N'Info', null;
GO
EXEC InsertOrUpdateResource 'View_MainWindow_Administration', 'fr-FR', N'Administration', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Administration', 'en-US', N'Administration', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Administration', 'pt-BR', N'Administração', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Administration', 'es-ES', N'Administración', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Administration', 'de-DE', N'Verwaltung', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Administration', 'pl-PL', N'Administracja', null;
GO
EXEC InsertOrUpdateResource 'View_MainWindow_Disconnect', 'fr-FR', N'Se déconnecter', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Disconnect', 'en-US', N'Disconnect', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Disconnect', 'pt-BR', N'Desconectar', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Disconnect', 'es-ES', N'Desconectarse', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Disconnect', 'de-DE', N'Ausloggen', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Disconnect', 'pl-PL', N'Rozłącz', null;
GO
EXEC InsertOrUpdateResource 'View_MainWindow_Help', 'fr-FR', N'Aide', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Help', 'en-US', N'Help', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Help', 'pt-BR', N'Ajuda', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Help', 'es-ES', N'Ayuda', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Help', 'de-DE', N'Hilfe', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Help', 'pl-PL', N'Pomoc', null;
GO
EXEC InsertOrUpdateResource 'View_MainWindow_MediaFailed', 'fr-FR', N'Le fichier vidéo n''a pas pu être ouvert. Le chemin d''accès est faux OU le format n''est pas compatible OU la plage de la séquence est au-delà de la fin de la vidéo.', null;
EXEC InsertOrUpdateResource 'View_MainWindow_MediaFailed', 'en-US', N'The video file couldn''t be opened (file not found or format not supported or sequence outside the video range).', null;
EXEC InsertOrUpdateResource 'View_MainWindow_MediaFailed', 'pt-BR', N'O ficheiro de video não pode ser aberto(ficheiro não encontrado ou formato não suportado ou sequencia fora do intervalo do video).', null;
EXEC InsertOrUpdateResource 'View_MainWindow_MediaFailed', 'es-ES', N'La video no puede ser abierta (fichero no encontrado o formato no valido o secuencia fuera de rango).', null;
EXEC InsertOrUpdateResource 'View_MainWindow_MediaFailed', 'de-DE', N'Das Video konnte nicht geöffnet werden (Datei nicht gefunden, das Format wird nicht unterstützt oder Sequenz ist außerhalb vom Videobereich).', null;
EXEC InsertOrUpdateResource 'View_MainWindow_MediaFailed', 'pl-PL', N'Film nie może być otworzony (plik nie został znaleziony lub format nie jest obsługiwany lubkolejnośc poza ramami filmu)', null;
GO
EXEC InsertOrUpdateResource 'View_MainWindow_VideoNotFound', 'fr-FR', N'Le fichier vidéo {0} n''existe pas.', '{0} correspond au chemin du fichier vidéo';
EXEC InsertOrUpdateResource 'View_MainWindow_VideoNotFound', 'en-US', N'The video file {0} doesn''t exist.', '{0} correspond au chemin du fichier vidéo';
EXEC InsertOrUpdateResource 'View_MainWindow_VideoNotFound', 'pt-BR', N'O Ficheiro Vídeo {0} não foi encontrado.', '{0} correspond au chemin du fichier vidéo';
EXEC InsertOrUpdateResource 'View_MainWindow_VideoNotFound', 'es-ES', N'El fichero video {0} no existe.', '{0} correspond au chemin du fichier vidéo';
EXEC InsertOrUpdateResource 'View_MainWindow_VideoNotFound', 'de-DE', N'Die Videodatei {0} existiert nicht.', '{0} correspond au chemin du fichier vidéo';
EXEC InsertOrUpdateResource 'View_MainWindow_VideoNotFound', 'pl-PL', N'Plik video {0} nie istnieje.', '{0} correspond au chemin du fichier vidéo';
GO
EXEC InsertOrUpdateResource 'View_MainWindow_Settings', 'fr-FR', N'Options', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Settings', 'en-US', N'Options', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Settings', 'pt-BR', N'Opções', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Settings', 'es-ES', N'Opciones', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Settings', 'de-DE', N'Optionen', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Settings', 'pl-PL', N'Opcje', null;
GO
EXEC InsertOrUpdateResource 'View_ParmetersDecoderView_VideoDecoderSource', 'fr-FR', N'Source du décodeur vidéo :', null;
EXEC InsertOrUpdateResource 'View_ParmetersDecoderView_VideoDecoderSource', 'en-US', N'Video decoder source :', null;
EXEC InsertOrUpdateResource 'View_ParmetersDecoderView_VideoDecoderSource', 'pt-BR', N'Fonte do decodificador de vídeo:', null;
EXEC InsertOrUpdateResource 'View_ParmetersDecoderView_VideoDecoderSource', 'es-ES', N'Fuente de decodificador de vídeo:', null;
EXEC InsertOrUpdateResource 'View_ParmetersDecoderView_VideoDecoderSource', 'de-DE', N'Quelle von Video-Decoder:', null;
EXEC InsertOrUpdateResource 'View_ParmetersDecoderView_VideoDecoderSource', 'pl-PL', N'Źródło filmu:', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareMembers_Email', 'fr-FR', N'Adresse e-mail', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Email', 'en-US', N'E-mail address', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Email', 'pt-BR', N'E-mail', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Email', 'es-ES', N'E-mail', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Email', 'de-DE', N'E-Mail', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Email', 'pl-PL', N'Adres email', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareMembers_KL2User', 'fr-FR', N'KL²® Directory', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_KL2User', 'en-US', N'KL²® Directory', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_KL2User', 'pt-BR', N'KL²® Directory', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_KL2User', 'es-ES', N'KL²® Directory', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_KL2User', 'de-DE', N'KL²® Directory', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_KL2User', 'pl-PL', N'KL²® Directory', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareMembers_Name', 'fr-FR', N'Nom', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Name', 'en-US', N'Name', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Name', 'pt-BR', N'Nome', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Name', 'es-ES', N'Nombre', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Name', 'de-DE', N'Name', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Name', 'pl-PL', N'Nazwisko', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareMembers_PhoneNumber', 'fr-FR', N'Numéro de téléphone', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_PhoneNumber', 'en-US', N'Phone number', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_PhoneNumber', 'pt-BR', N'Número de telefone', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_PhoneNumber', 'es-ES', N'Número de teléfono', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_PhoneNumber', 'de-DE', N'Telefonnummer', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_PhoneNumber', 'pl-PL', N'Numer telefonu', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareMembers_ProjectMembers', 'fr-FR', N'Membres du projet', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_ProjectMembers', 'en-US', N'Project members', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_ProjectMembers', 'pt-BR', N'Membros do projeto', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_ProjectMembers', 'es-ES', N'Los miembros del proyecto', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_ProjectMembers', 'de-DE', N'Projektmitglieder', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_ProjectMembers', 'pl-PL', N'Członkowie projektu', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareMembers_Roles', 'fr-FR', N'Rôles', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Roles', 'en-US', N'Roles', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Roles', 'pt-BR', N'Papéis', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Roles', 'es-ES', N'Roles', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Roles', 'de-DE', N'Funktionen', null;
EXEC InsertOrUpdateResource 'View_PrepareMembers_Roles', 'pl-PL', N'Role', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Autre', 'fr-FR', N'Autre', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Autre', 'en-US', N'Other', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Autre', 'pt-BR', N'Outro', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Autre', 'es-ES', N'Otro', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Autre', 'de-DE', N'Andere', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Autre', 'pl-PL', N'Inne', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Create', 'fr-FR', N'Créer un nouveau projet', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Create', 'en-US', N'Create a new project', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Create', 'pt-BR', N'Criar um novo projeto', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Create', 'es-ES', N'Crear un nuevo proyecto', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Create', 'de-DE', N'Erstellen Sie ein neues Projekt', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Create', 'pl-PL', N'Utwórz nowy projekt', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_CantCreateProject', 'fr-FR', N'Les projets précédents doivent être clotûrés', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantCreateProject', 'en-US', N'Previous projects must be closed', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantCreateProject', 'pt-BR', N'Previous projects must be closed', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantCreateProject', 'es-ES', N'Previous projects must be closed', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantCreateProject', 'de-DE', N'Previous projects must be closed', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantCreateProject', 'pl-PL', N'Previous projects must be closed', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Delete', 'fr-FR', N'Supprimer définitivement le projet sélectionné', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Delete', 'en-US', N'Delete permanently the selected project', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Delete', 'pt-BR', N'Excluir permanentemente o projeto selecionado', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Delete', 'es-ES', N'Eliminar definitivamente el proyecto seleccionado', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Delete', 'de-DE', N'Ausgewähltes Projekt unwiderruflich löschen', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Delete', 'pl-PL', N'Usuń trwale wskazany projekt', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Description', 'fr-FR', N'Description des enjeux', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Description', 'en-US', N'Challenge description', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Description', 'pt-BR', N'Descrição dos desafios', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Description', 'es-ES', N'Descripción de los objetivos', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Description', 'de-DE', N'Beschreibung der Probleme', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Description', 'pl-PL', N'Opis celu', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_ProcessDescription', 'fr-FR', N'Description', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ProcessDescription', 'en-US', N'Description', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ProcessDescription', 'pt-BR', N'Descrição', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ProcessDescription', 'es-ES', N'Descripción', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ProcessDescription', 'de-DE', N'Beschreibung', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ProcessDescription', 'pl-PL', N'Opis', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_ProcessIsSkill', 'fr-FR', N'Est une compétence', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ProcessIsSkill', 'en-US', N'Is a skill', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ProcessIsSkill', 'pt-BR', N'Is a skill', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ProcessIsSkill', 'es-ES', N'Is a skill', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ProcessIsSkill', 'de-DE', N'Is a skill', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ProcessIsSkill', 'pl-PL', N'Is a skill', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Export', 'fr-FR', N'Exporter un projet dans un fichier ksp (Rappel : les vidéos ne sont pas copiées dans le fichier ksp créé)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Export', 'en-US', N'Export a project into a ksp file (Note: Linked video files are not included ksp files)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Export', 'pt-BR', N'Exportar um projeto em um arquivo ksp (Nota: os arquivos de vídeo referenciados não estão copiados nos arquivos ksp)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Export', 'es-ES', N'Exportación de un proyecto en un archivo ksp (Nota: los videos no se copian en el archivo creado KSP)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Export', 'de-DE', N'Exportieren eines Projekts in einer ksp-Datei (Zur Erinnerung: die Videos sind nicht Bestandteil der ksp-Datei)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Export', 'pl-PL', N'Eksportuj projekt do pliku ksp (Uwaga: Załączony film nie jest zaliczony do pliku ksp)', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Import', 'fr-FR', N'Importer un projet depuis un fichier ksp. (Rappel: les vidéos sont à part du fichier ksp)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Import', 'en-US', N'Import a project from a ksp file (Note: Linked video files are not included ksp files)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Import', 'pt-BR', N'Importar um projeto de um arquivo ksp (Nota: os arquivos de vídeo referenciados não estão copiados nos arquivos ksp)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Import', 'es-ES', N'Importar un proyecto a partir de una ksp archivo. (Recuerde: los videos son parte del archivo ksp)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Import', 'de-DE', N'Import eines Projekts aus einer Datei ksp. (Zur Erinnerung: die Videos sind nicht Bestandteil der ksp-Datei)', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Import', 'pl-PL', N'Importuj projekt do pliku ksp (Uwaga: Załączony film nie jest zaliczony do pliku ksp)', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Name', 'fr-FR', N'Nom', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Name', 'en-US', N'Name', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Name', 'pt-BR', N'Nome', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Name', 'es-ES', N'Nombre', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Name', 'de-DE', N'Name', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Name', 'pl-PL', N'Nazwisko', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Objectives', 'fr-FR', N'Objectif', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Objectives', 'en-US', N'Goal', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Objectives', 'pt-BR', N'Objetivo', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Objectives', 'es-ES', N'Objetivo', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Objectives', 'de-DE', N'Ziele', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Objectives', 'pl-PL', N'Cel', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_TimeScale', 'fr-FR', N'Précision', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_TimeScale', 'en-US', N'Accuracy', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_TimeScale', 'pt-BR', N'Precisão', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_TimeScale', 'es-ES', N'Precisión', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_TimeScale', 'de-DE', N'Präzision', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_TimeScale', 'pl-PL', N'Dokładność', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_StartDate', 'fr-FR', N'Date de début', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_StartDate', 'en-US', N'Start date', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_StartDate', 'pt-BR', N'Start date', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_StartDate', 'es-ES', N'Start date', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_StartDate', 'de-DE', N'Start date', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_StartDate', 'pl-PL', N'Start date', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_ForecastEndDate', 'fr-FR', N'Date de fin prévisionnelle', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ForecastEndDate', 'en-US', N'Forecast end date', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ForecastEndDate', 'pt-BR', N'Forecast end date', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ForecastEndDate', 'es-ES', N'Forecast end date', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ForecastEndDate', 'de-DE', N'Forecast end date', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ForecastEndDate', 'pl-PL', N'Forecast end date', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_RealEndDate', 'fr-FR', N'Date de fin réelle', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RealEndDate', 'en-US', N'Real end date', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RealEndDate', 'pt-BR', N'Real end date', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RealEndDate', 'es-ES', N'Real end date', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RealEndDate', 'de-DE', N'Real end date', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RealEndDate', 'pl-PL', N'Real end date', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_ViewProjects', 'fr-FR', N'Afficher / masquer liste projets', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ViewProjects', 'en-US', N'Show / hide list of projects', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ViewProjects', 'pt-BR', N'Mostrar / ocultar a lista de projetos', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ViewProjects', 'es-ES', N'Mostrar / Ocultar lista de proyectos', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ViewProjects', 'de-DE', N'Zeigen/Verstecken der Projektliste', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_ViewProjects', 'pl-PL', N'Pokaż/ukryj listę projektów', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Activate', 'fr-FR', N'Activer', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Activate', 'en-US', N'Activate', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Activate', 'pt-BR', N'Ativar', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Activate', 'es-ES', N'Activar', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Activate', 'de-DE', N'Aktivieren', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Activate', 'pl-PL', N'Aktywuj', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomFields', 'fr-FR', N'Libellés des champs libres', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomFields', 'en-US', N'Free field labels', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomFields', 'pt-BR', N'Campos livres para novos referenciais', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomFields', 'es-ES', N'Etiquetas del campo libre', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomFields', 'de-DE', N'Beschreibung der freien Felder', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomFields', 'pl-PL', N'Wolne pola etykiety', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomNumericLabels', 'fr-FR', N'Champs libres numérique', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomNumericLabels', 'en-US', N'Free numeric fields', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomNumericLabels', 'pt-BR', N'Campos numéricos livres', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomNumericLabels', 'es-ES', N'Campo libre numerico', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomNumericLabels', 'de-DE', N'Freie Zahlenfelder', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomNumericLabels', 'pl-PL', N'Wolne pola numeryczne', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomTextLabels', 'fr-FR', N'Champs libres texte', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomTextLabels', 'en-US', N'Free text fields', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomTextLabels', 'pt-BR', N'Campos de texto livre', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomTextLabels', 'es-ES', N'Campo libre texto', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomTextLabels', 'de-DE', N'Freie Textfelder', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_CustomTextLabels', 'pl-PL', N'Wolne pola tekstowe', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareReferentials_KeepSelection', 'fr-FR', N'Conserver la sélection', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_KeepSelection', 'en-US', N'Keep selected for next task', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_KeepSelection', 'pt-BR', N'Mantenha a seleção', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_KeepSelection', 'es-ES', N'Conservar la selección', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_KeepSelection', 'de-DE', N'Auswahl behalten', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_KeepSelection', 'pl-PL', N'Zachowaj zaznaczenie dla nastepnego zadania', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareReferentials_MultipleSelection', 'fr-FR', N'Autoriser la sélection multiple', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_MultipleSelection', 'en-US', N'Allow multiple selection', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_MultipleSelection', 'pt-BR', N'Permitir a seleção múltipla', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_MultipleSelection', 'es-ES', N'Autorisar la selección multiple', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_MultipleSelection', 'de-DE', N'Mehrfache Auswahl erlauben', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_MultipleSelection', 'pl-PL', N'Zezwalaj na wielokrotny wybór', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Quantity', 'fr-FR', N'Autoriser la saisie de quantités', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Quantity', 'en-US', N'Allow quantity entry', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Quantity', 'pt-BR', N'Permitir a entrada de quantidades', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Quantity', 'es-ES', N'Autorisar la elección de cantidades', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Quantity', 'de-DE', N'Eingabe von Mengen erlauben', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Quantity', 'pl-PL', N'Zezwalaj na wejście ilościowe', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Referentials', 'fr-FR', N'Référentiels', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Referentials', 'en-US', N'Referentials', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Referentials', 'pt-BR', N'Referenciais', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Referentials', 'es-ES', N'Referencias', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Referentials', 'de-DE', N'Referenzen', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Referentials', 'pl-PL', N'Odniesienia', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Resources', 'fr-FR', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Resources', 'en-US', N'Resource', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Resources', 'pt-BR', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Resources', 'es-ES', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Resources', 'de-DE', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_PrepareReferentials_Resources', 'pl-PL', N'Zasoby', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Add_Tooltip', 'fr-FR', N'Créer un scénario hérité de celui sélectionné.', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Add_Tooltip', 'en-US', N'Create a scenario based on the selected one.', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Add_Tooltip', 'pt-BR', N'Criar um cenário baseado no selecionado.', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Add_Tooltip', 'es-ES', N'Crear un escenario heredado de uno seleccionado.', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Add_Tooltip', 'de-DE', N'Erstellen Sie ein Szenario aus einem ausgewählten Szenario.', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Add_Tooltip', 'pl-PL', N'Utwórz scenariusz bazując na zaznaczonym.', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ConvertToNewProject', 'fr-FR', N'Créer un nouveau projet en partant d''un scénario figé.', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ConvertToNewProject', 'en-US', N'Create a new project from a frozen scenario.', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ConvertToNewProject', 'pt-BR', N'Criar um novo projeto a partir de um congelado cenário.', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ConvertToNewProject', 'es-ES', N'Crear un nuevo proyecto a partir del fijado escenario.', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ConvertToNewProject', 'de-DE', N'Erstellen ein neues Projekt aus einem schreibgeschützten Szenario .', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ConvertToNewProject', 'pl-PL', N'Utwórz nowy projekt na bazie zamrożony scenariusza.', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Description', 'fr-FR', N'Description', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Description', 'en-US', N'Description', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Description', 'pt-BR', N'Descrição', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Description', 'es-ES', N'Descripción', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Description', 'de-DE', N'Beschreibung', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Description', 'pl-PL', N'Opis', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ExportVideoDecomposition', 'fr-FR', N'Exporter la décomposition d''une vidéo du scénario sélectionné', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ExportVideoDecomposition', 'en-US', N'Export the video decomposition of the selected scenario', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ExportVideoDecomposition', 'pt-BR', N'Exportação da decomposição do vídeo do cenário selecionado', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ExportVideoDecomposition', 'es-ES', N'Exportación de descomposición de un video de el escenario seleccionado', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ExportVideoDecomposition', 'de-DE', N'Export der Video Zusammenstellung von dem gewählten Szenario', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ExportVideoDecomposition', 'pl-PL', N'Eksportuj podzielony film z wybranego scenariusza', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ImportVideoDecomposition', 'fr-FR', N'Importer la décomposition d''une vidéo dans le scénario initial du projet en cours', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ImportVideoDecomposition', 'en-US', N'Import the video decomposition in the initial scenario of the current project', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ImportVideoDecomposition', 'pt-BR', N'Importar o vídeo da decomposição do cenário inicial do projeto atual', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ImportVideoDecomposition', 'es-ES', N'Importación de la descomposición de un vídeo en el escenario inicial del proyecto actual', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ImportVideoDecomposition', 'de-DE', N'Importieren Sie die Video Zusammenstellung in das erste Szenario  des aktuellen Projekts', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ImportVideoDecomposition', 'pl-PL', N'Importuj podzielony film do scenariusza początkowego bieżącego projektu', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_List', 'fr-FR', N'Liste', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_List', 'en-US', N'List', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_List', 'pt-BR', N'Lista', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_List', 'es-ES', N'Lista', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_List', 'de-DE', N'Liste', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_List', 'pl-PL', N'Lista', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Name', 'fr-FR', N'Nom', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Name', 'en-US', N'Name', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Name', 'pt-BR', N'Nome', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Name', 'es-ES', N'Nombre', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Name', 'de-DE', N'Name', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Name', 'pl-PL', N'Nazwisko', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Nature', 'fr-FR', N'Nature', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Nature', 'en-US', N'Nature', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Nature', 'pt-BR', N'Tipo', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Nature', 'es-ES', N'Naturaleza', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Nature', 'de-DE', N'Natur', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Nature', 'pl-PL', N'Charakter', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_OriginalScenario', 'fr-FR', N'Scénario d''origine', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_OriginalScenario', 'en-US', N'Original scenario', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_OriginalScenario', 'pt-BR', N'Cenário inicial', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_OriginalScenario', 'es-ES', N'Escenario Original', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_OriginalScenario', 'de-DE', N'Originalszenario', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_OriginalScenario', 'pl-PL', N'Oryginalny scenariusz', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Performance', 'fr-FR', N'Performance', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Performance', 'en-US', N'Performance', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Performance', 'pt-BR', N'Execução', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Performance', 'es-ES', N'Rendimiento', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Performance', 'de-DE', N'Leistung', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Performance', 'pl-PL', N'Osiągnięcie', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Remove_Tooltip', 'fr-FR', N'Supprimer le scénario sélectionné', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Remove_Tooltip', 'en-US', N'Delete the selected scenario', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Remove_Tooltip', 'pt-BR', N'Apagar o cenário escolhido', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Remove_Tooltip', 'es-ES', N'Eliminar el escenario seleccionado', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Remove_Tooltip', 'de-DE', N'Löschen Sie das ausgewählte Szenario', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Remove_Tooltip', 'pl-PL', N'Usuń zaznaczony scenariusz', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ShowInSummary', 'fr-FR', N'Afficher dans les histogrammes de l''onglet Synthèse', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ShowInSummary', 'en-US', N'To be displayed in Sum up tab histograms', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ShowInSummary', 'pt-BR', N'Disponibilizar nos histogramas da aba resumo', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ShowInSummary', 'es-ES', N'A mostrar en los histogramas de la ficha Resumen', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ShowInSummary', 'de-DE', N'In der Zusammenfassung anzeigen', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ShowInSummary', 'pl-PL', N'Wyświetl sumaryczne wykresy', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_State', 'fr-FR', N'Etat', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_State', 'en-US', N'State', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_State', 'pt-BR', N'Estado', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_State', 'es-ES', N'Estado', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_State', 'de-DE', N'Zustand', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_State', 'pl-PL', N'Stan', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Equipments', 'fr-FR', N'Equipements', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Equipments', 'en-US', N'Equipments', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Equipments', 'pt-BR', N'Equipamentos', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Equipments', 'es-ES', N'Equipo', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Equipments', 'de-DE', N'Ausrüstungen', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Equipments', 'pl-PL', N'Sprzęty', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Operators', 'fr-FR', N'Opérateurs', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Operators', 'en-US', N'Operators', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Operators', 'pt-BR', N'Operadores', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Operators', 'es-ES', N'Los operadores', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Operators', 'de-DE', N'Operatoren', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Operators', 'pl-PL', N'Operatorzy', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Scenarios', 'fr-FR', N'Scénarios', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Scenarios', 'en-US', N'Scenarios', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Scenarios', 'pt-BR', N'Cenários', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Scenarios', 'es-ES', N'Escenarios', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Scenarios', 'de-DE', N'Szenarien', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Scenarios', 'pl-PL', N'Scenariusze', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Total', 'fr-FR', N'Tous', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Total', 'en-US', N'All', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Total', 'pt-BR', N'Todos', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Total', 'es-ES', N'Todos', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Total', 'de-DE', N'Alle', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_Total', 'pl-PL', N'Wszyscy', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_TxOccCC', 'fr-FR', N'Taux charge / CC I+E', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_TxOccCC', 'en-US', N'Load rate / I+E CP', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_TxOccCC', 'pt-BR', N'Taxa de carga / CC I+E', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_TxOccCC', 'es-ES', N'Tasa de carga / CC I + E', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_TxOccCC', 'de-DE', N'Bewertungsrate / CC I + E', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_TxOccCC', 'pl-PL', N'Wskaźnik obłożenia / W+Z CP', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_TxSup', 'fr-FR', N'Taux superposition / CC I+E', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_TxSup', 'en-US', N'Comb. rate / I+E CP', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_TxSup', 'pt-BR', N'Taxa de superposição / CC I+E', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_TxSup', 'es-ES', N'Tasa de superposición / CC I + E', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_TxSup', 'de-DE', N'Rate Overlay / CC I + E', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_Summary_TxSup', 'pl-PL', N'Wskaźnik comb. / W+Z CP', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_Add_Tooltip', 'fr-FR', N'Ajouter une vidéo au projet', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Add_Tooltip', 'en-US', N'Add a video to the current project', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Add_Tooltip', 'pt-BR', N'Adicionar um vídeo para o projeto atual', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Add_Tooltip', 'es-ES', N'Agregar un video al proyecto', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Add_Tooltip', 'de-DE', N'Fügen Sie ein Video zum aktuellen Projekt hinzu', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Add_Tooltip', 'pl-PL', N'Dodaj film do bieżącego projektu', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_AvailableVideos', 'fr-FR', N'Vidéos disponibles', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_AvailableVideos', 'en-US', N'Available videos', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_AvailableVideos', 'pt-BR', N'Vídeos disponíveis', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_AvailableVideos', 'es-ES', N'Vídeos disponibles', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_AvailableVideos', 'de-DE', N'Verfügbare Videos', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_AvailableVideos', 'pl-PL', N'Dostępne filmy', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_Browse', 'fr-FR', N'...', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Browse', 'en-US', N'...', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Browse', 'pt-BR', N'...', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Browse', 'es-ES', N'...', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Browse', 'de-DE', N'...', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Browse', 'pl-PL', N'…', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_DefaultResource', 'fr-FR', N'Ressource dédiée', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_DefaultResource', 'en-US', N'Dedicated resource', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_DefaultResource', 'pt-BR', N'Recurso dedicado', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_DefaultResource', 'es-ES', N'Recurso específico', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_DefaultResource', 'de-DE', N'Zugeordnete Ressource', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_DefaultResource', 'pl-PL', N'Dedykowane zasoby', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_Resource', 'fr-FR', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Resource', 'en-US', N'Resource', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Resource', 'pt-BR', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Resource', 'es-ES', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Resource', 'de-DE', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Resource', 'pl-PL', N'Zasób', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_VideoInUsed', 'fr-FR', N'La vidéo est utilisée dans un projet.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_VideoInUsed', 'en-US', N'The video is used in a project.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_VideoInUsed', 'pt-BR', N'The video is used in a project.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_VideoInUsed', 'es-ES', N'The video is used in a project.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_VideoInUsed', 'de-DE', N'The video is used in a project.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_VideoInUsed', 'pl-PL', N'The video is used in a project.', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_VideoNotExists', 'fr-FR', N'Le fichier ''{0}'' n''existe pas.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_VideoNotExists', 'en-US', N'File ''{0}'' doesn''t exist.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_VideoNotExists', 'pt-BR', N'File ''{0}'' doesn''t exist.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_VideoNotExists', 'es-ES', N'File ''{0}'' doesn''t exist.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_VideoNotExists', 'de-DE', N'File ''{0}'' doesn''t exist.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_VideoNotExists', 'pl-PL', N'File ''{0}'' doesn''t exist.', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_CameraName', 'fr-FR', N'Caméra', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_CameraName', 'en-US', N'Camera', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_CameraName', 'pt-BR', N'Camera', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_CameraName', 'es-ES', N'Camera', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_CameraName', 'de-DE', N'Camera', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_CameraName', 'pl-PL', N'Camera', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_View', 'fr-FR', N'Vue', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_View', 'en-US', N'View', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_View', 'pt-BR', N'View', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_View', 'es-ES', N'View', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_View', 'de-DE', N'View', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_View', 'pl-PL', N'View', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_NumSeq', 'fr-FR', N'Numéro de séquence', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_NumSeq', 'en-US', N'Sequence number', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_NumSeq', 'pt-BR', N'Sequence number', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_NumSeq', 'es-ES', N'Sequence number', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_NumSeq', 'de-DE', N'Sequence number', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_NumSeq', 'pl-PL', N'Sequence number', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_Sync', 'fr-FR', N'Sync.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Sync', 'en-US', N'Sync.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Sync', 'pt-BR', N'Sync.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Sync', 'es-ES', N'Sync.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Sync', 'de-DE', N'Sync.', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Sync', 'pl-PL', N'Sync.', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_OnLocal', 'fr-FR', N'Sur le pc', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OnLocal', 'en-US', N'On local', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OnLocal', 'pt-BR', N'On local', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OnLocal', 'es-ES', N'On local', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OnLocal', 'de-DE', N'On local', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OnLocal', 'pl-PL', N'On local', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_OnServer', 'fr-FR', N'Sur le serveur', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OnServer', 'en-US', N'On server', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OnServer', 'pt-BR', N'On server', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OnServer', 'es-ES', N'On server', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OnServer', 'de-DE', N'On server', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OnServer', 'pl-PL', N'On server', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_Sending', 'fr-FR', N'Envoi', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Sending', 'en-US', N'Sending', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Sending', 'pt-BR', N'Sending', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Sending', 'es-ES', N'Sending', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Sending', 'de-DE', N'Sending', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Sending', 'pl-PL', N'Sending', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_Receiving', 'fr-FR', N'Réception', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Receiving', 'en-US', N'Receiving', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Receiving', 'pt-BR', N'Receiving', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Receiving', 'es-ES', N'Receiving', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Receiving', 'de-DE', N'Receiving', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Receiving', 'pl-PL', N'Receiving', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_Transfer', 'fr-FR', N'Transfert', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Transfer', 'en-US', N'Transfer', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Transfer', 'pt-BR', N'Transfer', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Transfer', 'es-ES', N'Transfer', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Transfer', 'de-DE', N'Transfer', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Transfer', 'pl-PL', N'Transfer', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_Description', 'fr-FR', N'Description', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Description', 'en-US', N'Description', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Description', 'pt-BR', N'Descrição', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Description', 'es-ES', N'Descripción', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Description', 'de-DE', N'Beschreibung', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Description', 'pl-PL', N'Opis', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_File', 'fr-FR', N'Fichier', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_File', 'en-US', N'File', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_File', 'pt-BR', N'Arquivo', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_File', 'es-ES', N'Expediente', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_File', 'de-DE', N'Datei', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_File', 'pl-PL', N'Plik', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_Format', 'fr-FR', N'Format', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Format', 'en-US', N'Format', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Format', 'pt-BR', N'Formato', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Format', 'es-ES', N'Formato', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Format', 'de-DE', N'Format', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Format', 'pl-PL', N'Format', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_Name', 'fr-FR', N'Nom', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Name', 'en-US', N'Name', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Name', 'pt-BR', N'Nome', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Name', 'es-ES', N'Nombre', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Name', 'de-DE', N'Name', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Name', 'pl-PL', N'Nazwisko', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_Nature', 'fr-FR', N'La vidéo est-elle liée à une ressource dédiée (opérateur ou équipement) ?', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Nature', 'en-US', N'Is the video linked to a dedicated resource (operator or equipment)?', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Nature', 'pt-BR', N'O vídeo está ligado a um recurso dedicado (operador ou equipamento)?', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Nature', 'es-ES', N'¿Está el video relacionado con un recurso específico (operador o equipo) ?', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Nature', 'de-DE', N'Ist  das Video mit einer fest zugeordneten Ressource verbunden? (ein Operator oder eine Ausrüstung)', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Nature', 'pl-PL', N'Czy film jest połączony z dedykowanym zasobem (operator lub sprzęt)?', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_OtherProjects', 'fr-FR', N'Autres projets', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OtherProjects', 'en-US', N'Other projects', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OtherProjects', 'pt-BR', N'Outros projetos', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OtherProjects', 'es-ES', N'Otros proyectos', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OtherProjects', 'de-DE', N'Andere Projekte', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_OtherProjects', 'pl-PL', N'Inne projekty', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_ProjectVideos', 'fr-FR', N'Vidéos du process', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_ProjectVideos', 'en-US', N'Process'' videos', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_ProjectVideos', 'pt-BR', N'Vídeos do processo', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_ProjectVideos', 'es-ES', N'Videos del proceso', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_ProjectVideos', 'de-DE', N'Videos des Prozesses', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_ProjectVideos', 'pl-PL', N'Filmy z procesu', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_Remove_Tooltip', 'fr-FR', N'Supprimer la vidéo sélectionnée du projet', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Remove_Tooltip', 'en-US', N'Remove the selected video from the current project', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Remove_Tooltip', 'pt-BR', N'Excluir o vídeo selecionado do projeto', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Remove_Tooltip', 'es-ES', N'Eliminar el proyecto de video seleccionado', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Remove_Tooltip', 'de-DE', N'Löschen Sie das ausgewählte Video vom Projekt', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_Remove_Tooltip', 'pl-PL', N'Usuń wskazany film z bieżacego projektu', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareVideos_ShootingDate', 'fr-FR', N'Date de prise de vue', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_ShootingDate', 'en-US', N'Shooting date', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_ShootingDate', 'pt-BR', N'Data do vídeo', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_ShootingDate', 'es-ES', N'Fecha de la fotografía', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_ShootingDate', 'de-DE', N'Datum der Aufnahme', null;
EXEC InsertOrUpdateResource 'View_PrepareVideos_ShootingDate', 'pl-PL', N'Data nagrania', null;
GO
EXEC InsertOrUpdateResource 'View_PublishScenario_SelectScenario', 'fr-FR', N'Sélectionner le scénario à publier', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_SelectScenario', 'en-US', N'Select the scenario to publish', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_SelectScenario', 'pt-BR', N'Select the scenario to publish', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_SelectScenario', 'es-ES', N'Select the scenario to publish', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_SelectScenario', 'de-DE', N'Select the scenario to publish', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_SelectScenario', 'pl-PL', N'Select the scenario to publish', null;
GO
EXEC InsertOrUpdateResource 'View_PublishScenario_PublishTo', 'fr-FR', N'Publier pour', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_PublishTo', 'en-US', N'Publish for', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_PublishTo', 'pt-BR', N'Publish for', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_PublishTo', 'es-ES', N'Publish for', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_PublishTo', 'de-DE', N'Publish for', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_PublishTo', 'pl-PL', N'Publish for', null;
GO
EXEC InsertOrUpdateResource 'View_PublishScenario_NotAllLinkedProcessArePublished', 'fr-FR', N'Impossible de publier ce scénario, les process suivants ne sont pas publiés :', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_NotAllLinkedProcessArePublished', 'en-US', N'Unable to publish this scenario, the following processes are not published :', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_NotAllLinkedProcessArePublished', 'pt-BR', N'Unable to publish this scenario, the following processes are not published :', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_NotAllLinkedProcessArePublished', 'es-ES', N'Unable to publish this scenario, the following processes are not published :', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_NotAllLinkedProcessArePublished', 'de-DE', N'Unable to publish this scenario, the following processes are not published :', null;
EXEC InsertOrUpdateResource 'View_PublishScenario_NotAllLinkedProcessArePublished', 'pl-PL', N'Unable to publish this scenario, the following processes are not published :', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_SetupOperatorView', 'fr-FR', N'Paramétrer la vue opérateur', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_SetupOperatorView', 'en-US', N'Set up the operator view', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_SetupOperatorView', 'pt-BR', N'Set up the operator view', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_SetupOperatorView', 'es-ES', N'Set up the operator view', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_SetupOperatorView', 'de-DE', N'Set up the operator view', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_SetupOperatorView', 'pl-PL', N'Set up the operator view', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Category', 'fr-FR', N'Catégorie', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Category', 'en-US', N'Category', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Category', 'pt-BR', N'Categoria', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Category', 'es-ES', N'Categoría', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Category', 'de-DE', N'Kategorie', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Category', 'pl-PL', N'Kategoria', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Consumable', 'fr-FR', N'Consommable', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Consumable', 'en-US', N'Consumable', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Consumable', 'pt-BR', N'Consumíveis', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Consumable', 'es-ES', N'Consumible', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Consumable', 'de-DE', N'Verbrauchsmaterial', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Consumable', 'pl-PL', N'Eksploatacyjne', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Document', 'fr-FR', N'Document', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Document', 'en-US', N'Document', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Document', 'pt-BR', N'Documento', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Document', 'es-ES', N'Documento', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Document', 'de-DE', N'Dokument', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Document', 'pl-PL', N'Dokument', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Duration', 'fr-FR', N'Durée', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Duration', 'en-US', N'Duration', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Duration', 'pt-BR', N'Duração', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Duration', 'es-ES', N'Duración', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Duration', 'de-DE', N'Dauer', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Duration', 'pl-PL', N'Czas trwania', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Improvement', 'fr-FR', N'Amélioration', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Improvement', 'en-US', N'Improvement', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Improvement', 'pt-BR', N'Melhoria', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Improvement', 'es-ES', N'Mejora', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Improvement', 'de-DE', N'Verbesserung', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Improvement', 'pl-PL', N'Usprawnienie', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_IsRandom', 'fr-FR', N'Aléa', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_IsRandom', 'en-US', N'Issue', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_IsRandom', 'pt-BR', N'Questão', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_IsRandom', 'es-ES', N'Asunto', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_IsRandom', 'de-DE', N'Gefahr', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_IsRandom', 'pl-PL', N'Kwestia', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Place', 'fr-FR', N'Lieu', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Place', 'en-US', N'Location', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Place', 'pt-BR', N'Localização', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Place', 'es-ES', N'Lugar', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Place', 'de-DE', N'Ort', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Place', 'pl-PL', N'Lokalizacja', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Predecessors', 'fr-FR', N'Prédécesseurs', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Predecessors', 'en-US', N'Predecessors', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Predecessors', 'pt-BR', N'Antecessores', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Predecessors', 'es-ES', N'Antecesores', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Predecessors', 'de-DE', N'Vorgänger', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Predecessors', 'pl-PL', N'Poprzednik', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Resource', 'fr-FR', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Resource', 'en-US', N'Resource', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Resource', 'pt-BR', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Resource', 'es-ES', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Resource', 'de-DE', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Resource', 'pl-PL', N'Zasoby', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Task', 'fr-FR', N'Tâche', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Task', 'en-US', N'Task', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Task', 'pt-BR', N'Tarefa', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Task', 'es-ES', N'Tarea', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Task', 'de-DE', N'Aufgabe', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Task', 'pl-PL', N'Zadanie', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Tool', 'fr-FR', N'Outil', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Tool', 'en-US', N'Tool', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Tool', 'pt-BR', N'Ferramenta', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Tool', 'es-ES', N'Herramienta', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Tool', 'de-DE', N'Werkzeug', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Tool', 'pl-PL', N'Narzędzie', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Thumbnail', 'fr-FR', N'Photo', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Thumbnail', 'en-US', N'Thumbnail', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Thumbnail', 'pt-BR', N'Thumbnail', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Thumbnail', 'es-ES', N'Thumbnail', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Thumbnail', 'de-DE', N'Thumbnail', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Thumbnail', 'pl-PL', N'Thumbnail', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Video', 'fr-FR', N'Vidéo', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Video', 'en-US', N'Video', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Video', 'pt-BR', N'Vídeo', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Video', 'es-ES', N'Vídeo', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Video', 'de-DE', N'Video', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_Video', 'pl-PL', N'Film', null;
GO
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_WBS', 'fr-FR', N'ID', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_WBS', 'en-US', N'ID', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_WBS', 'pt-BR', N'ID', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_WBS', 'es-ES', N'ID', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_WBS', 'de-DE', N'ID', null;
EXEC InsertOrUpdateResource 'View_PublishFormat_Column_WBS', 'pl-PL', N'Lp', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_VideoExportIsEnabled', 'fr-FR', N'Activer l''export des vidéos', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VideoExportIsEnabled', 'en-US', N'Activate video export', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VideoExportIsEnabled', 'pt-BR', N'Activate video export', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VideoExportIsEnabled', 'es-ES', N'Activate video export', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VideoExportIsEnabled', 'de-DE', N'Activate video export', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VideoExportIsEnabled', 'pl-PL', N'Activate video export', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_OnlyVideosOfKeyTasks', 'fr-FR', N'Exporter uniquement les tâches importantes', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_OnlyVideosOfKeyTasks', 'en-US', N'Export only videos of key tasks', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_OnlyVideosOfKeyTasks', 'pt-BR', N'Export only videos of key tasks', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_OnlyVideosOfKeyTasks', 'es-ES', N'Export only videos of key tasks', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_OnlyVideosOfKeyTasks', 'de-DE', N'Export only videos of key tasks', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_OnlyVideosOfKeyTasks', 'pl-PL', N'Export only videos of key tasks', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_SlowMotionIsEnabled', 'fr-FR', N'SlowMotion', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SlowMotionIsEnabled', 'en-US', N'SlowMotion', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SlowMotionIsEnabled', 'pt-BR', N'SlowMotion', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SlowMotionIsEnabled', 'es-ES', N'SlowMotion', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SlowMotionIsEnabled', 'de-DE', N'SlowMotion', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SlowMotionIsEnabled', 'pl-PL', N'SlowMotion', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_MinDurationVideo', 'fr-FR', N'Durée mini (secondes)', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_MinDurationVideo', 'en-US', N'Duration mini (seconds)', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_MinDurationVideo', 'pt-BR', N'Duration mini (seconds)', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_MinDurationVideo', 'es-ES', N'Duration mini (seconds)', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_MinDurationVideo', 'de-DE', N'Duration mini (seconds)', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_MinDurationVideo', 'pl-PL', N'Duration mini (seconds)', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_VideoMarkingEnable', 'fr-FR', N'Watermarking', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VideoMarkingEnable', 'en-US', N'Watermarking', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VideoMarkingEnable', 'pt-BR', N'Watermarking', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VideoMarkingEnable', 'es-ES', N'Watermarking', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VideoMarkingEnable', 'de-DE', N'Watermarking', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VideoMarkingEnable', 'pl-PL', N'Watermarking', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_ProjectLabelMarkingIsEnabled', 'fr-FR', N'Ajouter le nom du projet dans le watermark', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_ProjectLabelMarkingIsEnabled', 'en-US', N'Add project name in the watermark', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_ProjectLabelMarkingIsEnabled', 'pt-BR', N'Add project name in the watermark', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_ProjectLabelMarkingIsEnabled', 'es-ES', N'Add project name in the watermark', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_ProjectLabelMarkingIsEnabled', 'de-DE', N'Add project name in the watermark', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_ProjectLabelMarkingIsEnabled', 'pl-PL', N'Add project name in the watermark', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_WBSMarkingIsEnabled', 'fr-FR', N'Ajouter l''ID de la tâche dans le watermark', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_WBSMarkingIsEnabled', 'en-US', N'Add the ID of the task in the watermark', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_WBSMarkingIsEnabled', 'pt-BR', N'Add the ID of the task in the watermark', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_WBSMarkingIsEnabled', 'es-ES', N'Add the ID of the task in the watermark', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_WBSMarkingIsEnabled', 'de-DE', N'Add the ID of the task in the watermark', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_WBSMarkingIsEnabled', 'pl-PL', N'Add the ID of the task in the watermark', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_VerticalAlignement', 'fr-FR', N'Alignement vertical', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VerticalAlignement', 'en-US', N'Vertical alignement', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VerticalAlignement', 'pt-BR', N'Vertical alignement', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VerticalAlignement', 'es-ES', N'Vertical alignement', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VerticalAlignement', 'de-DE', N'Vertical alignement', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_VerticalAlignement', 'pl-PL', N'Vertical alignement', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_HorizontalAlignement', 'fr-FR', N'Alignement horizontal', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_HorizontalAlignement', 'en-US', N'Horizontal alignment', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_HorizontalAlignement', 'pt-BR', N'Horizontal alignment', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_HorizontalAlignement', 'es-ES', N'Horizontal alignment', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_HorizontalAlignement', 'de-DE', N'Horizontal alignment', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_HorizontalAlignement', 'pl-PL', N'Horizontal alignment', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_SendingVideosToServer', 'fr-FR', N'Envoi des vidéos brutes vers le serveur', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SendingVideosToServer', 'en-US', N'Sending original videos to server', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SendingVideosToServer', 'pt-BR', N'Sending original videos to server', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SendingVideosToServer', 'es-ES', N'Sending original videos to server', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SendingVideosToServer', 'de-DE', N'Sending original videos to server', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SendingVideosToServer', 'pl-PL', N'Sending original videos to server', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_SendingVideosToServerProgress', 'fr-FR', N'Envoi des vidéos brutes vers le serveur : {0}%', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SendingVideosToServerProgress', 'en-US', N'Sending original videos to server : {0}%', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SendingVideosToServerProgress', 'pt-BR', N'Sending original videos to server : {0}%', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SendingVideosToServerProgress', 'es-ES', N'Sending original videos to server : {0}%', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SendingVideosToServerProgress', 'de-DE', N'Sending original videos to server : {0}%', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SendingVideosToServerProgress', 'pl-PL', N'Sending original videos to server : {0}%', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_SavingPreferences', 'fr-FR', N'Sauvegarde des préférences', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SavingPreferences', 'en-US', N'Saving preferences', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SavingPreferences', 'pt-BR', N'Saving preferences', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SavingPreferences', 'es-ES', N'Saving preferences', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SavingPreferences', 'de-DE', N'Saving preferences', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_SavingPreferences', 'pl-PL', N'Saving preferences', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_Publishing', 'fr-FR', N'Publication en cours...', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_Publishing', 'en-US', N'Publishing...', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_Publishing', 'pt-BR', N'Publishing...', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_Publishing', 'es-ES', N'Publishing...', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_Publishing', 'de-DE', N'Publishing...', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_Publishing', 'pl-PL', N'Publishing...', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_PublishingError', 'fr-FR', N'Une erreur s''est produite lors de la publication', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_PublishingError', 'en-US', N'An error raised while publishing', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_PublishingError', 'pt-BR', N'An error raised while publishing', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_PublishingError', 'es-ES', N'An error raised while publishing', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_PublishingError', 'de-DE', N'An error raised while publishing', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_PublishingError', 'pl-PL', N'An error raised while publishing', null;
GO
EXEC InsertOrUpdateResource 'View_PublishVideos_PublishThumbnailsError', 'fr-FR', N'La création des vignettes suivantes n''a pas abouti :
{0}', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_PublishThumbnailsError', 'en-US', N'The following thumbnails were not created :
{0}', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_PublishThumbnailsError', 'pt-BR', N'The following thumbnails were not created :
{0}', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_PublishThumbnailsError', 'es-ES', N'The following thumbnails were not created :
{0}', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_PublishThumbnailsError', 'de-DE', N'The following thumbnails were not created :
{0}', null;
EXEC InsertOrUpdateResource 'View_PublishVideos_PublishThumbnailsError', 'pl-PL', N'The following thumbnails were not created :
{0}', null;
GO
EXEC InsertOrUpdateResource 'View_Restitution_AbsoluteValues', 'fr-FR', N'Durées cumulées', null;
EXEC InsertOrUpdateResource 'View_Restitution_AbsoluteValues', 'en-US', N'Accumulated times', null;
EXEC InsertOrUpdateResource 'View_Restitution_AbsoluteValues', 'pt-BR', N'Vezes acumuladas', null;
EXEC InsertOrUpdateResource 'View_Restitution_AbsoluteValues', 'es-ES', N'Duracion acumulada', null;
EXEC InsertOrUpdateResource 'View_Restitution_AbsoluteValues', 'de-DE', N'Kumulierte Zeiten', null;
EXEC InsertOrUpdateResource 'View_Restitution_AbsoluteValues', 'pl-PL', N'Skumulowany czas', null;
GO
EXEC InsertOrUpdateResource 'View_Restitution_OccurenceValues', 'fr-FR', N'Occurrences', null;
EXEC InsertOrUpdateResource 'View_Restitution_OccurenceValues', 'en-US', N'Occurrences', null;
EXEC InsertOrUpdateResource 'View_Restitution_OccurenceValues', 'pt-BR', N'Ocorrências', null;
EXEC InsertOrUpdateResource 'View_Restitution_OccurenceValues', 'es-ES', N'Occurrencias', null;
EXEC InsertOrUpdateResource 'View_Restitution_OccurenceValues', 'de-DE', N'Häufigkeiten', null;
EXEC InsertOrUpdateResource 'View_Restitution_OccurenceValues', 'pl-PL', N'Zdarzenie', null;
GO
EXEC InsertOrUpdateResource 'View_Restitution_RelativeValues', 'fr-FR', N'Pourcentages', null;
EXEC InsertOrUpdateResource 'View_Restitution_RelativeValues', 'en-US', N'Percentages', null;
EXEC InsertOrUpdateResource 'View_Restitution_RelativeValues', 'pt-BR', N'Porcentagens', null;
EXEC InsertOrUpdateResource 'View_Restitution_RelativeValues', 'es-ES', N'Porcentajes', null;
EXEC InsertOrUpdateResource 'View_Restitution_RelativeValues', 'de-DE', N'Prozentsätze', null;
EXEC InsertOrUpdateResource 'View_Restitution_RelativeValues', 'pl-PL', N'Procentowo', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_Columns_Consumable', 'fr-FR', N'Consommable', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_Columns_Consumable', 'en-US', N'Consumable', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_Columns_Consumable', 'pt-BR', N'Consumível', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_Columns_Consumable', 'es-ES', N'Consumible', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_Columns_Consumable', 'de-DE', N'Verbrauchsmaterial', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_Columns_Consumable', 'pl-PL', N'Eksploatacyjne', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_Columns_Resource', 'fr-FR', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_Columns_Resource', 'en-US', N'Resource', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_Columns_Resource', 'pt-BR', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_Columns_Resource', 'es-ES', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_Columns_Resource', 'de-DE', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_Columns_Resource', 'pl-PL', N'Zasoby', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_WithoutConsumable', 'fr-FR', N'Sans consommable', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_WithoutConsumable', 'en-US', N'Without consumable', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_WithoutConsumable', 'pt-BR', N'Sem consumível', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_WithoutConsumable', 'es-ES', N'No consumible', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_WithoutConsumable', 'de-DE', N'Ohne Verbrauchsmaterial', null;
EXEC InsertOrUpdateResource 'View_RestitutionConsumables_WithoutConsumable', 'pl-PL', N'Bez eksploatacji', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Columns_Document', 'fr-FR', N'Document', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Columns_Document', 'en-US', N'Document', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Columns_Document', 'pt-BR', N'Documento', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Columns_Document', 'es-ES', N'Documento', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Columns_Document', 'de-DE', N'Dokument', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Columns_Document', 'pl-PL', N'Dokumet', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Equipments', 'fr-FR', N'Equipements', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Equipments', 'en-US', N'Equipments', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Equipments', 'pt-BR', N'Equipamentos', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Equipments', 'es-ES', N'Equipo', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Equipments', 'de-DE', N'Ausrüstungen', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Equipments', 'pl-PL', N'Sprzęty', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Operators', 'fr-FR', N'Opérateurs', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Operators', 'en-US', N'Operators', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Operators', 'pt-BR', N'Operadores', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Operators', 'es-ES', N'Los operadores', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Operators', 'de-DE', N'Operatoren', null;
EXEC InsertOrUpdateResource 'View_RestitutionDocuments_Operators', 'pl-PL', N'Operatorzy', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_Columns_Place', 'fr-FR', N'Lieu', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_Columns_Place', 'en-US', N'Location', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_Columns_Place', 'pt-BR', N'Localização', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_Columns_Place', 'es-ES', N'Lugar', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_Columns_Place', 'de-DE', N'Ort', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_Columns_Place', 'pl-PL', N'Lokalizacja', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_Columns_Resource', 'fr-FR', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_Columns_Resource', 'en-US', N'Resource', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_Columns_Resource', 'pt-BR', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_Columns_Resource', 'es-ES', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_Columns_Resource', 'de-DE', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_Columns_Resource', 'pl-PL', N'Zasoby', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_WithoutPlace', 'fr-FR', N'Sans lieu', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_WithoutPlace', 'en-US', N'Without location', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_WithoutPlace', 'pt-BR', N'Sem localização', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_WithoutPlace', 'es-ES', N'Sin lugar', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_WithoutPlace', 'de-DE', N'Kein Ort', null;
EXEC InsertOrUpdateResource 'View_RestitutionPlaces_WithoutPlace', 'pl-PL', N'Bez lokalizacji', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Comments', 'fr-FR', N'Commentaires', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Comments', 'en-US', N'Comments', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Comments', 'pt-BR', N'Comentários', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Comments', 'es-ES', N'Comentarios', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Comments', 'de-DE', N'Hinweise', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Comments', 'pl-PL', N'Komentarze', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Cost', 'fr-FR', N'Coût', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Cost', 'en-US', N'Cost', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Cost', 'pt-BR', N'Custo', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Cost', 'es-ES', N'Coste', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Cost', 'de-DE', N'Kosten', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Cost', 'pl-PL', N'Koszt', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Cost_Tooltip', 'fr-FR', N'1 : Peu coûteux
2 : Moyennement coûteux
3 : Coûteux', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Cost_Tooltip', 'en-US', N'1: Cheap
2: Moderate
3: Expensive', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Cost_Tooltip', 'pt-BR', N'1: Barato
2: Custo moderado
3: Custo alto', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Cost_Tooltip', 'es-ES', N'1 : Barato
2 : Coste moderado
3 : Caro', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Cost_Tooltip', 'de-DE', N'1: Preiswert
2: Moderrater Preis
3: Teuer', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Cost_Tooltip', 'pl-PL', N'1 : Tani
2 : Średni
3 : Drogi', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_DCG', 'fr-FR', N'DC/G', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_DCG', 'en-US', N'DC/T', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_DCG', 'pt-BR', N'DC / E', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_DCG', 'es-ES', N'DC / T', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_DCG', 'de-DE', N'SK/Einsp.', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_DCG', 'pl-PL', N'TK/Z', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_DCG_Tooltip', 'fr-FR', N'Difficulté x Coût / Gain en minutes', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_DCG_Tooltip', 'en-US', N'Difficulty x Cost / Time saved', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_DCG_Tooltip', 'pt-BR', N'Dificuldade x Custo / Economia', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_DCG_Tooltip', 'es-ES', N'Dificultad x Coste / Tiempo ahorrado', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_DCG_Tooltip', 'de-DE', N'Schwierigkeitsgrad x Kosten /  Einsparung in Minuten', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_DCG_Tooltip', 'pl-PL', N'Trudność x Koszt / Czas zaoszczędzony', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Diffculty', 'fr-FR', N'Difficulté', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Diffculty', 'en-US', N'Difficulty', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Diffculty', 'pt-BR', N'Dificuldade', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Diffculty', 'es-ES', N'Dificultad', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Diffculty', 'de-DE', N'Schwierigkeitsgrad', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Diffculty', 'pl-PL', N'Trudność', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Difficulty_Tooltip', 'fr-FR', N'1 : Facile
2 : Moyen
3 : Difficile', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Difficulty_Tooltip', 'en-US', N'1: Easy
2: Medium
3: Difficult', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Difficulty_Tooltip', 'pt-BR', N'1: Fácil
2: Médio
3: Difícil', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Difficulty_Tooltip', 'es-ES', N'1 : Facil
2 : Medio
3 : Dificil', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Difficulty_Tooltip', 'de-DE', N'1: Einfach
2: Durchschnittlich
3: Schwierig', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Difficulty_Tooltip', 'pl-PL', N'1 : Łatwy
2 : Średni
3 : Trudny', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_IG', 'fr-FR', N'I/G', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_IG', 'en-US', N'I/T', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_IG', 'pt-BR', N'I / E', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_IG', 'es-ES', N'I/T', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_IG', 'de-DE', N'Inv /Einsp.', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_IG', 'pl-PL', N'I/Z', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_IG_Tooltip', 'fr-FR', N'Investissement / Gain en minutes', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_IG_Tooltip', 'en-US', N'Investment / Time saved', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_IG_Tooltip', 'pt-BR', N'Investimento / Economia em minutos', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_IG_Tooltip', 'es-ES', N'Inversión / Tiempo ahorrado', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_IG_Tooltip', 'de-DE', N'Investment / Einsparung in Minuten', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_IG_Tooltip', 'pl-PL', N'Inwestycja / Zaoszczędzony czas', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Investment', 'fr-FR', N'Investissement', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Investment', 'en-US', N'Investment', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Investment', 'pt-BR', N'Investimento', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Investment', 'es-ES', N'Inversión', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Investment', 'de-DE', N'Investition', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Investment', 'pl-PL', N'Inwestycja', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_OK', 'fr-FR', N'Ok', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_OK', 'en-US', N'OK', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_OK', 'pt-BR', N'OK', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_OK', 'es-ES', N'Ok', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_OK', 'de-DE', N'Ok', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_OK', 'pl-PL', N'OK', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_RelatedActions', 'fr-FR', N'Tâches liées', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_RelatedActions', 'en-US', N'Linked tasks', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_RelatedActions', 'pt-BR', N'Tarefas relacionadas', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_RelatedActions', 'es-ES', N'Las tareas relacionadas con', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_RelatedActions', 'de-DE', N'Verbundene Aufgaben', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_RelatedActions', 'pl-PL', N'Połączone zadania', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Saving', 'fr-FR', N'Gain de temps', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Saving', 'en-US', N'Time saved', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Saving', 'pt-BR', N'Economia', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Saving', 'es-ES', N'Tiempo ahorrado', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Saving', 'de-DE', N'eingesparte Zeit', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Saving', 'pl-PL', N'Czas zaoszczędzony', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Scenario', 'fr-FR', N'Scénario', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Scenario', 'en-US', N'Scenario', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Scenario', 'pt-BR', N'Cenário', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Scenario', 'es-ES', N'Escenario', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Scenario', 'de-DE', N'Szenario', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Scenario', 'pl-PL', N'Scenariusz', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Solution', 'fr-FR', N'Solution', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Solution', 'en-US', N'Solution', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Solution', 'pt-BR', N'Solução', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Solution', 'es-ES', N'Solución', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Solution', 'de-DE', N'Lösung', null;
EXEC InsertOrUpdateResource 'View_RestitutionSolutions_Columns_Solution', 'pl-PL', N'Rozwiązanie', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionTools_Columns_Resource', 'fr-FR', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_Columns_Resource', 'en-US', N'Resource', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_Columns_Resource', 'pt-BR', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_Columns_Resource', 'es-ES', N'Recurso', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_Columns_Resource', 'de-DE', N'Ressource', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_Columns_Resource', 'pl-PL', N'Zasoby', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionTools_Columns_Tool', 'fr-FR', N'Outil', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_Columns_Tool', 'en-US', N'Tool', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_Columns_Tool', 'pt-BR', N'Ferramenta', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_Columns_Tool', 'es-ES', N'Herramienta', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_Columns_Tool', 'de-DE', N'Werkzeug', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_Columns_Tool', 'pl-PL', N'Narzędzie', null;
GO
EXEC InsertOrUpdateResource 'View_RestitutionTools_WithoutTool', 'fr-FR', N'Sans outil', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_WithoutTool', 'en-US', N'Without tool', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_WithoutTool', 'pt-BR', N'Sem ferramenta', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_WithoutTool', 'es-ES', N'Sin herramientas', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_WithoutTool', 'de-DE', N'Ohne Werkzeuge', null;
EXEC InsertOrUpdateResource 'View_RestitutionTools_WithoutTool', 'pl-PL', N'Bez narzędzie', null;
GO
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPath', 'fr-FR', N'Chemin critique', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPath', 'en-US', N'Critical path', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPath', 'pt-BR', N'Caminho crítico', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPath', 'es-ES', N'Camino critico', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPath', 'de-DE', N'kritischer Weg', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPath', 'pl-PL', N'Ścieżka krytyczna', null;
GO
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDuration', 'fr-FR', N'Durée', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDuration', 'en-US', N'Duration', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDuration', 'pt-BR', N'Duração', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDuration', 'es-ES', N'Duración', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDuration', 'de-DE', N'Dauer', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDuration', 'pl-PL', N'Czas trwania', null;
GO
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDurationI', 'fr-FR', N'Durée I', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDurationI', 'en-US', N'I duration', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDurationI', 'pt-BR', N'Duração I', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDurationI', 'es-ES', N'Duración I', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDurationI', 'de-DE', N'Dauer I', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDurationI', 'pl-PL', N'W czas trwania', null;
GO
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDurationIE', 'fr-FR', N'Durée I+E', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDurationIE', 'en-US', N'I+E duration', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDurationIE', 'pt-BR', N'Duração I + E', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDurationIE', 'es-ES', N'Duración I + E', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDurationIE', 'de-DE', N'Dauer I + E', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathDurationIE', 'pl-PL', N'W+Z czas trwania', null;
GO
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarning', 'fr-FR', N'% Gain de temps', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarning', 'en-US', N'% Duration saved', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarning', 'pt-BR', N'Economia', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarning', 'es-ES', N'% Tiempo ahorrado', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarning', 'de-DE', N'Zeiteinsparung in %', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarning', 'pl-PL', N'% oszczędności czasu trwania', null;
GO
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarningI', 'fr-FR', N'% Gain de temps I', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarningI', 'en-US', N'% I duration saved', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarningI', 'pt-BR', N'Economia I', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarningI', 'es-ES', N'% Tiempo ahorrado I', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarningI', 'de-DE', N'I Zeiteinsparung in %', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarningI', 'pl-PL', N'% zaoszczędzonego czasu trwania W', null;
GO
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarningIE', 'fr-FR', N'% Gain de temps I+E', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarningIE', 'en-US', N'% I+E duration saved', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarningIE', 'pt-BR', N'Economia I + E', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarningIE', 'es-ES', N'% Tiempo ahorrado de I + E', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarningIE', 'de-DE', N'I + E Zeiteinsparung in %', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_CriticalPathEarningIE', 'pl-PL', N'% zaoszczędzonego czasu trwania W+Z', null;
GO
EXEC InsertOrUpdateResource 'View_Shared_Summary_Original', 'fr-FR', N'Scénario d''origine', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_Original', 'en-US', N'Original scenario', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_Original', 'pt-BR', N'Cenário inicial', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_Original', 'es-ES', N'Escenario Original', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_Original', 'de-DE', N'Originalszenario', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_Original', 'pl-PL', N'Scenariusz oryginalny', null;
GO
EXEC InsertOrUpdateResource 'View_Shared_Summary_Values', 'fr-FR', N'VA/BNVA/NVA/- (%)', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_Values', 'en-US', N'VA/BNVA/NVA/- (%)', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_Values', 'pt-BR', N'VA/BNVA/NVA/- (%)', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_Values', 'es-ES', N'VA/BNVA/NVA/- (%)', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_Values', 'de-DE', N'VA/BNVA/NVA/- (%)', null;
EXEC InsertOrUpdateResource 'View_Shared_Summary_Values', 'pl-PL', N'VA/BNVA/NVA/- (%)', null;
GO
EXEC InsertOrUpdateResource 'View_Shared_Summary_Values_StringFormat', 'fr-FR', N'{0:0}/{1:0}/{2:0}/{3:0}', N'0 : VA, 1 : BNVA, 2 : NVA, 3 : -';
EXEC InsertOrUpdateResource 'View_Shared_Summary_Values_StringFormat', 'en-US', N'{0:0}/{1:0}/{2:0}/{3:0}', N'0 : VA, 1 : BNVA, 2 : NVA, 3 : -';
EXEC InsertOrUpdateResource 'View_Shared_Summary_Values_StringFormat', 'pt-BR', N'{0:0}/{1:0}/{2:0}/{3:0}', N'0 : VA, 1 : BNVA, 2 : NVA, 3 : -';
EXEC InsertOrUpdateResource 'View_Shared_Summary_Values_StringFormat', 'es-ES', N'{0:0}/{1:0}/{2:0}/{3:0}', N'0 : VA, 1 : BNVA, 2 : NVA, 3 : -';
EXEC InsertOrUpdateResource 'View_Shared_Summary_Values_StringFormat', 'de-DE', N'{0:0}/{1:0}/{2:0}/{3:0}', N'0 : VA, 1 : BNVA, 2 : NVA, 3 : -';
EXEC InsertOrUpdateResource 'View_Shared_Summary_Values_StringFormat', 'pl-PL', N'{0:0}/{1:0}/{2:0}/{3:0}', N'0 : VA, 1 : BNVA, 2 : NVA, 3 : -';
GO
EXEC InsertOrUpdateResource 'View_Simulate_Column_DifferenceReason', 'fr-FR', N'Cause écart', null;
EXEC InsertOrUpdateResource 'View_Simulate_Column_DifferenceReason', 'en-US', N'Gap reason', null;
EXEC InsertOrUpdateResource 'View_Simulate_Column_DifferenceReason', 'pt-BR', N'Causa da diferença', null;
EXEC InsertOrUpdateResource 'View_Simulate_Column_DifferenceReason', 'es-ES', N'Causa desvio', null;
EXEC InsertOrUpdateResource 'View_Simulate_Column_DifferenceReason', 'de-DE', N'Ursache', null;
EXEC InsertOrUpdateResource 'View_Simulate_Column_DifferenceReason', 'pl-PL', N'Przyczyna luk', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildDuration', 'fr-FR', N'Process_duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildDuration', 'en-US', N'Process_duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildDuration', 'pt-BR', N'Process_duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildDuration', 'es-ES', N'Process_duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildDuration', 'de-DE', N'Process_duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildDuration', 'pl-PL', N'Process_duration', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildFinish', 'fr-FR', N'Process_finish', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildFinish', 'en-US', N'Process_finish', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildFinish', 'pt-BR', N'Process_finish', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildFinish', 'es-ES', N'Process_finish', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildFinish', 'de-DE', N'Process_finish', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildFinish', 'pl-PL', N'Process_finish', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildStart', 'fr-FR', N'Process_start', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildStart', 'en-US', N'Process_start', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildStart', 'pt-BR', N'Process_start', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildStart', 'es-ES', N'Process_start', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildStart', 'de-DE', N'Process_start', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_BuildStart', 'pl-PL', N'Process_start', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Category', 'fr-FR', N'Category', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Category', 'en-US', N'Category', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Category', 'pt-BR', N'Category', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Category', 'es-ES', N'Category', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Category', 'de-DE', N'Category', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Category', 'pl-PL', N'Category', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Consumable', 'fr-FR', N'Consumable', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Consumable', 'en-US', N'Consumable', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Consumable', 'pt-BR', N'Consumable', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Consumable', 'es-ES', N'Consumable', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Consumable', 'de-DE', N'Consumable', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Consumable', 'pl-PL', N'Consumable', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric1', 'fr-FR', N'Free numeric field 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric1', 'en-US', N'Free numeric field 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric1', 'pt-BR', N'Free numeric field 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric1', 'es-ES', N'Free numeric field 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric1', 'de-DE', N'Free numeric field 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric1', 'pl-PL', N'Free numeric field 1', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric2', 'fr-FR', N'Free numeric field 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric2', 'en-US', N'Free numeric field 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric2', 'pt-BR', N'Free numeric field 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric2', 'es-ES', N'Free numeric field 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric2', 'de-DE', N'Free numeric field 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric2', 'pl-PL', N'Free numeric field 2', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric3', 'fr-FR', N'Free numeric field 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric3', 'en-US', N'Free numeric field 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric3', 'pt-BR', N'Free numeric field 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric3', 'es-ES', N'Free numeric field 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric3', 'de-DE', N'Free numeric field 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric3', 'pl-PL', N'Free numeric field 3', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric4', 'fr-FR', N'Free numeric field 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric4', 'en-US', N'Free numeric field 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric4', 'pt-BR', N'Free numeric field 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric4', 'es-ES', N'Free numeric field 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric4', 'de-DE', N'Free numeric field 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric4', 'pl-PL', N'Free numeric field 4', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text1', 'fr-FR', N'Free text field 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text1', 'en-US', N'Free text field 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text1', 'pt-BR', N'Free text field 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text1', 'es-ES', N'Free text field 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text1', 'de-DE', N'Free text field 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text1', 'pl-PL', N'Free text field 1', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text2', 'fr-FR', N'Free text field 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text2', 'en-US', N'Free text field 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text2', 'pt-BR', N'Free text field 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text2', 'es-ES', N'Free text field 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text2', 'de-DE', N'Free text field 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text2', 'pl-PL', N'Free text field 2', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text3', 'fr-FR', N'Free text field 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text3', 'en-US', N'Free text field 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text3', 'pt-BR', N'Free text field 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text3', 'es-ES', N'Free text field 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text3', 'de-DE', N'Free text field 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text3', 'pl-PL', N'Free text field 3', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text4', 'fr-FR', N'Free text field 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text4', 'en-US', N'Free text field 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text4', 'pt-BR', N'Free text field 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text4', 'es-ES', N'Free text field 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text4', 'de-DE', N'Free text field 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text4', 'pl-PL', N'Free text field 4', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_DifferenceReason', 'fr-FR', N'Gap_reason', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_DifferenceReason', 'en-US', N'Gap_reason', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_DifferenceReason', 'pt-BR', N'Gap_reason', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_DifferenceReason', 'es-ES', N'Gap_reason', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_DifferenceReason', 'de-DE', N'Gap_reason', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_DifferenceReason', 'pl-PL', N'Gap_reason', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Document', 'fr-FR', N'Document', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Document', 'en-US', N'Document', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Document', 'pt-BR', N'Document', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Document', 'es-ES', N'Document', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Document', 'de-DE', N'Document', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Document', 'pl-PL', N'Document', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Duration', 'fr-FR', N'Video_duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Duration', 'en-US', N'Video_duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Duration', 'pt-BR', N'Video_duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Duration', 'es-ES', N'Video_duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Duration', 'de-DE', N'Video_duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Duration', 'pl-PL', N'Video_duration', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Finish', 'fr-FR', N'Video_finish', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Finish', 'en-US', N'Video_finish', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Finish', 'pt-BR', N'Video_finish', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Finish', 'es-ES', N'Video_finish', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Finish', 'de-DE', N'Video_finish', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Finish', 'pl-PL', N'Video_finish', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_IsRandom', 'fr-FR', N'Issue', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_IsRandom', 'en-US', N'Issue', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_IsRandom', 'pt-BR', N'Issue', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_IsRandom', 'es-ES', N'Issue', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_IsRandom', 'de-DE', N'Issue', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_IsRandom', 'pl-PL', N'Issue', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Original', 'fr-FR', N'Origin', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Original', 'en-US', N'Origin', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Original', 'pt-BR', N'Origin', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Original', 'es-ES', N'Origin', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Original', 'de-DE', N'Origin', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Original', 'pl-PL', N'Origin', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Place', 'fr-FR', N'Location', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Place', 'en-US', N'Location', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Place', 'pt-BR', N'Location', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Place', 'es-ES', N'Location', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Place', 'de-DE', N'Location', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Place', 'pl-PL', N'Location', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Predecessors', 'fr-FR', N'Predecessor', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Predecessors', 'en-US', N'Predecessor', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Predecessors', 'pt-BR', N'Predecessor', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Predecessors', 'es-ES', N'Predecessor', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Predecessors', 'de-DE', N'Predecessor', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Predecessors', 'pl-PL', N'Predecessor', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Deleted', 'fr-FR', N'Deleted', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Deleted', 'en-US', N'Deleted', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Deleted', 'pt-BR', N'Deleted', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Deleted', 'es-ES', N'Deleted', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Deleted', 'de-DE', N'Deleted', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Deleted', 'pl-PL', N'Deleted', N'export standardisé de I E S';
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_External', 'fr-FR', N'External', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_External', 'en-US', N'External', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_External', 'pt-BR', N'External', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_External', 'es-ES', N'External', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_External', 'de-DE', N'External', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_External', 'pl-PL', N'External', N'export standardisé de I E S';
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_IES', 'fr-FR', N'Improved_I_E_D', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_IES', 'en-US', N'Improved_I_E_D', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_IES', 'pt-BR', N'Improved_I_E_D', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_IES', 'es-ES', N'Improved_I_E_D', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_IES', 'de-DE', N'Improved_I_E_D', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_IES', 'pl-PL', N'Improved_I_E_D', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Internal', 'fr-FR', N'Internal', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Internal', 'en-US', N'Internal', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Internal', 'pt-BR', N'Internal', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Internal', 'es-ES', N'Internal', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Internal', 'de-DE', N'Internal', N'export standardisé de I E S';
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Internal', 'pl-PL', N'Internal', N'export standardisé de I E S';
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_ReductionRatio', 'fr-FR', N'perc_reduction', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_ReductionRatio', 'en-US', N'perc_reduction', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_ReductionRatio', 'pt-BR', N'perc_reduction', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_ReductionRatio', 'es-ES', N'perc_reduction', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_ReductionRatio', 'de-DE', N'perc_reduction', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_ReductionRatio', 'pl-PL', N'perc_reduction', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Solution', 'fr-FR', N'Solution', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Solution', 'en-US', N'Solution', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Solution', 'pt-BR', N'Solution', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Solution', 'es-ES', N'Solution', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Solution', 'de-DE', N'Solution', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Reduced_Solution', 'pl-PL', N'Solution', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Resource', 'fr-FR', N'Resource', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Resource', 'en-US', N'Resource', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Resource', 'pt-BR', N'Resource', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Resource', 'es-ES', N'Resource', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Resource', 'de-DE', N'Resource', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Resource', 'pl-PL', N'Resource', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Start', 'fr-FR', N'Video_start', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Start', 'en-US', N'Video_start', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Start', 'pt-BR', N'Video_start', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Start', 'es-ES', N'Video_start', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Start', 'de-DE', N'Video_start', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Start', 'pl-PL', N'Video_start', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Task', 'fr-FR', N'Task', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Task', 'en-US', N'Task', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Task', 'pt-BR', N'Task', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Task', 'es-ES', N'Task', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Task', 'de-DE', N'Task', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Task', 'pl-PL', N'Task', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Tool', 'fr-FR', N'Tool', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Tool', 'en-US', N'Tool', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Tool', 'pt-BR', N'Tool', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Tool', 'es-ES', N'Tool', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Tool', 'de-DE', N'Tool', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Tool', 'pl-PL', N'Tool', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Value', 'fr-FR', N'Value', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Value', 'en-US', N'Value', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Value', 'pt-BR', N'Value', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Value', 'es-ES', N'Value', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Value', 'de-DE', N'Value', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Value', 'pl-PL', N'Value', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Video', 'fr-FR', N'Video', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Video', 'en-US', N'Video', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Video', 'pt-BR', N'Video', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Video', 'es-ES', N'Video', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Video', 'de-DE', N'Video', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_Video', 'pl-PL', N'Video', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_WBS', 'fr-FR', N'ID', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_WBS', 'en-US', N'ID', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_WBS', 'pt-BR', N'ID', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_WBS', 'es-ES', N'ID', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_WBS', 'de-DE', N'ID', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Action_WBS', 'pl-PL', N'ID', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Members', 'fr-FR', N'Members', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Members', 'en-US', N'Members', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Members', 'pt-BR', N'Members', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Members', 'es-ES', N'Members', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Members', 'de-DE', N'Members', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Members', 'pl-PL', N'Members', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project', 'fr-FR', N'Project', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project', 'en-US', N'Project', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project', 'pt-BR', N'Project', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project', 'es-ES', N'Project', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project', 'de-DE', N'Project', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project', 'pl-PL', N'Project', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_ApplicationVersion', 'fr-FR', N'Application version', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_ApplicationVersion', 'en-US', N'Application version', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_ApplicationVersion', 'pt-BR', N'Application version', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_ApplicationVersion', 'es-ES', N'Application version', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_ApplicationVersion', 'de-DE', N'Application version', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_ApplicationVersion', 'pl-PL', N'Application version', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric1', 'fr-FR', N'Free numeric field label 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric1', 'en-US', N'Free numeric field label 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric1', 'pt-BR', N'Free numeric field label 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric1', 'es-ES', N'Free numeric field label 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric1', 'de-DE', N'Free numeric field label 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric1', 'pl-PL', N'Free numeric field label 1', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric2', 'fr-FR', N'Free numeric field label 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric2', 'en-US', N'Free numeric field label 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric2', 'pt-BR', N'Free numeric field label 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric2', 'es-ES', N'Free numeric field label 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric2', 'de-DE', N'Free numeric field label 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric2', 'pl-PL', N'Free numeric field label 2', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric3', 'fr-FR', N'Free numeric field label 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric3', 'en-US', N'Free numeric field label 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric3', 'pt-BR', N'Free numeric field label 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric3', 'es-ES', N'Free numeric field label 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric3', 'de-DE', N'Free numeric field label 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric3', 'pl-PL', N'Free numeric field label 3', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric4', 'fr-FR', N'Free numeric field label 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric4', 'en-US', N'Free numeric field label 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric4', 'pt-BR', N'Free numeric field label 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric4', 'es-ES', N'Free numeric field label 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric4', 'de-DE', N'Free numeric field label 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric4', 'pl-PL', N'Free numeric field label 4', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text1', 'fr-FR', N'Free text field label 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text1', 'en-US', N'Free text field label 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text1', 'pt-BR', N'Free text field label 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text1', 'es-ES', N'Free text field label 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text1', 'de-DE', N'Free text field label 1', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text1', 'pl-PL', N'Free text field label 1', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text2', 'fr-FR', N'Free text field label 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text2', 'en-US', N'Free text field label 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text2', 'pt-BR', N'Free text field label 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text2', 'es-ES', N'Free text field label 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text2', 'de-DE', N'Free text field label 2', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text2', 'pl-PL', N'Free text field label 2', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text3', 'fr-FR', N'Free text field label 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text3', 'en-US', N'Free text field label 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text3', 'pt-BR', N'Free text field label 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text3', 'es-ES', N'Free text field label 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text3', 'de-DE', N'Free text field label 3', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text3', 'pl-PL', N'Free text field label 3', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text4', 'fr-FR', N'Free text field label 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text4', 'en-US', N'Free text field label 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text4', 'pt-BR', N'Free text field label 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text4', 'es-ES', N'Free text field label 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text4', 'de-DE', N'Free text field label 4', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text4', 'pl-PL', N'Free text field label 4', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Objective', 'fr-FR', N'Objective', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Objective', 'en-US', N'Objective', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Objective', 'pt-BR', N'Objective', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Objective', 'es-ES', N'Objective', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Objective', 'de-DE', N'Objective', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Objective', 'pl-PL', N'Objective', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Precision', 'fr-FR', N'Precision (ms)', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Precision', 'en-US', N'Precision (ms)', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Precision', 'pt-BR', N'Precision (ms)', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Precision', 'es-ES', N'Precision (ms)', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Precision', 'de-DE', N'Precision (ms)', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Precision', 'pl-PL', N'Precision (ms)', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Workshop', 'fr-FR', N'Scope', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Workshop', 'en-US', N'Scope', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Workshop', 'pt-BR', N'Scope', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Workshop', 'es-ES', N'Scope', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Workshop', 'de-DE', N'Scope', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Project_Workshop', 'pl-PL', N'Scope', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Referentials', 'fr-FR', N'Referentials', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Referentials', 'en-US', N'Referentials', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Referentials', 'pt-BR', N'Referentials', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Referentials', 'es-ES', N'Referentials', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Referentials', 'de-DE', N'Referentials', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Referentials', 'pl-PL', N'Referentials', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Scenario_Original', 'fr-FR', N'Original scenario', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Scenario_Original', 'en-US', N'Original scenario', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Scenario_Original', 'pt-BR', N'Original scenario', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Scenario_Original', 'es-ES', N'Original scenario', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Scenario_Original', 'de-DE', N'Original scenario', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Scenario_Original', 'pl-PL', N'Original scenario', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Video_Duration', 'fr-FR', N'Duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Video_Duration', 'en-US', N'Duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Video_Duration', 'pt-BR', N'Duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Video_Duration', 'es-ES', N'Duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Video_Duration', 'de-DE', N'Duration', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Video_Duration', 'pl-PL', N'Duration', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Video_POV', 'fr-FR', N'Video linked to a dedicated resource (operator or equipment)', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Video_POV', 'en-US', N'Video linked to a dedicated resource (operator or equipment)', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Video_POV', 'pt-BR', N'Video linked to a dedicated resource (operator or equipment)', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Video_POV', 'es-ES', N'Video linked to a dedicated resource (operator or equipment)', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Video_POV', 'de-DE', N'Video linked to a dedicated resource (operator or equipment)', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Video_POV', 'pl-PL', N'Video linked to a dedicated resource (operator or equipment)', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Videos', 'fr-FR', N'Videos', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Videos', 'en-US', N'Videos', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Videos', 'pt-BR', N'Videos', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Videos', 'es-ES', N'Videos', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Videos', 'de-DE', N'Videos', null;
EXEC InsertOrUpdateResource 'ViewModel_AnalyzeRestitution_Export_Videos', 'pl-PL', N'Videos', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_Restitution_EmptyReferential', 'fr-FR', N'Non défini', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_EmptyReferential', 'en-US', N'Not defined', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_EmptyReferential', 'pt-BR', N'Não definido', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_EmptyReferential', 'es-ES', N'Indefinido', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_EmptyReferential', 'de-DE', N'Undefiniert', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_EmptyReferential', 'pl-PL', N'Niezdefiniowany', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Equipments', 'fr-FR', N'Equipements', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Equipments', 'en-US', N'Equipments', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Equipments', 'pt-BR', N'Equipamentos', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Equipments', 'es-ES', N'Equipo', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Equipments', 'de-DE', N'Ausrüstungen', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Equipments', 'pl-PL', N'Sprzęty', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Operators', 'fr-FR', N'Opérateurs', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Operators', 'en-US', N'Operators', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Operators', 'pt-BR', N'Operadores', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Operators', 'es-ES', N'Los operadores', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Operators', 'de-DE', N'Operatoren', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Operators', 'pl-PL', N'Operatorzy', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Total', 'fr-FR', N'Total', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Total', 'en-US', N'Sum', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Total', 'pt-BR', N'Total', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Total', 'es-ES', N'Total', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Total', 'de-DE', N'Gesamt', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_Total', 'pl-PL', N'Suma', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_All', 'fr-FR', N'{0} - Tous', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_All', 'en-US', N'{0} - All', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_All', 'pt-BR', N'{0} - Todos', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_All', 'es-ES', N'{0} - Todos', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_All', 'de-DE', N'{0} - Alles', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_All', 'pl-PL', N'{0} - Wszyscy', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_Global', 'fr-FR', N'Global', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_Global', 'en-US', N'Global', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_Global', 'pt-BR', N'Global', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_Global', 'es-ES', N'Global', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_Global', 'de-DE', N'Insgesamt', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_Global', 'pl-PL', N'Całościowe', null;
GO
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_Per', 'fr-FR', N'Par {0}', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_Per', 'en-US', N'Per {0}', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_Per', 'pt-BR', N'Por {0}', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_Per', 'es-ES', N'Por {0}', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_Per', 'de-DE', N'Über {0}', null;
EXEC InsertOrUpdateResource 'ViewModel_Restitution_View_Per', 'pl-PL', N'Dla {0}', null;
GO
EXEC InsertOrUpdateResource 'Views_ReferentialMergeView_Merge', 'fr-FR', N'Sélectionner les éléments à remplacer par', null;
EXEC InsertOrUpdateResource 'Views_ReferentialMergeView_Merge', 'en-US', N'Select data to be replaced by', null;
EXEC InsertOrUpdateResource 'Views_ReferentialMergeView_Merge', 'pt-BR', N'Selecionar os elementos para serem substituídos por', null;
EXEC InsertOrUpdateResource 'Views_ReferentialMergeView_Merge', 'es-ES', N'Seleccione los elementos que hay que reemplazar', null;
EXEC InsertOrUpdateResource 'Views_ReferentialMergeView_Merge', 'de-DE', N'Wählen Sie die zu ersetzenden Elemente', null;
EXEC InsertOrUpdateResource 'Views_ReferentialMergeView_Merge', 'pl-PL', N'Wskazane dane będą zastąpione przez', null;
GO
EXEC InsertOrUpdateResource 'VM_Acquire_Message_CannotSetThumbnailOutOfBounds', 'fr-FR', N'Impossible de définir une image se trouvant en dehors des bornes de la tâche.', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_CannotSetThumbnailOutOfBounds', 'en-US', N'Unable to set a picture outside task bounds.', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_CannotSetThumbnailOutOfBounds', 'pt-BR', N'Não é possível definir uma imagem que está fora dos limites da tarefa.', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_CannotSetThumbnailOutOfBounds', 'es-ES', N'Es impossible de sacar una foto que se encuentra fuera de los margenes de la tarea', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_CannotSetThumbnailOutOfBounds', 'de-DE', N'Ein Bild, das ausserhalb der Grenzen der Aufgabe ist, kann nicht definiert werden.', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_CannotSetThumbnailOutOfBounds', 'pl-PL', N'Niezmożliwe ustawienie zdjęcia poza wyznaczonym zadaniem', null;
GO
EXEC InsertOrUpdateResource 'VM_Acquire_Message_InvalidThumbnailFile', 'fr-FR', N'Le fichier spécifié ne peut être lu.', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_InvalidThumbnailFile', 'en-US', N'The selected file cannot be read.', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_InvalidThumbnailFile', 'pt-BR', N'O arquivo especificado não pode ser lido.', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_InvalidThumbnailFile', 'es-ES', N'Este fichero específico no puede ser leido', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_InvalidThumbnailFile', 'de-DE', N'Die ausgewählte Datei kann nicht gelesen werden.', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_InvalidThumbnailFile', 'pl-PL', N'Wskazany plik nie może być odczytany', null;
GO
EXEC InsertOrUpdateResource 'VM_Acquire_Message_SureToUngroup', 'fr-FR', N'Etes vous sûr(e) de vouloir dégrouper ces éléments ?', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_SureToUngroup', 'en-US', N'Are you sure you want to ungroup those tasks ?', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_SureToUngroup', 'pt-BR', N'Tem certeza de que deseja desagrupar esses elementos?', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_SureToUngroup', 'es-ES', N'¿Está seguro (a) de querer  separar estos elementos?', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_SureToUngroup', 'de-DE', N'Sind Sie sicher dass sie die Gruppierung aufheben wollen?', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_SureToUngroup', 'pl-PL', N'Czy na pewno rozgrupować zadania?', null;
GO
EXEC InsertOrUpdateResource 'VM_Acquire_Message_ThumbnailTooLarge', 'fr-FR', N'L''image spécifiée possède de trop grandes dimensions. La largeur ou hauteur maximale est de {0} pixels.', N'{0} est la largeur ou hauteur maximale en pixels';
EXEC InsertOrUpdateResource 'VM_Acquire_Message_ThumbnailTooLarge', 'en-US', N'The selected picture is too big. The maximal width or height is {0} pixels.', N'{0} est la largeur ou hauteur maximale en pixels';
EXEC InsertOrUpdateResource 'VM_Acquire_Message_ThumbnailTooLarge', 'pt-BR', N'A imagem especificada tem muito grande. A largura ou altura máxima é de {0} pixels.', N'{0} est la largeur ou hauteur maximale en pixels';
EXEC InsertOrUpdateResource 'VM_Acquire_Message_ThumbnailTooLarge', 'es-ES', N'La imagén específica es de dimensiones muy grandes. El ancho o el alto maximo es de {0} pixeles.', N'{0} est la largeur ou hauteur maximale en pixels';
EXEC InsertOrUpdateResource 'VM_Acquire_Message_ThumbnailTooLarge', 'de-DE', N'Das ausgewählte Miniaturbild ist zu groß. Die maximale Breite oder Höhe ist {0} Pixel.', N'{0} est la largeur ou hauteur maximale en pixels';
EXEC InsertOrUpdateResource 'VM_Acquire_Message_ThumbnailTooLarge', 'pl-PL', N'Obrazek jest za duży. Maksymalna szerokość lub wyskość to  {0} pikseli.', N'{0} est la largeur ou hauteur maximale en pixels';
GO
EXEC InsertOrUpdateResource 'VM_Acquire_Message_KeepVideos', 'fr-FR', N'Souhaitez-vous conserver les mêmes vidéos ?', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_KeepVideos', 'en-US', N'Would you like to keep the same videos?', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_KeepVideos', 'pt-BR', N'Gostaria de manter os mesmos vídeos?', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_KeepVideos', 'es-ES', N'¿Quieres guardar los mismos vídeos?', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_KeepVideos', 'de-DE', N'Möchten Sie die gleichen Videos behalten?', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_KeepVideos', 'pl-PL', N'Czy chcesz zachować te same filmy?', null;
GO
EXEC InsertOrUpdateResource 'VM_Acquire_Message_KeepVideosTitle', 'fr-FR', N'Duplication des vidéos', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_KeepVideosTitle', 'en-US', N'Videos duplication', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_KeepVideosTitle', 'pt-BR', N'Duplicação de vídeos', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_KeepVideosTitle', 'es-ES', N'Duplicación de videos', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_KeepVideosTitle', 'de-DE', N'Videos duplikation', null;
EXEC InsertOrUpdateResource 'VM_Acquire_Message_KeepVideosTitle', 'pl-PL', N'Duplikowanie plików wideo', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Duplicate', 'fr-FR', N'Dupliquer la tâche', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Duplicate', 'en-US', N'Duplicate the task', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Duplicate', 'pt-BR', N'Duplicar a tarefa', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Duplicate', 'es-ES', N'Duplicar la tarea', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Duplicate', 'de-DE', N'Duplizieren die Aufgabe', null;
EXEC InsertOrUpdateResource 'View_AnalyzeGridCommon_Duplicate', 'pl-PL', N'Zduplikować zadanie', null;
GO
EXEC InsertOrUpdateResource 'VM_Acquire_ThumbnailCaption', 'fr-FR', N'Veuillez choisir une vignette', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ThumbnailCaption', 'en-US', N'Please select a thumbnail', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ThumbnailCaption', 'pt-BR', N'Selecione uma miniatura', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ThumbnailCaption', 'es-ES', N'Tiene que elegir una miniatura', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ThumbnailCaption', 'de-DE', N'Bitte wählen Sie ein Miniaturbild.', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ThumbnailCaption', 'pl-PL', N'Wskaż szkic', null;
GO
EXEC InsertOrUpdateResource 'VM_Acquire_ValidationActionLabel', 'fr-FR', N'{0} (#{1})', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ValidationActionLabel', 'en-US', N'{0} (#{1})', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ValidationActionLabel', 'pt-BR', N'{0} (#{1})', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ValidationActionLabel', 'es-ES', N'{0} (#{1})', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ValidationActionLabel', 'de-DE', N'{0} (#{1})', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ValidationActionLabel', 'pl-PL', N'{0} (#{1})', null;
GO
EXEC InsertOrUpdateResource 'VM_Acquire_ValidationActionLabel_WBSOnly', 'fr-FR', N'#{0}', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ValidationActionLabel_WBSOnly', 'en-US', N'#{0}', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ValidationActionLabel_WBSOnly', 'pt-BR', N'#{0}', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ValidationActionLabel_WBSOnly', 'es-ES', N'#{0}', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ValidationActionLabel_WBSOnly', 'de-DE', N'#{0}', null;
EXEC InsertOrUpdateResource 'VM_Acquire_ValidationActionLabel_WBSOnly', 'pl-PL', N'#{0}', null;
GO
EXEC InsertOrUpdateResource 'VM_ActionManager_ExternalToolTip', 'fr-FR', N'Externe', null;
EXEC InsertOrUpdateResource 'VM_ActionManager_ExternalToolTip', 'en-US', N'External', null;
EXEC InsertOrUpdateResource 'VM_ActionManager_ExternalToolTip', 'pt-BR', N'Externo', null;
EXEC InsertOrUpdateResource 'VM_ActionManager_ExternalToolTip', 'es-ES', N'Externo', null;
EXEC InsertOrUpdateResource 'VM_ActionManager_ExternalToolTip', 'de-DE', N'Extern', null;
EXEC InsertOrUpdateResource 'VM_ActionManager_ExternalToolTip', 'pl-PL', N'Zewnętrzny', null;
GO
EXEC InsertOrUpdateResource 'VM_ActionManager_NewToolTip', 'fr-FR', N'Nouveau', null;
EXEC InsertOrUpdateResource 'VM_ActionManager_NewToolTip', 'en-US', N'New', null;
EXEC InsertOrUpdateResource 'VM_ActionManager_NewToolTip', 'pt-BR', N'Novo', null;
EXEC InsertOrUpdateResource 'VM_ActionManager_NewToolTip', 'es-ES', N'Nuevo', null;
EXEC InsertOrUpdateResource 'VM_ActionManager_NewToolTip', 'de-DE', N'Neu', null;
EXEC InsertOrUpdateResource 'VM_ActionManager_NewToolTip', 'pl-PL', N'Nowy', null;
GO
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_OpenWindow', 'fr-FR', N'Ouvrir la fenêtre de gestion de l''activation', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_OpenWindow', 'en-US', N'Open the activation management window', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_OpenWindow', 'pt-BR', N'Abra a janela de gestão de ativação', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_OpenWindow', 'es-ES', N'Abrir la ventana de activación de la gestión', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_OpenWindow', 'de-DE', N'Öffnen Sie das Verwaltungs-Fenster.', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_OpenWindow', 'pl-PL', N'Otwórz okno zarządzania aktywacją', null;
GO
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_AllowSendReport', 'fr-FR', N'Envoyer automatiquement les rapports d''erreur détaillés au support K-process.', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_AllowSendReport', 'en-US', N'Automatically send detailed error reports to K-process support.', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_AllowSendReport', 'pt-BR', N'Enviar relatório de erros detalhados automaticamente para o suporte da K-process.', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_AllowSendReport', 'es-ES', N'Envía automáticamente informes detallados de errores al soporte de K-process.', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_AllowSendReport', 'de-DE', N'Automatisch detaillierte Fehlerberichte an Support K-process senden.', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_AllowSendReport', 'pl-PL', N'Automatyczne wysyłanie szczegółowych raportów błędu do obsługi K-process.', null;
GO
EXEC InsertOrUpdateResource 'VM_AdminReferentials_ReferentialNameAlreadyUsed', 'fr-FR', N'Le nom de référentiel "{0}" est déjà utilisé.', N'{0} est le nom de référentiel.';
EXEC InsertOrUpdateResource 'VM_AdminReferentials_ReferentialNameAlreadyUsed', 'en-US', N'Label "{0}" is already in use.', N'{0} est le nom de référentiel.';
EXEC InsertOrUpdateResource 'VM_AdminReferentials_ReferentialNameAlreadyUsed', 'pt-BR', N'O rótulo "{0}" já está em uso.', N'{0} est le nom de référentiel.';
EXEC InsertOrUpdateResource 'VM_AdminReferentials_ReferentialNameAlreadyUsed', 'es-ES', N'La etiqueta "{0}" ya está en uso.', N'{0} est le nom de référentiel.';
EXEC InsertOrUpdateResource 'VM_AdminReferentials_ReferentialNameAlreadyUsed', 'de-DE', N'Die Formulierung "{0}" ist bereits in Gebrauch.', N'{0} est le nom de référentiel.';
EXEC InsertOrUpdateResource 'VM_AdminReferentials_ReferentialNameAlreadyUsed', 'pl-PL', N'Etykieta "{0}" jest używana.', N'{0} est le nom de référentiel.';
GO
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotCreateTargetScenarioIfInitialNotValidated', 'fr-FR', N'Impossible de créer un scénario cible si le scénario initial n''a pas été figé.', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotCreateTargetScenarioIfInitialNotValidated', 'en-US', N'Could not create a target scenario if the initial scenario has not been frozen.', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotCreateTargetScenarioIfInitialNotValidated', 'pt-BR', N'Não foi possível criar um cenário alvo se o cenário inicial não foi congelado.', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotCreateTargetScenarioIfInitialNotValidated', 'es-ES', N'No se puede crear el escenario de destino si el escenario inicial no se ha fijado.', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotCreateTargetScenarioIfInitialNotValidated', 'de-DE', N'Kann kein Zielszenario erstellen, wenn das Anfangs-Szenario nicht eingefroren wurde.', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotCreateTargetScenarioIfInitialNotValidated', 'pl-PL', N'Nie można stworzyć scenariusza docelowego, jeżeli początkowy scenariusz nie jest zamrożony.', null;
GO
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotInvalidateATargetScenarioWhenHavingRealizedScenario', 'fr-FR', N'Impossible de défiger un scénario (Initial ou Cible) s''il existe un scénario de validation.', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotInvalidateATargetScenarioWhenHavingRealizedScenario', 'en-US', N'Unable to unfreeze a scenario  (Initital or Target) if a Validation scenario exists.', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotInvalidateATargetScenarioWhenHavingRealizedScenario', 'pt-BR', N'Impossivel de descongelar um cenário (ou Objetivo Inicial) se um cenário de validação já existe.', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotInvalidateATargetScenarioWhenHavingRealizedScenario', 'es-ES', N'No se puede invalidar un escenario (o Objetivo Inicial), si no existe un escenario de validación.', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotInvalidateATargetScenarioWhenHavingRealizedScenario', 'de-DE', N'Das (Initial oder Ziel) Szenario kann nicht bearbeitet werden, solange es ein Validierungsszenario gibt.', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotInvalidateATargetScenarioWhenHavingRealizedScenario', 'pl-PL', N'Nie można odmrozić scenariusza (początkowego lub docelowego) jeżeli zatwierdzony scenariusz istnieje.', null;
GO
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotValidateScenario', 'fr-FR', N'Impossible de passer ce scénario à l''état figé car le scénario ''{0}'' est déjà marqué comme figé.', N'{0} est le nom du scénario déjà marqué comme figé';
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotValidateScenario', 'en-US', N'Cannot pass this scenario in the frozen state because the ''{0}'' scenario is already marked as fixed.', N'{0} est le nom du scénario déjà marqué comme figé';
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotValidateScenario', 'pt-BR', N'Não pode congelar este cenário, porque o cenário ''{0}'' já está marcado como congelado.', N'{0} est le nom du scénario déjà marqué comme figé';
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotValidateScenario', 'es-ES', N'Imposible de fijar este escenario ya que el escenario ''{0}'' ya está marcado como fijo.', N'{0} est le nom du scénario déjà marqué comme figé';
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotValidateScenario', 'de-DE', N'Szenario kann nicht eingefroren werden, da das Szenario ''{0}'' bereits als schreibgeschützt markiert ist.', N'{0} est le nom du scénario déjà marqué comme figé';
EXEC InsertOrUpdateResource 'VM_AnalyzeBuild_Message_CannotValidateScenario', 'pl-PL', N'Nie można traktować tego scenariusza jako zamrożonego ponieważ ''{0}'' scenariusz jest oznaczony jako wybrany.', N'{0} est le nom du scénario déjà marqué comme figé';
GO
EXEC InsertOrUpdateResource 'VM_AnalyzeCommon_DeleteImpactedScenarios', 'fr-FR', N'Les modifications que vous avez faîtes impacteront les scénarios suivants :

{0}

Êtes vous sûr(e) de vouloir supprimer cet élément ?', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeCommon_DeleteImpactedScenarios', 'en-US', N'The changes that you make affect the following scenarios:

{0}

Are you sure you want to delete this item?', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeCommon_DeleteImpactedScenarios', 'pt-BR', N'As mudanças feitas afetam os seguintes cenários:

{0}

Tem certeza de que deseja excluir este item?', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeCommon_DeleteImpactedScenarios', 'es-ES', N'Los cambios realizados afectarán los siguientes escenarios:

{0}

¿Está seguro (s) que desea eliminar este elemento?', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeCommon_DeleteImpactedScenarios', 'de-DE', N'Die von Ihnen vorgenommenen Änderungen können Auswirkungen auf die folgenden Szenarien haben:

{0}

Sind Sie sicher, diesen Artikel zu löschen ?', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeCommon_DeleteImpactedScenarios', 'pl-PL', N'Zmiany które robisz mogą mieć wpływ na następujące scenariusze :

{0}

Czy na pewno usunąć te sprawy?', null;
GO
EXEC InsertOrUpdateResource 'VM_AnalyzeCommon_ImpactedScenarios', 'fr-FR', N'Les modifications que vous avez faîtes impacteront les scénarios suivants :

{0}

Etes-vous sûr(e) de vouloir appliquer ces modifications ?', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeCommon_ImpactedScenarios', 'en-US', N'The changes that you make affect the following scenarios:

{0}

Are you sure you want to apply these changes ?', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeCommon_ImpactedScenarios', 'pt-BR', N'As mudanças feitas afetma os seguintes cenários:

{0}

Tem certeza de que deseja aplicar essas mudanças?', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeCommon_ImpactedScenarios', 'es-ES', N'Los cambios realizados afectarán los siguientes escenarios:

{0}

¿Está seguro (e) de querer  aplicar estos cambios?', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeCommon_ImpactedScenarios', 'de-DE', N'Die von Ihnen vorgenommenen Änderungen werden Auswirkungen auf die folgenden Szenarien haben:

{0}

Sind Sie sicher, diese Änderungen zu übernehmen?', null;
EXEC InsertOrUpdateResource 'VM_AnalyzeCommon_ImpactedScenarios', 'pl-PL', N'Zmiany które robisz mogą mieć wpływ na następujące scenariusze :

{0}

Czy na pewno zachowac te zmiany?', null;
GO
EXEC InsertOrUpdateResource 'VM_AppActionsReferentialsViewModelBase_Message_CannotDeleteAttachedActions', 'fr-FR', N'Impossible de supprimer cet élément car des tâches ou des vidéos en dépendent.', null;
EXEC InsertOrUpdateResource 'VM_AppActionsReferentialsViewModelBase_Message_CannotDeleteAttachedActions', 'en-US', N'Cannot delete this item because it is still related to tasks or videos.', null;
EXEC InsertOrUpdateResource 'VM_AppActionsReferentialsViewModelBase_Message_CannotDeleteAttachedActions', 'pt-BR', N'Não é possível excluir este item porque ele ainda está relacionada às tarefas ou videos.', null;
EXEC InsertOrUpdateResource 'VM_AppActionsReferentialsViewModelBase_Message_CannotDeleteAttachedActions', 'es-ES', N'No se puede eliminar este punto ya que las tareas o videos dependen de ella.', null;
EXEC InsertOrUpdateResource 'VM_AppActionsReferentialsViewModelBase_Message_CannotDeleteAttachedActions', 'de-DE', N'Dieser Artikel kann nicht gelöscht werden, weil Aufgaben oder Videos davon abhängen.', null;
EXEC InsertOrUpdateResource 'VM_AppActionsReferentialsViewModelBase_Message_CannotDeleteAttachedActions', 'pl-PL', N'Nie można usunąc tej sprawy, ponieważ jest nadal połaczona do zadania lub filmu.', null;
GO
EXEC InsertOrUpdateResource 'VM_AppActionsReferentialsViewModelBase_Message_WantToSave', 'fr-FR', N'Voulez-vous enregistrer vos modifications ?
Toute modification non sauvegardée sera perdue', null;
EXEC InsertOrUpdateResource 'VM_AppActionsReferentialsViewModelBase_Message_WantToSave', 'en-US', N'Do you want to save your changes?
Any changes not saved will be lost.', null;
EXEC InsertOrUpdateResource 'VM_AppActionsReferentialsViewModelBase_Message_WantToSave', 'pt-BR', N'Você quer salvar as alterações?
Quaisquer alterações não salvas serão perdidas.', null;
EXEC InsertOrUpdateResource 'VM_AppActionsReferentialsViewModelBase_Message_WantToSave', 'es-ES', N'¿Quiere guardar los cambios?
Cualquier cambio no guardado se perderá', null;
EXEC InsertOrUpdateResource 'VM_AppActionsReferentialsViewModelBase_Message_WantToSave', 'de-DE', N'Wollen Sie Ihre Änderungen speichern?
Alle nicht gespeicherten Änderungen gehen verloren', null;
EXEC InsertOrUpdateResource 'VM_AppActionsReferentialsViewModelBase_Message_WantToSave', 'pl-PL', N'Czy zachowac zmiany? Niezapisane zmiany nie będą zachowane.', null;
GO
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_CannotUseSameUserName', 'fr-FR', N'Les utilisateurs suivant utilisent un nom d''utilisateur déjà existant :

{0}', N'{0} : liste d''utilisateurs utilisant le même nom';
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_CannotUseSameUserName', 'en-US', N'The following users use an already existing user name:

{0}', N'{0} : liste d''utilisateurs utilisant le même nom';
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_CannotUseSameUserName', 'pt-BR', N'Os seguintes usuários usam um nome já existentes:

{0}', N'{0} : liste d''utilisateurs utilisant le même nom';
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_CannotUseSameUserName', 'es-ES', N'Los usuarios siguientes utilizan un nombre de usuario ya existente

{0}', N'{0} : liste d''utilisateurs utilisant le même nom';
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_CannotUseSameUserName', 'de-DE', N'Diese Nutzer verwenden einen bereits vorhanden Benutzer-Namen:

{0}', N'{0} : liste d''utilisateurs utilisant le même nom';
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_CannotUseSameUserName', 'pl-PL', N'Nastepujący użytkownicy używają już istniejących nazwisk:

{0}', N'{0} : liste d''utilisateurs utilisant le même nom';
GO
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_SureToDeleteUser', 'fr-FR', N'Etes-vous sûr(e) de vouloir supprimer cet utilisateur ?', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_SureToDeleteUser', 'en-US', N'Are you sure you want to delete this user?', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_SureToDeleteUser', 'pt-BR', N'Tem certeza de que deseja excluir esse usuário?', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_SureToDeleteUser', 'es-ES', N'¿Está seguro (s) que desea eliminar este usuario?', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_SureToDeleteUser', 'de-DE', N'Sind Sie sicher, dass Sie diesen Benutzer löschen wollen?', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_SureToDeleteUser', 'pl-PL', N'Czy na pewno usunąc tego użytkownika?', null;
GO
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_WantToSave', 'fr-FR', N'Voulez-vous enregistrer vos modifications ?
Toute modification non sauvegardée sera perdue', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_WantToSave', 'en-US', N'Do you want to save your changes?
Any changes not saved will be lost', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_WantToSave', 'pt-BR', N'Você quer salvar as alterações?
Quaisquer alterações não salvas serão perdidas', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_WantToSave', 'es-ES', N'¿Quiere guardar los cambios?
Cualquier cambio no guardado se perderá', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_WantToSave', 'de-DE', N'Wollen Sie Ihre Änderungen speichern?
Alle nicht gespeicherten Änderungen gehen verloren.', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Message_WantToSave', 'pl-PL', N'Czy zachowac zmiany? Niezapisane zmiany nie będą zachowane.', null;
GO
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Validation_CannotUseSameUserName', 'fr-FR', N'L''identifiant spécifié existe déjà. Veuillez en utiliser un autre', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Validation_CannotUseSameUserName', 'en-US', N'The specified username already exists. Please use another one.', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Validation_CannotUseSameUserName', 'pt-BR', N'O nome de usuário especificado já existe. Por favor, use outro.', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Validation_CannotUseSameUserName', 'es-ES', N'El identificador ya existe. Por favor, use otro.', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Validation_CannotUseSameUserName', 'de-DE', N'Der Benutzer ist bereits vorhanden. Bitte benutzen Sie einen anderen.', null;
EXEC InsertOrUpdateResource 'VM_ApplicationMembers_Validation_CannotUseSameUserName', 'pl-PL', N'Ta nazwa użytkownika już istnieje. Użyj innej.', null;
GO
EXEC InsertOrUpdateResource 'VM_Authentication_Message_InvalidCredentials', 'fr-FR', N'Les informations entrées sont invalides.', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Message_InvalidCredentials', 'en-US', N'Entered information is invalid.', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Message_InvalidCredentials', 'pt-BR', N'Informações digitados são inválidas.', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Message_InvalidCredentials', 'es-ES', N'La información no es válida.', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Message_InvalidCredentials', 'de-DE', N'Die eingegebenen Informationen sind ungültig.', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Message_InvalidCredentials', 'pl-PL', N'Wprowadzone dane są błędne.', null;
GO
EXEC InsertOrUpdateResource 'VM_Authentication_Message_SharedDatabaseLocked', 'fr-FR', N'La base de données est actuellement vérouillée par un autre utilisateur.
Attendez que cet utilisateur se déconnecte ou contactez votre administrateur système.', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Message_SharedDatabaseLocked', 'en-US', N'The database is currently locked by another user.
Please wait for the end of its session.', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Message_SharedDatabaseLocked', 'pt-BR', N'The database is currently locked by another user.
Please wait for the end of its session.', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Message_SharedDatabaseLocked', 'es-ES', N'The database is currently locked by another user.
Please wait for the end of its session.', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Message_SharedDatabaseLocked', 'de-DE', N'Die Datenbank ist durch einen anderen Benutzer belegt.
Bitte warten Sie bis zum Sizungsende.', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Message_SharedDatabaseLocked', 'pl-PL', N'Baza danych jest teraz zablokowana przez innego użytkownika. Poczekaj na zakończenie jego sesji.', null;
GO
EXEC InsertOrUpdateResource 'VM_Authentication_Title', 'fr-FR', N'Authentification', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Title', 'en-US', N'Authentication', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Title', 'pt-BR', N'Autenticação', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Title', 'es-ES', N'Autentificación', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Title', 'de-DE', N'Authentifizierung', null;
EXEC InsertOrUpdateResource 'VM_Authentication_Title', 'pl-PL', N'Autoryzajca', null;
GO
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupFailureMessage', 'fr-FR', N'Le backup de la base de données a échoué.
Merci de vérifier la console pour plus d''informations.', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupFailureMessage', 'en-US', N'The backup failed.
Please check the console for more information', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupFailureMessage', 'pt-BR', N'Falha no backup.
Por favor verifique o painel de comando para maiores infomrações.', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupFailureMessage', 'es-ES', N'El backup falló
Chequear la consola para mas información', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupFailureMessage', 'de-DE', N'Backup der Datenbank konnte nicht durchgeführt werden.
Für weitere Informationen bitte die Anzeigeeinheit aufrufen', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupFailureMessage', 'pl-PL', N'Kopia zapasowa jest uszkodzona. Poszukaj więcej informacji', null;
GO
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupFailureMessageTitle', 'fr-FR', N'Le backup a échoué', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupFailureMessageTitle', 'en-US', N'Backup failed', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupFailureMessageTitle', 'pt-BR', N'Falha no backup.', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupFailureMessageTitle', 'es-ES', N'Backup falló', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupFailureMessageTitle', 'de-DE', N'Backup konnte nicht erfolgen', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupFailureMessageTitle', 'pl-PL', N'Kopia zapasowa jest uszkodzona.', null;
GO
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupSuccessMessage', 'fr-FR', N'Le backup de la base de données s''est terminé avec succés.', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupSuccessMessage', 'en-US', N'Succesfull backup', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupSuccessMessage', 'pt-BR', N'Backup realizado com sucesso.', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupSuccessMessage', 'es-ES', N'Backup excitóso', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupSuccessMessage', 'de-DE', N'Backup wurde erfolgreich durchgeführt', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupSuccessMessage', 'pl-PL', N'Kopia zapasowa powiodła się.', null;
GO
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupSuccessMessageTitle', 'fr-FR', N'Backup terminé', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupSuccessMessageTitle', 'en-US', N'Backup finished', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupSuccessMessageTitle', 'pt-BR', N'Backup finalizado.', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupSuccessMessageTitle', 'es-ES', N'Backup finalizado', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupSuccessMessageTitle', 'de-DE', N'Backup beendet', null;
EXEC InsertOrUpdateResource 'VM_BackupRestore_BackupSuccessMessageTitle', 'pl-PL', N'Kopia zapasowa zakończona', null;
GO
EXEC InsertOrUpdateResource 'VM_GridGanttItem_WBSTooltip', 'fr-FR', N'ID {0}', N'{0} est le ID';
EXEC InsertOrUpdateResource 'VM_GridGanttItem_WBSTooltip', 'en-US', N'ID {0}', N'{0} est le ID';
EXEC InsertOrUpdateResource 'VM_GridGanttItem_WBSTooltip', 'pt-BR', N'ID {0}', N'{0} est le ID';
EXEC InsertOrUpdateResource 'VM_GridGanttItem_WBSTooltip', 'es-ES', N'ID {0}', N'{0} est le ID';
EXEC InsertOrUpdateResource 'VM_GridGanttItem_WBSTooltip', 'de-DE', N'ID {0}', N'{0} est le ID';
EXEC InsertOrUpdateResource 'VM_GridGanttItem_WBSTooltip', 'pl-PL', N'Numer identyfikacyjny {0}', N'{0} est le ID';
GO
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_ConfirmExit', 'fr-FR', N'Êtes vous sûr de vouloir quitter l''application?', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_ConfirmExit', 'en-US', N'Are you sure you want to quit the application?', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_ConfirmExit', 'pt-BR', N'Tem certeza de que gostaria de finalizar o aplicativo?', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_ConfirmExit', 'es-ES', N'Esta seguro de querer salir de la aplicación?', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_ConfirmExit', 'de-DE', N'Möchten Sie das Programm wirklich verlassen ?', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_ConfirmExit', 'pl-PL', N'Zamknąc aplikację?', null;
GO
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_ErrorLaunchingUserManual', 'fr-FR', N'Il y a eu une erreur lors de l''affichage de la notice utilisateur.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_ErrorLaunchingUserManual', 'en-US', N'Unable to load the user manual.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_ErrorLaunchingUserManual', 'pt-BR', N'Não é possível carregar o manual do usuário.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_ErrorLaunchingUserManual', 'es-ES', N'Hubo un error al mostrar el manual de usuario.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_ErrorLaunchingUserManual', 'de-DE', N'Es gab einen Fehler bei der Anzeige der Bedienungsanleitung.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_ErrorLaunchingUserManual', 'pl-PL', N'Nie można otworzyć podręcznika użytkownika.', null;
GO
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_Title_ConfirmExit', 'fr-FR', N'Confirmation, quitter l''application KL²®', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_Title_ConfirmExit', 'en-US', N'Confirm, quit KL²®', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_Title_ConfirmExit', 'pt-BR', N'Confirme, sair de KL²®.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_Title_ConfirmExit', 'es-ES', N'Confirmar, salir de KL²®', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_Title_ConfirmExit', 'de-DE', N'KL²® verlassen', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_Title_ConfirmExit', 'pl-PL', N'Potwierdź zamknięcie KL²®', null;
GO
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_Title_PublicationInProgress', 'fr-FR', N'Publication en cours', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_Title_PublicationInProgress', 'en-US', N'Publication in progress', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_Title_PublicationInProgress', 'pt-BR', N'Publication in progress', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_Title_PublicationInProgress', 'es-ES', N'Publication in progress', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_Title_PublicationInProgress', 'de-DE', N'Publication in progress', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_Title_PublicationInProgress', 'pl-PL', N'Publication in progress', null;
GO
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_PublicationInProgress', 'fr-FR', N'Impossible de quitter KL²®, une publication est en cours.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_PublicationInProgress', 'en-US', N'Can''t quit KL²®, a publication is in progress.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_PublicationInProgress', 'pt-BR', N'Can''t quit KL²®, a publication is in progress.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_PublicationInProgress', 'es-ES', N'Can''t quit KL²®, a publication is in progress.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_PublicationInProgress', 'de-DE', N'Can''t quit KL²®, a publication is in progress.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowViewModel_Message_PublicationInProgress', 'pl-PL', N'Can''t quit KL²®, a publication is in progress.', null;
GO
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_Auto', 'fr-FR', N'Automatique selon la configuration du système d''exploitation', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_Auto', 'en-US', N'Automatic based on OS configuration', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_Auto', 'pt-BR', N'Automático baseado na configuração de OS', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_Auto', 'es-ES', N'Automático dependiendo de la configuración del sistema operativo', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_Auto', 'de-DE', N'Automatisch je nach Konfiguration des Betriebssystems', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_Auto', 'pl-PL', N'Automatycznie bazuj na konfiguracji OS', null;
GO
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_DMO', 'fr-FR', N'DMO (codecs DirectX Windows)', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_DMO', 'en-US', N'DMO (Windows DIrectX codecs)', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_DMO', 'pt-BR', N'DMO (codecs Windows DirectX)', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_DMO', 'es-ES', N'DMO (DirectX codecs Windows)', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_DMO', 'de-DE', N'DMO (DirectX Windows-Codecs)', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_DMO', 'pl-PL', N'DMO (kodeki DirectX Windows)', null;
GO
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_External', 'fr-FR', N'Source externe', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_External', 'en-US', N'External source', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_External', 'pt-BR', N'Fonte externa', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_External', 'es-ES', N'Fuente externa', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_External', 'de-DE', N'Externe Quelle', null;
EXEC InsertOrUpdateResource 'VM_ParametersDecoderViewModel_DecoderSource_External', 'pl-PL', N'Zewnętrzne źródło', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_DuplicateUserRoleProject', 'fr-FR', N'Un rôle pour cet utilisateur existe déjà.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_DuplicateUserRoleProject', 'en-US', N'A role for this user already exists.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_DuplicateUserRoleProject', 'pt-BR', N'Um papel para este usuário já existe.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_DuplicateUserRoleProject', 'es-ES', N'La funcion de este usuario ya existe.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_DuplicateUserRoleProject', 'de-DE', N'Eine Rolle für diesen Benutzer existiert bereits.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_DuplicateUserRoleProject', 'pl-PL', N'Role dla tego użytkownika już istnieje', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ErrorDuringExport', 'fr-FR', N'Une erreur a eu lieu durant l''exportation du projet.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ErrorDuringExport', 'en-US', N'An error occurred during the export of the project.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ErrorDuringExport', 'pt-BR', N'Ocorreu um erro durante a exportação do projeto.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ErrorDuringExport', 'es-ES', N'Se produjo un error durante la exportación del proyecto.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ErrorDuringExport', 'de-DE', N'Ein Fehler ist beim Export des Projekts aufgetreten.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ErrorDuringExport', 'pl-PL', N'Podczas eksportu pliku pojawił się błąd', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ErrorDuringimport', 'fr-FR', N'Une erreur a eu lieu durant l''importation du projet.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ErrorDuringimport', 'en-US', N'An error occurred during the import of the project.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ErrorDuringimport', 'pt-BR', N'Ocorreu um erro durante a importação do projeto.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ErrorDuringimport', 'es-ES', N'Se produjo un error durante la importación del proyecto.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ErrorDuringimport', 'de-DE', N'Ein Fehler ist beim Importieren des Projekts aufgetreten.', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ErrorDuringimport', 'pl-PL', N'Podczas importu pliku pojawił się błąd', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ImportProjectMergeReferentials', 'fr-FR', N'Souhaitez-vous fusionner les éléments suivants avec ceux de votre base de données ?
{0}', N'{0} contient les libellés des référentiels identiques';
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ImportProjectMergeReferentials', 'en-US', N'Do you want to merge following items with current ones from your database ?
{0}', N'{0} contient les libellés des référentiels identiques';
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ImportProjectMergeReferentials', 'pt-BR', N'Gostaria de unir items de referência com items da sua base de dados atual?
{0}', N'{0} contient les libellés des référentiels identiques';
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ImportProjectMergeReferentials', 'es-ES', N'¿Usted desea fusionar elementos con los elementos en su base de datos actual?
{0}', N'{0} contient les libellés des référentiels identiques';
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ImportProjectMergeReferentials', 'de-DE', N'Möchten Sie diese Referenzelemente mit den Elementen in der aktuellen Datenbank zusammenführen?
{0}', N'{0} contient les libellés des référentiels identiques';
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ImportProjectMergeReferentials', 'pl-PL', N'Czy chcesz połączyć nastepujące sprawy z bieżącymi z bazy danych?
{0}', N'{0} contient les libellés des référentiels identiques';
GO
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ImportProjectMergeReferentials_Title', 'fr-FR', N'Fusion des éléments des référentiels ?', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ImportProjectMergeReferentials_Title', 'en-US', N'Merging referential items ?', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ImportProjectMergeReferentials_Title', 'pt-BR', N'Unir items de referência?', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ImportProjectMergeReferentials_Title', 'es-ES', N'¿Fusión de referencias elementos?', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ImportProjectMergeReferentials_Title', 'de-DE', N'Zusammenführung von Referenzelementen?', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_Message_ImportProjectMergeReferentials_Title', 'pl-PL', N'Połazcyć wskazane sprawy?', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_Second', 'fr-FR', N'Seconde', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_Second', 'en-US', N'Second', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_Second', 'pt-BR', N'Segundo', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_Second', 'es-ES', N'Segundo', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_Second', 'de-DE', N'Sekunde', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_Second', 'pl-PL', N'Sekunda', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondHundredth', 'fr-FR', N'1/100 de seconde', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondHundredth', 'en-US', N'A 100th of second', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondHundredth', 'pt-BR', N'1/100 de segundo', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondHundredth', 'es-ES', N'1 / 100 de segundo', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondHundredth', 'de-DE', N'1 / 100 einer Sekunde', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondHundredth', 'pl-PL', N'1/100 sekundy', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondTenth', 'fr-FR', N'1/10 de seconde', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondTenth', 'en-US', N'A 10th of second', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondTenth', 'pt-BR', N'1/10 de segundo', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondTenth', 'es-ES', N'1 / 10 de segundo', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondTenth', 'de-DE', N'1 / 10 einer Sekunde', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondTenth', 'pl-PL', N'1/10 sekundy', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondThousandth', 'fr-FR', N'1/1000 de seconde', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondThousandth', 'en-US', N'A 1000th of second', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondThousandth', 'pt-BR', N'1/1000 de segundo', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondThousandth', 'es-ES', N'1 / 1000 de segundo', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondThousandth', 'de-DE', N'1 / 1000 einer Sekunde', null;
EXEC InsertOrUpdateResource 'VM_PrepareProject_TimeScale_SecondThousandth', 'pl-PL', N'1/1000 sekundy', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_CannotDeleteScenarioBecauseLinked', 'fr-FR', N'Impossible de supprimer le scénario car il est lié à d''autres scénarios.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_CannotDeleteScenarioBecauseLinked', 'en-US', N'Unable to delete the scenario because it is linked to other scenarios.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_CannotDeleteScenarioBecauseLinked', 'pt-BR', N'Não foi possível excluir o cenário, porque está ligado a outros cenários.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_CannotDeleteScenarioBecauseLinked', 'es-ES', N'No se puede eliminar el guión, ya que está vinculado a otros escenarios.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_CannotDeleteScenarioBecauseLinked', 'de-DE', N'Kann das Skript nicht löschen, weil es mit anderem Szenarien verknüpft ist.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_CannotDeleteScenarioBecauseLinked', 'pl-PL', N'Nie można usunąc scenariusza, ponieważ jest połaczony z innymi scenariuszami.', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_CannotCreateMultipleRealizedScenario', 'fr-FR', N'Impossible de créer plusieurs scénarios de validation.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_CannotCreateMultipleRealizedScenario', 'en-US', N'Unable to create several Validation scenarios.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_CannotCreateMultipleRealizedScenario', 'pt-BR', N'Não foi possível criar vários cenários de validação.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_CannotCreateMultipleRealizedScenario', 'es-ES', N'No se puede crear varios escenarios de validación.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_CannotCreateMultipleRealizedScenario', 'de-DE', N'Kann keine Validierung Szenarien erstellen.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_CannotCreateMultipleRealizedScenario', 'pl-PL', N'Nie można stworzyć kilku scenariuszy zatwierdzonych.', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_CannotCreateTargetScenarioIfInitialNotValidated', 'fr-FR', N'Impossible de créer un scénario cible si le scénario initial n''a pas été figé.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_CannotCreateTargetScenarioIfInitialNotValidated', 'en-US', N'Could not create a target scenario if the initial scenario has not been frozen.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_CannotCreateTargetScenarioIfInitialNotValidated', 'pt-BR', N'Não é possível criar um cenário alvo se o cenário inicial não tiver sido congelado.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_CannotCreateTargetScenarioIfInitialNotValidated', 'es-ES', N'No se puede crear el escenario de destino si el escenario inicial no se ha fijado.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_CannotCreateTargetScenarioIfInitialNotValidated', 'de-DE', N'Kann kein Zielszenario erstellen, da das ersten Szenario  nicht schreibgeschützt wurde.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_CannotCreateTargetScenarioIfInitialNotValidated', 'pl-PL', N'Nie można stworzyć scenariusza docelowego, jeżeli początkowy scenariusz nie jest zamrożony.', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ErrorDuringExport', 'fr-FR', N'Une erreur a eu lieu durant l''exportation de la décomposition de la vidéo.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ErrorDuringExport', 'en-US', N'An error occurred during the export of the video decomposition.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ErrorDuringExport', 'pt-BR', N'Ocorreu um erro durante a exportação da decomposição do vídeo.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ErrorDuringExport', 'es-ES', N'Se produjo un error durante la exportación  de la descomposición del video.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ErrorDuringExport', 'de-DE', N'Ein Fehler ist beim Export des Videos aufgetreten.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ErrorDuringExport', 'pl-PL', N'Podczas eksportu podzielonego filmy pojaiwł się błąd.', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ErrorDuringimport', 'fr-FR', N'Une erreur a eu lieu durant l''importation de la décomposition de la vidéo.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ErrorDuringimport', 'en-US', N'An error occurred during the import of the video decomposition.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ErrorDuringimport', 'pt-BR', N'Ocorreu um erro durante a importação da decomposição de vídeo.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ErrorDuringimport', 'es-ES', N'Se produjo un error durante la importación de la descomposición del vídeo.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ErrorDuringimport', 'de-DE', N'Ein Fehler ist beim Import des Videos aufgetreten.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ErrorDuringimport', 'pl-PL', N'Podczas importu podzielonego filmy pojaiwł się błąd.', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ImportVideoDecompositionMergeReferentials', 'fr-FR', N'Souhaitez-vous fusionner les éléments suivants avec ceux de votre base de données ?
{0}', N'{0} contient les libellés des référentiels identiques';
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ImportVideoDecompositionMergeReferentials', 'en-US', N'Do you want to merge following items with current ones from your database ?
{0}', N'{0} contient les libellés des référentiels identiques';
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ImportVideoDecompositionMergeReferentials', 'pt-BR', N'Gostaria de unir items de referência com items da sua base de dados atual?
{0}', N'{0} contient les libellés des référentiels identiques';
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ImportVideoDecompositionMergeReferentials', 'es-ES', N'¿Usted desea fusionar elementos con los elementos en su base de datos actual?
{0}', N'{0} contient les libellés des référentiels identiques';
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ImportVideoDecompositionMergeReferentials', 'de-DE', N'Möchten Sie diese Referenzelemente mit den Elementen in der aktuellen Datenbank zusammenführen?
{0}', N'{0} contient les libellés des référentiels identiques';
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ImportVideoDecompositionMergeReferentials', 'pl-PL', N'Czy chcesz połączyc nastepujące sprawy z bieżacymi z bazy danych?
{0}', N'{0} contient les libellés des référentiels identiques';
GO
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ImportVideoDecompositionMergeReferentials_Title', 'fr-FR', N'Fusion des éléments des référentiels ?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ImportVideoDecompositionMergeReferentials_Title', 'en-US', N'Merging referential items ?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ImportVideoDecompositionMergeReferentials_Title', 'pt-BR', N'Unir items de referência?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ImportVideoDecompositionMergeReferentials_Title', 'es-ES', N'¿Fusión de referencias elementos?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ImportVideoDecompositionMergeReferentials_Title', 'de-DE', N'Zusammenführung von Referenzelementen?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_ImportVideoDecompositionMergeReferentials_Title', 'pl-PL', N'Połaczyć nastepujące sprawy?', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_KeepVideoValidation', 'fr-FR', N'Conserver les séquences vidéo pour les tâches dont la durée n''a pas été modifiée ?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_KeepVideoValidation', 'en-US', N'Keep video clips for tasks whose duration has not been changed?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_KeepVideoValidation', 'pt-BR', N'Manter vídeo clips para tarefas cuja duração não foi alterada?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_KeepVideoValidation', 'es-ES', N'Conservar los video clips para tareas cuya duración no se ha cambiado?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_KeepVideoValidation', 'de-DE', N'Halten Sie Videoclips für Aufgaben, deren Dauer nicht geändert wurde?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_KeepVideoValidation', 'pl-PL', N'Przechowuj klipy wideo dla zadań, których czas trwania nie został zmieniony?', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_Title_KeepVideoValidation', 'fr-FR', N'Création du scénario de validation', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_Title_KeepVideoValidation', 'en-US', N'Validation scenario creation', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_Title_KeepVideoValidation', 'pt-BR', N'Criação de cenário para validação.', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_Title_KeepVideoValidation', 'es-ES', N'Creación del scenario de validación', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_Title_KeepVideoValidation', 'de-DE', N'Erstellung des Validierungs-Szenarios', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_Title_KeepVideoValidation', 'pl-PL', N'Tworzenie scenariusza zatwierdzonego', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareVideos_CannotDeleteVideoWithRelatedActions', 'fr-FR', N'Impossible de supprimer la vidéo car elle a des tâches qui lui sont associées.', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_CannotDeleteVideoWithRelatedActions', 'en-US', N'Unable to delete the video because it has associated tasks.', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_CannotDeleteVideoWithRelatedActions', 'pt-BR', N'Não foi possível excluir o vídeo porque ele tem tarefas associadas.', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_CannotDeleteVideoWithRelatedActions', 'es-ES', N'No se puede eliminar el video debido a que tiene tareas asociadas a ella.', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_CannotDeleteVideoWithRelatedActions', 'de-DE', N'Das Video kann nicht gelöscht werden, weil es Aufgaben zugeordnet ist.', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_CannotDeleteVideoWithRelatedActions', 'pl-PL', N'Nie można usunąc filmu, ponieważ ma połączone sparwy', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareVideos_VideoFileSourceNotReachable', 'fr-FR', N'Le fichier "{0}" n''est pas accessible.
Note : Les lecteurs réseaux et mappés ne sont pas supportés.', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_VideoFileSourceNotReachable', 'en-US', N'The file "{0}" is not reachable.
Note : Network and mapped drives are not supported.', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_VideoFileSourceNotReachable', 'pt-BR', N'The file "{0}" is not reachable.
Note : Network and mapped drives are not supported.', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_VideoFileSourceNotReachable', 'es-ES', N'The file "{0}" is not reachable.
Note : Network and mapped drives are not supported.', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_VideoFileSourceNotReachable', 'de-DE', N'The file "{0}" is not reachable.
Note : Network and mapped drives are not supported.', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_VideoFileSourceNotReachable', 'pl-PL', N'The file "{0}" is not reachable.
Note : Network and mapped drives are not supported.', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Message_ResourceVideo', 'fr-FR', N'La vidéo est-elle liée à une ressource dédiée (opérateur ou équipement) ?', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Message_ResourceVideo', 'en-US', N'Is the video linked to a dedicated resource (operator or equipment)?', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Message_ResourceVideo', 'pt-BR', N'O vídeo está ligado a um recurso dedicado (operador ou equipamento)?', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Message_ResourceVideo', 'es-ES', N'¿Está el video relacionado con un recurso específico (operador o equipo) ?', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Message_ResourceVideo', 'de-DE', N'Ist  das Video mit einer fest zugeordneten Ressource verbunden? (ein Operator oder eine Ausrüstung)', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Message_ResourceVideo', 'pl-PL', N'Czy film jest połączony z dedykowanym zasobem (operator lub sprzęt)?', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Validation_ResourceNameRequired', 'fr-FR', N'Le nom de la ressource est requis', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Validation_ResourceNameRequired', 'en-US', N'Resource name is required', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Validation_ResourceNameRequired', 'pt-BR', N'O nome do recurso é necessário', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Validation_ResourceNameRequired', 'es-ES', N'El nombre del recurso es necesario', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Validation_ResourceNameRequired', 'de-DE', N'Der Name der Ressource wird benötigt', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Validation_ResourceNameRequired', 'pl-PL', N'Nazwa zasoby jest wymagana', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Validation_ResourceTypeRequired', 'fr-FR', N'Le type de la ressource est requis', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Validation_ResourceTypeRequired', 'en-US', N'Resource type is required', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Validation_ResourceTypeRequired', 'pt-BR', N'Tipo de recurso é necessário', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Validation_ResourceTypeRequired', 'es-ES', N'El tipo de recurso es necesario', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Validation_ResourceTypeRequired', 'de-DE', N'Die Art der Ressource wird benötigt', null;
EXEC InsertOrUpdateResource 'VM_PrepareVideos_Validation_ResourceTypeRequired', 'pl-PL', N'Rodzaj zasobu jest wymagany', null;
GO
EXEC InsertOrUpdateResource 'VM_PrepareVideos_VideoFormat', 'fr-FR', N'Codec audio : {0}, Codec vidéo : {1}, Taille : {2}/{3}', N'{0} : AudioCodec, {1} : VideoCodec, {2} : FrameWidth, {3} : FrameWidth';
EXEC InsertOrUpdateResource 'VM_PrepareVideos_VideoFormat', 'en-US', N'Audio codec: {0}, Video Codec: {1}, size: {2} / {3}', N'{0} : AudioCodec, {1} : VideoCodec, {2} : FrameWidth, {3} : FrameWidth';
EXEC InsertOrUpdateResource 'VM_PrepareVideos_VideoFormat', 'pt-BR', N'Codec de áudio: {0}, Vídeo Codec: {1} tamanho: {2} / {3}', N'{0} : AudioCodec, {1} : VideoCodec, {2} : FrameWidth, {3} : FrameWidth';
EXEC InsertOrUpdateResource 'VM_PrepareVideos_VideoFormat', 'es-ES', N'Audio Codec: {0}, Video Codec: {1} Tamaño: {2} / {3}', N'{0} : AudioCodec, {1} : VideoCodec, {2} : FrameWidth, {3} : FrameWidth';
EXEC InsertOrUpdateResource 'VM_PrepareVideos_VideoFormat', 'de-DE', N'Audio Codec: {0}, Video Codec: {1} Größe: {2} / {3}', N'{0} : AudioCodec, {1} : VideoCodec, {2} : FrameWidth, {3} : FrameWidth';
EXEC InsertOrUpdateResource 'VM_PrepareVideos_VideoFormat', 'pl-PL', N'Kodek audio : {0}, Kodek filmu : {1}, Rozmiar : {2}/{3}', N'{0} : AudioCodec, {1} : VideoCodec, {2} : FrameWidth, {3} : FrameWidth';
GO
EXEC InsertOrUpdateResource 'VM_ReferentialMergeViewModel_Title', 'fr-FR', N'Fusion d''éléments de référentiels', null;
EXEC InsertOrUpdateResource 'VM_ReferentialMergeViewModel_Title', 'en-US', N'Referential data merging', null;
EXEC InsertOrUpdateResource 'VM_ReferentialMergeViewModel_Title', 'pt-BR', N'Unir dados de referência', null;
EXEC InsertOrUpdateResource 'VM_ReferentialMergeViewModel_Title', 'es-ES', N'Fusión de referencias elementos', null;
EXEC InsertOrUpdateResource 'VM_ReferentialMergeViewModel_Title', 'de-DE', N'Referenzelemente Zusammenführung', null;
EXEC InsertOrUpdateResource 'VM_ReferentialMergeViewModel_Title', 'pl-PL', N'Połączenie danych wzorcowych', null;
GO
EXEC InsertOrUpdateResource 'VM_ReferentialsGroupSortDescription_New', 'fr-FR', N'A affecter', null;
EXEC InsertOrUpdateResource 'VM_ReferentialsGroupSortDescription_New', 'en-US', N'To be assigned', null;
EXEC InsertOrUpdateResource 'VM_ReferentialsGroupSortDescription_New', 'pt-BR', N'Para atribuir', null;
EXEC InsertOrUpdateResource 'VM_ReferentialsGroupSortDescription_New', 'es-ES', N'A definir', null;
EXEC InsertOrUpdateResource 'VM_ReferentialsGroupSortDescription_New', 'de-DE', N'zu übertragen', null;
EXEC InsertOrUpdateResource 'VM_ReferentialsGroupSortDescription_New', 'pl-PL', N'Do wyznaczenia', null;
GO
EXEC InsertOrUpdateResource 'VM_ReferentialsGroupSortDescription_Standard', 'fr-FR', N'Standard', null;
EXEC InsertOrUpdateResource 'VM_ReferentialsGroupSortDescription_Standard', 'en-US', N'Standard', null;
EXEC InsertOrUpdateResource 'VM_ReferentialsGroupSortDescription_Standard', 'pt-BR', N'Padrão', null;
EXEC InsertOrUpdateResource 'VM_ReferentialsGroupSortDescription_Standard', 'es-ES', N'Estándar', null;
EXEC InsertOrUpdateResource 'VM_ReferentialsGroupSortDescription_Standard', 'de-DE', N'Standard', null;
EXEC InsertOrUpdateResource 'VM_ReferentialsGroupSortDescription_Standard', 'pl-PL', N'Standard', null;
GO
EXEC InsertOrUpdateResource 'VMHelpers_ActionsManager_EmptyReferentialLabel', 'fr-FR', N'<Non défini>', null;
EXEC InsertOrUpdateResource 'VMHelpers_ActionsManager_EmptyReferentialLabel', 'en-US', N'<Not defined>', null;
EXEC InsertOrUpdateResource 'VMHelpers_ActionsManager_EmptyReferentialLabel', 'pt-BR', N'Não definido', null;
EXEC InsertOrUpdateResource 'VMHelpers_ActionsManager_EmptyReferentialLabel', 'es-ES', N'<No definido>', null;
EXEC InsertOrUpdateResource 'VMHelpers_ActionsManager_EmptyReferentialLabel', 'de-DE', N'Undefiniert', null;
EXEC InsertOrUpdateResource 'VMHelpers_ActionsManager_EmptyReferentialLabel', 'pl-PL', N'<Nie zdefiniowane>', null;
GO
EXEC InsertOrUpdateResource 'VMHelpers_ActionsManager_ResourceLoadDescription', 'fr-FR', N'{0}
Charge : {1} / {2:F}%
Superposition : {3} / {4:F}%', N'{0} est le nom de la ressource - {1} est le temps de la charge - {2} est le % de la charge - {3} est le temps de la surcharge - {4} est le % de la surcharge';
EXEC InsertOrUpdateResource 'VMHelpers_ActionsManager_ResourceLoadDescription', 'en-US', N'{0}
Load: {1} / {2:F} %
Combination: {3} / {4:F} %', N'{0} est le nom de la ressource - {1} est le temps de la charge - {2} est le % de la charge - {3} est le temps de la surcharge - {4} est le % de la surcharge';
EXEC InsertOrUpdateResource 'VMHelpers_ActionsManager_ResourceLoadDescription', 'pt-BR', N'{0}
Carga: {1} / {2:F}%
Combinação: {3} / {4:F}%', N'{0} est le nom de la ressource - {1} est le temps de la charge - {2} est le % de la charge - {3} est le temps de la surcharge - {4} est le % de la surcharge';
EXEC InsertOrUpdateResource 'VMHelpers_ActionsManager_ResourceLoadDescription', 'es-ES', N'{0}
Cargo: {1} / {2:F}%
Superposición: {3} / {4:F}%', N'{0} est le nom de la ressource - {1} est le temps de la charge - {2} est le % de la charge - {3} est le temps de la surcharge - {4} est le % de la surcharge';
EXEC InsertOrUpdateResource 'VMHelpers_ActionsManager_ResourceLoadDescription', 'de-DE', N'{0}
Lade : {1} / {2:F}%
Overlay : {3} / {4:F}%', N'{0} est le nom de la ressource - {1} est le temps de la charge - {2} est le % de la charge - {3} est le temps de la surcharge - {4} est le % de la surcharge';
EXEC InsertOrUpdateResource 'VMHelpers_ActionsManager_ResourceLoadDescription', 'pl-PL', N'{0}
Ładowanie : {1} / {2:F}%
Kombinacja : {3} / {4:F}%', N'{0} est le nom de la ressource - {1} est le temps de la charge - {2} est le % de la charge - {3} est le temps de la surcharge - {4} est le % de la surcharge';
GO
EXEC InsertOrUpdateResource 'VMView_PrepareResources_Untyped', 'fr-FR', N'Non typé', null;
EXEC InsertOrUpdateResource 'VMView_PrepareResources_Untyped', 'en-US', N'Untyped', null;
EXEC InsertOrUpdateResource 'VMView_PrepareResources_Untyped', 'pt-BR', N'Sem tipo', null;
EXEC InsertOrUpdateResource 'VMView_PrepareResources_Untyped', 'es-ES', N'Sin tipo', null;
EXEC InsertOrUpdateResource 'VMView_PrepareResources_Untyped', 'de-DE', N'Nicht typisiert', null;
EXEC InsertOrUpdateResource 'VMView_PrepareResources_Untyped', 'pl-PL', N'Bez typu', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_LoopPlaying', 'fr-FR', N'En boucle', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_LoopPlaying', 'en-US', N'Loop', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_LoopPlaying', 'pt-BR', N'Loop', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_LoopPlaying', 'es-ES', N'En bucle', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_LoopPlaying', 'de-DE', N'Schleife', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_LoopPlaying', 'pl-PL', N'Pętla', null;
GO
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Title', 'fr-FR', N'Erreur d''accès concurrents', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Title', 'en-US', N'Access concurrency error', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Title', 'pt-BR', N'Concorrentes acesso erro', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Title', 'es-ES', N'Error de competidores acceso', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Title', 'de-DE', N'Konkurrenten Zugang Fehler', null;
EXEC InsertOrUpdateResource 'Common_Context_DbConcurrency_Title', 'pl-PL', N'Konkurentom dostępu błędu', null;
GO
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
EXEC InsertOrUpdateResource 'Common_Context_ChangeVideoTooltip_Content', 'fr-FR', N'Cliquer pour changer la séquence vidéo associée à la tâche', null;
EXEC InsertOrUpdateResource 'Common_Context_ChangeVideoTooltip_Content', 'en-US', N'Click to change the video clip associated to the task', null;
EXEC InsertOrUpdateResource 'Common_Context_ChangeVideoTooltip_Content', 'pt-BR', N'Clique para mudar a sequência do vídeo associado à tarefa', null;
EXEC InsertOrUpdateResource 'Common_Context_ChangeVideoTooltip_Content', 'es-ES', N'Pulse para cambiar el clip de vídeo asociado a la tarea', null;
EXEC InsertOrUpdateResource 'Common_Context_ChangeVideoTooltip_Content', 'de-DE', N'Klicken Sie hier um die an die Aufgabe zugeordnete Videosequenz zu ändern', null;
EXEC InsertOrUpdateResource 'Common_Context_ChangeVideoTooltip_Content', 'pl-PL', N'Kliknij aby zmienić clip video skojarzony z zadaniem', null;
GO
EXEC InsertOrUpdateResource 'VM_MainWindowView_BackupRestoreOnlyOnLocal', 'fr-FR', N'La sauvegarde et la restauration ne peuvent pas être effectuées en mode distant.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowView_BackupRestoreOnlyOnLocal', 'en-US', N'Backup and restore can not be performed in remote mode.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowView_BackupRestoreOnlyOnLocal', 'pt-BR', N'Backup e restauração não pode ser realizada remotamente.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowView_BackupRestoreOnlyOnLocal', 'es-ES', N'Copia de seguridad y restauración no se pueden realizar de forma remota.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowView_BackupRestoreOnlyOnLocal', 'de-DE', N'Backup und Wiederherstellung kann nicht aus der Ferne durchgeführt werden.', null;
EXEC InsertOrUpdateResource 'VM_MainWindowView_BackupRestoreOnlyOnLocal', 'pl-PL', N'Tworzenie kopii zapasowych i przywracanie nie może być wykonywana zdalnie.', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateProcess', 'fr-FR', N'Créer un process', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateProcess', 'en-US', N'Create a process', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateProcess', 'pt-BR', N'Create a process', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateProcess', 'es-ES', N'Create a process', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateProcess', 'de-DE', N'Create a process', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateProcess', 'pl-PL', N'Create a process', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_DeleteProcess', 'fr-FR', N'Supprimer un process', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_DeleteProcess', 'en-US', N'Delete a process', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_DeleteProcess', 'pt-BR', N'Delete a process', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_DeleteProcess', 'es-ES', N'Delete a process', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_DeleteProcess', 'de-DE', N'Delete a process', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_DeleteProcess', 'pl-PL', N'Delete a process', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateFolder', 'fr-FR', N'Créer un dossier', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateFolder', 'en-US', N'Create a folder', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateFolder', 'pt-BR', N'Criar uma pasta', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateFolder', 'es-ES', N'Crear una carpeta', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateFolder', 'de-DE', N'Ordner erstellen', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CreateFolder', 'pl-PL', N'Utwórz katalog', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_RemoveFolder', 'fr-FR', N'Supprimer un dossier', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RemoveFolder', 'en-US', N'Delete a folder', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RemoveFolder', 'pt-BR', N'Excluir uma pasta', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RemoveFolder', 'es-ES', N'Borrar una carpeta', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RemoveFolder', 'de-DE', N'Ordner löschen', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_RemoveFolder', 'pl-PL', N'Usuń katalog', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveFolder', 'fr-FR', N'Impossible de supprimer un dossier qui contient un process avec des projets', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveFolder', 'en-US', N'The folder has to not contain a process with projects to be deleted', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveFolder', 'pt-BR', N'The folder has to not contain a process with projects to be deleted', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveFolder', 'es-ES', N'The folder has to not contain a process with projects to be deleted', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveFolder', 'de-DE', N'The folder has to not contain a process with projects to be deleted', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveFolder', 'pl-PL', N'The folder has to not contain a process with projects to be deleted', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveProcess', 'fr-FR', N'Impossible de supprimer un process qui contient un projet avec des scénarios', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveProcess', 'en-US', N'The folder has to not contain a project with scenarios to be deleted', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveProcess', 'pt-BR', N'The folder has to not contain a project with scenarios to be deleted', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveProcess', 'es-ES', N'The folder has to not contain a project with scenarios to be deleted', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveProcess', 'de-DE', N'The folder has to not contain a project with scenarios to be deleted', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_CantRemoveProcess', 'pl-PL', N'The folder has to not contain a project with scenarios to be deleted', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_AllProjects', 'fr-FR', N'Tous les projets', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_AllProjects', 'en-US', N'All projects', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_AllProjects', 'pt-BR', N'Todos os projetos', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_AllProjects', 'es-ES', N'Todos los proyectos', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_AllProjects', 'de-DE', N'Alle projekte', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_AllProjects', 'pl-PL', N'Wszystkie projekty', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Empty', 'fr-FR', N'Projet vierge', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Empty', 'en-US', N'New project', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Empty', 'pt-BR', N'Novo projeto', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Empty', 'es-ES', N'Nuevo Proyecto', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Empty', 'de-DE', N'Neues Projekt', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Empty', 'pl-PL', N'Nowy projekt', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSI', 'fr-FR', N'Scénario initial figé', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSI', 'en-US', N'Initial scenario frozen', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSI', 'pt-BR', N'Cenário inicial congelado', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSI', 'es-ES', N'Escenario inicial fijado', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSI', 'de-DE', N'Initiales Szenario eingefroren', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSI', 'pl-PL', N'Scenariusz początkowy zablokowany', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSC', 'fr-FR', N'Au moins un scénario cible', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSC', 'en-US', N'At least one target scenario', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSC', 'pt-BR', N'Pelo menos um cenário alvo', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSC', 'es-ES', N'Al menos un escenario objetivo', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSC', 'de-DE', N'Mindestens ein Zielszenario', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSC', 'pl-PL', N'Przynajmniej jeden scenariusz docelowy', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSC', 'fr-FR', N'Au moins un scénario cible figé', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSC', 'en-US', N'At least one frozen target scenario', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSC', 'pt-BR', N'Pelo menos um cenário alvo congelado', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSC', 'es-ES', N'Al menos un escenario objetivo fijado', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSC', 'de-DE', N'Mindestens ein eingefrorenes Zielszenario', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSC', 'pl-PL', N'Przynajmniej jeden scenariusz docelowy zablokowany', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSV', 'fr-FR', N'Un scénario de validation', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSV', 'en-US', N'One validation scenario', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSV', 'pt-BR', N'Um cenário de validação', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSV', 'es-ES', N'Un escenario de validación', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSV', 'de-DE', N'Ein Validierungsszenario', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_OneSV', 'pl-PL', N'Scenariusz zatwierdzony', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSV', 'fr-FR', N'Scénario de validation figé', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSV', 'en-US', N'Validation scenario frozen', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSV', 'pt-BR', N'Cenário de validação congelado', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSV', 'es-ES', N'Escenario de validación fijado', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSV', 'de-DE', N'Validierungsszenario eingefroren', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_FixedSV', 'pl-PL', N'Zatwierdzony scenariusz zablokowany', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone', 'fr-FR', N'Abandonner', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone', 'en-US', N'Abandone', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone', 'pt-BR', N'Abandone', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone', 'es-ES', N'Abandone', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone', 'de-DE', N'Abandone', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone', 'pl-PL', N'Abandone', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover', 'fr-FR', N'Récupérer', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover', 'en-US', N'Recover', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover', 'pt-BR', N'Recover', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover', 'es-ES', N'Recover', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover', 'de-DE', N'Recover', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover', 'pl-PL', N'Recover', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone_Tooltip', 'fr-FR', N'Abandonner le projet', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone_Tooltip', 'en-US', N'Abandone the project', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone_Tooltip', 'pt-BR', N'Abandone the project', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone_Tooltip', 'es-ES', N'Abandone the project', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone_Tooltip', 'de-DE', N'Abandone the project', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone_Tooltip', 'pl-PL', N'Abandone the project', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover_Tooltip', 'fr-FR', N'Récupérer le projet', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover_Tooltip', 'en-US', N'Recover the project', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover_Tooltip', 'pt-BR', N'Recover the project', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover_Tooltip', 'es-ES', N'Recover the project', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover_Tooltip', 'de-DE', N'Recover the project', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover_Tooltip', 'pl-PL', N'Recover the project', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone_Ask', 'fr-FR', N'Voulez-vous abandonner le projet {0}?', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone_Ask', 'en-US', N'Do you want abandone the {0} project?', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone_Ask', 'pt-BR', N'Do you want abandone the {0} project?', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone_Ask', 'es-ES', N'Do you want abandone the {0} project?', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone_Ask', 'de-DE', N'Do you want abandone the {0} project?', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Abandone_Ask', 'pl-PL', N'Do you want abandone the {0} project?', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover_Ask', 'fr-FR', N'Voulez-vous récupérer le projet {0}?', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover_Ask', 'en-US', N'Do you want recover the {0} project?', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover_Ask', 'pt-BR', N'Do you want recover the {0} project?', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover_Ask', 'es-ES', N'Do you want recover the {0} project?', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover_Ask', 'de-DE', N'Do you want recover the {0} project?', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_Recover_Ask', 'pl-PL', N'Do you want recover the {0} project?', null;
GO
EXEC InsertOrUpdateResource 'View_PrepareProject_SyncVideo', 'fr-FR', N'Synchroniser les vidéos', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_SyncVideo', 'en-US', N'Synchronize videos', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_SyncVideo', 'pt-BR', N'Synchronize videos', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_SyncVideo', 'es-ES', N'Synchronize videos', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_SyncVideo', 'de-DE', N'Synchronize videos', null;
EXEC InsertOrUpdateResource 'View_PrepareProject_SyncVideo', 'pl-PL', N'Synchronize videos', null;
GO
EXEC InsertOrUpdateResource 'View_MainWindow_Synchronize', 'fr-FR', N'Synchroniser', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Synchronize', 'en-US', N'Synchronize', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Synchronize', 'pt-BR', N'Sincronizar', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Synchronize', 'es-ES', N'Sincronizar', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Synchronize', 'de-DE', N'Synchronisieren', null;
EXEC InsertOrUpdateResource 'View_MainWindow_Synchronize', 'pl-PL', N'Synchronizować', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Who', 'fr-FR', N'Qui', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Who', 'en-US', N'Who', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Who', 'pt-BR', N'Quem', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Who', 'es-ES', N'Quién', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Who', 'de-DE', N'Wer', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_Who', 'pl-PL', N'Kto', null;
GO
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_When', 'fr-FR', N'Quand', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_When', 'en-US', N'When', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_When', 'pt-BR', N'Quando', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_When', 'es-ES', N'Cuándo', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_When', 'de-DE', N'Wann', null;
EXEC InsertOrUpdateResource 'View_AnalyzeBuild_When', 'pl-PL', N'Kiedy', null;
GO
EXEC InsertOrUpdateResource 'Common_Context_BindingValidationFailed', 'fr-FR', N'La valeur ''{0}'' n''a pas pu être convertie', N'La valeur d''origine';
EXEC InsertOrUpdateResource 'Common_Context_BindingValidationFailed', 'en-US', N'Value ''{0}'' could not be converted', N'La valeur d''origine';
EXEC InsertOrUpdateResource 'Common_Context_BindingValidationFailed', 'pt-BR', N'O valor ''{0}'' não pôde ser convertido', N'La valeur d''origine';
EXEC InsertOrUpdateResource 'Common_Context_BindingValidationFailed', 'es-ES', N'El valor ''{0}'' no se pudó convertir', N'La valeur d''origine';
EXEC InsertOrUpdateResource 'Common_Context_BindingValidationFailed', 'de-DE', N'Der Wert ''{0}'' könnte nicht konvertiert werden', N'La valeur d''origine';
EXEC InsertOrUpdateResource 'Common_Context_BindingValidationFailed', 'pl-PL', N'Wartość ''{0}'' nie mogła zostać przekształcona', N'La valeur d''origine';
GO
EXEC InsertOrUpdateResource 'View_PrepareScenarios_CantChangeStatut', 'fr-FR', N'Défiger un scénario n’est pas autorisé tant qu''un scénario de Validation existe dans ce projet', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_CantChangeStatut', 'en-US', N'Unfreezing a scenario is not allowed as long a Validation scenario exists in this project', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_CantChangeStatut', 'pt-BR', N'Não é permitido descongelar um cenário enquanto o cenário de validação existir no projeto', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_CantChangeStatut', 'es-ES', N'No es posible liberar el escenario debido a que hay un escenario de validación existente en este proyecto', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_CantChangeStatut', 'de-DE', N'Wiederaktivieren eines Szenarios ist nicht erlaubt, solange ein Validierungsszenario in diesem Projekt existiert', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_CantChangeStatut', 'pl-PL', N'Odblokowanie scenariusza nie jest możliwe tak długo jsk w tym projekcie istnieje scenariusz zatwierdzający', null;
GO
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_WBS', 'fr-FR', N'Id', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_WBS', 'en-US', N'Id', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_WBS', 'pt-BR', N'Id', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_WBS', 'es-ES', N'Id', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_WBS', 'de-DE', N'Id', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_WBS', 'pl-PL', N'Id', null;
GO
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Label', 'fr-FR', N'Tâche', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Label', 'en-US', N'Label', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Label', 'pt-BR', N'Label', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Label', 'es-ES', N'Label', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Label', 'de-DE', N'Label', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Label', 'pl-PL', N'Label', null;
GO
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Start', 'fr-FR', N'Début vidéo', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Start', 'en-US', N'Video start', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Start', 'pt-BR', N'Video start', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Start', 'es-ES', N'Video start', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Start', 'de-DE', N'Video start', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Start', 'pl-PL', N'Video start', null;
GO
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Finish', 'fr-FR', N'Fin vidéo', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Finish', 'en-US', N'Video finish', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Finish', 'pt-BR', N'Video finish', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Finish', 'es-ES', N'Video finish', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Finish', 'de-DE', N'Video finish', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Finish', 'pl-PL', N'Video finish', null;
GO
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_BuildStart', 'fr-FR', N'Début process', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_BuildStart', 'en-US', N'Process start', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_BuildStart', 'pt-BR', N'Process start', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_BuildStart', 'es-ES', N'Process start', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_BuildStart', 'de-DE', N'Process start', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_BuildStart', 'pl-PL', N'Process start', null;
GO
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_BuildFinish', 'fr-FR', N'Fin process', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_BuildFinish', 'en-US', N'Process finish', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_BuildFinish', 'pt-BR', N'Process finish', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_BuildFinish', 'es-ES', N'Process finish', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_BuildFinish', 'de-DE', N'Process finish', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_BuildFinish', 'pl-PL', N'Process finish', null;
GO
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Thumbnail', 'fr-FR', N'Vignette', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Thumbnail', 'en-US', N'Thumbnail', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Thumbnail', 'pt-BR', N'Thumbnail', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Thumbnail', 'es-ES', N'Thumbnail', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Thumbnail', 'de-DE', N'Thumbnail', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_Thumbnail', 'pl-PL', N'Thumbnail', null;
GO
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_IsRandom', 'fr-FR', N'Au hasard', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_IsRandom', 'en-US', N'Random', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_IsRandom', 'pt-BR', N'Random', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_IsRandom', 'es-ES', N'Random', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_IsRandom', 'de-DE', N'Random', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_IsRandom', 'pl-PL', N'Random', null;
GO
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_DifferenceReason', 'fr-FR', N'Raison de la différence', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_DifferenceReason', 'en-US', N'Difference reason', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_DifferenceReason', 'pt-BR', N'Difference reason', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_DifferenceReason', 'es-ES', N'Difference reason', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_DifferenceReason', 'de-DE', N'Difference reason', null;
EXEC InsertOrUpdateResource 'View_Publish_DataGrid_DifferenceReason', 'pl-PL', N'Difference reason', null;
GO
EXEC InsertOrUpdateResource 'Wizard_AddVideo_Title', 'fr-FR', N'Assistant d''ajout de vidéo', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_Title', 'en-US', N'Video add wizard', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_Title', 'pt-BR', N'Video add wizard', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_Title', 'es-ES', N'Video add wizard', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_Title', 'de-DE', N'Video add wizard', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_Title', 'pl-PL', N'Video add wizard', null;
GO
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyCameraName', 'fr-FR', N'Veuillez préciser le nom de la caméra ayant filmé la vidéo (Optionel) :', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyCameraName', 'en-US', N'Please specify the name of the camera that filmed the video (Optional) :', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyCameraName', 'pt-BR', N'Please specify the name of the camera that filmed the video (Optional) :', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyCameraName', 'es-ES', N'Please specify the name of the camera that filmed the video (Optional) :', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyCameraName', 'de-DE', N'Please specify the name of the camera that filmed the video (Optional) :', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyCameraName', 'pl-PL', N'Please specify the name of the camera that filmed the video (Optional) :', null;
GO
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyResource', 'fr-FR', N'Veuillez préciser la ressource dédiée :', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyResource', 'en-US', N'Please specify the dedicated resource :', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyResource', 'pt-BR', N'Please specify the dedicated resource :', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyResource', 'es-ES', N'Please specify the dedicated resource :', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyResource', 'de-DE', N'Please specify the dedicated resource :', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyResource', 'pl-PL', N'Please specify the dedicated resource :', null;
GO
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyResourceView', 'fr-FR', N'La vue de la ressource est-elle interne ou externe ?', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyResourceView', 'en-US', N'Is the view of the resource internal or external ?', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyResourceView', 'pt-BR', N'Is the view of the resource internal or external ?', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyResourceView', 'es-ES', N'Is the view of the resource internal or external ?', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyResourceView', 'de-DE', N'Is the view of the resource internal or external ?', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifyResourceView', 'pl-PL', N'Is the view of the resource internal or external ?', null;
GO
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifySync', 'fr-FR', N'Désirez vous envoyer la vidéo sur le serveur maintenant ?', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifySync', 'en-US', N'Do you want to upload the video to the server now ?', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifySync', 'pt-BR', N'Do you want to upload the video to the server now ?', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifySync', 'es-ES', N'Do you want to upload the video to the server now ?', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifySync', 'de-DE', N'Do you want to upload the video to the server now ?', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SpecifySync', 'pl-PL', N'Do you want to upload the video to the server now ?', null;
GO
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SendVideoNow', 'fr-FR', N'Envoyer la video vers le serveur maintenant', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SendVideoNow', 'en-US', N'Send the video to the server now', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SendVideoNow', 'pt-BR', N'Send the video to the server now', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SendVideoNow', 'es-ES', N'Send the video to the server now', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SendVideoNow', 'de-DE', N'Send the video to the server now', null;
EXEC InsertOrUpdateResource 'Wizard_AddVideo_SendVideoNow', 'pl-PL', N'Send the video to the server now', null;
GO

INSERT INTO [dbo].[AppResourceValue]
		([ResourceId]
		,[LanguageCode]
		,[Value]
		,[Comment]
		,[CreatedByUserId]
		,[CreationDate]
		,[ModifiedByUserId]
		,[LastModificationDate])
		
	SELECT [ResourceId]
		,'en-US'
		,[Value] + '¤'
		,[Comment]
		,[CreatedByUserId]
		,[CreationDate]
		,[ModifiedByUserId]
		,[LastModificationDate]
		 FROM [AppResourceValue] v
		 WHERE [LanguageCode] = 'fr-FR' AND
		 (SELECT COUNT([ResourceId]) FROM [AppResourceValue] WHERE [AppResourceValue].[ResourceId] = v.[ResourceId] AND [AppResourceValue].[LanguageCode] = 'en-US') = 0;
                

INSERT INTO [dbo].[AppResourceValue]
		([ResourceId]
		,[LanguageCode]
		,[Value]
		,[Comment]
		,[CreatedByUserId]
		,[CreationDate]
		,[ModifiedByUserId]
		,[LastModificationDate])
		
	SELECT [ResourceId]
		,'pt-BR'
		,[Value] + '¤'
		,[Comment]
		,[CreatedByUserId]
		,[CreationDate]
		,[ModifiedByUserId]
		,[LastModificationDate]
		 FROM [AppResourceValue] v
		 WHERE [LanguageCode] = 'fr-FR' AND
		 (SELECT COUNT([ResourceId]) FROM [AppResourceValue] WHERE [AppResourceValue].[ResourceId] = v.[ResourceId] AND [AppResourceValue].[LanguageCode] = 'pt-BR') = 0;
                

INSERT INTO [dbo].[AppResourceValue]
		([ResourceId]
		,[LanguageCode]
		,[Value]
		,[Comment]
		,[CreatedByUserId]
		,[CreationDate]
		,[ModifiedByUserId]
		,[LastModificationDate])
		
	SELECT [ResourceId]
		,'es-ES'
		,[Value] + '¤'
		,[Comment]
		,[CreatedByUserId]
		,[CreationDate]
		,[ModifiedByUserId]
		,[LastModificationDate]
		 FROM [AppResourceValue] v
		 WHERE [LanguageCode] = 'fr-FR' AND
		 (SELECT COUNT([ResourceId]) FROM [AppResourceValue] WHERE [AppResourceValue].[ResourceId] = v.[ResourceId] AND [AppResourceValue].[LanguageCode] = 'es-ES') = 0;
                

INSERT INTO [dbo].[AppResourceValue]
		([ResourceId]
		,[LanguageCode]
		,[Value]
		,[Comment]
		,[CreatedByUserId]
		,[CreationDate]
		,[ModifiedByUserId]
		,[LastModificationDate])
		
	SELECT [ResourceId]
		,'de-DE'
		,[Value] + '¤'
		,[Comment]
		,[CreatedByUserId]
		,[CreationDate]
		,[ModifiedByUserId]
		,[LastModificationDate]
		 FROM [AppResourceValue] v
		 WHERE [LanguageCode] = 'fr-FR' AND
		 (SELECT COUNT([ResourceId]) FROM [AppResourceValue] WHERE [AppResourceValue].[ResourceId] = v.[ResourceId] AND [AppResourceValue].[LanguageCode] = 'de-DE') = 0;
                

INSERT INTO [dbo].[AppResourceValue]
		([ResourceId]
		,[LanguageCode]
		,[Value]
		,[Comment]
		,[CreatedByUserId]
		,[CreationDate]
		,[ModifiedByUserId]
		,[LastModificationDate])
		
	SELECT [ResourceId]
		,'pl-PL'
		,[Value] + '¤'
		,[Comment]
		,[CreatedByUserId]
		,[CreationDate]
		,[ModifiedByUserId]
		,[LastModificationDate]
		 FROM [AppResourceValue] v
		 WHERE [LanguageCode] = 'fr-FR' AND
		 (SELECT COUNT([ResourceId]) FROM [AppResourceValue] WHERE [AppResourceValue].[ResourceId] = v.[ResourceId] AND [AppResourceValue].[LanguageCode] = 'pl-PL') = 0;
GO


-- Roles par défaut
/* Insère les rôles par défaut */

DECLARE @admin int;
SET @admin = (SELECT [UserId] FROM [dbo].[User] WHERE [UserName] = 'admin');

INSERT INTO [dbo].[Role] ([RoleCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('ANA001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_ANA_SHORT'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_ANA_LONG'));
INSERT INTO [dbo].[Role] ([RoleCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('CON001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_CON_SHORT'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_CON_LONG'));
INSERT INTO [dbo].[Role] ([RoleCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('TEC001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_TEC_SHORT'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_TEC_LONG'));
INSERT INTO [dbo].[Role] ([RoleCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('ADM001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_ADM_SHORT'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_ADM_LONG'));
INSERT INTO [dbo].[Role] ([RoleCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('SUP001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_SUP_SHORT'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_SUP_LONG'));
INSERT INTO [dbo].[Role] ([RoleCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('OPE001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_OPE_SHORT'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_OPE_LONG'));
INSERT INTO [dbo].[Role] ([RoleCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('DOC001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_DOC_SHORT'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_DOC_LONG'));
INSERT INTO [dbo].[Role] ([RoleCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('TRA001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_TRA_SHORT'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_TRA_LONG'));
INSERT INTO [dbo].[Role] ([RoleCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('EVA001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_EVA_SHORT'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Role_EVA_LONG'));

-- Rôles des utilisateurs par défaut
/* Définit les rôles des utilisateurs par défaut */

INSERT INTO [dbo].[UserRole] ([UserId], [RoleCode]) VALUES (@admin, 'ADM001');
GO


-- Référentiels


/* Contient les différent référentiels */

INSERT INTO [dbo].[Objective]([ObjectiveCode], [ShortLabelResourceId],[LongLabelResourceId]) VALUES ('PROD01', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Obj_Prod_Short'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Obj_Prod_Long'));
INSERT INTO [dbo].[Objective]([ObjectiveCode], [ShortLabelResourceId],[LongLabelResourceId]) VALUES ('STAND1', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Obj_Std_Short'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Obj_Std_Long'));
INSERT INTO [dbo].[Objective]([ObjectiveCode], [ShortLabelResourceId],[LongLabelResourceId]) VALUES ('VSM001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Obj_VSM_Short'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'Obj_VSM_Long'));

INSERT INTO [dbo].[ActionType]([ActionTypeCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('I00001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ActionType_I_Short'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ActionType_I_Long'));
INSERT INTO [dbo].[ActionType]([ActionTypeCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('E00001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ActionType_E_Short'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ActionType_E_Long'));
INSERT INTO [dbo].[ActionType]([ActionTypeCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('S00001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ActionType_S_Short'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ActionType_S_Long'));

INSERT INTO [dbo].[ActionValue]([ActionValueCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('VA0001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ActionValue_VA_Short'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ActionValue_VA_Long'));
INSERT INTO [dbo].[ActionValue]([ActionValueCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('NVA001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ActionValue_NVA_Short'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ActionValue_NVA_Long'));
INSERT INTO [dbo].[ActionValue]([ActionValueCode], [ShortLabelResourceId], [LongLabelResourceId]) VALUES ('BNVA01', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ActionValue_BNVA_Short'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ActionValue_BNVA_Long'));

GO

/* SCENARIO STATES */
INSERT INTO [dbo].[ScenarioState] ([ScenarioStateCode] ,[ShortLabelResourceId] ,[LongLabelResourceId]) VALUES ('BRO001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ScenarioState_BRO_Short'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ScenarioState_BRO_Long'));
INSERT INTO [dbo].[ScenarioState] ([ScenarioStateCode] ,[ShortLabelResourceId] ,[LongLabelResourceId]) VALUES ('VAL001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ScenarioState_VAL_Short'), (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ScenarioState_VAL_Long'));
GO

/* SCENARIO NATURES */
INSERT INTO [dbo].[ScenarioNature] ([ScenarioNatureCode] ,[ShortLabelResourceId] ,[LongLabelResourceId]) VALUES ('INI001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ScenarioState_INI_Short'),(SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ScenarioState_INI_Long'));
INSERT INTO [dbo].[ScenarioNature] ([ScenarioNatureCode] ,[ShortLabelResourceId] ,[LongLabelResourceId]) VALUES ('CIB001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ScenarioState_CIB_Short'),(SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ScenarioState_CIB_Long'));
INSERT INTO [dbo].[ScenarioNature] ([ScenarioNatureCode] ,[ShortLabelResourceId] ,[LongLabelResourceId]) VALUES ('REA001', (SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ScenarioState_REA_Short'),(SELECT [ResourceId] FROM [AppResourceKey] WHERE [ResourceKey] = 'ScenarioState_REA_Long'));
GO

/* Utilise les libellés par défaut des référentiels */
INSERT INTO [dbo].[Referentials] ([ReferentialId], [Label]) VALUES (1, (SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT OUTER JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_Operator' AND v.[LanguageCode] = 'en-US'))
INSERT INTO [dbo].[Referentials] ([ReferentialId], [Label]) VALUES (2, (SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT OUTER JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_Equipment' AND v.[LanguageCode] = 'en-US'))
INSERT INTO [dbo].[Referentials] ([ReferentialId], [Label]) VALUES (3, (SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT OUTER JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_Category' AND v.[LanguageCode] = 'en-US'))
INSERT INTO [dbo].[Referentials] ([ReferentialId], [Label]) VALUES (100, (SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT OUTER JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_Skill' AND v.[LanguageCode] = 'en-US'))

INSERT INTO [dbo].[Referentials] ([ReferentialId], [Label]) VALUES (4, (SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT OUTER JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_R1' AND v.[LanguageCode] = 'en-US'))
INSERT INTO [dbo].[Referentials] ([ReferentialId], [Label]) VALUES (5, (SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT OUTER JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_R2' AND v.[LanguageCode] = 'en-US'))
INSERT INTO [dbo].[Referentials] ([ReferentialId], [Label]) VALUES (6, (SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT OUTER JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_R3' AND v.[LanguageCode] = 'en-US'))
INSERT INTO [dbo].[Referentials] ([ReferentialId], [Label]) VALUES (7, (SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT OUTER JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_R4' AND v.[LanguageCode] = 'en-US'))
INSERT INTO [dbo].[Referentials] ([ReferentialId], [Label]) VALUES (8, (SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT OUTER JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_R5' AND v.[LanguageCode] = 'en-US'))
INSERT INTO [dbo].[Referentials] ([ReferentialId], [Label]) VALUES (9, (SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT OUTER JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_R6' AND v.[LanguageCode] = 'en-US'))
INSERT INTO [dbo].[Referentials] ([ReferentialId], [Label]) VALUES (10, (SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT OUTER JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_R7' AND v.[LanguageCode] = 'en-US'))
GO

INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (1,N'Do not know',0)
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (2,N'Incorrect or incomplete answer',0)
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (3,N'Do not know how to do',0)
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (4,N'Incorrect or incomplete method',0)
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (5,N'Inadequate or missing PPE',0)
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (6,N'Unsuitable or missing material',0)
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (7,N'Other',1)
INSERT INTO [dbo].[QualificationReason] ([Number],[Comment],[IsEditable]) VALUES (8,N'Not done',0)
GO

INSERT INTO [dbo].[NotificationTypeSetting]([Description],[BodyTemplate],[PdfTemplate]) VALUES
('Evaluation', 'Evaluation.cshtml', 'PdfEvaluation.cshtml'),
('Inspection', 'Inspection.cshtml', 'PdfInspection.cshtml'),
('Audit', 'Audit.cshtml', 'PdfAudit.cshtml'),
('Anomaly', 'Anomaly.cshtml', 'PdfAnomaly.cshtml'),
('Formation', 'Formation.cshtml', 'PdfFormation.cshtml')
GO
INSERT INTO [dbo].[NotificationType]([Label], [NotificationTypeSettingId], [NotificationCategory]) VALUES
('Evaluation', (SELECT [Id] FROM [NotificationTypeSetting] WHERE [Description]='Evaluation'), 0),
('Inspection', (SELECT [Id] FROM [NotificationTypeSetting] WHERE [Description]='Inspection'), 1),
('Audit', (SELECT [Id] FROM [NotificationTypeSetting] WHERE [Description]='Audit'), 2),
('Anomaly', (SELECT [Id] FROM [NotificationTypeSetting] WHERE [Description]='Anomaly'), 3),
('Formation', (SELECT [Id] FROM [NotificationTypeSetting] WHERE [Description]='Formation'), 4)
GO

INSERT INTO [dbo].[AppSetting]([Key],[Value])
VALUES
('SMTP_UseAnonymMode',cast('false' as varbinary)),
('SMTP_Port',cast('587' as varbinary)),
('SMTP_DeliveryMethod',cast('0' as varbinary)),
('SMTP_UseDefaultCredentials',cast('false' as varbinary)),
('SMTP_EnableSsl',cast('true' as varbinary)),
('SMTP_Client',cast('smtp.gmail.com' as varbinary)),
('Email_Sender_Address', Cast('' AS varbinary(max))),
('Email_Sender_Password', Cast('' AS varbinary(max)))
GO

-- Version

/* Définit la version de l'application avec laquelle l'application est compatible */
EXEC InsertOrUpdateDatabaseVersion N'4.0.0.35';
GO