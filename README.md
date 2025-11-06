# 出張報告書提出システム (KdxReport)

Blazor Server Side で構築された出張報告書管理システムです。

## 技術スタック

- **フロントエンド**: Blazor Server Side, Bootstrap 5, FontAwesome
- **バックエンド**: ASP.NET Core 8.0, C#
- **データベース**: PostgreSQL 16
- **ORM**: Entity Framework Core
- **ストレージ**: MinIO S3 (将来的にAWS S3に移行予定)
- **認証**: Cookie Authentication with BCrypt
- **その他**: Markdig (Markdown), Minio SDK

## 主要機能

### ✅ 実装済み

- **認証システム**
  - ログイン/ログアウト
  - パスワード変更（初回ログイン時の強制変更）
  - BCryptによるパスワードハッシュ化
  - ロールベースアクセス制御（Admin/User）

- **既読管理機能**
  - ユーザーごとの報告書既読状態の追跡
  - 未読報告書の自動表示

- **上長承認機能**
  - 承認/却下ワークフロー
  - 承認ステータス管理（pending/approved/rejected）
  - 承認履歴の記録

- **ダッシュボード**
  - 未読報告書一覧
  - 承認待ち報告書（管理者向け）
  - 統計カード表示

- **データベースモデル**
  - ユーザー管理
  - 会社・担当者管理
  - 設備管理
  - 出張報告書
  - スレッド・コメント
  - 添付ファイル
  - 既読ステータス

### 🚧 実装予定

- 報告書詳細ページ
- 報告書作成・編集機能
- 会社管理画面
- 設備管理画面
- コメント・スレッド機能
- ファイルアップロード（MinIO S3統合）
- Markdownレンダリング
- OCR検索機能（Tesseract）

## セットアップ

### 前提条件

- .NET 8.0 SDK
- Docker Desktop (WSL2統合が有効)
- PostgreSQL 16 (またはDocker経由)
- MinIO (またはDocker経由)

### 1. Docker Desktop のWSL統合を有効化

1. Docker Desktop を開く
2. **Settings** > **Resources** > **WSL Integration**
3. 使用しているWSLディストリビューションを有効化
4. **Apply & Restart**

### 2. データベースとストレージの起動

```bash
cd /home/amdet/KdxReport
docker-compose up -d postgres minio
```

コンテナの状態確認:
```bash
docker ps
```

### 3. データベースの初期化

アプリケーション起動時に自動的にマイグレーションが実行されます。

手動でマイグレーションを実行する場合:
```bash
cd KdxReportApp
dotnet ef database update
```

### 4. アプリケーションの起動

```bash
cd KdxReportApp
dotnet run
```

アプリケーションは `http://localhost:5000` で起動します。

## 初期ユーザー

初期ユーザーを作成するには、ユーザー登録機能を実装するか、データベースに直接挿入します。

### データベースに直接ユーザーを作成

```bash
# PostgreSQLコンテナに接続
docker exec -it kdxreport-postgres psql -U kdxuser -d kdxreport

# 管理者ユーザーを作成（パスワード: 123456）
INSERT INTO users (user_name, email, password, created_at, updated_at)
VALUES ('管理者', 'admin@example.com', '$2a$11$YourBCryptHashHere', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

# Admin ロールを割り当て
INSERT INTO role_users (role_id, user_id, created_at, updated_at)
VALUES (1, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
```

**注意**: BCryptハッシュは実際のアプリケーションで生成してください。

## 開発

### プロジェクト構造

```
KdxReportApp/
├── Components/
│   ├── Layout/          # レイアウトコンポーネント
│   └── Pages/           # ページコンポーネント
├── Data/
│   └── KdxReportDbContext.cs  # EF Core DbContext
├── Models/              # エンティティモデル
├── Services/            # ビジネスロジック
├── Migrations/          # EF Core マイグレーション
├── wwwroot/             # 静的ファイル
├── Program.cs           # アプリケーションエントリーポイント
└── appsettings.json     # 設定ファイル
```

### データベース接続

接続文字列は `appsettings.json` で設定:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=kdxreport;Username=kdxuser;Password=kdxpassword;Timezone=Asia/Tokyo"
  }
}
```

### MinIO設定

MinIOの接続設定:

```json
{
  "MinIO": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "BucketName": "kdxreport",
    "UseSSL": false
  }
}
```

MinIO管理コンソール: http://localhost:9001

## Docker Compose サービス

- **postgres**: PostgreSQL 16 (ポート 5432)
- **minio**: MinIO S3互換ストレージ (ポート 9000, 9001)
- **app**: Blazorアプリケーション (ポート 8080)

### 個別サービスの起動

```bash
# PostgreSQLのみ起動
docker-compose up -d postgres

# MinIOのみ起動
docker-compose up -d minio

# アプリケーションコンテナも含めて全て起動
docker-compose up -d
```

### ログの確認

```bash
# 全サービスのログ
docker-compose logs -f

# 特定サービスのログ
docker-compose logs -f postgres
docker-compose logs -f app
```

### サービスの停止

```bash
# 全サービス停止
docker-compose down

# データも削除
docker-compose down -v
```

## トラブルシューティング

### データベース接続エラー

PostgreSQLコンテナが起動しているか確認:
```bash
docker ps | grep postgres
```

PostgreSQLのログを確認:
```bash
docker-compose logs postgres
```

### マイグレーションエラー

既存のマイグレーションを削除して再作成:
```bash
cd KdxReportApp
rm -rf Migrations/
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### ポート競合

他のアプリケーションが同じポートを使用している場合、`docker-compose.yml` のポート設定を変更してください。

## タイムゾーン

本システムは日本標準時（JST, Asia/Tokyo）を使用しています。全ての日時データはUTCで保存され、表示時にJSTに変換されます。

## ライセンス

Kanamori-System.co.jp

## サポート

問題が発生した場合は、Issuesページで報告してください。
