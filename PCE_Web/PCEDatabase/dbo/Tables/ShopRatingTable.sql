CREATE TABLE [dbo].[ShopRatingTable]
(
	[ShopId] INT NOT NULL PRIMARY KEY IDENTITY (1, 1),
	[ShopName] NVARCHAR (MAX) NOT NULL,
    [VotesNumber] INT NOT NULL,
    [VotersNumber] INT NOT NULL
)
