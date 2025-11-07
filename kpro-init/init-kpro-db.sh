#!/bin/bash

# KPRO Library データベース初期化スクリプト（開発・テスト環境用）
# SQL Server コンテナ起動後、このスクリプトを実行してデータベースを初期化します
#
# ■■■ 警告 ■■■
# このスクリプトは開発・テスト環境専用です。
# 本番環境では既存のSQL Serverに接続してください。

echo "=========================================="
echo "KPRO Library データベース初期化"
echo "（開発・テスト環境用）"
echo "=========================================="
echo ""

# SQL Serverの起動を待つ
echo "SQL Serverの起動を待機中..."
sleep 30

# SQL スクリプトを実行
echo "データベースとテーブル、読み取り専用ユーザーを作成中..."
echo "（サンプルデータは挿入されません）"
echo ""
docker exec -i kpro-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword123" \
  -i /docker-entrypoint-initdb.d/01-init-db.sql

if [ $? -eq 0 ]; then
    echo ""
    echo "=========================================="
    echo "✓ データベース初期化完了"
    echo "=========================================="
    echo ""
    echo "読み取り専用接続情報（推奨）:"
    echo "  Server: localhost:1433"
    echo "  Database: CompanyDB"
    echo "  User: kpro_readonly"
    echo "  Password: KproReadOnly123!"
    echo ""
    echo "管理者接続情報（テーブル作成/デバッグ用）:"
    echo "  Server: localhost:1433"
    echo "  Database: CompanyDB"
    echo "  User: sa"
    echo "  Password: YourPassword123"
    echo ""
    echo "----------------------------------------"
    echo "注意: サンプルデータは挿入されていません。"
    echo "テスト用にサンプルデータが必要な場合は、"
    echo "以下のコマンドを実行してください:"
    echo ""
    echo "docker exec -i kpro-sqlserver /opt/mssql-tools/bin/sqlcmd \\"
    echo "  -S localhost -U sa -P \"YourPassword123\" \\"
    echo "  -i /docker-entrypoint-initdb.d/02-insert-sample-data.sql"
    echo "=========================================="
else
    echo "✗ データベースの初期化に失敗しました。"
    exit 1
fi
