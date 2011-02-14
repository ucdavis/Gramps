ALTER TABLE [dbo].[EmailQueue]
    ADD CONSTRAINT [DF_EmailQueues_Pending] DEFAULT ((1)) FOR [Pending];

