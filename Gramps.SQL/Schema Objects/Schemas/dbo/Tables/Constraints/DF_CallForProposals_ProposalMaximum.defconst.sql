ALTER TABLE [dbo].[CallForProposals]
    ADD CONSTRAINT [DF_CallForProposals_ProposalMaximum] DEFAULT ((0)) FOR [ProposalMaximum];

