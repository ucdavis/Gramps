ALTER TABLE [dbo].[Reports]
    ADD CONSTRAINT [DF_Reports_ShowUnsubmitted] DEFAULT ((0)) FOR [ShowUnsubmitted];

