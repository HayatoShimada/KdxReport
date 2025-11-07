using KPRO_Library.Models.Generated;
using KPRO_Library.Services;

namespace KdxReport.Services;

/// <summary>
/// 外部DB（KPRO_Library）の会社・取引先情報を取得するサービス（読み取り専用）
/// </summary>
public class CompanyService
{
    private readonly ReadOnlyDataService _readOnlyDataService;

    public CompanyService(ReadOnlyDataService readOnlyDataService)
    {
        _readOnlyDataService = readOnlyDataService;
    }

    /// <summary>
    /// すべての会社（MstCompany）を取得
    /// </summary>
    public async Task<List<MstCompany>> GetAllCompaniesAsync()
    {
        return await _readOnlyDataService.GetAllCompaniesAsync();
    }

    /// <summary>
    /// 会社コードで会社（MstCompany）を取得
    /// </summary>
    public async Task<MstCompany?> GetCompanyByIdAsync(string companyCd)
    {
        return await _readOnlyDataService.GetCompanyByIdAsync(companyCd);
    }

    /// <summary>
    /// すべての取引先（MstCustomer）を取得
    /// </summary>
    public async Task<List<MstCustomer>> GetAllCustomersAsync()
    {
        return await _readOnlyDataService.GetAllCustomersAsync();
    }

    /// <summary>
    /// 取引先コードで取引先（MstCustomer）を取得
    /// </summary>
    public async Task<MstCustomer?> GetCustomerByIdAsync(string customerCd)
    {
        return await _readOnlyDataService.GetCustomerByIdAsync(customerCd);
    }

    /// <summary>
    /// 得意先コードで担当者（MstCustomerContact）一覧を取得
    /// </summary>
    public async Task<List<MstCustomerContact>> GetContactsByCustomerCdAsync(string customerCd)
    {
        return await _readOnlyDataService.GetCustomerContactsByCustomerCdAsync(customerCd);
    }

    /// <summary>
    /// 得意先コードと担当者コードで担当者（MstCustomerContact）を取得
    /// </summary>
    public async Task<MstCustomerContact?> GetContactByIdAsync(string customerCd, string staffCd)
    {
        return await _readOnlyDataService.GetCustomerContactByIdAsync(customerCd, staffCd);
    }
}
