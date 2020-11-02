CREATE TABLE [dbo].[CommentsTable]
(
	[CommentId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Email] NVARCHAR (MAX) NOT NULL,
    [ShopId] INT NOT NULL,
    [Date] NVARCHAR (MAX) NOT NULL,
    [Rating] INT NOT NULL,
    [Comment] NVARCHAR (MAX) NULL
)
