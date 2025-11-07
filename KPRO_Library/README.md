# KPRO_Library

**既存の本番SQL Server**から会社情報を**読み取り専用**で取得するための独立したC#ライブラリです。

> **重要**:
> - このライブラリは**本番環境の既存データベースへの接続**を想定しています
> - **読み取り専用**です。外部DBへの書き込み、更新、削除は一切行いません
> - データベース側のデータは一切変更しません

## 機能

- SQL Server接続とデータ取得（読み取り専用）
- 柔軟なカラム名マッピング（既存DBのテーブル/カラム構造に対応）
- 自動データマッピング
- 接続テスト機能

## 設計思想

KPRO_Libraryは既存の外部システム（SQL Server）からデータを参照するための読み取り専用ライブラリとして設計されています。

### 想定される利用シーン

- 既存の基幹系システム（販売管理、顧客管理など）の会社マスタを参照
- データを変更せずに既存DBから情報を取得
- 複数のシステム間でマスタデータを共有

### 操作の制限

- ✅ **許可される操作**: SELECT クエリによるデータ取得のみ
- ❌ **禁止される操作**: INSERT, UPDATE, DELETE などのデータ変更
- ❌ **実装されていない機能**: データの登録、更新、削除機能

## 使い方

### 1. NuGetパッケージの追加

メインプロジェクトで以下のコマンドを実行：

```bash
dotnet add reference ../KPRO_Library/KPRO_Library.csproj
```

### 2. appsettings.jsonに設定を追加

```json
{
  "ExternalDatabase": {
    "ConnectionString": "Server=localhost;Database=CompanyDB;User Id=sa;Password=YourPassword123;TrustServerCertificate=true;",
    "CompanyTableName": "Companies",
    "ColumnMappings": {
      "CompanyId": "company_id",
      "CompanyName": "company_name",
      "CompanyCode": "company_code",
      "PostalCode": "postal_code",
      "Address": "address",
      "PhoneNumber": "phone_number",
      "FaxNumber": "fax_number",
      "Email": "email",
      "Remarks": "remarks",
      "CreatedAt": "created_at",
      "UpdatedAt": "updated_at",
      "IsActive": "is_active"
    },
    "CommandTimeout": 30,
    "ExcludeInactive": true
  }
}
```

### 3. 設定のカスタマイズ

#### 接続文字列

既存のSQL Serverに合わせて接続文字列を変更してください：

```
Server=<サーバー名>;Database=<データベース名>;User Id=<ユーザー名>;Password=<パスワード>;TrustServerCertificate=true;
```

Windows認証を使用する場合：

```
Server=<サーバー名>;Database=<データベース名>;Integrated Security=true;TrustServerCertificate=true;
```

#### テーブル名

既存のDBのテーブル名に合わせて変更：

```json
"CompanyTableName": "YourCompanyTableName"
```

#### カラム名マッピング

既存DBのカラム名に合わせてマッピングを変更：

```json
"ColumnMappings": {
  "CompanyId": "your_company_id_column",
  "CompanyName": "your_company_name_column",
  ...
}
```

### 4. サービスの登録（Program.cs）

```csharp
using KPRO_Library.Services;

builder.Services.AddScoped<ExternalCompanyService>();
```

### 5. 使用例

```csharp
public class YourService
{
    private readonly ExternalCompanyService _externalCompanyService;

    public YourService(ExternalCompanyService externalCompanyService)
    {
        _externalCompanyService = externalCompanyService;
    }

    public async Task Example()
    {
        // 接続テスト
        var isConnected = await _externalCompanyService.TestConnectionAsync();
        if (!isConnected)
        {
            throw new Exception("外部DBに接続できません");
        }

        // すべての会社情報を取得
        var companies = await _externalCompanyService.GetAllCompaniesAsync();

        // 特定の会社を取得
        var company = await _externalCompanyService.GetCompanyByIdAsync(1);

        // 会社名で検索
        var searchResults = await _externalCompanyService.SearchCompaniesByNameAsync("株式会社");
    }
}
```

## 既存DBからローカルDBへのマイグレーション例

```csharp
public class CompanyMigrationService
{
    private readonly ExternalCompanyService _externalService;
    private readonly CompanyService _localService;

    public CompanyMigrationService(
        ExternalCompanyService externalService,
        CompanyService localService)
    {
        _externalService = externalService;
        _localService = localService;
    }

    public async Task MigrateCompaniesAsync()
    {
        // 外部DBから会社情報を取得
        var externalCompanies = await _externalService.GetAllCompaniesAsync();

        foreach (var externalCompany in externalCompanies)
        {
            // ローカルDBに存在するか確認
            var existingCompany = await _localService.GetCompanyByNameAsync(externalCompany.CompanyName);

            if (existingCompany == null)
            {
                // 新規作成
                var newCompany = new Company
                {
                    CompanyName = externalCompany.CompanyName
                };
                await _localService.CreateCompanyAsync(newCompany);
            }
        }
    }
}
```

