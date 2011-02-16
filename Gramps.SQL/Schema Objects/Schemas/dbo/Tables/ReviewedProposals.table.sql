CREATE TABLE [dbo].[ReviewedProposals] (
    [id]              INT  IDENTITY (1, 1) NOT NULL,
    [ProposalId]      INT  NOT NULL,
    [EditorId]        INT  NOT NULL,
    [FirstViewedDate] DATE NOT NULL,
    [LastViewedDate]  DATE NOT NULL
);

