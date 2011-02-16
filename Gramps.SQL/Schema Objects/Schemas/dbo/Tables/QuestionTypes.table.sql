CREATE TABLE [dbo].[QuestionTypes] (
    [id]               INT          IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (50) NOT NULL,
    [HasOptions]       BIT          NOT NULL,
    [ExtendedProperty] BIT          NULL
);



