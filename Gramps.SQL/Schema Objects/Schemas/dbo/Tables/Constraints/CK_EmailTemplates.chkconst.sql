ALTER TABLE [dbo].[EmailTemplates]
    ADD CONSTRAINT [CK_EmailTemplates] CHECK ([TemplateType]='ReadyForReview' OR [TemplateType]='ProposalConfirmation' OR [TemplateType]='ProposalDenied' OR [TemplateType]='ProposalApproved' OR [TemplateType]='ReminderCallIsAboutToClose' OR [TemplateType]='InitialCall');

