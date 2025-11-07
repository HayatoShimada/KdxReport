-- KPRO Library 初期化スクリプト（開発・テスト環境用）
--
-- ■■■ 警告 ■■■
-- このスクリプトは開発・テスト環境専用です。
-- 本番環境では既存のSQL Serverに接続するため、このスクリプトは不要です。
--
-- 処理内容:
-- 1. CompanyDB データベースを作成
-- 2. Companies テーブルを作成
-- 3. 読み取り専用ユーザー (kpro_readonly) を作成
--
-- 注意: サンプルデータは挿入されません。
--       テスト用データが必要な場合は、02-insert-sample-data.sql を実行してください。

-- データベース作成
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CompanyDB')
BEGIN
    CREATE DATABASE CompanyDB;
END
GO

USE CompanyDB;
GO

-- Companies テーブル作成
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Companies]') AND type in (N'U'))
BEGIN
    CREATE TABLE Companies (
        company_id INT PRIMARY KEY IDENTITY(1,1),
        company_name NVARCHAR(200) NOT NULL,
        company_code NVARCHAR(50),
        postal_code NVARCHAR(20),
        address NVARCHAR(500),
        phone_number NVARCHAR(20),
        fax_number NVARCHAR(20),
        email NVARCHAR(100),
        remarks NVARCHAR(MAX),
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 DEFAULT GETDATE(),
        is_active BIT DEFAULT 1
    );
END
GO

-- ■■■ 注意 ■■■
-- KPRO_Libraryは本番環境の既存データベースへの接続を想定しています。
-- サンプルデータの挿入は行いません。
-- テスト用のサンプルデータが必要な場合は、02-insert-sample-data.sql を実行してください。

-- 読み取り専用ユーザーの作成（セキュリティのベストプラクティス）
USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = 'kpro_readonly')
BEGIN
    CREATE LOGIN kpro_readonly WITH PASSWORD = 'KproReadOnly123!';
    PRINT '読み取り専用ログイン kpro_readonly を作成しました。';
END
ELSE
BEGIN
    PRINT 'ログイン kpro_readonly は既に存在します。';
END
GO

USE CompanyDB;
GO

IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = 'kpro_readonly')
BEGIN
    CREATE USER kpro_readonly FOR LOGIN kpro_readonly;
    PRINT 'データベースユーザー kpro_readonly を作成しました。';
END
ELSE
BEGIN
    PRINT 'データベースユーザー kpro_readonly は既に存在します。';
END
GO

-- 読み取り権限のみ付与
GRANT SELECT ON SCHEMA::dbo TO kpro_readonly;
GO

PRINT '----------------------------------------';
PRINT 'KPRO Library 初期化完了';
PRINT '----------------------------------------';
PRINT '読み取り専用接続情報:';
PRINT '  User: kpro_readonly';
PRINT '  Password: KproReadOnly123!';
PRINT '';
PRINT 'セキュリティ上、本番環境では必ずパスワードを変更してください。';
PRINT '----------------------------------------';
GO
