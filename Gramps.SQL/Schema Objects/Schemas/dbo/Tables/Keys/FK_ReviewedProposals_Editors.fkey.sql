ALTER TABLE [dbo].[ReviewedProposals]
    ADD CONSTRAINT [FK_ReviewedProposals_Editors] FOREIGN KEY ([EditorId]) REFERENCES [dbo].[Editors] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

