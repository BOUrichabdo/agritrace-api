using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using System.Text;
using TracAgriApi.Services;
using TracAgriApi.Servises;

var builder = WebApplication.CreateBuilder(args);

// ---------------- PORT RAILWAY FIX ----------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// ---------------- SERVICES ----------------

builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
    );

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services métier
builder.Services.AddScoped<IReceptionService, ReceptionService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<QrService>();
builder.Services.AddScoped<PdfService>();

QuestPDF.Settings.License = LicenseType.Community;

// ---------------- APP ----------------
var app = builder.Build();

app.UseCors("AllowAll");

// Swagger always ON (important Railway)
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// test endpoint (IMPORTANT DEBUG)
//app.MapGet("/", () => "AgriTrace API is running 🚀");

// 👇 AJOUTE ICI
app.MapGet("/", () => Results.Ok("API AgriTrace is running"));

app.Run();






















//using AgriTraceAPI.Data;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using QuestPDF.Infrastructure;
//using System.Text;
//using TracAgriApi.Services;
//using TracAgriApi.Servises;

//var builder = WebApplication.CreateBuilder(args);


//var port = Environment.GetEnvironmentVariable("PORT");

//if (string.IsNullOrEmpty(port))
//{
//    port = "8080";
//}

//builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

////builder.WebHost.UseUrls("http://0.0.0.0:5124");




//builder.Services.AddControllers()
//    .AddJsonOptions(x =>
//        x.JsonSerializerOptions.ReferenceHandler =
//            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);



//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters =
//            new TokenValidationParameters
//            {
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidateLifetime = true,
//                ValidateIssuerSigningKey = true,

//                ValidIssuer = builder.Configuration["Jwt:Issuer"],
//                ValidAudience = builder.Configuration["Jwt:Audience"],

//                IssuerSigningKey = new SymmetricSecurityKey(
//                    Encoding.UTF8.GetBytes(
//                        builder.Configuration["Jwt:Key"]!))
//            };
//    });

//builder.Services.AddAuthorization();




//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("DefaultConnection")
//    ));

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

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//builder.Services.AddScoped<IReceptionService, ReceptionService>();// app.UseHttpsRedirection();
//builder.Services.AddScoped<IStockService, StockService>();// app.UseHttpsRedirection();

//builder.Services.AddScoped<QrService>();

//builder.Services.AddScoped<PdfService>();
//var app = builder.Build();
//app.UseCors("AllowAll");

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//QuestPDF.Settings.License = LicenseType.Community;

////builder.Services.AddScoped<PdfService>();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();