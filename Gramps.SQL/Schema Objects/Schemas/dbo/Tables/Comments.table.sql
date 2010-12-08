CREATE TABLE [dbo].[Comments] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [Text]        VARCHAR (MAX) NULL,
    [ProposalId]  INT           NOT NULL,
    [EditorId]    INT           NOT NULL,
    [CreatedDate] DATETIME      NOT NULL
);

