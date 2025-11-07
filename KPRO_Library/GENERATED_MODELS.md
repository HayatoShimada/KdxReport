# 生成されたモデルクラス一覧

本番データベース `KANAMORI_KPRO3` から自動生成されたモデルクラスの一覧です。

## 生成統計

- **モデルクラス数**: 147個
- **総行数**: 約15,600行
- **DbContext行数**: 17,333行

## テーブル分類

### AspNet系（ASP.NET Identity）
- AspNetRole
- AspNetRoleClaim
- AspNetUser
- AspNetUserClaim
- AspNetUserLogin
- AspNetUserToken

### Dat系（トランザクションデータ）

#### 請求・入金
- DatBilling（請求）
- DatPayment（入金）

#### 見積・受注
- DatEstimate（見積）
- DatEstimateComposition（見積構成）
- DatEstimateDetail（見積明細）
- DatOrder（受注）
- DatOrderCategory（受注カテゴリ）
- DatOrderCategory2（受注カテゴリ2）
- DatOrderComposition（受注構成）
- DatOrderDetail（受注明細）

#### 出荷・販売
- DatShipping（出荷）
- DatShippingDetail（出荷明細）
- DatShippingList（出荷リスト）
- DatShippingPrint（出荷印刷）
- DatSale（売上）

#### 購買
- DatPurchaseDetail（仕入明細）
- DatPurchaseSplit（仕入分割）

#### 日報・工程
- DatDailyReport（日報）
- DatWorkreport（作業報告）
- DatProcessPlan（工程計画）
- DatPostingInformation（投入情報）
- DatPostingStaff（投入担当者）

#### システム
- DatSystemLock（システムロック）

### Elk系（不具合管理）
- ElkDefect（不具合）
- ElkDefectStatus（不具合ステータス）
- ElkDefectThread（不具合スレッド）

### Hst系（履歴）
- HstBug（バグ履歴）
- HstCost（原価履歴）
- HstItem（品目履歴）
- HstMemo（メモ履歴）
- HstModel（モデル履歴）
- HstProductionNumber（製造番号履歴）
- HstSellingPrice（販売価格履歴）
- HstStock（在庫履歴）

### Mst系（マスタ）

#### 顧客・取引先
- **MstCustomer**（取引先マスタ）⭐ 主要
- MstCustomerContact（取引先担当者）
- MstContact（連絡先）

#### 仕入先
- **MstSupplier**（仕入先マスタ）⭐ 主要
- MstSuppliers2（仕入先2）
- MstSuppliersContact（仕入先担当者）
- MstPartner（協力業者）

#### 品目
- **MstItem**（品目マスタ）⭐ 主要
- MstItemEtc（品目その他）
- MstItemComposition（品目構成）

#### 組織・人員
- **MstStaff**（社員マスタ）⭐ 主要
- MstDepartment（部門）
- MstCompany（会社）

#### 工程・製造
- MstProcess（工程）
- MstProcessPlan（工程計画）
- MstDailyreport（日報マスタ）
- MstDailyreportComposition（日報構成）
- MstDailytime（日報時間）

#### 分類・コード
- MstClass（分類）
- MstCode（コード）
- MstMajorClass（大分類）
- MstMiddleClass（中分類）
- MstSmallClass（小分類）

#### その他
- MstCalendar（カレンダー）
- MstMenu（メニュー）
- MstMenuControl（メニュー制御）
- MstSystemControl（システム制御）

### Vw系（ビュー - 読み取り専用）

#### 顧客・仕入先
- VwCustomerList（顧客一覧）
- VwCustomerRoulse（顧客連携）
- VwSuppliersList（仕入先一覧）
- VwSupplierRoulse（仕入先連携）

#### 見積・受注
- VwEstimateList（見積一覧）
- VwEstimateDetail（見積明細）
- VwOrderPending（受注保留）

#### 出荷・梱包
- VwShippingList（出荷一覧）
- VwShippingDatum（出荷データ）
- VwPackingList（梱包リスト）
- VwPackingListZp（梱包リストZP）

#### 請求・入金
- VwBillingList（請求一覧）
- VwPaymentList（入金一覧）
- VwPaymentList2（入金一覧2）

#### 購買・材料
- VwPurchaseList（仕入一覧）
- VwPurchaseList2（仕入一覧2）
- VwPurchaseList3（仕入一覧3）
- VwPurchaseDetail（仕入明細）
- VwMaterualList（材料リスト）

#### 部品表
- VwPartsList（部品表一覧）
- VwPartsList2（部品表一覧2）
- VwPartsList3（部品表一覧3）
- VwPartsDetail（部品明細）
- VwPartsDetails2-7（部品明細2〜7）

#### 製造・加工
- VwProductList（製品一覧）
- VwProductList2（製品一覧2）
- VwMachiningList（加工一覧）
- VwMachiningListExcluded（加工一覧除外）
- VwMachiningListManufactureing（加工一覧製造中）
- VwMachiningListProcessing（加工一覧加工中）
- VwMachiningListProgress（加工一覧進捗）

#### 日報・工程
- VwDailyreportList（日報一覧）
- VwDailyreportAchievement（日報達成）
- VwDailyreportAchievementDetail（日報達成明細）
- VwDailyreportProduct（日報製品）
- VwDailyreportResult（日報実績）
- VwDailyreportResults2（日報実績2）
- VwProcessRoulse（工程連携）

#### その他
- VwStaffList（社員一覧）
- VwDepartmentList（部門一覧）
- VwItemComposition（品目構成）

### 日本語テーブル名

- Kkm日報集計（KKM日報集計）
- Zp梱包出荷一覧（ZP梱包出荷一覧）
- 仕入先マスタ
- 作業指示未完了リスト
- 作業日報一覧
- 入金履歴一覧表
- 加工用工数情報
- 原価計算
- 原価集計
- 取引先連携一覧
- 受注情報
- 工程別工数集計表
- 工程情報一覧
- 得意先マスタ
- 担当者マスタ
- 材料納期リスト
- 梱包ラベル作成
- 社内工数集計表
- 組立工数集計
- 組立進捗率
- 製缶加工計画
- 製缶用工数情報
- 部品表情報
- 部品表情報tech

### その他テーブル

- PartsListTech（部品表Tech）
- PartsListTechs1（部品表Techs1）
- PartsListTechsBk（部品表Techsバックアップ）
- PartsListTechsTmp（部品表Techs一時）
- PartsListTechsUp（部品表Techs更新）
- TmpPurchaseList3（仕入一覧一時3）
- XapnmxMst（APNMマスタ）
- XlogxDat（ログデータ）
- XlogxDat1（ログデータ1）
- XnamexMst（名前マスタ）

## 使用方法

すべてのテーブルは `CompanyDbContext` 経由でアクセスできます：

```csharp
@inject CompanyDbContext dbContext

// 取引先マスタを取得
var customers = await dbContext.MstCustomers
    .Where(c => c.DeleteFlg == "0")
    .ToListAsync();

// 受注データを取得
var orders = await dbContext.DatOrders
    .Include(o => o.DatOrderDetails)
    .ToListAsync();

// ビューから集計データを取得
var billingList = await dbContext.VwBillingLists
    .ToListAsync();
```

## 注意事項

- ⚠️ これらのモデルは**読み取り専用**として使用してください
- ⚠️ 本番データベースへの書き込みは推奨されません
- ⚠️ ビュー（Vw系）は読み取り専用です（INSERT/UPDATE/DELETE不可）
- ⚠️ 日本語テーブル名は、C#の識別子として適切に変換されています
