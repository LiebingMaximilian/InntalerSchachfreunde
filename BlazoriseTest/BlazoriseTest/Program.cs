using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using InntalerSchachfreunde;
using InntalerSchachfreunde.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Blazored.SessionStorage;
using System.Text.Json;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();


        var con = builder.Configuration.GetConnectionString("DefaultConnection");
        var serverVersion = new MySqlServerVersion(new Version(10, 11, 6));
        builder.Services.AddDbContext<AppDbContext>(options => options
            .UseLazyLoadingProxies() // Enable lazy loading
            .UseMySql(con, serverVersion), ServiceLifetime.Transient);
        builder.Services.AddDbContext<IdentityContext>(options => options
            .UseMySql(con, serverVersion), ServiceLifetime.Transient);
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(config =>
            config.SignIn.RequireConfirmedEmail = false
            )
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders()
            .AddRoles<IdentityRole>();
        builder.Services.AddHttpClient<AuthService>(options =>
        {
            options.BaseAddress = new Uri("http://localhost:7122/api/v1/");
        });

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //.AddCookie(options =>
            //{
            //    options.LoginPath = "/Login";
            //    options.AccessDeniedPath = "/AccessDenied";
            //})
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });;
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            options.AddPolicy("RequireWriterRole", policy => policy.RequireRole("Writer"));
        });

        builder.Services.AddTransient<ITournamentService, TournamentService>();
        builder.Services.AddTransient<IArticleService, ArticleService>();
        builder.Services.AddTransient<IPlayerService, PlayerService>();
        builder.Services.AddTransient<IKeyValueService, KeyValueService>();
        builder.Services.AddTransient<ITerminService, TerminService>();

        builder.Services.AddBlazoredSessionStorage(config => {
            config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            config.JsonSerializerOptions.IgnoreNullValues = true;
            config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
            config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            config.JsonSerializerOptions.WriteIndented = false;
        }
        );
        builder.Services.AddScoped<CustomAuthenticationStateProvider>();
        builder.Services.AddCascadingAuthenticationState();
        AddBlazorise(builder.Services);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();


        void AddBlazorise(IServiceCollection services)
        {
            services
                .AddBlazorise()
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
        }
    }
}
