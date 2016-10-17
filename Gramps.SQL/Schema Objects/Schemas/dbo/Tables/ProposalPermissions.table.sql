CREATE TABLE [dbo].[ProposalPermissions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ProposalId] [int] NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[AllowReview] [bit] NOT NULL,
	[AllowEdit] [bit] NOT NULL,
	[AllowSubmit] [bit] NOT NULL,
	[DateAdded] [datetime] NOT NULL,
	[DateUpdated] [datetime] NOT NULL
) ON [PRIMARY]