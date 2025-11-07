using KdxReport.Components;
using KdxReport.Data;
using KdxReport.Models;
using KPRO_Library.Data;
using KPRO_Library.Services;
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

// Add External Database Context (SQL Server - Read Only)
builder.Services.AddDbContext<CompanyDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetSection("ExternalDatabase")["ConnectionString"] ??
        throw new InvalidOperationException("ExternalDatabase:ConnectionString is not configured")
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

// External Database Service
builder.Services.AddScoped<ReadOnlyDataService>();

// User Service
builder.Services.AddScoped<KdxReport.Services.UserService>();

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

// Read-only API endpoints for KPRO_Library data
var apiGroup = app.MapGroup("/api/kpro").RequireAuthorization();

// MstCompany endpoints
apiGroup.MapGet("/companies", async (ReadOnlyDataService service) =>
    Results.Ok(await service.GetAllCompaniesAsync()))
    .WithName("GetAllCompanies")
    .WithTags("MstCompany");

apiGroup.MapGet("/companies/{companyCd}", async (string companyCd, ReadOnlyDataService service) =>
{
    var company = await service.GetCompanyByIdAsync(companyCd);
    return company is null ? Results.NotFound() : Results.Ok(company);
})
    .WithName("GetCompanyById")
    .WithTags("MstCompany");

// MstCustomer endpoints
apiGroup.MapGet("/customers", async (ReadOnlyDataService service) =>
    Results.Ok(await service.GetAllCustomersAsync()))
    .WithName("GetAllCustomers")
    .WithTags("MstCustomer");

apiGroup.MapGet("/customers/{customerCd}", async (string customerCd, ReadOnlyDataService service) =>
{
    var customer = await service.GetCustomerByIdAsync(customerCd);
    return customer is null ? Results.NotFound() : Results.Ok(customer);
})
    .WithName("GetCustomerById")
    .WithTags("MstCustomer");

// MstCustomerContact endpoints
apiGroup.MapGet("/customer-contacts", async (ReadOnlyDataService service) =>
    Results.Ok(await service.GetAllCustomerContactsAsync()))
    .WithName("GetAllCustomerContacts")
    .WithTags("MstCustomerContact");

apiGroup.MapGet("/customer-contacts/{customerCd}/{staffCd}", async (string customerCd, string staffCd, ReadOnlyDataService service) =>
{
    var contact = await service.GetCustomerContactByIdAsync(customerCd, staffCd);
    return contact is null ? Results.NotFound() : Results.Ok(contact);
})
    .WithName("GetCustomerContactById")
    .WithTags("MstCustomerContact");

apiGroup.MapGet("/customer-contacts/{customerCd}", async (string customerCd, ReadOnlyDataService service) =>
    Results.Ok(await service.GetCustomerContactsByCustomerCdAsync(customerCd)))
    .WithName("GetCustomerContactsByCustomerCd")
    .WithTags("MstCustomerContact");

// DatEstimate endpoints
apiGroup.MapGet("/estimates", async (ReadOnlyDataService service) =>
    Results.Ok(await service.GetAllEstimatesAsync()))
    .WithName("GetAllEstimates")
    .WithTags("DatEstimate");

apiGroup.MapGet("/estimates/{estimateId}", async (string estimateId, ReadOnlyDataService service) =>
{
    var estimate = await service.GetEstimateByIdAsync(estimateId);
    return estimate is null ? Results.NotFound() : Results.Ok(estimate);
})
    .WithName("GetEstimateById")
    .WithTags("DatEstimate");

// DatOrder endpoints
apiGroup.MapGet("/orders", async (ReadOnlyDataService service) =>
    Results.Ok(await service.GetAllOrdersAsync()))
    .WithName("GetAllOrders")
    .WithTags("DatOrder");

apiGroup.MapGet("/orders/{orderId}", async (string orderId, ReadOnlyDataService service) =>
{
    var order = await service.GetOrderByIdAsync(orderId);
    return order is null ? Results.NotFound() : Results.Ok(order);
})
    .WithName("GetOrderById")
    .WithTags("DatOrder");

