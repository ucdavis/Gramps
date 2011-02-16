CREATE TABLE [dbo].[CallForProposals] (
    [id]            INT           IDENTITY (1, 1) NOT NULL,
    [TemplateId]    INT           NULL,
    [Name]          VARCHAR (100) NOT NULL,
    [IsActive]      BIT           NOT NULL,
    [EndDate]       DATETIME      NOT NULL,
    [CallsSentDate] DATETIME      NULL,
    [CreatedDate]   DATETIME      NOT NULL
);



