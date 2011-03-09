CREATE TABLE [dbo].[Reports] (
    [id]                INT           IDENTITY (1, 1) NOT NULL,
    [Name]              VARCHAR (100) NOT NULL,
    [TemplateId]        INT           NULL,
    [CallForProposalId] INT           NULL
);

