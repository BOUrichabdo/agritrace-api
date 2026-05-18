using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔥 RAILWAY PORT FIX
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        p => p.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

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

var app = builder.Build();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 🔥 HEALTH CHECK
app.MapGet("/", () => "API OK");

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