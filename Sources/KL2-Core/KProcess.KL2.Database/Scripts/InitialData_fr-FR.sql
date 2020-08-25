USE [KProcess.KL2]
GO

UPDATE [dbo].[User] SET DefaultLanguageCode='fr-FR' WHERE Username='admin';
GO

/* Utilise les libellés par défaut des référentiels */
UPDATE [dbo].[Referentials] SET Label=(SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_Operator' AND v.[LanguageCode] = 'fr-FR') WHERE ReferentialId=1;
UPDATE [dbo].[Referentials] SET Label=(SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_Equipment' AND v.[LanguageCode] = 'fr-FR') WHERE ReferentialId=2;
UPDATE [dbo].[Referentials] SET Label=(SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_Category' AND v.[LanguageCode] = 'fr-FR') WHERE ReferentialId=3;
UPDATE [dbo].[Referentials] SET Label=(SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_Skill' AND v.[LanguageCode] = 'fr-FR') WHERE ReferentialId=100;
UPDATE [dbo].[Referentials] SET Label=(SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_R1' AND v.[LanguageCode] = 'fr-FR') WHERE ReferentialId=4;
UPDATE [dbo].[Referentials] SET Label=(SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_R2' AND v.[LanguageCode] = 'fr-FR') WHERE ReferentialId=5;
UPDATE [dbo].[Referentials] SET Label=(SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_R3' AND v.[LanguageCode] = 'fr-FR') WHERE ReferentialId=6;
UPDATE [dbo].[Referentials] SET Label=(SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_R4' AND v.[LanguageCode] = 'fr-FR') WHERE ReferentialId=7;
UPDATE [dbo].[Referentials] SET Label=(SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_R5' AND v.[LanguageCode] = 'fr-FR') WHERE ReferentialId=8;
UPDATE [dbo].[Referentials] SET Label=(SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_R6' AND v.[LanguageCode] = 'fr-FR') WHERE ReferentialId=9;
UPDATE [dbo].[Referentials] SET Label=(SELECT v.[Value] FROM [dbo].[AppResourceKey] k LEFT JOIN [dbo].[AppResourceValue] v ON v.[ResourceId] = k.[ResourceId] WHERE k.[ResourceKey] = N'Common_Referential_R7' AND v.[LanguageCode] = 'fr-FR') WHERE ReferentialId=10;
GO

UPDATE [dbo].[QualificationReason] SET Comment = N'Ne sait pas' WHERE Number = 1;
UPDATE [dbo].[QualificationReason] SET Comment = N'Réponse incorrecte ou incomplète' WHERE Number = 2;
UPDATE [dbo].[QualificationReason] SET Comment = N'Ne sait pas faire' WHERE Number = 3;
UPDATE [dbo].[QualificationReason] SET Comment = N'Méthode incorrecte ou incomplète' WHERE Number = 4;
UPDATE [dbo].[QualificationReason] SET Comment = N'EPI inadapté ou manquant' WHERE Number = 5;
UPDATE [dbo].[QualificationReason] SET Comment = N'Matériel inadapté ou manquant' WHERE Number = 6;
UPDATE [dbo].[QualificationReason] SET Comment = N'Autre' WHERE Number = 7;
UPDATE [dbo].[QualificationReason] SET Comment = N'Non fait' WHERE Number = 8;
GO