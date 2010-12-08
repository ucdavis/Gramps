ALTER TABLE [dbo].[Editors]
    ADD CONSTRAINT [FK_Editors_Templates] FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[Templates] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

