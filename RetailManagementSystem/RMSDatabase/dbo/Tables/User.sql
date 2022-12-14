CREATE TABLE [dbo].[User]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
    [EmailAddress] NVARCHAR(256) NOT NULL, 
    [Title] NVARCHAR(50),
    [CreatedDate] DATETIME2 NOT NULL DEFAULT getutcdate()
)
