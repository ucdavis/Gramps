ALTER TABLE [dbo].[Comments]
    ADD CONSTRAINT [FK_Comments_Editors] FOREIGN KEY ([EditorId]) REFERENCES [dbo].[Editors] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

