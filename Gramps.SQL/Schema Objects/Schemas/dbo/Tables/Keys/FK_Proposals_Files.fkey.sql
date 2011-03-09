ALTER TABLE [dbo].[Proposals]
    ADD CONSTRAINT [FK_Proposals_Files] FOREIGN KEY ([FileId]) REFERENCES [dbo].[Files] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

