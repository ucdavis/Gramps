CREATE TABLE [dbo].[EmailQueue] (
    [id]                INT           IDENTITY (1, 1) NOT NULL,
    [CallForProposalId] INT           NOT NULL,
    [EmailAddress]      VARCHAR (200) NOT NULL,
    [Created]           DATETIME      NOT NULL,
    [Pending]           BIT           NOT NULL,
    [SentDateTime]      DATETIME      NULL,
    [Subject]           VARCHAR (200) NOT NULL,
    [Body]              VARCHAR (MAX) NOT NULL
);

