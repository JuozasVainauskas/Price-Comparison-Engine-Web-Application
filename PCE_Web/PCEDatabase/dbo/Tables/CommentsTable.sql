CREATE TABLE [dbo].[CommentsTable]
(
	[CommentId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Email] NVARCHAR (50) NOT NULL,
    [ShopId] INT NOT NULL,
    [Date] NVARCHAR (50) NOT NULL,
    [ServiceRating] INT NOT NULL,
    [ProductsQualityRating] INT NOT NULL,
    [DeliveryRating] INT NOT NULL,
    [Comment] NVARCHAR (MAX) NULL
)
