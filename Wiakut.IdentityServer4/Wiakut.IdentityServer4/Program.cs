using Wiakut.IdentityServer4.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureIdentityPersistence(builder.Configuration);

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseRouting();
app.UseIdentityServer();

app.UseEndpoints(endpoints =>
    endpoints.MapDefaultControllerRoute());

app.Run();