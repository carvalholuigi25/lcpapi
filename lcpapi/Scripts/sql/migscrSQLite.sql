CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "ActionFigures" (
    "ActionFigureId" INTEGER NOT NULL CONSTRAINT "PK_ActionFigures" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Image" TEXT NULL,
    "Manufacturer" TEXT NULL,
    "IsFeatured" INTEGER NULL,
    "Price" TEXT NULL,
    "ReleaseDate" TEXT NULL,
    "ScoreRating" decimal(18,2) NULL,
    "Category" TEXT NULL
);

CREATE TABLE "Animes" (
    "AnimeId" INTEGER NOT NULL CONSTRAINT "PK_Animes" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Image" TEXT NULL,
    "Artwork" TEXT NULL,
    "Studio" TEXT NULL,
    "IsFeatured" INTEGER NULL,
    "ScoreRating" decimal(18,2) NULL,
    "ReleaseDate" TEXT NULL,
    "Genre" TEXT NULL,
    "Format" TEXT NULL
);

CREATE TABLE "Books" (
    "BookId" INTEGER NOT NULL CONSTRAINT "PK_Books" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Image" TEXT NULL,
    "Artwork" TEXT NULL,
    "Author" TEXT NULL,
    "IsFeatured" INTEGER NULL,
    "ScoreRating" decimal(18,2) NULL,
    "ReleaseDate" TEXT NULL,
    "Genre" TEXT NULL
);

CREATE TABLE "FileUploadInfos" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_FileUploadInfos" PRIMARY KEY AUTOINCREMENT,
    "FileName" TEXT NULL,
    "FileData" BLOB NULL,
    "FileType" INTEGER NOT NULL
);

CREATE TABLE "Games" (
    "GameId" INTEGER NOT NULL CONSTRAINT "PK_Games" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Image" TEXT NULL,
    "Artwork" TEXT NULL,
    "Publisher" TEXT NULL,
    "Developer" TEXT NULL,
    "IsFeatured" INTEGER NULL,
    "ScoreRating" decimal(18,2) NULL,
    "ReleaseDate" TEXT NULL,
    "ESRB" INTEGER NULL,
    "Edition" TEXT NULL,
    "Genre" TEXT NULL,
    "Platform" TEXT NULL
);

CREATE TABLE "Movies" (
    "MovieId" INTEGER NOT NULL CONSTRAINT "PK_Movies" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Image" TEXT NULL,
    "Artwork" TEXT NULL,
    "Studio" TEXT NULL,
    "IsFeatured" INTEGER NULL,
    "ScoreRating" decimal(18,2) NULL,
    "ReleaseDate" TEXT NULL,
    "Genre" TEXT NULL,
    "Format" TEXT NULL
);

CREATE TABLE "Musics" (
    "MusicId" INTEGER NOT NULL CONSTRAINT "PK_Musics" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Image" TEXT NULL,
    "Artwork" TEXT NULL,
    "Artist" TEXT NULL,
    "Album" TEXT NULL,
    "IsFeatured" INTEGER NULL,
    "ScoreRating" decimal(18,2) NULL,
    "ReleaseDate" TEXT NULL,
    "Genre" TEXT NULL,
    "Format" TEXT NULL
);

CREATE TABLE "Pets" (
    "PetsId" INTEGER NOT NULL CONSTRAINT "PK_Pets" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Image" TEXT NOT NULL,
    "Type" TEXT NOT NULL,
    "DateOfBirthday" TEXT NOT NULL,
    "Description" TEXT NULL,
    "isFavorite" INTEGER NULL
);

CREATE TABLE "Posts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Posts" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NULL,
    "Description" TEXT NULL,
    "Image" TEXT NULL,
    "DateCreation" TEXT NULL,
    "Status" INTEGER NULL,
    "UserId" INTEGER NULL
);

CREATE TABLE "RecipesFoods" (
    "RecipesFoodsId" INTEGER NOT NULL CONSTRAINT "PK_RecipesFoods" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Image" TEXT NULL,
    "IsFeatured" INTEGER NULL
);

