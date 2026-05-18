

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AgriTraceAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ---------------- RAILWAY PORT ----------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// ---------------- SERVICES ----------------

// DB PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Controllers
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// AUTH (test mode)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = false
    };
});

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---------------- PIPELINE ----------------

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Swagger FIX IMPORTANT
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgriTrace API V1");
    c.RoutePrefix = "swagger";
});

app.MapControllers();

// TEST ENDPOINTS
app.MapGet("/", () => Results.Ok("API OK + DB READY 🚀"));
app.MapGet("/ping", () => "pong");

app.Run();






















//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using AgriTraceAPI.Data;
//using Npgsql.EntityFrameworkCore.PostgreSQL;

//var builder = WebApplication.CreateBuilder(args);

//// ---------------- RAILWAY PORT ----------------
//var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(int.Parse(port));
//});

//// ---------------- SERVICES ----------------

//// DB
//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
//});

//// Controllers
//builder.Services.AddControllers();

//// CORS (IMPORTANT Swagger)
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll",
//        policy =>
//        {
//            policy.AllowAnyOrigin()
//                  .AllowAnyHeader()
//                  .AllowAnyMethod();
//        });
//});

//// AUTH (désactivé pour test)
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = false,
//        ValidateAudience = false,
//        ValidateLifetime = false,
//        ValidateIssuerSigningKey = false
//    };
//});

//builder.Services.AddAuthorization();

//// Swagger
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// ---------------- PIPELINE ORDER (IMPORTANT) ----------------

//app.UseRouting();

//app.UseCors("AllowAll");

//// Swagger MUST be before auth sometimes (important fix)
//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgriTrace API V1");
//    c.RoutePrefix = "swagger";
//});

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//// TEST API
//app.MapGet("/", () => Results.Ok("API OK  1500 + DB READY 🚀"));
//app.MapGet("/ping", () => "pong");

//app.Run();