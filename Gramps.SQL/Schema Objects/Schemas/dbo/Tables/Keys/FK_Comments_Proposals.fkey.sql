ALTER TABLE [dbo].[Comments]
    ADD CONSTRAINT [FK_Comments_Proposals] FOREIGN KEY ([ProposalId]) REFERENCES [dbo].[Proposals] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

