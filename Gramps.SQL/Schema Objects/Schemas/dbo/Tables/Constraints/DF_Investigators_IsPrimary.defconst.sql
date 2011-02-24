ALTER TABLE [dbo].[Investigators]
    ADD CONSTRAINT [DF_Investigators_IsPrimary] DEFAULT ((0)) FOR [IsPrimary];

