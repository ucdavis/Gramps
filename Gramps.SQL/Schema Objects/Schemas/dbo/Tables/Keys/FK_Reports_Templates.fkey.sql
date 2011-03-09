ALTER TABLE [dbo].[Reports]
    ADD CONSTRAINT [FK_Reports_Templates] FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[Templates] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

