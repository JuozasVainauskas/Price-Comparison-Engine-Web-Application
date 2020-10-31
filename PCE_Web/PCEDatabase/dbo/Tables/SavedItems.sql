CREATE TABLE [dbo].[SavedItems]
(
	[SavedItemId] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Email] NVARCHAR(MAX) NOT NULL, 
    [PageUrl] NVARCHAR(MAX) NULL, 
    [ImgUrl] NVARCHAR(MAX) NULL, 
    [ShopName] NVARCHAR(MAX) NULL, 
    [ItemName] NVARCHAR(MAX) NULL, 
    [Price] NVARCHAR(MAX) NULL
)