CREATE TABLE "Softwares" (
    "SoftwareId" INTEGER NOT NULL CONSTRAINT "PK_Softwares" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Version" TEXT NULL,
    "Image" TEXT NULL,
    "DownloadUrl" TEXT NULL,
    "ReleaseDate" TEXT NULL,
    "IsActive" INTEGER NULL,
    "Category" INTEGER NULL,
    "License" INTEGER NULL
);

CREATE TABLE "Tvseries" (
    "TvserieId" INTEGER NOT NULL CONSTRAINT "PK_Tvseries" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Image" TEXT NULL,
    "Artwork" TEXT NULL,
    "Studio" TEXT NULL,
    "IsFeatured" INTEGER NULL,
    "ScoreRating" decimal(18,2) NULL,
    "NumSeasons" INTEGER NULL,
    "NumEpisodes" INTEGER NULL,
    "ReleaseDate" TEXT NULL,
    "Genre" TEXT NULL,
    "Format" TEXT NULL
);

CREATE TABLE "Users" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
    "Username" TEXT NOT NULL,
    "Password" TEXT NOT NULL,
    "Email" TEXT NULL,
    "DisplayName" TEXT NULL,
    "Avatar" TEXT NULL,
    "Cover" TEXT NULL,
    "DateBirthday" TEXT NULL,
    "Role" INTEGER NULL,
    "Privacy" INTEGER NULL,
    "UsersInfoId" INTEGER NULL,
    "OtpSecret" TEXT NULL,
    "OtpEnabled" INTEGER NOT NULL
);

CREATE TABLE "ActionFiguresMediaInfo" (
    "MediaId" INTEGER NOT NULL CONSTRAINT "PK_ActionFiguresMediaInfo" PRIMARY KEY AUTOINCREMENT,
    "ActionFigureId" INTEGER NOT NULL,
    "Url" TEXT NOT NULL,
    "IsFeatured" INTEGER NULL,
    "TypeMedia" TEXT NULL,
    CONSTRAINT "FK_ActionFiguresMediaInfo_ActionFigures_ActionFigureId" FOREIGN KEY ("ActionFigureId") REFERENCES "ActionFigures" ("ActionFigureId") ON DELETE CASCADE
);

CREATE TABLE "AnimesMediaInfo" (
    "MediaId" INTEGER NOT NULL CONSTRAINT "PK_AnimesMediaInfo" PRIMARY KEY AUTOINCREMENT,
    "AnimeId" INTEGER NOT NULL,
    "Url" TEXT NOT NULL,
    "IsFeatured" INTEGER NULL,
    "TypeMedia" TEXT NULL,
    CONSTRAINT "FK_AnimesMediaInfo_Animes_AnimeId" FOREIGN KEY ("AnimeId") REFERENCES "Animes" ("AnimeId") ON DELETE CASCADE
);

CREATE TABLE "BooksMediaInfo" (
    "BooksMediaInfoId" INTEGER NOT NULL CONSTRAINT "PK_BooksMediaInfo" PRIMARY KEY AUTOINCREMENT,
    "BookId" INTEGER NOT NULL,
    "MediaType" TEXT NULL,
    "Link" TEXT NULL,
    CONSTRAINT "FK_BooksMediaInfo_Books_BookId" FOREIGN KEY ("BookId") REFERENCES "Books" ("BookId") ON DELETE CASCADE
);

CREATE TABLE "GamesMedias" (
    "MediaId" INTEGER NOT NULL CONSTRAINT "PK_GamesMedias" PRIMARY KEY AUTOINCREMENT,
    "GameId" INTEGER NOT NULL,
    "Url" TEXT NOT NULL,
    "IsFeatured" INTEGER NULL,
    "TypeMedia" TEXT NULL,
    CONSTRAINT "FK_GamesMedias_Games_GameId" FOREIGN KEY ("GameId") REFERENCES "Games" ("GameId") ON DELETE CASCADE
);

