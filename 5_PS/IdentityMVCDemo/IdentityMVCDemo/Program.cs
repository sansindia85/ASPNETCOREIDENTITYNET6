using System.Reflection;
using IdentityMVCDemo;
using IdentityMVCDemo.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var migrationAssembly = typeof(HomeController).GetTypeInfo().Assembly.GetName().Name;

var connectionString = @"Data Source= (localdb)\ProjectModels; Initial Catalog=IdentityDemo.DemoUser";

builder.Services.AddDbContext<DemoUserDbContext>(opt => opt.UseSqlServer(connectionString,
    sql => sql.MigrationsAssembly(migrationAssembly)));

builder.Services.AddIdentity<DemoUser, IdentityRole>(options =>
    {
        //options.SignIn.RequireConfirmedEmail = true;
        options.Tokens.EmailConfirmationTokenProvider = "emailconf";
    })
    .AddEntityFrameworkStores<DemoUserDbContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<EmailConfirmationTokenProvider<DemoUser>>("emailconf");

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromHours(3));

builder.Services.Configure<EMailConfirmationTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromHours(2));

builder.Services.AddScoped<IUserClaimsPrincipalFactory<DemoUser>,
    DemoUserClaimsPrincipalFactory>();

builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/Home/Login");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
