CREATE TABLE [dbo].[ItemsTable]
(
	[ItemId] INT NOT NULL PRIMARY KEY IDENTITY (1, 1),
    [PageUrl]  NVARCHAR (100) NULL,
    [ImgUrl]   NVARCHAR (100) NULL,
    [ShopName] NVARCHAR (50) NULL,
    [ItemName] NVARCHAR (50) NULL,
    [Price]    NVARCHAR (50) NULL,
    [Keyword]  NVARCHAR (50) NULL,
)
