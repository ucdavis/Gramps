ALTER TABLE [dbo].[Editors]
    ADD CONSTRAINT [DF_Editors_HasBeenNotified] DEFAULT ((0)) FOR [HasBeenNotified];

