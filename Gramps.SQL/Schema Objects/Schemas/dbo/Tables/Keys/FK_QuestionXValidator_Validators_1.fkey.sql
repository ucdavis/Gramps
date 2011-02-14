ALTER TABLE [dbo].[QuestionsXValidators]
    ADD CONSTRAINT [FK_QuestionXValidator_Validators] FOREIGN KEY ([ValidatorsId]) REFERENCES [dbo].[Validators] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

