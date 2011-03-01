CREATE TABLE [dbo].[Questions] (
    [id]                INT           IDENTITY (1, 1) NOT NULL,
    [QuestionTypeId]    INT           NOT NULL,
    [Name]              VARCHAR (500) NOT NULL,
    [Order]             INT           NOT NULL,
    [TemplateId]        INT           NULL,
    [CallForProposalId] INT           NULL
);





