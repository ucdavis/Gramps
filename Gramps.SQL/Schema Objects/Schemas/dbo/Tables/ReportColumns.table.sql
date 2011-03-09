CREATE TABLE [dbo].[ReportColumns] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [ReportId]    INT           NOT NULL,
    [ColumnOrder] INT           NOT NULL,
    [Name]        VARCHAR (500) NOT NULL,
    [Format]      VARCHAR (50)  NULL,
    [IsProperty]  BIT           NOT NULL
);

