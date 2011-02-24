CREATE TABLE [dbo].[Investigators] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [ProposalId]  INT           NOT NULL,
    [IsPrimary]   BIT           NOT NULL,
    [Name]        VARCHAR (200) NOT NULL,
    [Institution] VARCHAR (200) NOT NULL,
    [Address1]    VARCHAR (200) NOT NULL,
    [Address2]    VARCHAR (200) NULL,
    [Address3]    VARCHAR (200) NULL,
    [Phone]       VARCHAR (50)  NOT NULL,
    [Email]       VARCHAR (200) NOT NULL,
    [Notes]       VARCHAR (MAX) NULL
);

