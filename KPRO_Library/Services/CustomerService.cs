using KPRO_Library.Data;
using KPRO_Library.Models.Generated;
using Microsoft.EntityFrameworkCore;

namespace KPRO_Library.Services;

/// <summary>
/// 取引先マスタサービス（読み取り専用）
/// </summary>
/// <remarks>
/// 本番データベースのMST_CUSTOMERテーブルから取引先情報を取得します。
/// このサービスは読み取り専用です。データの変更は行いません。
/// </remarks>
public class CustomerService
{
    private readonly CompanyDbContext _context;

    public CustomerService(CompanyDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// すべての取引先を取得（削除フラグ = 0 のみ）
    /// </summary>
    public async Task<List<MstCustomer>> GetAllCustomersAsync()
    {
        return await _context.MstCustomers
            .AsNoTracking() // 読み取り専用のため追跡を無効化
            .Where(c => c.DeleteFlg == "0")
            .OrderBy(c => c.CustomerCd)
            .ToListAsync();
    }

    /// <summary>
    /// 取引先コードで検索
    /// </summary>
    public async Task<MstCustomer?> GetCustomerByCodeAsync(string customerCd)
    {
        return await _context.MstCustomers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CustomerCd == customerCd && c.DeleteFlg == "0");
    }

    /// <summary>
    /// 会社名で検索（部分一致）
    /// </summary>
    public async Task<List<MstCustomer>> SearchCustomersByNameAsync(string searchTerm)
    {
        return await _context.MstCustomers
            .AsNoTracking()
            .Where(c => c.DeleteFlg == "0" &&
                       (c.CompanyNm!.Contains(searchTerm) ||
                        c.CompanyAb!.Contains(searchTerm) ||
                        c.CompanyKn!.Contains(searchTerm)))
            .OrderBy(c => c.CustomerCd)
            .ToListAsync();
    }

    /// <summary>
    /// 取引先コードのリストで検索
    /// </summary>
    public async Task<List<MstCustomer>> GetCustomersByCodesAsync(List<string> customerCodes)
    {
        return await _context.MstCustomers
            .AsNoTracking()
            .Where(c => customerCodes.Contains(c.CustomerCd) && c.DeleteFlg == "0")
            .OrderBy(c => c.CustomerCd)
            .ToListAsync();
    }

    /// <summary>
    /// 郵便番号で検索
    /// </summary>
    public async Task<List<MstCustomer>> GetCustomersByPostalCodeAsync(string postalCode)
    {
        return await _context.MstCustomers
            .AsNoTracking()
            .Where(c => c.PostCd == postalCode && c.DeleteFlg == "0")
            .ToListAsync();
    }

    /// <summary>
    /// 取引先数を取得
    /// </summary>
    public async Task<int> GetCustomerCountAsync()
    {
        return await _context.MstCustomers
            .AsNoTracking()
            .CountAsync(c => c.DeleteFlg == "0");
    }

    /// <summary>
    /// データベース接続テスト
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            return await _context.Database.CanConnectAsync();
        }
        catch
        {
            return false;
        }
    }
}
