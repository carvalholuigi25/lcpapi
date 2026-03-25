using AspNetCoreRateLimit;
using lcpapi.Authorization;
using lcpapi.Context;
using lcpapi.Functions;
using lcpapi.Helpers;
using lcpapi.Hubs;
using lcpapi.Interfaces;
using lcpapi.Localization;
using lcpapi.Operations;
using lcpapi.Repositories;
using lcpapi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var env = builder.Environment;

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

switch (config.GetSection("DefDBMode").Value)
{
    case "MySQL":
        builder.Services.AddDbContext<MyDBContext, MyDBContextMySQL>();
        break;
    case "PostgresSQL":
        builder.Services.AddDbContext<MyDBContext, MyDBContextPostgresSQL>();
        break;
    case "SQLite":
        builder.Services.AddDbContext<MyDBContext, MyDBContextSQLite>();
        break;
    case "SQLServer":
        builder.Services.AddDbContext<MyDBContext, MyDBContextSQLServer>();
        break;
    default:
        builder.Services.AddDbContext<MyDBContext>();
        break;
}

builder.Services.AddCors();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddControllers()
    .AddJsonOptions(x => {
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UsersOnly", policy => policy.RequireRole("user"));
    options.AddPolicy("TeamMembersOnly", policy => policy.RequireRole("member"));
    options.AddPolicy("StaffOnly", policy => policy.RequireRole("admin", "moderator"));
    options.AddPolicy("AllUsers", policy => policy.RequireRole("user", "member", "moderator", "admin"));
});

builder.Services.AddOpenApiDocument(options => {
     options.PostProcess = document =>
     {
         document.Info = new OpenApiInfo
         {
             Version = "v1",
             Title = "LCP Api",
             Description = "LCPApi",
             TermsOfService = "https://localhost:5000/terms",
             Contact = new OpenApiContact
             {
                 Name = "LCP Contacts",
                 Url = "https://localhost:5000/contacts"
             },
             License = new OpenApiLicense
             {
                 Name = "LCP License",
                 Url = "https://localhost:5000/license"
             }
         };
     };

    options.AddSecurity("Bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT", 
        Description = "Type into the textbox: {your JWT token}."
    });

    options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
    options.OperationProcessors.Add(new ExcludeSpecificActionsProcessor());
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<AppSettings>(config.GetSection("AppSettings"));
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

builder.Services.AddLocalization();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddInMemoryRateLimiting();

builder.Services.AddScoped<IUsersRepo, UsersRepo>();
builder.Services.AddScoped<IPostsRepo, PostsRepo>();
builder.Services.AddScoped<IGamesRepo, GamesRepo>();
builder.Services.AddScoped<IAnimesRepo, AnimesRepo>();
builder.Services.AddScoped<IMoviesRepo, MoviesRepo>();
builder.Services.AddScoped<ITvseriesRepo, TvseriesRepo>();
builder.Services.AddScoped<ITvseriesReviewsRepo, TvseriesReviewsRepo>();
builder.Services.AddScoped<ITvseriesEpisodesRepo, TvseriesEpisodesRepo>();
builder.Services.AddScoped<ITvseriesSeasonsRepo, TvseriesSeasonsRepo>();
builder.Services.AddScoped<IBooksRepo, BooksRepo>();
builder.Services.AddScoped<ISoftwaresRepo, SoftwaresRepo>();
builder.Services.AddScoped<IActionFiguresRepo, ActionFiguresRepo>();
builder.Services.AddScoped<IRecipesFoodsRepo, RecipesFoodsRepo>();
builder.Services.AddScoped<IMusicsRepo, MusicsRepo>();
builder.Services.AddScoped<IPetsRepo, PetRepo>();
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IUploadedFilesRepo, UploadedFilesRepo>();

builder.Services.AddSingleton<LocalizationMiddleware>();
builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddSignalR();

var app = builder.Build();

var supportedCultures = await Functions.GetLanguagesCultureList();

var options = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(supportedCultures[0], supportedCultures[0]),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,
    RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider()
    }
};

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .WithOrigins("https://localhost:5000", "http://localhost:5001",
    "https://10.0.2.2:5000", "http://10.0.2.2:5001", "https://192.168.1.72:5000",
    "http://192.168.1.72:5001", "https://192.168.1.72:8080", "https://192.168.1.72:8081",
    "https://0.0.0.0:5000", "http://0.0.0.0:5001")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseRequestLocalization(options);
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
// app.UseHttpsRedirection();
app.UseIpRateLimiting();
app.UseAuthorization();
app.UseAuthentication();

app.UseOpenApi();
app.UseSwaggerUi(settings => 
{
    settings.PersistAuthorization = true;
});

app.MapOpenApi();

app.UseReDoc(options =>
{
    options.Path = "/docs";
});

app.UseMiddleware<LocalizationMiddleware>();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.MapHub<DataHub>("/dataHub");

app.Run();