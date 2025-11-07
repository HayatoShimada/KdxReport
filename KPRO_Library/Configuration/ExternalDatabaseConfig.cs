namespace KPRO_Library.Configuration;

/// <summary>
/// 既存SQL Server接続設定（読み取り専用アクセス用）
/// </summary>
public class ExternalDatabaseConfig
{
    /// <summary>
    /// 設定セクション名
    /// </summary>
    public const string SectionName = "ExternalDatabase";

    /// <summary>
    /// SQL Server接続文字列
    /// 例: "Server=localhost;Database=CompanyDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 会社情報テーブル名
    /// デフォルト: "Companies"
    /// </summary>
    public string CompanyTableName { get; set; } = "Companies";

    /// <summary>
    /// カラム名のマッピング設定
    /// キー：ExternalCompanyのプロパティ名、値：DBのカラム名
    /// </summary>
    public Dictionary<string, string> ColumnMappings { get; set; } = new()
    {
        // デフォルトのマッピング（DBカラム名が異なる場合は上書きしてください）
        { "CompanyId", "company_id" },
        { "CompanyName", "company_name" },
        { "CompanyCode", "company_code" },
        { "PostalCode", "postal_code" },
        { "Address", "address" },
        { "PhoneNumber", "phone_number" },
        { "FaxNumber", "fax_number" },
        { "Email", "email" },
        { "Remarks", "remarks" },
        { "CreatedAt", "created_at" },
        { "UpdatedAt", "updated_at" },
        { "IsActive", "is_active" }
    };

    /// <summary>
    /// 接続タイムアウト（秒）
    /// </summary>
    public int CommandTimeout { get; set; } = 30;

    /// <summary>
    /// 削除済みデータを除外するかどうか
    /// </summary>
    public bool ExcludeInactive { get; set; } = true;
}
