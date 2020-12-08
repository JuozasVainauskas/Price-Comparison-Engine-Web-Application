CREATE TABLE [dbo].[UserData]
(
	[UserId] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Email] NVARCHAR(255) NOT NULL, 
    [PasswordHash] NVARCHAR(255) NOT NULL, 
    [PasswordSalt] NVARCHAR(255) NOT NULL, 
    [Role] NVARCHAR(1) NOT NULL
)
