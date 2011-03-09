CREATE TABLE [dbo].[Proposals] (
    [id]                INT              IDENTITY (1, 1) NOT NULL,
    [Guid]              UNIQUEIDENTIFIER NOT NULL,
    [CallForProposalId] INT              NOT NULL,
    [Email]             VARCHAR (100)    NOT NULL,
    [IsApproved]        BIT              NOT NULL,
    [IsDenied]          BIT              NOT NULL,
    [IsNotified]        BIT              NOT NULL,
    [RequestedAmount]   DECIMAL (18, 2)  NULL,
    [ApprovedAmount]    DECIMAL (18, 2)  NULL,
    [IsSubmitted]       BIT              NOT NULL,
    [CreatedDate]       DATETIME         NOT NULL,
    [SubmittedDate]     DATETIME         NULL,
    [NotifiedDate]      DATETIME         NULL,
    [WasWarned]         BIT              NOT NULL,
    [Sequence]          INT              NOT NULL,
    [FileId]            INT              NULL
);









