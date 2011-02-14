ALTER TABLE [dbo].[EmailQueue]
    ADD CONSTRAINT [FK_EmailQueue_CallForProposals] FOREIGN KEY ([CallForProposalId]) REFERENCES [dbo].[CallForProposals] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

