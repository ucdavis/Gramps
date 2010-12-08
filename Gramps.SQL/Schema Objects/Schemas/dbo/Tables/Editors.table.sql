CREATE TABLE [dbo].[Editors] (
    [id]                INT              IDENTITY (1, 1) NOT NULL,
    [TemplateId]        INT              NULL,
    [CallForProposalId] INT              NULL,
    [UserId]            INT              NULL,
    [IsOwner]           BIT              NOT NULL,
    [ReviewerEmail]     VARCHAR (100)    NULL,
    [ReviewerName]      VARCHAR (200)    NULL,
    [ReviewerId]        UNIQUEIDENTIFIER NULL
);

