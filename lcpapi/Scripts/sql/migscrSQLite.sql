CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "Posts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Posts" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NULL,
    "Description" TEXT NULL,
    "Image" TEXT NULL,
    "DateCreation" TEXT NULL,
    "Status" INTEGER NULL,
    "UserId" INTEGER NULL
);

CREATE TABLE "Users" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
    "Username" TEXT NOT NULL,
    "Password" TEXT NOT NULL,
    "Email" TEXT NULL,
    "DisplayName" TEXT NULL,
    "Avatar" TEXT NULL,
    "Cover" TEXT NULL,
    "Role" INTEGER NULL,
    "Privacy" INTEGER NULL,
    "UsersInfoId" INTEGER NULL
);

CREATE TABLE "RefreshToken" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RefreshToken" PRIMARY KEY AUTOINCREMENT,
    "Token" TEXT NOT NULL,
    "Expires" TEXT NOT NULL,
    "Created" TEXT NOT NULL,
    "CreatedByIp" TEXT NOT NULL,
    "Revoked" TEXT NULL,
    "RevokedByIp" TEXT NOT NULL,
    "ReplacedByToken" TEXT NOT NULL,
    "ReasonRevoked" TEXT NOT NULL,
    "UserId" INTEGER NOT NULL,
    CONSTRAINT "FK_RefreshToken_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_RefreshToken_UserId" ON "RefreshToken" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250109091805_InitialCreateSQLite', '9.0.0');

COMMIT;

