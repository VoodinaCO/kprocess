-- ADD Vidéo not found
EXEC InsertOrUpdateResource 'View_MainWindow_VideoNotFound', 'fr-FR', N'Le fichier vidéo {0} n''existe pas.', '{0} correspond au chemin du fichier vidéo';
EXEC InsertOrUpdateResource 'View_MainWindow_VideoNotFound', 'en-US', N'The video file {0} doesn''t exist.', '{0} correspond au chemin du fichier vidéo';
EXEC InsertOrUpdateResource 'View_MainWindow_VideoNotFound', 'pt-BR', N'O Ficheiro Vídeo {0} não foi encontrado.', '{0} correspond au chemin du fichier vidéo';
EXEC InsertOrUpdateResource 'View_MainWindow_VideoNotFound', 'es-ES', N'El fichero video {0} no existe.', '{0} correspond au chemin du fichier vidéo';
EXEC InsertOrUpdateResource 'View_MainWindow_VideoNotFound', 'de-DE', N'Die Videodatei {0} existiert nicht.', '{0} correspond au chemin du fichier vidéo';
EXEC InsertOrUpdateResource 'View_MainWindow_VideoNotFound', 'pl-PL', N'Plik video {0} nie istnieje.', '{0} correspond au chemin du fichier vidéo';
GO

--ADD Envoyer le rapport
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_AllowSendReport', 'fr-FR', N'Envoyer automatiquement les rapports d''erreur détaillés au support K-process.', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_AllowSendReport', 'en-US', N'Automatically send detailed error reports to K-process support.', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_AllowSendReport', 'pt-BR', N'Enviar relatório de erros detalhados automaticamente para o suporte da K-process.', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_AllowSendReport', 'es-ES', N'Envía automáticamente informes detallados de errores al soporte de K-process.', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_AllowSendReport', 'de-DE', N'Automatisch detaillierte Fehlerberichte an Support K-process senden.', null;
EXEC InsertOrUpdateResource 'VM_ActivationFrameView_AllowSendReport', 'pl-PL', N'Automatyczne wysyłanie szczegółowych raportów błędu do obsługi K-process.', null;
GO

--EDIT Choisir fichier pour backup
EXEC InsertOrUpdateResource 'View_BackupRestore_SelectABackup', 'fr-FR', N'Entrer un nom pour le fichier de backup', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_SelectABackup', 'en-US', N'Enter a name for the backup file', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_SelectABackup','pt-BR', N'Digite o nome para o arquivo de backup', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_SelectABackup', 'es-ES', N'Introduzca un nombre del archivo', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_SelectABackup', 'de-DE', N'Geben Sie bitte einen Namen für den Backupordner ein', null;
EXEC InsertOrUpdateResource 'View_BackupRestore_SelectABackup', 'pl-PL', N'Nazwij plik zapasowy', null;
GO

--EDIT
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ConvertToNewProject', 'fr-FR', N'Créer un nouveau projet en partant d''un scénario figé.', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ConvertToNewProject', 'en-US', N'Create a new project from a frozen scenario.', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ConvertToNewProject', 'pt-BR', N'Criar um novo projeto a partir de um congelado cenário.', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ConvertToNewProject', 'es-ES', N'Crear un nuevo proyecto a partir del fijado escenario.', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ConvertToNewProject', 'de-DE', N'Erstellen ein neues Projekt aus einem schreibgeschützten Szenario .', null;
EXEC InsertOrUpdateResource 'View_PrepareScenarios_ConvertToNewProject', 'pl-PL', N'Utwórz nowy projekt na bazie zamrożony scenariusza.', null;
GO

--EDIT
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_KeepVideoValidation', 'fr-FR', N'Conserver les séquences vidéo pour les tâches dont la durée n''a pas été modifiée ?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_KeepVideoValidation', 'en-US', N'Keep video clips for tasks whose duration has not been changed?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_KeepVideoValidation', 'pt-BR', N'Manter vídeo clips para tarefas cuja duração não foi alterada?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_KeepVideoValidation', 'es-ES', N'Conservar los video clips para tareas cuya duración no se ha cambiado?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_KeepVideoValidation', 'de-DE', N'Halten Sie Videoclips für Aufgaben, deren Dauer nicht geändert wurde?', null;
EXEC InsertOrUpdateResource 'VM_PrepareScenarios_Message_KeepVideoValidation', 'pl-PL', N'Przechowuj klipy wideo dla zadań, których czas trwania nie został zmieniony?', null;
GO