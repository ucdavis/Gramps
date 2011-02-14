ALTER TABLE [dbo].[Emails]
    ADD CONSTRAINT [DF_Emails_HasBeenCalled] DEFAULT ((0)) FOR [HasBeenEmailed];

