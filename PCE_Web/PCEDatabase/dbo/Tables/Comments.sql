CREATE TABLE [dbo].[Comments]
(
	[CommentId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Email] NVARCHAR (255) NOT NULL,
    [ShopId] INT NOT NULL,
    [Date] NVARCHAR (255) NOT NULL,
    [Rating] INT NOT NULL,
    [Comment] NVARCHAR (MAX) NULL
)
