CREATE TABLE [dbo].[UserData]
(
	[UserId] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Email] NVARCHAR(50) NOT NULL, 
    [PasswordHash] NVARCHAR(100) NOT NULL, 
    [PasswordSalt] NVARCHAR(100) NOT NULL, 
    [Role] NVARCHAR(50) NOT NULL
)
