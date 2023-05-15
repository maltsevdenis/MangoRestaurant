using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Mongo.Services.Identity.DbContext;
using Mongo.Services.Identity.Initializer;
using Mongo.Services.Identity.Models;
using Mongo.Services.Identity.Services;

using Serilog;

namespace Mongo.Services.Identity;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryIdentityResources(SD.IdentityResources)
            .AddInMemoryApiScopes(SD.ApiScopes)
            .AddInMemoryClients(SD.Clients)
            .AddAspNetIdentity<ApplicationUser>();

        builder.Services.AddScoped<IDbIntializer, DbIntializer>();

        builder.Services.AddScoped<IProfileService, ProfileService>();

        //builder.AddDeveloperSigningCredentails();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var dbIntializer = scope.ServiceProvider.GetService<IDbIntializer>();

            dbIntializer.Initialize();
        }

        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}