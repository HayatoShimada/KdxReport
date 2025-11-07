using KdxReport.Data;
using KdxReport.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace KdxReport.Services;

public class AuthService
{
    private readonly KdxReportDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(KdxReportDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
        {
            return null;
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .Include(u => u.RoleUsers)
            .ThenInclude(ru => ru.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

        if (user == null)
            return null;

        // Verify password using BCrypt
        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            return null;

        return user;
    }

    public async Task<bool> SignInAsync(User user)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return false;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // Add roles
        foreach (var roleUser in user.RoleUsers)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleUser.Role.RoleName));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return true;
    }

    public async Task SignOutAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public async Task<User?> RegisterUserAsync(string userName, string email, string password, string roleName = "User")
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == email))
            return null;

        // Hash password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            UserName = userName,
            Email = email,
            Password = hashedPassword
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Assign role
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
        if (role != null)
        {
            var roleUser = new RoleUser
            {
                UserId = user.UserId,
                RoleId = role.RoleId
            };
            _context.RoleUsers.Add(roleUser);
            await _context.SaveChangesAsync();
        }

        return user;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return null;

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return null;

        return await _context.Users
            .Include(u => u.RoleUsers)
            .ThenInclude(ru => ru.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        // Verify old password
        if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
            return false;

        // Hash and update new password
        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Admin用: ユーザー情報を更新
    /// </summary>
    public async Task<bool> UpdateUserAsync(int userId, string userName, string email)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        // メールアドレスの重複チェック（自分以外）
        if (await _context.Users.AnyAsync(u => u.Email == email && u.UserId != userId))
            return false;

        user.UserName = userName;
        user.Email = email;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Admin用: ユーザーのパスワードを変更（旧パスワードの確認不要）
    /// </summary>
    public async Task<bool> ChangeUserPasswordAsync(int userId, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        // Hash and update new password
        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Admin用: ユーザーのロールを更新
    /// </summary>
    public async Task<bool> UpdateUserRolesAsync(int userId, List<string> roleNames)
    {
        var user = await _context.Users
            .Include(u => u.RoleUsers)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return false;

        // 既存のロールを削除
        _context.RoleUsers.RemoveRange(user.RoleUsers);

        // 新しいロールを追加
        foreach (var roleName in roleNames)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
            if (role != null)
            {
                _context.RoleUsers.Add(new RoleUser
                {
                    UserId = userId,
                    RoleId = role.RoleId
                });
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Admin用: ユーザーを削除
    /// </summary>
    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.RoleUsers)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return false;

        // 関連するRoleUserを削除
        _context.RoleUsers.RemoveRange(user.RoleUsers);

        // ユーザーを削除
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// すべてのロールを取得
    /// </summary>
    public async Task<List<Role>> GetAllRolesAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<bool> IsDefaultPasswordAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        // Check if password is still the default "123456"
        return BCrypt.Net.BCrypt.Verify("123456", user.Password);
    }
}
