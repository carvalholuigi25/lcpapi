using lcpapi.Models;
using lcpapi.Models.Enums;
using Microsoft.EntityFrameworkCore;

public class MyDBSeed
{
    private readonly ModelBuilder _modelBuilder;

    public MyDBSeed(ModelBuilder modelBuilder)
    {
        _modelBuilder = modelBuilder;
    }

    public void Seed(bool isseed = false)
    {
        if(isseed) {
            _modelBuilder.Entity<NGamesMediaInfo>().HasKey(gm => new { gm.GameId, gm.ScreenshotId, gm.VideoId });
            _modelBuilder.Entity<NGamesMediaInfo>().HasOne(gm => gm.Games).WithMany().HasForeignKey(gm => gm.GameId);
            _modelBuilder.Entity<NGamesMediaInfo>().HasOne(gm => gm.ScreenshotsInfo).WithMany().HasForeignKey(gm => gm.ScreenshotId);
            _modelBuilder.Entity<NGamesMediaInfo>().HasOne(gm => gm.VideosInfo).WithMany().HasForeignKey(gm => gm.VideoId);

            _modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = 1,
                    Username = "admin",
                    DisplayName = "Luis Carvalho",
                    Password = BCrypt.Net.BCrypt.HashPassword("1234", 10, false),
                    Role = EUsersRoles.admin
                }
            );
            
            _modelBuilder.Entity<GamesMediaInfo>().HasData(
                 new GamesMediaInfo
                 {
                     MediaId = 1,
                     GameId = 1,
                     Url = "https://example.com/images/gtavcs_ss1.jpg",
                     TypeMedia = "screenshot",
                     IsFeatured = true
                 },
                new GamesMediaInfo
                {
                    MediaId = 2,
                    GameId = 1,
                    Url = "https://example.com/images/gtavcs_ss2.jpg",
                    TypeMedia = "screenshot",
                    IsFeatured = false
                },
                new GamesMediaInfo
                {
                    MediaId = 3,
                    GameId = 2,
                    Url = "https://example.com/images/zelda_oot_ss1.jpg",
                    TypeMedia = "screenshot",
                    IsFeatured = true
                },
               new GamesMediaInfo
               {
                   MediaId = 4,
                   GameId = 2,
                   Url = "https://example.com/images/zelda_oot_ss2.jpg",
                   TypeMedia = "screenshot",
                   IsFeatured = false
               },
                new GamesMediaInfo
                {
                    MediaId = 5,
                    GameId = 3,
                    Url = "https://example.com/images/halo_ce_ss1.jpg",
                    TypeMedia = "screenshot",
                    IsFeatured = true
                },
                new GamesMediaInfo
                {
                    MediaId = 6,
                    GameId = 3,
                    Url = "https://example.com/images/halo_ce_ss2.jpg",
                    TypeMedia = "screenshot",
                    IsFeatured = false
                },
                new GamesMediaInfo
                {
                    MediaId = 7,
                    GameId = 1,
                    Url = "https://example.com/videos/gtavcs_trailer.mp4",
                    TypeMedia = "video",
                    IsFeatured = true
                },
                new GamesMediaInfo
                {
                    MediaId = 8,
                    GameId = 2,
                    Url = "https://example.com/videos/zelda_oot_trailer.mp4",
                    TypeMedia = "video",
                    IsFeatured = false
                },
                new GamesMediaInfo
                {
                    MediaId = 9,
                    GameId = 3,
                    Url = "https://example.com/videos/halo_ce_trailer.mp4",
                    TypeMedia = "video",
                    IsFeatured = false
                }
            );

            _modelBuilder.Entity<Game>().HasData(
                new Game()
                {
                    GameId = 1,
                    Title = "Grand Theft Auto Vice City Stories",
                    Description = "Experience the criminal underworld of Vice City in this action-packed open-world game.",
                    Image = "https://example.com/images/gtavcs.jpg",
                    Publisher = "Rockstar Games",
                    Developer = "Rockstar Leeds",
                    IsFeatured = true,
                    ScoreRating = 88,
                    ReleaseDate = new DateTime(2006, 10, 31),
                    ESRB = EGameESRB.Mature,
                    Edition = new List<EGameEdition> { EGameEdition.Standard },
                    Genre = new List<EGameGenre> { EGameGenre.Action, EGameGenre.Adventure },
                    Platform = new List<EGamePlatform> { EGamePlatform.PlayStation2, EGamePlatform.PlayStationPortable }
                },
                new Game()
                {
                    GameId = 2,
                    Title = "The Legend of Zelda: Ocarina of Time",
                    Description = "Embark on an epic quest as Link to save Hyrule and defeat the evil Ganondorf.",
                    Image = "https://example.com/images/zelda_oot.jpg",
                    Publisher = "Nintendo",
                    Developer = "Nintendo EAD",
                    IsFeatured = false,
                    ScoreRating = 95,
                    ReleaseDate = new DateTime(1998, 11, 21),
                    ESRB = EGameESRB.Teen,
                    Edition = new List<EGameEdition> { EGameEdition.Collector },
                    Genre = new List<EGameGenre> { EGameGenre.Action, EGameGenre.Adventure, EGameGenre.RPG },
                    Platform = new List<EGamePlatform> { EGamePlatform.Nintendo64 },
                },
                new Game()
                {
                    GameId = 3,
                    Title = "Halo: Combat Evolved",
                    Description = "Join Master Chief in the fight against the alien Covenant in this groundbreaking first-person shooter.",
                    Image = "https://example.com/images/halo_ce.jpg",
                    Publisher = "Microsoft Game Studios",
                    Developer = "Bungie",
                    IsFeatured = false,
                    ScoreRating = 97,
                    ReleaseDate = new DateTime(2001, 11, 15),
                    ESRB = EGameESRB.Mature17Plus,
                    Edition = new List<EGameEdition> { EGameEdition.Standard, EGameEdition.Deluxe },
                    Genre = new List<EGameGenre> { EGameGenre.Action, EGameGenre.Shooter },
                    Platform = new List<EGamePlatform> { EGamePlatform.Xbox },
                }
            );
        }
    }
}
