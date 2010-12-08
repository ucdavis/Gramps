CREATE TABLE [dbo].[Emails] (
    [id]                INT           IDENTITY (1, 1) NOT NULL,
    [Email]             VARCHAR (100) NOT NULL,
    [TemplateId]        INT           NULL,
    [CallForProposalId] INT           NULL
);

