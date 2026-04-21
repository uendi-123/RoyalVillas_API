using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using RoyalVillas_API.Data;
using RoyalVillas_API.Models;
using RoyalVillas_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.OpenApi;
using RoyalVillaDTO;

var builder = WebApplication.CreateBuilder(args);
var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtSettings")["Secret"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme= JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter JWT Bearer token"
            }
        };
        document.Security =
        [
            new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference("Bearer"),new List<string>()}
            }
        ];
        return Task.CompletedTask;
    });
});
builder.Services.AddAutoMapper(o=>
{
    o.CreateMap<Villa, VillaCreateDTO>().ReverseMap();
    o.CreateMap<Villa, VillaUpdateDTO>().ReverseMap();
    o.CreateMap<Villa, VillaDTO>().ReverseMap();
    o.CreateMap<VillaUpdateDTO, VillaDTO>().ReverseMap();
    o.CreateMap<User, UserDTO>().ReverseMap();

    o.CreateMap<VillaAmenities, VillaAmenitiesCreateDTO>().ReverseMap();
    o.CreateMap<VillaAmenitiesDTO, VillaAmenitiesUpdateDTO>().ReverseMap();
    o.CreateMap<VillaAmenities, VillaAmenitiesDTO>()
    .ForMember(dest => dest.VillaName, opt => opt.MapFrom(src => src.Villa!=null? src.Villa.Name : null));
    o.CreateMap<VillaAmenitiesDTO, VillaAmenities>();
});

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

await SeedDataAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


static async Task SeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();
}