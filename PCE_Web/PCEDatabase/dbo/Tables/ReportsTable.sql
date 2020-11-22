CREATE TABLE [dbo].[ReportsTable]
(
	[ReportsId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Email] NVARCHAR (MAX) NOT NULL,
    [Comment] NVARCHAR (MAX) NOT NULL,
    [Solved] INT NOT NULL, 
    [Date] NVARCHAR(MAX) NOT NULL
)
