using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using PatrickBotman.AdminPortal.Auth;
using PatrickBotman.AdminPortal.Services;
using PatrickBotman.Common.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddScheme<JwtBearerOptions, GoogleJwtBearerHandler>(JwtBearerDefaults.AuthenticationScheme, options => {
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidIssuers = new[] { "https://accounts.google.com", "accounts.google.com" },
        ValidAudience = "678092915142-sbh7tmh3demtir4b4tupig8gl47fsfh6.apps.googleusercontent.com"
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserIsAdmin", policy =>
    {
        policy.Requirements.Add(new AdminAuthorizationRequirement());
    });
});


builder.Services.ConfigurePersistence(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(cfg =>
{
    cfg.AllowAnyHeader()
        .AllowAnyOrigin()
        .AllowAnyMethod();
});

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
