using ABVInvest.Common.Helpers.RssFeeds;
using ABVInvest.Common.Helpers.Serialisation;
using ABVInvest.Common.Mapping;
using ABVInvest.Components;
using ABVInvest.Components.Account;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using ABVInvest.Seeders;
using ABVInvest.Services.Balances;
using ABVInvest.Services.Data;
using ABVInvest.Services.Deals;
using ABVInvest.Services.Portfolios;
using AutoMapper;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddBlazorBootstrap();
builder.Services.AddSyncfusionBlazor();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
        .UseLazyLoadingProxies());
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var mapperConfiguration = new MapperConfiguration(configuration =>
{
    var profile = new MappingProfile();
    configuration.AddProfile(profile);
});
var mapper = mapperConfiguration.CreateMapper();
builder.Services.AddSingleton(mapper);


builder.Services.AddHttpClient();
builder.Services.AddScoped<IRssFeedParser, RssFeedParser>();
builder.Services.AddScoped<IDeserialiser, Deserialiser>();

builder.Services.AddScoped<IPortfoliosService, PortfoliosService>();
builder.Services.AddScoped<IBalancesService, BalancesService>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IDealsService, DealsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseMiddleware<RolesSeedMiddleware>();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ABVInvest.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetConnectionString("SyncfusionLicenseCode") ?? throw new InvalidOperationException("Connection string 'SyncfusionLicenseCode' not found."));

app.Run();
