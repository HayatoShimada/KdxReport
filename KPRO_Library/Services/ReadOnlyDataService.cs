using KPRO_Library.Data;
using KPRO_Library.Models.Generated;
using Microsoft.EntityFrameworkCore;

namespace KPRO_Library.Services;

/// <summary>
/// 読み取り専用データアクセスサービス
/// </summary>
public class ReadOnlyDataService
{
    private readonly CompanyDbContext _context;

    public ReadOnlyDataService(CompanyDbContext context)
    {
        _context = context;
    }

    #region MstCompany

    /// <summary>
    /// 会社マスタを全件取得
    /// </summary>
    public async Task<List<MstCompany>> GetAllCompaniesAsync()
    {
        return await _context.MstCompanies
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// 会社コードで会社マスタを取得
    /// </summary>
    public async Task<MstCompany?> GetCompanyByIdAsync(string companyCd)
    {
        return await _context.MstCompanies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CompanyCd == companyCd);
    }

    #endregion

    #region MstCustomer

    /// <summary>
    /// 取引先マスタを全件取得
    /// </summary>
    public async Task<List<MstCustomer>> GetAllCustomersAsync()
    {
        return await _context.MstCustomers
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// 取引先コードで取引先マスタを取得
    /// </summary>
    public async Task<MstCustomer?> GetCustomerByIdAsync(string customerCd)
    {
        return await _context.MstCustomers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CustomerCd == customerCd);
    }

    /// <summary>
    /// 取引先名で取引先マスタを検索（部分一致）
    /// </summary>
    public async Task<List<MstCustomer>> SearchCustomersByNameAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllCustomersAsync();
        }

        var term = searchTerm.Trim();
        return await _context.MstCustomers
            .AsNoTracking()
            .Where(c => 
                (c.CompanyNm != null && c.CompanyNm.Contains(term)) ||
                (c.CompanyAb != null && c.CompanyAb.Contains(term)) ||
                (c.CompanyKn != null && c.CompanyKn.Contains(term)) ||
                (c.CustomerCd != null && c.CustomerCd.Contains(term)) ||
                (c.Address1 != null && c.Address1.Contains(term)) ||
                (c.Tel != null && c.Tel.Contains(term)) ||
                (c.Tel2 != null && c.Tel2.Contains(term))
            )
            .OrderBy(c => c.CompanyNm)
            .Take(200) // 最大200件まで
            .ToListAsync();
    }

    #endregion

    #region MstCustomerContact

    /// <summary>
    /// 得意先担当者マスタを全件取得
    /// </summary>
    public async Task<List<MstCustomerContact>> GetAllCustomerContactsAsync()
    {
        return await _context.MstCustomerContacts
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// 得意先コードと担当者コードで得意先担当者マスタを取得
    /// </summary>
    public async Task<MstCustomerContact?> GetCustomerContactByIdAsync(string customerCd, string staffCd)
    {
        return await _context.MstCustomerContacts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CustomerCd == customerCd && c.StaffCd == staffCd);
    }

    /// <summary>
    /// 得意先コードで得意先担当者マスタを取得（複数）
    /// </summary>
    public async Task<List<MstCustomerContact>> GetCustomerContactsByCustomerCdAsync(string customerCd)
    {
        return await _context.MstCustomerContacts
            .AsNoTracking()
            .Where(c => c.CustomerCd == customerCd)
            .ToListAsync();
    }

    #endregion

    #region DatEstimate

    /// <summary>
    /// 見積情報を全件取得
    /// </summary>
    public async Task<List<DatEstimate>> GetAllEstimatesAsync()
    {
        return await _context.DatEstimates
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// 登録番号で見積情報を取得
    /// </summary>
    public async Task<DatEstimate?> GetEstimateByIdAsync(string estimateId)
    {
        return await _context.DatEstimates
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EstimateId == estimateId);
    }

    #endregion

    #region DatOrder

    /// <summary>
    /// 受注情報を全件取得
    /// </summary>
    public async Task<List<DatOrder>> GetAllOrdersAsync()
    {
        return await _context.DatOrders
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// 登録番号で受注情報を取得
    /// </summary>
    public async Task<DatOrder?> GetOrderByIdAsync(string orderId)
    {
        return await _context.DatOrders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    #endregion

    #region DatOrderDetail

    /// <summary>
    /// 部品表明細情報を全件取得
    /// </summary>
    public async Task<List<DatOrderDetail>> GetAllOrderDetailsAsync()
    {
        return await _context.DatOrderDetails
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// 登録番号、受注番号、明細番号で部品表明細情報を取得
    /// </summary>
    public async Task<DatOrderDetail?> GetOrderDetailByIdAsync(string orderId, string orderNo, int detailNo)
    {
        return await _context.DatOrderDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.OrderId == orderId && d.OrderNo == orderNo && d.DetailNo == detailNo);
    }

    /// <summary>
    /// 登録番号で部品表明細情報を取得（複数）
    /// </summary>
    public async Task<List<DatOrderDetail>> GetOrderDetailsByOrderIdAsync(string orderId)
    {
        return await _context.DatOrderDetails
            .AsNoTracking()
            .Where(d => d.OrderId == orderId)
            .ToListAsync();
    }

    #endregion

    #region MstStaff

    /// <summary>
    /// 担当者マスタを全件取得
    /// </summary>
    public async Task<List<MstStaff>> GetAllStaffsAsync()
    {
        return await _context.MstStaffs
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// 登録番号で担当者マスタを取得
    /// </summary>
    public async Task<MstStaff?> GetStaffBySerialNoAsync(string serialNo)
    {
        return await _context.MstStaffs
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SerialNo == serialNo);
    }

    /// <summary>
    /// 担当者コードで担当者マスタを取得
    /// </summary>
    public async Task<MstStaff?> GetStaffByStaffCdAsync(string staffCd)
    {
        return await _context.MstStaffs
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StaffCd == staffCd);
    }

    /// <summary>
    /// 担当者名（氏名）で担当者マスタを検索（部分一致）
    /// </summary>
    public async Task<List<MstStaff>> SearchStaffsByNameAsync(string staffName)
    {
        if (string.IsNullOrWhiteSpace(staffName))
        {
            return new List<MstStaff>();
        }

        return await _context.MstStaffs
            .AsNoTracking()
            .Where(s => s.StaffNm != null && s.StaffNm.Contains(staffName))
            .OrderBy(s => s.StaffNm)
            .Take(50) // 最大50件まで
            .ToListAsync();
    }

    #endregion
}