## データベースからモデルクラスを自動生成（推奨）

既存の本番データベース（例: `dbo.MST_CUSTOMER`）から、Entity Frameworkのスキャフォールディング機能を使ってモデルクラスを自動生成できます。

### 方法1: スクリプトを使用（推奨）

1. **`scaffold-from-database.sh` を編集**

   本番環境の接続情報とテーブル名を設定してください：

   ```bash
   # 本番環境の接続文字列
   CONNECTION_STRING="Server=192.168.100.191;Database=CompanyDB;User Id=readonly_user;Password=Pass;TrustServerCertificate=true;"

   # スキャフォールド対象のテーブル（カンマ区切りで複数指定可能）
   TABLES="MST_CUSTOMER"
   ```

2. **スクリプトを実行**

   ```bash
   cd KPRO_Library
   ./scaffold-from-database.sh
   ```

3. **生成されたファイルを確認**

   以下のファイルが自動生成されます：
   - `Data/CompanyDbContext.cs` - Entity Framework DbContext
   - `Models/Generated/MstCustomer.cs` - MST_CUSTOMERテーブルのモデルクラス

### 方法2: 手動でコマンド実行

```bash
cd KPRO_Library

dotnet ef dbcontext scaffold \
  "Server=your-server;Database=CompanyDB;User Id=readonly_user;Password=Pass;TrustServerCertificate=true;" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --output-dir Models/Generated \
  --context-dir Data \
  --context CompanyDbContext \
  --table MST_CUSTOMER \
  --force \
  --no-onconfiguring
```

### 生成後の確認事項

1. **モデルクラスの確認**
   - プロパティ名がC#の命名規則に従っているか
   - null許容型が正しく設定されているか
   - ✅ 完了: MstCustomer.cs が生成されました

2. **appsettings.jsonに接続文字列を追加**

   ```json
   {
     "ConnectionStrings": {
       "ExternalDatabase": "Server=192.168.100.191;Database=KANAMORI_KPRO3;User Id=readonly_user;Password=Pass;TrustServerCertificate=true;"
     }
   }
   ```

3. **DbContextの登録（Program.cs）**

   ```csharp
   using KPRO_Library.Data;
   using Microsoft.EntityFrameworkCore;

   // KPRO_Library - CompanyDbContext の登録
   builder.Services.AddDbContext<CompanyDbContext>(options =>
       options.UseSqlServer(
           builder.Configuration.GetConnectionString("ExternalDatabase"),
           sqlOptions => sqlOptions.CommandTimeout(30))
       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)); // 読み取り専用のため追跡無効化
   ```

4. **CustomerServiceの登録（Program.cs）**

   ```csharp
   using KPRO_Library.Services;

   // KPRO_Library - CustomerService の登録
   builder.Services.AddScoped<CustomerService>();
   ```

5. **使用例**

   ```csharp
   @inject CustomerService CustomerService

   @code {
       private List<MstCustomer> customers = new();

       protected override async Task OnInitializedAsync()
       {
           // すべての取引先を取得
           customers = await CustomerService.GetAllCustomersAsync();

           // 会社名で検索
           var searchResults = await CustomerService.SearchCustomersByNameAsync("株式会社");

           // 取引先コードで取得
           var customer = await CustomerService.GetCustomerByCodeAsync("0001");
       }
   }
   ```

> **注意**: スキャフォールドは既存ファイルを上書きします（`--force`オプション）。必要に応じてバックアップを取ってください。

### 生成されたサービス

`Services/CustomerService.cs` が自動作成されています。以下のメソッドが利用可能です：

- `GetAllCustomersAsync()` - すべての取引先を取得
- `GetCustomerByCodeAsync(string customerCd)` - コードで検索
- `SearchCustomersByNameAsync(string searchTerm)` - 名前で検索
- `GetCustomersByCodesAsync(List<string> codes)` - 複数コード検索
- `GetCustomersByPostalCodeAsync(string postalCode)` - 郵便番号で検索
- `GetCustomerCountAsync()` - 取引先数を取得
- `TestConnectionAsync()` - 接続テスト

## 本番環境での使用

KPRO_Libraryは**既存の本番データベースに接続することを想定**しています。

### 接続手順（手動マッピングを使用する場合）

手動でモデルクラスを作成し、カラムマッピングを設定する場合の手順です。

1. **既存データベースの会社情報テーブルを確認**

   既存のSQL Serverに会社情報が格納されているテーブルを確認してください。

