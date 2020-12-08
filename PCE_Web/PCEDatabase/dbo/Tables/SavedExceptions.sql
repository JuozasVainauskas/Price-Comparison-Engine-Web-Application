CREATE TABLE [dbo].[SavedExceptions]
(
	[SavedExceptionId] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Message] NVARCHAR (MAX) NOT NULL, 
    [StackTrace] NVARCHAR(MAX) NOT NULL, 
    [Source] NVARCHAR(MAX) NOT NULL, 
    [Date] NVARCHAR(255) NOT NULL
)
