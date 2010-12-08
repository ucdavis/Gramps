ALTER TABLE [dbo].[Proposals]
    ADD CONSTRAINT [FK_Proposals_CallForProposals] FOREIGN KEY ([CallForProposalId]) REFERENCES [dbo].[CallForProposals] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

