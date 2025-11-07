-- KPRO Library サンプルデータ挿入スクリプト（開発・テスト用）
--
-- ■■■ 警告 ■■■
-- このスクリプトは開発・テスト環境専用です。
-- 本番環境では実行しないでください！
--
-- 使用方法:
-- docker exec -i kpro-sqlserver /opt/mssql-tools/bin/sqlcmd \
--   -S localhost -U sa -P "YourPassword123" \
--   -i /docker-entrypoint-initdb.d/02-insert-sample-data.sql

USE CompanyDB;
GO

-- サンプルデータ挿入
IF NOT EXISTS (SELECT * FROM Companies)
BEGIN
    INSERT INTO Companies (company_name, company_code, postal_code, address, phone_number, fax_number, email, remarks, is_active)
    VALUES
        ('株式会社サンプル商事', 'COMP001', '100-0001', '東京都千代田区千代田1-1-1', '03-1234-5678', '03-1234-5679', 'info@sample-corp.co.jp', '本社', 1),
        ('テスト株式会社', 'COMP002', '530-0001', '大阪府大阪市北区梅田2-2-2', '06-9876-5432', '06-9876-5433', 'contact@test-inc.co.jp', '関西支社', 1),
        ('デモ工業株式会社', 'COMP003', '450-0002', '愛知県名古屋市中村区名駅3-3-3', '052-1111-2222', '052-1111-2223', 'info@demo-industry.co.jp', '中部工場', 1),
        ('サンプル技術株式会社', 'COMP004', '812-0011', '福岡県福岡市博多区博多駅前4-4-4', '092-3333-4444', '092-3333-4445', 'support@sample-tech.co.jp', '九州営業所', 1),
        ('テスト商事株式会社', 'COMP005', '060-0001', '北海道札幌市中央区北1条西5-5-5', '011-5555-6666', '011-5555-6667', 'hello@test-trade.co.jp', '北海道支店', 1),
        ('株式会社開発テスト', 'COMP006', '220-0012', '神奈川県横浜市西区みなとみらい6-6-6', '045-7777-8888', '045-7777-8889', 'dev@test-dev.co.jp', '開発拠点', 1),
        ('株式会社デモシステム', 'COMP007', '460-0008', '愛知県名古屋市中区栄7-7-7', '052-9999-0000', '052-9999-0001', 'demo@demo-sys.co.jp', 'デモセンター', 1),
        ('テストソリューション株式会社', 'COMP008', '810-0001', '福岡県福岡市中央区天神8-8-8', '092-1111-2222', '092-1111-2223', 'info@test-solution.co.jp', 'ソリューション事業部', 1),
        ('株式会社サンプルテック', 'COMP009', '150-0002', '東京都渋谷区渋谷9-9-9', '03-3333-4444', '03-3333-4445', 'contact@sample-tech.co.jp', '技術開発部', 1),
        ('デモ株式会社', 'COMP010', '550-0002', '大阪府大阪市西区江戸堀10-10-10', '06-5555-6666', '06-5555-6667', 'hello@demo-corp.co.jp', '西日本営業所', 1);

    PRINT '✓ サンプルデータ10件を挿入しました。';
    PRINT '';
    PRINT '登録された会社:';
    SELECT company_code, company_name FROM Companies ORDER BY company_id;
END
ELSE
BEGIN
    PRINT '⚠ データが既に存在します。';
    PRINT '現在のデータ件数: ' + CAST((SELECT COUNT(*) FROM Companies) AS NVARCHAR(10)) + '件';
    PRINT '';
    PRINT '既存データを削除してサンプルデータを再挿入する場合は、以下を実行してください:';
    PRINT 'TRUNCATE TABLE Companies;';
END
GO

PRINT '';
PRINT '----------------------------------------';
PRINT 'サンプルデータスクリプト実行完了';
PRINT '----------------------------------------';
GO
