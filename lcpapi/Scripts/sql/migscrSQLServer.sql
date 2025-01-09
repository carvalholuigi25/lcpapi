IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Posts] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [Image] nvarchar(max) NULL,
    [DateCreation] datetime2 NULL,
    [Status] int NULL,
    [UserId] int NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NULL,
    [DisplayName] nvarchar(max) NULL,
    [Avatar] nvarchar(max) NULL,
    [Cover] nvarchar(max) NULL,
    [Role] int NULL,
    [Privacy] int NULL,
    [UsersInfoId] int NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [RefreshToken] (
    [Id] int NOT NULL IDENTITY,
    [Token] nvarchar(max) NOT NULL,
    [Expires] datetime2 NOT NULL,
    [Created] datetime2 NOT NULL,
    [CreatedByIp] nvarchar(max) NOT NULL,
    [Revoked] datetime2 NULL,
    [RevokedByIp] nvarchar(max) NOT NULL,
    [ReplacedByToken] nvarchar(max) NOT NULL,
    [ReasonRevoked] nvarchar(max) NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_RefreshToken] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RefreshToken_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_RefreshToken_UserId] ON [RefreshToken] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250109091841_InitialCreateSQLServer', N'9.0.0');

COMMIT;
GO

