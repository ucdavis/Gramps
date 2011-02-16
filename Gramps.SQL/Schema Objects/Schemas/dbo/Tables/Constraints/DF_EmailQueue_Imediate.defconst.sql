ALTER TABLE [dbo].[EmailQueue]
    ADD CONSTRAINT [DF_EmailQueue_Imediate] DEFAULT ((0)) FOR [Immediate];

