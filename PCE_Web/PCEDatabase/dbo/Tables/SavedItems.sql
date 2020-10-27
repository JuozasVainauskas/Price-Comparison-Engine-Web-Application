CREATE TABLE [dbo].[SavedItems]
(
	[SavedItemId] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Email] NVARCHAR(50) NOT NULL, 
    [PageUrl] NVARCHAR(50) NULL, 
    [ImgUrl] NVARCHAR(50) NULL, 
    [ShopName] NVARCHAR(50) NULL, 
    [ItemName] NVARCHAR(50) NULL, 
    [Price] NVARCHAR(50) NULL
)
