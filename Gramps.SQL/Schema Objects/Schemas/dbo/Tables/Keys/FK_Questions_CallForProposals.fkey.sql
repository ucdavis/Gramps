ALTER TABLE [dbo].[Questions]
    ADD CONSTRAINT [FK_Questions_CallForProposals] FOREIGN KEY ([CallForProposalId]) REFERENCES [dbo].[CallForProposals] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

