CREATE TABLE [dbo].[EmailTemplates] (
    [id]                INT           IDENTITY (1, 1) NOT NULL,
    [TemplateType]      VARCHAR (50)  NULL,
    [Text]              VARCHAR (MAX) NULL,
    [TemplateId]        INT           NULL,
    [CallForProposalId] INT           NULL,
    [Subject]           VARCHAR (100) NOT NULL
);



