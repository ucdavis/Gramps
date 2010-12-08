ALTER TABLE [dbo].[Emails]
    ADD CONSTRAINT [FK_Emails_CallForProposals] FOREIGN KEY ([CallForProposalId]) REFERENCES [dbo].[CallForProposals] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

