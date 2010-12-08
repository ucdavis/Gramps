ALTER TABLE [dbo].[Answers]
    ADD CONSTRAINT [FK_Answers_Proposals] FOREIGN KEY ([ProposalId]) REFERENCES [dbo].[Proposals] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

