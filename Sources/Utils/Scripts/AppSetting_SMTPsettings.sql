INSERT INTO [dbo].[AppSetting]([Key],[Value])
VALUES
('SMTP_Port',cast('587' as varbinary)),
('SMTP_DeliveryMethod',cast('0' as varbinary)),
('SMTP_UseDefaultCredentials',cast('false' as varbinary)),
('SMTP_EnableSsl',cast('true' as varbinary)),
('SMTP_Client',cast('smtp.gmail.com' as varbinary)),
('Email_Sender_Address', Cast('testnalysa@gmail.com' AS varbinary(max))),
('Email_Sender_Password', Cast('123ASD!@#' AS varbinary(max)))