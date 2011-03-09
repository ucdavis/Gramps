ALTER TABLE [dbo].[ReportColumns]
    ADD CONSTRAINT [DF_ReportColumns_IsProperty] DEFAULT ((0)) FOR [IsProperty];

