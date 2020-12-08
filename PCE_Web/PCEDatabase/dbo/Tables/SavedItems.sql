CREATE TABLE [dbo].[SavedItems]
(
	[SavedItemId] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Email] NVARCHAR(255) NOT NULL, 
    [PageUrl] NVARCHAR(MAX) NULL, 
    [ImgUrl] NVARCHAR(MAX) NULL, 
    [ShopName] NVARCHAR(255) NULL, 
    [ItemName] NVARCHAR(MAX) NULL, 
    [PriceWithSymbol] NVARCHAR(MAX) NULL
)
