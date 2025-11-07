#!/bin/bash

# KPRO_Library - 既存データベースからモデルクラスを自動生成するスクリプト
#
# 使用方法:
# 1. 本番環境のSQL Server接続情報を設定
# 2. このスクリプトを実行: ./scaffold-from-database.sh
#
# 注意:
# - 既存のモデルクラスは上書きされます（--forceオプション）
# - 必要に応じて接続文字列とテーブル名を変更してください

echo "=========================================="
echo "KPRO_Library - Database Scaffolding"
echo "=========================================="
echo ""

# ========================================
# 設定: 本番環境の接続情報を設定してください
# ========================================

# 本番環境の接続文字列（例）
CONNECTION_STRING="Server=localhost;Database=CompanyDB;User Id=sa;Password=YourPassword123;TrustServerCertificate=true;"

# スキャフォールド対象のテーブル（カンマ区切りで複数指定可能）
TABLES="MST_CUSTOMER"

# DbContextクラス名
CONTEXT_NAME="CompanyDbContext"

# 出力ディレクトリ
OUTPUT_DIR="Models/Generated"
CONTEXT_DIR="Data"

# ========================================
# スキャフォールド実行
# ========================================

echo "接続先: $CONNECTION_STRING"
echo "対象テーブル: $TABLES"
echo ""
echo "モデルクラスを生成中..."
echo ""

# KPRO_Libraryプロジェクトのディレクトリに移動
cd "$(dirname "$0")"

# EF Core Scaffold コマンドを実行
dotnet ef dbcontext scaffold "$CONNECTION_STRING" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --project KPRO_Library.csproj \
  --output-dir "$OUTPUT_DIR" \
  --context-dir "$CONTEXT_DIR" \
  --context "$CONTEXT_NAME" \
  --table "$TABLES" \
  --force \
  --no-onconfiguring

if [ $? -eq 0 ]; then
    echo ""
    echo "=========================================="
    echo "✓ モデルクラスの生成が完了しました"
    echo "=========================================="
    echo ""
    echo "生成されたファイル:"
    echo "  - $CONTEXT_DIR/$CONTEXT_NAME.cs (DbContext)"
    echo "  - $OUTPUT_DIR/*.cs (モデルクラス)"
    echo ""
    echo "次の手順:"
    echo "1. 生成されたモデルクラスを確認"
    echo "2. 必要に応じてプロパティ名やアノテーションを調整"
    echo "3. DbContextをDIコンテナに登録（Program.cs）"
    echo "=========================================="
else
    echo ""
    echo "✗ モデルクラスの生成に失敗しました"
    echo ""
    echo "トラブルシューティング:"
    echo "1. 接続文字列が正しいか確認"
    echo "2. SQL Serverが起動しているか確認"
    echo "3. テーブル名が正しいか確認（dbo.MST_CUSTOMER）"
    echo "4. データベースユーザーに読み取り権限があるか確認"
    exit 1
fi
