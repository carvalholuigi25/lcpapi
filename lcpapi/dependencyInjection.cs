using lcpapi.Authorization;
using lcpapi.Interfaces;
using lcpapi.Repositories;
using lcpapi.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepo, UsersRepo>();
        services.AddScoped<IPostsRepo, PostsRepo>();
        services.AddScoped<IGamesRepo, GamesRepo>();
        services.AddScoped<IAnimesRepo, AnimesRepo>();
        services.AddScoped<IMoviesRepo, MoviesRepo>();
        services.AddScoped<ITvseriesRepo, TvseriesRepo>();
        services.AddScoped<ITvseriesReviewsRepo, TvseriesReviewsRepo>();
        services.AddScoped<ITvseriesEpisodesRepo, TvseriesEpisodesRepo>();
        services.AddScoped<ITvseriesSeasonsRepo, TvseriesSeasonsRepo>();
        services.AddScoped<IBooksRepo, BooksRepo>();
        services.AddScoped<ISoftwaresRepo, SoftwaresRepo>();
        services.AddScoped<IActionFiguresRepo, ActionFiguresRepo>();
        services.AddScoped<IRecipesFoodsRepo, RecipesFoodsRepo>();
        services.AddScoped<IMusicsRepo, MusicsRepo>();
        services.AddScoped<IPetsRepo, PetRepo>();
        services.AddScoped<IUploadedFilesRepo, UploadedFilesRepo>();
        services.AddScoped<ISettingsRepo, SettingsRepo>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtUtils, JwtUtils>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOtpService, OtpService>();

        return services;
    }
}