ALTER TABLE [dbo].[Reports]
    ADD CONSTRAINT [FK_Reports_CallForProposals] FOREIGN KEY ([CallForProposalId]) REFERENCES [dbo].[CallForProposals] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

