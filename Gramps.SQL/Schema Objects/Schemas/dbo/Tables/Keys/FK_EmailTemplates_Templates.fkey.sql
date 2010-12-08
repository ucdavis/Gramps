ALTER TABLE [dbo].[EmailTemplates]
    ADD CONSTRAINT [FK_EmailTemplates_Templates] FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[Templates] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

