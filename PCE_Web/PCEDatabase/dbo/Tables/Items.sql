CREATE TABLE [dbo].[Items]
(
	[ItemId] INT NOT NULL PRIMARY KEY IDENTITY (1, 1),
    [PageUrl]  NVARCHAR (MAX) NULL,
    [ImgUrl]   NVARCHAR (MAX) NULL,
    [ShopName] NVARCHAR (MAX) NULL,
    [ItemName] NVARCHAR (MAX) NULL,
    [Price]    NVARCHAR (MAX) NULL,
    [Keyword]  NVARCHAR (MAX) NULL,
)