// DatOrderDetail endpoints
apiGroup.MapGet("/order-details", async (ReadOnlyDataService service) =>
    Results.Ok(await service.GetAllOrderDetailsAsync()))
    .WithName("GetAllOrderDetails")
    .WithTags("DatOrderDetail");

apiGroup.MapGet("/order-details/{orderId}/{orderNo}/{detailNo}", async (string orderId, string orderNo, int detailNo, ReadOnlyDataService service) =>
{
    var detail = await service.GetOrderDetailByIdAsync(orderId, orderNo, detailNo);
    return detail is null ? Results.NotFound() : Results.Ok(detail);
})
    .WithName("GetOrderDetailById")
    .WithTags("DatOrderDetail");

apiGroup.MapGet("/order-details/{orderId}", async (string orderId, ReadOnlyDataService service) =>
    Results.Ok(await service.GetOrderDetailsByOrderIdAsync(orderId)))
    .WithName("GetOrderDetailsByOrderId")
    .WithTags("DatOrderDetail");

// MstStaff endpoints
apiGroup.MapGet("/staffs", async (ReadOnlyDataService service) =>
    Results.Ok(await service.GetAllStaffsAsync()))
    .WithName("GetAllStaffs")
    .WithTags("MstStaff");

apiGroup.MapGet("/staffs/serial/{serialNo}", async (string serialNo, ReadOnlyDataService service) =>
{
    var staff = await service.GetStaffBySerialNoAsync(serialNo);
    return staff is null ? Results.NotFound() : Results.Ok(staff);
})
    .WithName("GetStaffBySerialNo")
    .WithTags("MstStaff");

apiGroup.MapGet("/staffs/code/{staffCd}", async (string staffCd, ReadOnlyDataService service) =>
{
    var staff = await service.GetStaffByStaffCdAsync(staffCd);
    return staff is null ? Results.NotFound() : Results.Ok(staff);
})
    .WithName("GetStaffByStaffCd")
    .WithTags("MstStaff");

// User-Staff linking endpoints
var userStaffGroup = app.MapGroup("/api/users").RequireAuthorization();

userStaffGroup.MapGet("/{userId}/staff", async (int userId, KdxReport.Services.UserService userService) =>
{
    var result = await userService.GetUserWithStaffAsync(userId);
    return result is null ? Results.NotFound() : Results.Ok(new { result.Value.User, result.Value.Staff });
})
    .WithName("GetUserWithStaff")
    .WithTags("UserStaff");

userStaffGroup.MapPost("/{userId}/staff/{staffSerialNo}", async (int userId, string staffSerialNo, KdxReport.Services.UserService userService) =>
{
    var success = await userService.LinkStaffToUserAsync(userId, staffSerialNo);
    return success ? Results.Ok(new { message = "Staff linked successfully" }) : Results.BadRequest(new { message = "Failed to link staff. User or Staff not found." });
})
    .WithName("LinkStaffToUser")
    .WithTags("UserStaff");

userStaffGroup.MapPost("/{userId}/staff/by-code/{staffCd}", async (int userId, string staffCd, KdxReport.Services.UserService userService) =>
{
    var success = await userService.LinkStaffByStaffCdAsync(userId, staffCd);
    return success ? Results.Ok(new { message = "Staff linked successfully" }) : Results.BadRequest(new { message = "Failed to link staff. User or Staff not found." });
})
    .WithName("LinkStaffByStaffCd")
    .WithTags("UserStaff");

userStaffGroup.MapDelete("/{userId}/staff", async (int userId, KdxReport.Services.UserService userService) =>
{
    var success = await userService.UnlinkStaffFromUserAsync(userId);
    return success ? Results.Ok(new { message = "Staff unlinked successfully" }) : Results.BadRequest(new { message = "Failed to unlink staff. User not found." });
})
    .WithName("UnlinkStaffFromUser")
    .WithTags("UserStaff");

userStaffGroup.MapGet("/with-staff", async (KdxReport.Services.UserService userService) =>
{
    var result = await userService.GetAllUsersWithStaffAsync();
    return Results.Ok(result.Select(r => new { r.User, r.Staff }));
})
    .WithName("GetAllUsersWithStaff")
    .WithTags("UserStaff");

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
