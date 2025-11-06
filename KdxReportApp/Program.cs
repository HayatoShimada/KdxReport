using KdxReport.Components;
using KdxReport.Data;
using KdxReport.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Minio;

var builder = WebApplication.CreateBuilder(args);

var resetAdminPassword = args.Contains("--reset-admin-password", StringComparer.OrdinalIgnoreCase);
var verifyAdminPasswordArg = args.FirstOrDefault(a => a.StartsWith("--verify-admin-password=", StringComparison.OrdinalIgnoreCase));
string? verifyAdminPassword = verifyAdminPasswordArg?.Split('=', 2)[1];

// Configure timezone to JST
var jstTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<KdxReportDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    ));

// Add MinIO client
builder.Services.AddMinio(configureClient => configureClient
    .WithEndpoint(builder.Configuration["MinIO:Endpoint"])
    .WithCredentials(
        builder.Configuration["MinIO:AccessKey"],
        builder.Configuration["MinIO:SecretKey"]
    )
    .WithSSL(bool.Parse(builder.Configuration["MinIO:UseSSL"] ?? "false"))
    .Build());

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add Services
builder.Services.AddScoped<KdxReport.Services.AuthService>();
builder.Services.AddScoped<KdxReport.Services.TripReportService>();
builder.Services.AddScoped<KdxReport.Services.CompanyService>();
builder.Services.AddScoped<KdxReport.Services.EquipmentService>();
builder.Services.AddScoped<KdxReport.Services.FileStorageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Initialize database with retry logic
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<KdxReportDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    const int maxRetries = 5;
    const int delaySeconds = 3;
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            logger.LogInformation("Attempting database migration (attempt {Attempt}/{MaxRetries})...", attempt, maxRetries);
            dbContext.Database.Migrate();
            logger.LogInformation("Database migration completed successfully.");
            EnsureDefaultAdmin(dbContext, logger, forceReset: resetAdminPassword);

            break;
        }
        catch (Npgsql.NpgsqlException ex) when (attempt < maxRetries)
        {
            logger.LogWarning(ex, "Database connection failed (attempt {Attempt}/{MaxRetries}). Retrying in {Delay} seconds...", attempt, maxRetries, delaySeconds);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            if (attempt == maxRetries)
            {
                logger.LogCritical("Database migration failed after {MaxRetries} attempts. Please ensure PostgreSQL is running.", maxRetries);
                logger.LogInformation("To start PostgreSQL, run: docker-compose up -d postgres");
            }
            break;
        }
    }
}

if (resetAdminPassword)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<KdxReportDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    dbContext.Database.Migrate();
    EnsureDefaultAdmin(dbContext, logger, forceReset: true);

    logger.LogInformation("Default admin password reset command completed successfully. Exiting application.");
    return;
}

if (!string.IsNullOrEmpty(verifyAdminPassword))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<KdxReportDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    var isValid = VerifyAdminPassword(dbContext, verifyAdminPassword);
    logger.LogInformation(isValid
        ? "Verification succeeded: supplied password matches the stored admin password."
        : "Verification failed: supplied password does not match the stored admin password.");

    return;
}

app.Run();

static void EnsureDefaultAdmin(KdxReportDbContext dbContext, ILogger logger, bool forceReset)
{
    const string defaultAdminEmail = "admin@example.com";
    const string defaultAdminPassword = "123456";
    const string defaultAdminUserName = "管理者";

    var adminRole = dbContext.Roles.FirstOrDefault(r => r.RoleName == "Admin");
    if (adminRole == null)
    {
        adminRole = new Role
        {
            RoleName = "Admin",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        dbContext.Roles.Add(adminRole);
        logger.LogWarning("'Admin' role was missing and has been recreated. Please review role data consistency.");
    }

    var adminUser = dbContext.Users
        .Include(u => u.RoleUsers)
        .FirstOrDefault(u => u.Email == defaultAdminEmail);

    var passwordShouldBeReset = forceReset;

    if (adminUser == null)
    {
        logger.LogInformation("No admin user found with email {Email}. Creating default admin user...", defaultAdminEmail);

        adminUser = new User
        {
            UserName = defaultAdminUserName,
            Email = defaultAdminEmail,
            Password = BCrypt.Net.BCrypt.HashPassword(defaultAdminPassword),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Users.Add(adminUser);
        passwordShouldBeReset = true;
    }
    else if (forceReset)
    {
        adminUser.Password = BCrypt.Net.BCrypt.HashPassword(defaultAdminPassword);
        adminUser.UpdatedAt = DateTime.UtcNow;
        logger.LogInformation("Default admin password has been reset to the initial value.");
    }

    if (!adminUser.RoleUsers.Any(ru => ru.RoleId == adminRole.RoleId))
    {
        adminUser.RoleUsers.Add(new RoleUser
        {
            Role = adminRole,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        logger.LogInformation("Admin role assigned to default admin user.");
    }

    dbContext.SaveChanges();

    if (passwordShouldBeReset)
    {
        logger.LogInformation("Default admin credentials -> Email: {Email}, Password: {Password}", defaultAdminEmail, defaultAdminPassword);
        logger.LogInformation("Please change this password immediately after logging in.");
    }
}

static bool VerifyAdminPassword(KdxReportDbContext dbContext, string password)
{
    var adminUser = dbContext.Users.FirstOrDefault(u => u.Email == "admin@example.com");
    if (adminUser == null)
    {
        return false;
    }

    return BCrypt.Net.BCrypt.Verify(password, adminUser.Password);
}
