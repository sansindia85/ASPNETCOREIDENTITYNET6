using System.Reflection;
using Duende.IdentityServer;
using DuendeWithIdentity.Data;
using DuendeWithIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DuendeWithIdentity
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddRazorPages();

            builder.Services.AddScoped<IUserStore<DemoUser>,
                UserOnlyStore<DemoUser, DemoUserDbContext>>();

            var migrationAssembly = typeof(DemoUser).GetTypeInfo().Assembly.GetName().Name;

            var connectionString = @"Data Source= (localdb)\ProjectModels; Initial Catalog=Duende.DemoUser";

            builder.Services.AddDbContext<DemoUserDbContext>(opt => opt.UseSqlServer(connectionString,
                sql => sql.MigrationsAssembly(migrationAssembly)));

            //builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            builder.Services.AddIdentityCore<DemoUser>(options => { })
                .AddEntityFrameworkStores<DemoUserDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.TryAddScoped<SignInManager<DemoUserDbContext>>();

            builder.Services.AddScoped<IUserStore<DemoUser>,
                UserOnlyStore<DemoUser, DemoUserDbContext>>();

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
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients);
                //.AddAspNetIdentity<DemoUser>();




            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:5001/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });

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

            app.MapRazorPages()
                .RequireAuthorization();

            return app;
        }
    }
}