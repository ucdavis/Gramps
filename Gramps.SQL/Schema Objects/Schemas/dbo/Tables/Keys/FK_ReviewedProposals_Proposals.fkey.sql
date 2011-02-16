ALTER TABLE [dbo].[ReviewedProposals]
    ADD CONSTRAINT [FK_ReviewedProposals_Proposals] FOREIGN KEY ([ProposalId]) REFERENCES [dbo].[Proposals] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

