using Wiakut.IdentityServer4.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureIdentityPersistence(builder.Configuration);

var app = builder.Build();

app.UseIdentityServer();

app.Run();