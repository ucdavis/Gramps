CREATE TABLE [dbo].[Answers] (
    [id]         INT           IDENTITY (1, 1) NOT NULL,
    [Answer]     VARCHAR (MAX) NOT NULL,
    [ProposalId] INT           NOT NULL,
    [QuestionId] INT           NOT NULL
);

