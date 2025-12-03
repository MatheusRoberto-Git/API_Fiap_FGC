using FGC.Application.GameManagement.UseCases;
using FGC.Application.PaymentManagement.UseCases;
using FGC.Application.UserManagement.UseCases;
using FGC.Domain.GameManagement.Interfaces;
using FGC.Domain.PaymentManagement.Interfaces;
using FGC.Domain.UserManagement.Interfaces;
using FGC.Infrastructure.Data.Context;
using FGC.Infrastructure.Repositories;
using FGC.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region [Services - Controllers & Swagger]

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "FIAP Cloud Games API",
        Version = "v3",
        Description = "API para gerenciamento de usuários, jogos e pagamentos da plataforma FCG - Fase 3 (Microsserviços)"
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu_token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

#region [Services - Authentication & Authorization]

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var secretKey = jwtSettings["SecretKey"];

    if (string.IsNullOrEmpty(secretKey))
        throw new InvalidOperationException("JWT SecretKey não configurada");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"[JWT] Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var email = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            Console.WriteLine($"[JWT] Token validated for user: {email}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"[JWT] Authentication challenged: {context.Error}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy =>
        policy.RequireAuthenticatedUser());

    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});

#endregion

#region [Services - Database & Repositories]

builder.Services.AddDbContext<FGCDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// User Management
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserUniquenessService, UserUniquenessService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Game Management
builder.Services.AddScoped<IGameRepository, GameRepository>();

// Payment Management
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

#endregion

#region [Services - Use Cases]

// User Use Cases
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<AuthenticateUserUseCase>();
builder.Services.AddScoped<GetUserProfileUseCase>();
builder.Services.AddScoped<ChangePasswordUseCase>();
builder.Services.AddScoped<DeactivateUserUseCase>();
builder.Services.AddScoped<ReactivateUserUseCase>();
builder.Services.AddScoped<CreateAdminUserUseCase>();
builder.Services.AddScoped<PromoteUserToAdminUseCase>();
builder.Services.AddScoped<DemoteAdminToUserUseCase>();

// Game Use Cases
builder.Services.AddScoped<CreateGameUseCase>();
builder.Services.AddScoped<GetGameByIdUseCase>();
builder.Services.AddScoped<GetAllGamesUseCase>();
builder.Services.AddScoped<SearchGamesUseCase>();
builder.Services.AddScoped<UpdateGamePriceUseCase>();

// Payment Use Cases
builder.Services.AddScoped<CreatePaymentUseCase>();
builder.Services.AddScoped<ProcessPaymentUseCase>();
builder.Services.AddScoped<GetPaymentStatusUseCase>();
builder.Services.AddScoped<GetPaymentByIdUseCase>();
builder.Services.AddScoped<GetUserPaymentsUseCase>();
builder.Services.AddScoped<RefundPaymentUseCase>();

#endregion

#region [Services - CORS & HealthChecks]

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHealthChecks().AddDbContextCheck<FGCDbContext>();

#endregion

var app = builder.Build();

#region [Middleware - Swagger]

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FCG API v3 - Microsserviços");
        c.RoutePrefix = string.Empty;
    });
}

#endregion

#region [Middleware - Pipeline]

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.MapGet("/", () => new
{
    Application = "FIAP Cloud Games API",
    Version = "3.0.0",
    Phase = "Fase 3 - Microsserviços",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow,
    Microservices = new
    {
        Users = new { Status = "Active", Endpoints = "/api/users, /api/auth, /api/admin" },
        Games = new { Status = "Active", Endpoints = "/api/games" },
        Payments = new { Status = "Active", Endpoints = "/api/payments" }
    },
    Endpoints = new
    {
        Swagger = "/swagger",
        Health = "/health",
        Users = "/api/users",
        Auth = "/api/auth",
        Admin = "/api/admin",
        Games = "/api/games",
        Payments = "/api/payments"
    }
});

#endregion

#region [Database Initialization - Auto Migration]

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FGCDbContext>();
    try
    {
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.WriteLine("🔄 Verificando e aplicando migrations...");

        // Aplica migrations pendentes automaticamente
        await context.Database.MigrateAsync();

        Console.WriteLine("✅ Banco de dados atualizado com sucesso");
        Console.WriteLine("📦 Tabelas: Users, Games, Payments");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao aplicar migrations: {ex.Message}");
        // Não falha a aplicação, apenas loga o erro
    }
}

#endregion

#region [Logs Startup Info]

Console.WriteLine("═══════════════════════════════════════════════════════");
Console.WriteLine("🎮 FIAP Cloud Games API - Fase 3 (Microsserviços)");
Console.WriteLine("═══════════════════════════════════════════════════════");
Console.WriteLine($"📍 Ambiente: {app.Environment.EnvironmentName}");
Console.WriteLine($"📖 Swagger: Habilitado");
Console.WriteLine("───────────────────────────────────────────────────────");
Console.WriteLine("🔹 Microsserviço: Users     → /api/users, /api/auth, /api/admin");
Console.WriteLine("🔹 Microsserviço: Games     → /api/games");
Console.WriteLine("🔹 Microsserviço: Payments  → /api/payments");
Console.WriteLine("═══════════════════════════════════════════════════════");

#endregion

app.Run();