CREATE TABLE "MoviesMediaInfo" (
    "MediaId" INTEGER NOT NULL CONSTRAINT "PK_MoviesMediaInfo" PRIMARY KEY AUTOINCREMENT,
    "MovieId" INTEGER NOT NULL,
    "Url" TEXT NOT NULL,
    "IsFeatured" INTEGER NULL,
    "TypeMedia" TEXT NULL,
    "MoviesMovieId" INTEGER NULL,
    CONSTRAINT "FK_MoviesMediaInfo_Movies_MoviesMovieId" FOREIGN KEY ("MoviesMovieId") REFERENCES "Movies" ("MovieId")
);

CREATE TABLE "MusicsMediaInfo" (
    "MediaId" INTEGER NOT NULL CONSTRAINT "PK_MusicsMediaInfo" PRIMARY KEY AUTOINCREMENT,
    "MusicId" INTEGER NOT NULL,
    "Url" TEXT NOT NULL,
    "IsFeatured" INTEGER NULL,
    "TypeMedia" TEXT NULL,
    CONSTRAINT "FK_MusicsMediaInfo_Musics_MusicId" FOREIGN KEY ("MusicId") REFERENCES "Musics" ("MusicId") ON DELETE CASCADE
);

CREATE TABLE "RecipesFoodsMediaInfo" (
    "RecipesFoodsMediaInfoId" INTEGER NOT NULL CONSTRAINT "PK_RecipesFoodsMediaInfo" PRIMARY KEY AUTOINCREMENT,
    "RecipesFoodsId" INTEGER NOT NULL,
    "MediaType" TEXT NULL,
    "Link" TEXT NULL,
    CONSTRAINT "FK_RecipesFoodsMediaInfo_RecipesFoods_RecipesFoodsId" FOREIGN KEY ("RecipesFoodsId") REFERENCES "RecipesFoods" ("RecipesFoodsId") ON DELETE CASCADE
);

CREATE TABLE "SoftwaresMediaInfo" (
    "MediaId" INTEGER NOT NULL CONSTRAINT "PK_SoftwaresMediaInfo" PRIMARY KEY AUTOINCREMENT,
    "SoftwareId" INTEGER NOT NULL,
    "Url" TEXT NOT NULL,
    "IsFeatured" INTEGER NULL,
    "TypeMedia" TEXT NULL,
    CONSTRAINT "FK_SoftwaresMediaInfo_Softwares_SoftwareId" FOREIGN KEY ("SoftwareId") REFERENCES "Softwares" ("SoftwareId") ON DELETE CASCADE
);

CREATE TABLE "TvseriesEpisodesInfos" (
    "EpisodesId" INTEGER NOT NULL CONSTRAINT "PK_TvseriesEpisodesInfos" PRIMARY KEY AUTOINCREMENT,
    "EpisodesTitle" TEXT NOT NULL,
    "EpisodesDescription" TEXT NULL,
    "EpisodesImage" TEXT NULL,
    "EpisodesStudio" TEXT NULL,
    "EpisodesIsFeatured" INTEGER NULL,
    "EpisodesIsWatched" INTEGER NULL,
    "EpisodesScoreRating" decimal(18,2) NULL,
    "EpisodesReleaseDate" TEXT NULL,
    "SeasonsId" INTEGER NULL,
    "TvserieId" INTEGER NULL,
    "TvseriesTvserieId" INTEGER NULL,
    CONSTRAINT "FK_TvseriesEpisodesInfos_Tvseries_TvseriesTvserieId" FOREIGN KEY ("TvseriesTvserieId") REFERENCES "Tvseries" ("TvserieId")
);

CREATE TABLE "TvseriesMediaInfo" (
    "MediaId" INTEGER NOT NULL CONSTRAINT "PK_TvseriesMediaInfo" PRIMARY KEY AUTOINCREMENT,
    "EpisodesId" INTEGER NOT NULL,
    "SeasonsId" INTEGER NOT NULL,
    "TvserieId" INTEGER NOT NULL,
    "Url" TEXT NOT NULL,
    "IsFeatured" INTEGER NULL,
    "TypeMedia" TEXT NULL,
    "TvseriesTvserieId" INTEGER NULL,
    CONSTRAINT "FK_TvseriesMediaInfo_Tvseries_TvseriesTvserieId" FOREIGN KEY ("TvseriesTvserieId") REFERENCES "Tvseries" ("TvserieId")
);

