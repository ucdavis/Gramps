ALTER TABLE [dbo].[ReportColumns]
    ADD CONSTRAINT [FK_ReportColumns_Reports] FOREIGN KEY ([ReportId]) REFERENCES [dbo].[Reports] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

