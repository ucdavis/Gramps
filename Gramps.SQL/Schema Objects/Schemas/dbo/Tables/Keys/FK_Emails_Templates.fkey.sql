ALTER TABLE [dbo].[Emails]
    ADD CONSTRAINT [FK_Emails_Templates] FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[Templates] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

