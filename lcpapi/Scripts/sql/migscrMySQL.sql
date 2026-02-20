CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Posts` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Title` longtext CHARACTER SET utf8mb4 NULL,
    `Description` longtext CHARACTER SET utf8mb4 NULL,
    `Image` longtext CHARACTER SET utf8mb4 NULL,
    `DateCreation` datetime NULL,
    `Status` int NULL,
    `UserId` int NULL,
    CONSTRAINT `PK_Posts` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Users` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Username` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Password` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Email` longtext CHARACTER SET utf8mb4 NULL,
    `DisplayName` longtext CHARACTER SET utf8mb4 NULL,
    `Avatar` longtext CHARACTER SET utf8mb4 NULL,
    `Cover` longtext CHARACTER SET utf8mb4 NULL,
    `Role` int NULL,
    `Privacy` int NULL,
    `UsersInfoId` int NULL,
    CONSTRAINT `PK_Users` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `RefreshToken` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Token` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Expires` datetime NOT NULL,
    `Created` datetime NOT NULL,
    `CreatedByIp` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Revoked` datetime NULL,
    `RevokedByIp` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ReplacedByToken` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ReasonRevoked` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UserId` int NOT NULL,
    CONSTRAINT `PK_RefreshToken` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_RefreshToken_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_RefreshToken_UserId` ON `RefreshToken` (`UserId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250109091952_InitialCreateMySQL', '9.0.0');

COMMIT;

