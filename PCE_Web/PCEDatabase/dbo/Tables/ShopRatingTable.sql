CREATE TABLE [dbo].[ShopRatingTable]
(
	[ShopId] INT NOT NULL PRIMARY KEY IDENTITY (1, 1),
	[ShopName] NVARCHAR (50) NOT NULL,
    [VotesNumber] INT NOT NULL,
    [VotersNumber] INT NOT NULL
)