2. **appsettings.jsonで接続設定**

   既存データベースのテーブル名とカラム名に合わせて設定を調整してください：

   ```json
   {
     "ExternalDatabase": {
       "ConnectionString": "Server=your-server;Database=YourDB;User Id=readonly_user;Password=SecurePassword;TrustServerCertificate=true;",
       "CompanyTableName": "MST_CUSTOMER",
       "ColumnMappings": {
         "CompanyId": "customer_id",
         "CompanyName": "customer_name",
         ...
       }
     }
   }
   ```

3. **接続テスト**

   ```csharp
   var isConnected = await _externalCompanyService.TestConnectionAsync();
   ```

> **重要**: KPRO_Libraryは読み取り専用です。既存データベースのデータを変更することはありません。

> **推奨**: 手動マッピングよりも、EFスキャフォールディングでモデルを自動生成する方が、型安全で保守性が高くなります。

## Docker Composeでの使用（開発・テスト環境専用）

ローカル開発・テスト用にSQL Serverコンテナが用意されています。

> ⚠️ **警告**: 本番環境ではDockerコンテナではなく、既存のSQL Serverに接続してください。

### 1. SQL Serverコンテナの起動

```bash
docker-compose up -d sqlserver
```

### 2. データベースの初期化（テーブルとユーザーのみ作成）

```bash
./kpro-init/init-kpro-db.sh
```

または、手動で実行する場合：

```bash
docker exec -i kpro-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword123" \
  -i /docker-entrypoint-initdb.d/01-init-db.sql
```

**データベースの初期化では、テーブル構造と読み取り専用ユーザーのみを作成します。サンプルデータは挿入されません。**

### 3. サンプルデータの挿入（オプション - 開発・テスト用のみ）

テスト用にサンプル会社データが必要な場合のみ、以下を実行してください：

```bash
docker exec -i kpro-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword123" \
  -i /docker-entrypoint-initdb.d/02-insert-sample-data.sql
```

### 4. 接続確認

**読み取り専用ユーザー（推奨）:**
- **Server**: localhost:1433
- **Database**: CompanyDB
- **User**: kpro_readonly
- **Password**: KproReadOnly123!

**管理者ユーザー（テーブル作成/デバッグ用）:**
- **Server**: localhost:1433
- **Database**: CompanyDB
- **User**: sa
- **Password**: YourPassword123

> **セキュリティ**: テスト環境でも読み取り専用ユーザー（kpro_readonly）の使用を推奨します。

### Docker環境での設定

docker-compose.ymlのappサービス環境変数で接続文字列が設定されています：

```yaml
ExternalDatabase__ConnectionString: "Server=sqlserver;Database=CompanyDB;User Id=sa;Password=YourPassword123;TrustServerCertificate=true;"
```

注：コンテナ間通信では `Server=sqlserver` を使用しますが、ホストマシンから接続する場合は `Server=localhost` を使用します。

## セキュリティのベストプラクティス

KPRO_Libraryは読み取り専用として設計されていますが、データベース側でもセキュリティを強化することを強く推奨します。

### 推奨事項

1. **読み取り専用ユーザーを作成**

   外部SQLServerには、読み取り専用権限を持つ専用ユーザーを作成してください：

   ```sql
   -- 読み取り専用ユーザーの作成例
   CREATE LOGIN kpro_readonly WITH PASSWORD = 'SecurePassword123';
   USE CompanyDB;
   CREATE USER kpro_readonly FOR LOGIN kpro_readonly;

   -- 読み取り権限のみ付与
   GRANT SELECT ON SCHEMA::dbo TO kpro_readonly;
   ```

2. **appsettings.jsonで読み取り専用ユーザーを使用**

   ```json
   {
     "ExternalDatabase": {
       "ConnectionString": "Server=localhost;Database=CompanyDB;User Id=kpro_readonly;Password=SecurePassword123;TrustServerCertificate=true;"
     }
   }
   ```

3. **本番環境では環境変数または Secret Manager を使用**

   接続文字列をソースコードにハードコーディングせず、環境変数やAzure Key Vaultなどで管理してください。

### なぜ読み取り専用ユーザーを使用すべきか

- ライブラリにバグがあった場合でもデータ破損を防止
- 不正アクセスがあった場合の被害を最小限に抑制
- コンプライアンス要件（データ保護規制）への対応
- 監査ログの明確化（読み取り専用ユーザーのアクティビティを追跡）

## トラブルシューティング

### 接続できない場合

1. SQL Serverが起動しているか確認
2. ファイアウォール設定を確認
3. SQL Server認証が有効か確認
4. 接続文字列が正しいか確認

### カラムが見つからない場合

`ColumnMappings` の設定を既存DBのカラム名に合わせて調整してください。

## ライセンス

このライブラリはプロジェクト内部で使用するためのものです。
