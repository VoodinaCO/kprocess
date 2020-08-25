INSERT INTO dbo.[NotificationTypeSetting]([Description],[BodyTemplate],[PdfTemplate]) VALUES
('Evaluation', 'Evaluation.cshtml', 'PdfEvaluation.cshtml'),
('Inspection', 'Inspection.cshtml', 'PdfInspection.cshtml'),
('Audit', 'Audit.cshtml', 'PdfAudit.cshtml'),
('Anomaly', 'Anomaly.cshtml', 'PdfAnomaly.cshtml')


INSERT INTO dbo.[NotificationType]([Label], [NotificationTypeSettingId], [NotificationCategory]) VALUES
('Evaluation', (SELECT [Id] FROM [NotificationTypeSetting] WHERE [Description]='Evaluation'), 0),
('Inspection', (SELECT [Id] FROM [NotificationTypeSetting] WHERE [Description]='Inspection'), 0),
('Audit', (SELECT [Id] FROM [NotificationTypeSetting] WHERE [Description]='Audit'), 0),
('Anomaly', (SELECT [Id] FROM [NotificationTypeSetting] WHERE [Description]='Anomaly'), 0)