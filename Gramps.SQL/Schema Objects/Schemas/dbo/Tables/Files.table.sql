CREATE TABLE [dbo].[Files] (
    [id]          INT             IDENTITY (1, 1) NOT NULL,
    [ContentType] VARCHAR (50)    NULL,
    [Name]        VARCHAR (512)   NOT NULL,
    [DateAdded]   DATETIME        NOT NULL,
    [Contents]    VARBINARY (MAX) NULL
);

