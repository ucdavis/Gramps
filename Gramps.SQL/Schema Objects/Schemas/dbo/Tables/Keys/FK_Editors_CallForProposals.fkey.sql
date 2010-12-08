ALTER TABLE [dbo].[Editors]
    ADD CONSTRAINT [FK_Editors_CallForProposals] FOREIGN KEY ([CallForProposalId]) REFERENCES [dbo].[CallForProposals] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

