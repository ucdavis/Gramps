ALTER TABLE [dbo].[CallForProposals]
    ADD CONSTRAINT [FK_CallForProposals_Templates] FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[Templates] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Used as a Lookup', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CallForProposals', @level2type = N'CONSTRAINT', @level2name = N'FK_CallForProposals_Templates';

