USE [Gramps]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Validators](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Class] [varchar](50) NULL,
	[RegEx] [varchar](max) NULL,
	[ErrorMessage] [varchar](200) NULL,
 CONSTRAINT [PK_Validators] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Templates](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Templates] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[QuestionTypes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[HasOptions] [bit] NOT NULL,
	[ExtendedProperty] [bit] NULL,
 CONSTRAINT [PK_QuestionTypes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vUsers]
AS
SELECT     UserID AS id, LoginID, Email, Phone, FirstName, LastName, EmployeeID, SID, UserKey
FROM         Catbert3.dbo.Users
WHERE     (Inactive = 0)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Users (Catbert3.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 283
               Right = 277
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vUsers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vUsers'
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CallForProposals](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[TemplateId] [int] NULL,
	[Name] [varchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[CallsSentDate] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_CallForProposals] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Proposals](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[CallForProposalId] [int] NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[IsApproved] [bit] NOT NULL,
	[IsDenied] [bit] NOT NULL,
	[IsNotified] [bit] NOT NULL,
	[RequestedAmount] [money] NULL,
	[ApprovedAmount] [money] NULL,
	[IsSubmitted] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[SubmittedDate] [datetime] NULL,
	[NotifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Proposals] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmailTemplates](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[TemplateType] [varchar](50) NULL,
	[Text] [varchar](max) NULL,
	[TemplateId] [int] NULL,
	[CallForProposalId] [int] NULL,
	[Subject] [varchar](100) NOT NULL,
 CONSTRAINT [PK_EmailTemplates] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Emails](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[TemplateId] [int] NULL,
	[CallForProposalId] [int] NULL,
	[HasBeenEmailed] [bit] NOT NULL,
	[EmailedOnDate] [datetime] NULL,
 CONSTRAINT [PK_Emails] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmailQueue](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[CallForProposalId] [int] NOT NULL,
	[EmailAddress] [varchar](200) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Pending] [bit] NOT NULL,
	[SentDateTime] [datetime] NULL,
	[Subject] [varchar](200) NOT NULL,
	[Body] [varchar](max) NOT NULL,
 CONSTRAINT [PK_EmailQueues] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Editors](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[TemplateId] [int] NULL,
	[CallForProposalId] [int] NULL,
	[UserId] [int] NULL,
	[IsOwner] [bit] NOT NULL,
	[ReviewerEmail] [varchar](100) NULL,
	[ReviewerName] [varchar](200) NULL,
	[ReviewerId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Editors] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Questions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[QuestionTypeId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Order] [int] NOT NULL,
	[TemplateId] [int] NULL,
	[CallForProposalId] [int] NULL,
 CONSTRAINT [PK_Questions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[QuestionOptions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[QuestionId] [int] NOT NULL,
 CONSTRAINT [PK_QuestionOptions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestionsXValidators](
	[QuestionsId] [int] NOT NULL,
	[ValidatorsId] [int] NOT NULL,
 CONSTRAINT [PK_QuestionXValidator] PRIMARY KEY CLUSTERED 
(
	[QuestionsId] ASC,
	[ValidatorsId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Comments](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [varchar](max) NULL,
	[ProposalId] [int] NOT NULL,
	[EditorId] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Comments] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Answers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Answer] [varchar](max) NOT NULL,
	[ProposalId] [int] NOT NULL,
	[QuestionId] [int] NOT NULL,
 CONSTRAINT [PK_Answers] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[EmailQueue] ADD  CONSTRAINT [DF_EmailQueues_Pending]  DEFAULT ((1)) FOR [Pending]
GO
ALTER TABLE [dbo].[Emails] ADD  CONSTRAINT [DF_Emails_HasBeenCalled]  DEFAULT ((0)) FOR [HasBeenEmailed]
GO
ALTER TABLE [dbo].[Templates] ADD  CONSTRAINT [DF_Templates_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[EmailTemplates]  WITH CHECK ADD  CONSTRAINT [CK_EmailTemplates] CHECK  (([TemplateType]='ReadyForReview' OR [TemplateType]='ProposalConfirmation' OR [TemplateType]='ProposalDenied' OR [TemplateType]='ProposalApproved' OR [TemplateType]='ReminderCallIsAboutToClose' OR [TemplateType]='InitialCall'))
GO
ALTER TABLE [dbo].[EmailTemplates] CHECK CONSTRAINT [CK_EmailTemplates]
GO
ALTER TABLE [dbo].[Answers]  WITH CHECK ADD  CONSTRAINT [FK_Answers_Proposals] FOREIGN KEY([ProposalId])
REFERENCES [dbo].[Proposals] ([id])
GO
ALTER TABLE [dbo].[Answers] CHECK CONSTRAINT [FK_Answers_Proposals]
GO
ALTER TABLE [dbo].[Answers]  WITH CHECK ADD  CONSTRAINT [FK_Answers_Questions] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[Questions] ([id])
GO
ALTER TABLE [dbo].[Answers] CHECK CONSTRAINT [FK_Answers_Questions]
GO
ALTER TABLE [dbo].[CallForProposals]  WITH CHECK ADD  CONSTRAINT [FK_CallForProposals_Templates] FOREIGN KEY([TemplateId])
REFERENCES [dbo].[Templates] ([id])
GO
ALTER TABLE [dbo].[CallForProposals] CHECK CONSTRAINT [FK_CallForProposals_Templates]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Used as a Lookup' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CallForProposals', @level2type=N'CONSTRAINT',@level2name=N'FK_CallForProposals_Templates'
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_Comments_Editors] FOREIGN KEY([EditorId])
REFERENCES [dbo].[Editors] ([id])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK_Comments_Editors]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_Comments_Proposals] FOREIGN KEY([ProposalId])
REFERENCES [dbo].[Proposals] ([id])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK_Comments_Proposals]
GO
ALTER TABLE [dbo].[Editors]  WITH CHECK ADD  CONSTRAINT [FK_Editors_CallForProposals] FOREIGN KEY([CallForProposalId])
REFERENCES [dbo].[CallForProposals] ([id])
GO
ALTER TABLE [dbo].[Editors] CHECK CONSTRAINT [FK_Editors_CallForProposals]
GO
ALTER TABLE [dbo].[Editors]  WITH CHECK ADD  CONSTRAINT [FK_Editors_Templates] FOREIGN KEY([TemplateId])
REFERENCES [dbo].[Templates] ([id])
GO
ALTER TABLE [dbo].[Editors] CHECK CONSTRAINT [FK_Editors_Templates]
GO
ALTER TABLE [dbo].[EmailQueue]  WITH CHECK ADD  CONSTRAINT [FK_EmailQueue_CallForProposals] FOREIGN KEY([CallForProposalId])
REFERENCES [dbo].[CallForProposals] ([id])
GO
ALTER TABLE [dbo].[EmailQueue] CHECK CONSTRAINT [FK_EmailQueue_CallForProposals]
GO
ALTER TABLE [dbo].[Emails]  WITH CHECK ADD  CONSTRAINT [FK_Emails_CallForProposals] FOREIGN KEY([CallForProposalId])
REFERENCES [dbo].[CallForProposals] ([id])
GO
ALTER TABLE [dbo].[Emails] CHECK CONSTRAINT [FK_Emails_CallForProposals]
GO
ALTER TABLE [dbo].[Emails]  WITH CHECK ADD  CONSTRAINT [FK_Emails_Templates] FOREIGN KEY([TemplateId])
REFERENCES [dbo].[Templates] ([id])
GO
ALTER TABLE [dbo].[Emails] CHECK CONSTRAINT [FK_Emails_Templates]
GO
ALTER TABLE [dbo].[EmailTemplates]  WITH CHECK ADD  CONSTRAINT [FK_EmailTemplates_CallForProposals] FOREIGN KEY([CallForProposalId])
REFERENCES [dbo].[CallForProposals] ([id])
GO
ALTER TABLE [dbo].[EmailTemplates] CHECK CONSTRAINT [FK_EmailTemplates_CallForProposals]
GO
ALTER TABLE [dbo].[EmailTemplates]  WITH CHECK ADD  CONSTRAINT [FK_EmailTemplates_Templates] FOREIGN KEY([TemplateId])
REFERENCES [dbo].[Templates] ([id])
GO
ALTER TABLE [dbo].[EmailTemplates] CHECK CONSTRAINT [FK_EmailTemplates_Templates]
GO
ALTER TABLE [dbo].[Proposals]  WITH CHECK ADD  CONSTRAINT [FK_Proposals_CallForProposals] FOREIGN KEY([CallForProposalId])
REFERENCES [dbo].[CallForProposals] ([id])
GO
ALTER TABLE [dbo].[Proposals] CHECK CONSTRAINT [FK_Proposals_CallForProposals]
GO
ALTER TABLE [dbo].[QuestionOptions]  WITH CHECK ADD  CONSTRAINT [FK_QuestionOptions_Questions] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[Questions] ([id])
GO
ALTER TABLE [dbo].[QuestionOptions] CHECK CONSTRAINT [FK_QuestionOptions_Questions]
GO
ALTER TABLE [dbo].[Questions]  WITH CHECK ADD  CONSTRAINT [FK_Questions_CallForProposals] FOREIGN KEY([CallForProposalId])
REFERENCES [dbo].[CallForProposals] ([id])
GO
ALTER TABLE [dbo].[Questions] CHECK CONSTRAINT [FK_Questions_CallForProposals]
GO
ALTER TABLE [dbo].[Questions]  WITH CHECK ADD  CONSTRAINT [FK_Questions_QuestionTypes] FOREIGN KEY([QuestionTypeId])
REFERENCES [dbo].[QuestionTypes] ([id])
GO
ALTER TABLE [dbo].[Questions] CHECK CONSTRAINT [FK_Questions_QuestionTypes]
GO
ALTER TABLE [dbo].[Questions]  WITH CHECK ADD  CONSTRAINT [FK_Questions_Templates] FOREIGN KEY([TemplateId])
REFERENCES [dbo].[Templates] ([id])
GO
ALTER TABLE [dbo].[Questions] CHECK CONSTRAINT [FK_Questions_Templates]
GO
ALTER TABLE [dbo].[QuestionsXValidators]  WITH CHECK ADD  CONSTRAINT [FK_QuestionXValidator_Questions] FOREIGN KEY([QuestionsId])
REFERENCES [dbo].[Questions] ([id])
GO
ALTER TABLE [dbo].[QuestionsXValidators] CHECK CONSTRAINT [FK_QuestionXValidator_Questions]
GO
ALTER TABLE [dbo].[QuestionsXValidators]  WITH CHECK ADD  CONSTRAINT [FK_QuestionXValidator_Validators] FOREIGN KEY([ValidatorsId])
REFERENCES [dbo].[Validators] ([id])
GO
ALTER TABLE [dbo].[QuestionsXValidators] CHECK CONSTRAINT [FK_QuestionXValidator_Validators]
GO
