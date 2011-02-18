ALTER TABLE [dbo].[Proposals]
    ADD CONSTRAINT [DF_Proposals_WasWarned] DEFAULT ((0)) FOR [WasWarned];

