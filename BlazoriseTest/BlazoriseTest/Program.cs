using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using InntalerSchachfreunde;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Runtime.CompilerServices;
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
           .UseMySql(con, serverVersion));

        AddBlazorise(builder.Services);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();


        void AddBlazorise(IServiceCollection services)
        {
            services
                .AddBlazorise();
            services
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
        }
    }
}