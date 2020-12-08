CREATE TABLE [dbo].[Items]
(
	[ItemId] INT NOT NULL PRIMARY KEY IDENTITY (1, 1),
    [PageUrl]  NVARCHAR (MAX) NULL,
    [ImgUrl]   NVARCHAR (MAX) NULL,
    [ShopName] NVARCHAR (255) NULL,
    [ItemName] NVARCHAR (MAX) NULL,
    [Price]    NVARCHAR (255) NULL,
    [Keyword]  NVARCHAR (255) NULL,
)
