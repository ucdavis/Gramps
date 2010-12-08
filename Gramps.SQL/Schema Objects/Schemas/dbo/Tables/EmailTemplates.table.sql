CREATE TABLE [dbo].[EmailTemplates] (
    [id]                INT           IDENTITY (1, 1) NOT NULL,
    [TemplateType]      VARCHAR (10)  NULL,
    [Text]              VARCHAR (MAX) NULL,
    [TemplateId]        INT           NULL,
    [CallForProposalId] INT           NULL
);

