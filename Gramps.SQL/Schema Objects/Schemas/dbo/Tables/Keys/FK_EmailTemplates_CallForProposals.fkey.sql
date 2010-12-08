ALTER TABLE [dbo].[EmailTemplates]
    ADD CONSTRAINT [FK_EmailTemplates_CallForProposals] FOREIGN KEY ([CallForProposalId]) REFERENCES [dbo].[CallForProposals] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

