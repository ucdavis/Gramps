ALTER TABLE [dbo].[Questions]
    ADD CONSTRAINT [FK_Questions_QuestionTypes] FOREIGN KEY ([QuestionTypeId]) REFERENCES [dbo].[QuestionTypes] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

