using api.Data;
using api.Middleware;
using api.Models;
using api.Options;
//repositories
using api.Repositories.AuctionRepo;
using api.Repositories.BidRepo;
//repositories
using api.Repositories.UserRepo;
//services
using api.Services.AuctionS; 
using api.Services.BidS;
using api.Services.UserS;
//framework itp.
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<api.Services.AuthS.ITokenService, api.Services.AuthS.TokenService>();
builder.Services.AddScoped<api.Services.AuthS.IAuthService, api.Services.AuthS.AuthService>();
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<api.Models.User>, Microsoft.AspNetCore.Identity.PasswordHasher<api.Models.User>>();
builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
builder.Services.AddScoped<IAuctionService, AuctionService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBidRepository, BidRepository>();
builder.Services.AddScoped<IBidService, BidService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http, // Nazwa Headera
        Scheme = "Bearer", //Schemat: Bearer (JWT)
        BearerFormat = "JWT", //Format: JWT token
        Description = "Paste ur JWT token here. {token}" //instrukcja dla usera
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
           
            [new OpenApiSecuritySchemeReference("bearer", document)] = [],
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services
    .AddOptions<JwtOptions>() //ustawienie konfiguracji jwt
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))//mapuje sekcje "jwt" na obiekt JwtOptions Issuer=ProjektRESTapi, Audience=ProjektRestApi, ExpiresMinutes = 60, Key przychodzi z user-secrets/env
    .Validate(o => //sprawdza czy key istnieje czy expires minutes ma sens czy issuer/audience nie sa puste | o jest obiektem JwtOptions zbidnowany z appsettings + user-secrets/env
        !string.IsNullOrWhiteSpace(o.Issuer) && //issuer musi istniec i nie moze byc null ani pusty
        !string.IsNullOrWhiteSpace(o.Audience) && 
        !string.IsNullOrWhiteSpace(o.Key) &&
        o.Key.Length >= 32 && //key musi posiadac 32 znaki
        o.ExpiresMinutes >= 1 && //token musi miec wygasac po 1 lub wiecej minutach
        o.ExpiresMinutes <= 24 * 60, //token moze byc wazny max 24h
        "Invalid JWT configuration. Provide Jwt:Issuer, Jwt:Audience, Jwt:Key (min 32 chars) and sensible Jwt:ExpiresMinutes.")
    .ValidateOnStart(); //fail fast dla aplikacji gdy cos jest nie tak przykladowo zapomnimy ustawić Jwt:Key
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //AddAuthentication pokazuje domyslny schemat dla apki
    .AddJwtBearer(options =>
    {
        var jwtIssuer = builder.Configuration["Jwt:Issuer"]; 
        var jwtAudience = builder.Configuration["Jwt:Audience"];
        var jwtKey = builder.Configuration["Jwt:Key"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, //sprawdza czy to Issuer token
            ValidIssuer = jwtIssuer,

            ValidateAudience = true, //sprawdza czy to Audience token
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)), //klucz do sprawdzania podpisu tokena

            ValidateLifetime = true, //sprawdzamy czy token nie wygasl
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication(); //serwer bierze token z requesta weryfikuje go i jezeli przejdzie weryfikacje tworzy httpcontext.user
app.UseAuthorization(); // serwer sprawdza czy endpoint ma [Authorize], czy user jest zalogowany i czy ma wymagana role czyli musi zajsc taka kolejnosc poniewaz najpierw ustalamy request pozniej sprawdzamy czy ma dostep

app.MapControllers();

app.Run();
