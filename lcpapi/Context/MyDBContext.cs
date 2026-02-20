using lcpapi.Models;
using Microsoft.EntityFrameworkCore;

namespace lcpapi.Context;

public class MyDBContext : DbContext
{
    private readonly IConfiguration _config;

    public MyDBContext(DbContextOptions options, IConfiguration config)
        : base(options)
    {
        _config = config;
    }

    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<Anime> Animes { get; set; } = null!;
    public DbSet<Movies> Movies { get; set; } = null!;
    public DbSet<Tvseries> Tvseries { get; set; } = null!;
    public DbSet<TvseriesSeasonsInfo> TvseriesSeasonsInfo { get; set; } = null!;
    public DbSet<TvseriesEpisodesInfo> TvseriesEpisodesInfos { get; set; } = null!;
    public DbSet<TvseriesReviewsInfo> TvseriesReviewsInfos { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Software> Softwares { get; set; } = null!;
    public DbSet<Music> Musics { get; set; } = null!;
    public DbSet<Pet> Pets { get; set; } = null!;
    public DbSet<ActionFigure> ActionFigures { get; set; } = null!;
    public DbSet<RecipesFoods> RecipesFoods { get; set; } = null!;
    public DbSet<GamesMediaInfo> GamesMedias { get; set; } = null!;
    public DbSet<FileUploadInfo> FileUploadInfos { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured) {
            var defdbm = _config.GetSection("DefDBMode").Value ?? "MemoryDB";

            if(defdbm == "MemoryDB") {
                optionsBuilder.UseInMemoryDatabase("DBContext");
            }
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        new MyDBSeed(modelBuilder).Seed(false);

        // modelBuilder.Entity<TvseriesReviewsInfo>()
        //     .Property(h => h.ReviewsScoreRating)
        //     .HasColumnType("decimal(18,2)");

        // modelBuilder.Entity<TvseriesSeasonsInfo>()
        // .HasMany(e => e.EpisodesInfo)
        // .WithMany(x => x.SeasonsInfo);
    }
}