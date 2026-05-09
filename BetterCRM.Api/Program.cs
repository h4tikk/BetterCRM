using BetterCRM.Api.Middleware;
using BetterCRM.Business.Helpers;
using BetterCRM.Business.Services;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.DataAccess;
using BetterCRM.DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IWorkSessionRepository, WorkSessionRepository>();
builder.Services.AddScoped<ITimeLogRepository, TimeLogRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketParticipantRepository, TicketParticipantRepository>();
builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<ITimeTrackingService, TimeTrackingService>();
builder.Services.AddScoped<IPayrollService, PayrollService>();

builder.Services.AddSingleton<JwtHelper>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key не задан в конфигурации");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero 
        };
    });

builder.Services.AddAuthorization();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? ["http://localhost:5173"];

builder.Services.AddCors(opt => opt.AddPolicy("AllowFrontend", p => p
    .WithOrigins(allowedOrigins)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var pending = db.Database.GetPendingMigrations().ToList();
    if (pending.Count > 0)
    {
        Console.WriteLine($"⏳ Применяем {pending.Count} миграций...");
        db.Database.Migrate();
        Console.WriteLine("✅ Миграции применены.");
    }

    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.Title = "BetterCRM API";
        opt.Theme = ScalarTheme.Moon;
    });
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");      
app.UseAuthentication();
app.UseAuthorization();
app.UseOrganizationContext();

app.MapControllers();
app.Run();