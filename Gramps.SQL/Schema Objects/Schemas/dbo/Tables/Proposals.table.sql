CREATE TABLE [dbo].[Proposals] (
    [id]                INT              IDENTITY (1, 1) NOT NULL,
    [Guid]              UNIQUEIDENTIFIER NOT NULL,
    [CallForProposalId] INT              NOT NULL,
    [Email]             VARCHAR (100)    NOT NULL,
    [IsApproved]        BIT              NOT NULL,
    [IsDenied]          BIT              NOT NULL,
    [IsNotified]        BIT              NOT NULL,
    [RequestedAmount]   MONEY            NULL,
    [ApprovedAmount]    MONEY            NULL,
    [IsSubmitted]       BIT              NOT NULL,
    [CreatedDate]       DATETIME         NOT NULL,
    [SubmittedDate]     DATETIME         NULL,
    [NotifiedDate]      DATETIME         NULL
);



