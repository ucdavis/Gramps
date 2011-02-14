ALTER TABLE [dbo].[QuestionsXValidators]
    ADD CONSTRAINT [FK_QuestionXValidator_Questions] FOREIGN KEY ([QuestionsId]) REFERENCES [dbo].[Questions] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

