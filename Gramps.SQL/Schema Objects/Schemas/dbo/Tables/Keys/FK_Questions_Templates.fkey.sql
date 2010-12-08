ALTER TABLE [dbo].[Questions]
    ADD CONSTRAINT [FK_Questions_Templates] FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[Templates] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

