ALTER TABLE [dbo].[Investigators]
    ADD CONSTRAINT [FK_Investigators_Proposals] FOREIGN KEY ([ProposalId]) REFERENCES [dbo].[Proposals] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

