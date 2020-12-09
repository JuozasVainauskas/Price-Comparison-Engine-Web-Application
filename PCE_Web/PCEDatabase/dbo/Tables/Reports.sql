CREATE TABLE [dbo].[Reports]
(
	[ReportsId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Email] NVARCHAR (255) NOT NULL,
    [Comment] NVARCHAR (MAX) NOT NULL,
    [Solved] INT NOT NULL, 
    [Date] NVARCHAR(255) NOT NULL
)
