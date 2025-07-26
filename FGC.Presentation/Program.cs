using FCG.Infrastructure.Data.Context;
using FGC.Application.UserManagement.UseCases;
using FGC.Domain.UserManagement.Interfaces;
using FGC.Infrastructure.Repositories;
using FGC.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "FIAP Cloud Games API",
        Version = "v1",
        Description = "API para gerenciamento de usuários e jogos da plataforma FCG"
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddDbContext<FCGDbContext>(options =>
{
    if (builder.Environment.IsProduction())
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        options.UseInMemoryDatabase("FCGDevelopmentDb");
    }

    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserUniquenessService, UserUniquenessService>();
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<AuthenticateUserUseCase>();
builder.Services.AddScoped<GetUserProfileUseCase>();
builder.Services.AddScoped<ChangePasswordUseCase>();
builder.Services.AddScoped<DeactivateUserUseCase>();
builder.Services.AddScoped<CreateAdminUserUseCase>();
builder.Services.AddScoped<PromoteUserToAdminUseCase>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHealthChecks().AddDbContextCheck<FCGDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FCG API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.MapGet("/", () => new
{
    Application = "FIAP Cloud Games API",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow,
    Endpoints = new
    {
        Swagger = "/swagger",
        Health = "/health",
        Users = "/api/users",
        Auth = "/api/auth",
        Admin = "/api/admin",
        DebugUsers = "/debug/users"
    }
});

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<FCGDbContext>();

    try
    {
        await context.Database.EnsureCreatedAsync();
        Console.WriteLine("Banco de dados inicializado com sucesso");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao inicializar banco: {ex.Message}");
    }
}

Console.WriteLine("FIAP Cloud Games API iniciada!");
Console.WriteLine($"Ambiente: {app.Environment.EnvironmentName}");
Console.WriteLine($"Swagger: {(app.Environment.IsDevelopment() ? "Habilitado" : "Desabilitado")}");

app.Run();