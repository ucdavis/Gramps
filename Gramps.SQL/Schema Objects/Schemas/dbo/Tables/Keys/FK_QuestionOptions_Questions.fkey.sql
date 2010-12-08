ALTER TABLE [dbo].[QuestionOptions]
    ADD CONSTRAINT [FK_QuestionOptions_Questions] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Questions] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