CREATE TABLE "TvseriesReviewsInfos" (
    "ReviewsId" INTEGER NOT NULL CONSTRAINT "PK_TvseriesReviewsInfos" PRIMARY KEY AUTOINCREMENT,
    "ReviewsTitle" TEXT NOT NULL,
    "ReviewsDescription" TEXT NOT NULL,
    "ReviewsImage" TEXT NULL,
    "ReviewsCover" TEXT NULL,
    "ReviewsIsFeatured" INTEGER NULL,
    "ReviewsDateTime" TEXT NULL,
    "ReviewsStatus" INTEGER NULL,
    "ReviewsViews" INTEGER NULL,
    "ReviewsScoreRating" decimal(18,2) NULL,
    "EpisodesId" INTEGER NULL,
    "SeasonsId" INTEGER NULL,
    "TvserieId" INTEGER NULL,
    "UserId" INTEGER NULL,
    "TvseriesTvserieId" INTEGER NULL,
    CONSTRAINT "FK_TvseriesReviewsInfos_Tvseries_TvseriesTvserieId" FOREIGN KEY ("TvseriesTvserieId") REFERENCES "Tvseries" ("TvserieId")
);

CREATE TABLE "TvseriesSeasonsInfo" (
    "SeasonsId" INTEGER NOT NULL CONSTRAINT "PK_TvseriesSeasonsInfo" PRIMARY KEY AUTOINCREMENT,
    "SeasonsTitle" TEXT NOT NULL,
    "SeasonsDescription" TEXT NULL,
    "SeasonsImage" TEXT NULL,
    "SeasonsStudio" TEXT NULL,
    "SeasonsIsFeatured" INTEGER NULL,
    "SeasonsIsWatched" INTEGER NULL,
    "SeasonsScoreRating" decimal(18,2) NULL,
    "SeasonsReleaseDate" TEXT NULL,
    "TvserieId" INTEGER NULL,
    "TvseriesTvserieId" INTEGER NULL,
    CONSTRAINT "FK_TvseriesSeasonsInfo_Tvseries_TvseriesTvserieId" FOREIGN KEY ("TvseriesTvserieId") REFERENCES "Tvseries" ("TvserieId")
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

CREATE TABLE "UserModerationSettings" (
    "UserModerationSettingsId" INTEGER NOT NULL CONSTRAINT "PK_UserModerationSettings" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "AdminId" INTEGER NOT NULL,
    "CurAttempts" INTEGER NOT NULL,
    "IsUserConfirmed" INTEGER NOT NULL,
    "IsUserBanned" INTEGER NULL,
    "IsUserAuthLocked" INTEGER NULL,
    "TimeCreation" TEXT NULL,
    "TimeUserAuthLocked" TEXT NULL,
    CONSTRAINT "FK_UserModerationSettings_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "UserPermissionsSettings" (
    "UserPermissionsSettingsId" INTEGER NOT NULL CONSTRAINT "PK_UserPermissionsSettings" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "TypePermissions" TEXT NOT NULL,
    "CodePermissions" INTEGER NULL,
    "DescPermissions" TEXT NULL,
    CONSTRAINT "FK_UserPermissionsSettings_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "TvseriesCommentsInfo" (
    "CommentsId" INTEGER NOT NULL CONSTRAINT "PK_TvseriesCommentsInfo" PRIMARY KEY AUTOINCREMENT,
    "CommentsTitle" TEXT NOT NULL,
    "CommentsDescription" TEXT NOT NULL,
    "CommentsIsFeatured" INTEGER NULL,
    "CommentsDateTime" TEXT NULL,
    "CommentsStatus" INTEGER NULL,
    "ReviewsId" INTEGER NULL,
    "EpisodesId" INTEGER NULL,
    "SeasonsId" INTEGER NULL,
    "TvserieId" INTEGER NULL,
    "UserId" INTEGER NULL,
    "TvseriesEpisodesInfoEpisodesId" INTEGER NULL,
    CONSTRAINT "FK_TvseriesCommentsInfo_TvseriesEpisodesInfos_TvseriesEpisodesInfoEpisodesId" FOREIGN KEY ("TvseriesEpisodesInfoEpisodesId") REFERENCES "TvseriesEpisodesInfos" ("EpisodesId")
);

CREATE TABLE "TvseriesReactions" (
    "TvseriesReactionsId" INTEGER NOT NULL CONSTRAINT "PK_TvseriesReactions" PRIMARY KEY AUTOINCREMENT,
    "ReactionsType" INTEGER NULL,
    "ReviewId" INTEGER NOT NULL,
    "EpisodeId" INTEGER NOT NULL,
    "SeasonId" INTEGER NOT NULL,
    "TvserieId" INTEGER NOT NULL,
    "UserId" INTEGER NOT NULL,
    "TvseriesEpisodesInfoEpisodesId" INTEGER NULL,
    CONSTRAINT "FK_TvseriesReactions_TvseriesEpisodesInfos_TvseriesEpisodesInfoEpisodesId" FOREIGN KEY ("TvseriesEpisodesInfoEpisodesId") REFERENCES "TvseriesEpisodesInfos" ("EpisodesId")
);

CREATE INDEX "IX_ActionFiguresMediaInfo_ActionFigureId" ON "ActionFiguresMediaInfo" ("ActionFigureId");

CREATE INDEX "IX_AnimesMediaInfo_AnimeId" ON "AnimesMediaInfo" ("AnimeId");

CREATE INDEX "IX_BooksMediaInfo_BookId" ON "BooksMediaInfo" ("BookId");

CREATE INDEX "IX_GamesMedias_GameId" ON "GamesMedias" ("GameId");

CREATE INDEX "IX_MoviesMediaInfo_MoviesMovieId" ON "MoviesMediaInfo" ("MoviesMovieId");

CREATE INDEX "IX_MusicsMediaInfo_MusicId" ON "MusicsMediaInfo" ("MusicId");

CREATE INDEX "IX_RecipesFoodsMediaInfo_RecipesFoodsId" ON "RecipesFoodsMediaInfo" ("RecipesFoodsId");

CREATE INDEX "IX_RefreshToken_UserId" ON "RefreshToken" ("UserId");

CREATE INDEX "IX_SoftwaresMediaInfo_SoftwareId" ON "SoftwaresMediaInfo" ("SoftwareId");

CREATE INDEX "IX_TvseriesCommentsInfo_TvseriesEpisodesInfoEpisodesId" ON "TvseriesCommentsInfo" ("TvseriesEpisodesInfoEpisodesId");

CREATE INDEX "IX_TvseriesEpisodesInfos_TvseriesTvserieId" ON "TvseriesEpisodesInfos" ("TvseriesTvserieId");

CREATE INDEX "IX_TvseriesMediaInfo_TvseriesTvserieId" ON "TvseriesMediaInfo" ("TvseriesTvserieId");

CREATE INDEX "IX_TvseriesReactions_TvseriesEpisodesInfoEpisodesId" ON "TvseriesReactions" ("TvseriesEpisodesInfoEpisodesId");

CREATE INDEX "IX_TvseriesReviewsInfos_TvseriesTvserieId" ON "TvseriesReviewsInfos" ("TvseriesTvserieId");

CREATE INDEX "IX_TvseriesSeasonsInfo_TvseriesTvserieId" ON "TvseriesSeasonsInfo" ("TvseriesTvserieId");

CREATE UNIQUE INDEX "IX_UserModerationSettings_UserId" ON "UserModerationSettings" ("UserId");

CREATE UNIQUE INDEX "IX_UserPermissionsSettings_UserId" ON "UserPermissionsSettings" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260325160221_InitialCreateSQLite', '10.0.2');

COMMIT;

