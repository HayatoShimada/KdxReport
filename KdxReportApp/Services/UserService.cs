using KdxReport.Data;
using KdxReport.Models;
using KPRO_Library.Data;
using KPRO_Library.Models.Generated;
using KPRO_Library.Services;
using Microsoft.EntityFrameworkCore;

namespace KdxReport.Services;

/// <summary>
/// ユーザー管理サービス（MstStaffとの紐づけ機能を含む）
/// </summary>
public class UserService
{
    private readonly KdxReportDbContext _context;
    private readonly CompanyDbContext _companyContext;
    private readonly ReadOnlyDataService _readOnlyDataService;

    public UserService(
        KdxReportDbContext context,
        CompanyDbContext companyContext,
        ReadOnlyDataService readOnlyDataService)
    {
        _context = context;
        _companyContext = companyContext;
        _readOnlyDataService = readOnlyDataService;
    }

    /// <summary>
    /// ユーザーにMstStaffを紐づける
    /// </summary>
    public async Task<bool> LinkStaffToUserAsync(int userId, string staffSerialNo)
    {
        // MstStaffが存在するか確認
        var staff = await _readOnlyDataService.GetStaffBySerialNoAsync(staffSerialNo);
        if (staff == null)
        {
            return false;
        }

        // ユーザーを取得
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        // 紐づけを設定
        user.StaffSerialNo = staffSerialNo;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// ユーザーとMstStaffの紐づけを解除
    /// </summary>
    public async Task<bool> UnlinkStaffFromUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.StaffSerialNo = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// ユーザーに紐づいているMstStaff情報を取得
    /// </summary>
    public async Task<MstStaff?> GetLinkedStaffAsync(int userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null || string.IsNullOrEmpty(user.StaffSerialNo))
        {
            return null;
        }

        return await _readOnlyDataService.GetStaffBySerialNoAsync(user.StaffSerialNo);
    }

    /// <summary>
    /// ユーザーに紐づいているMstStaff情報を取得（ユーザー情報と一緒に）
    /// </summary>
    public async Task<(User User, MstStaff? Staff)?> GetUserWithStaffAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.RoleUsers)
            .ThenInclude(ru => ru.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            return null;
        }

        MstStaff? staff = null;
        if (!string.IsNullOrEmpty(user.StaffSerialNo))
        {
            staff = await _readOnlyDataService.GetStaffBySerialNoAsync(user.StaffSerialNo);
        }

        return (user, staff);
    }

    /// <summary>
    /// 担当者コードでMstStaffを検索してユーザーに紐づける
    /// </summary>
    public async Task<bool> LinkStaffByStaffCdAsync(int userId, string staffCd)
    {
        var staff = await _readOnlyDataService.GetStaffByStaffCdAsync(staffCd);
        if (staff == null)
        {
            return false;
        }

        return await LinkStaffToUserAsync(userId, staff.SerialNo);
    }

    /// <summary>
    /// すべてのユーザーとその紐づけられたMstStaff情報を取得
    /// </summary>
    public async Task<List<(User User, MstStaff? Staff)>> GetAllUsersWithStaffAsync()
    {
        var users = await _context.Users
            .Include(u => u.RoleUsers)
            .ThenInclude(ru => ru.Role)
            .ToListAsync();

        var result = new List<(User User, MstStaff? Staff)>();

        foreach (var user in users)
        {
            MstStaff? staff = null;
            if (!string.IsNullOrEmpty(user.StaffSerialNo))
            {
                staff = await _readOnlyDataService.GetStaffBySerialNoAsync(user.StaffSerialNo);
            }

            result.Add((user, staff));
        }

        return result;
    }

    /// <summary>
    /// すべてのユーザーを取得
    /// </summary>
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.RoleUsers)
            .ThenInclude(ru => ru.Role)
            .OrderBy(u => u.UserName)
            .ToListAsync();
    }

    /// <summary>
    /// ユーザーIDでユーザーを取得
    /// </summary>
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.RoleUsers)
            .ThenInclude(ru => ru.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }
}

