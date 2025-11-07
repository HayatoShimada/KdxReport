using KdxReport.Data;
using KdxReport.Models;
using Microsoft.EntityFrameworkCore;



namespace KdxReport.Services;

/// <summary>
/// 出張報告書に関する読み取り・承認・更新処理をまとめたサービス層。
/// DbContext を経由してレポートの検索や状態更新を行います。
/// </summary>
public class TripReportService
{
    private readonly KdxReportDbContext _context;

    /// <summary>
    /// <see cref="TripReportService"/> のインスタンスを生成します。
    /// </summary>
    /// <param name="context">Entity Framework Core のコンテキスト。</param>
    public TripReportService(KdxReportDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 指定ユーザーが未読の出張報告書一覧を取得します。
    /// </summary>
    /// <param name="userId">未読状況を確認するユーザー ID。</param>
    /// <returns>未読レポートのリスト。</returns>
    public async Task<List<TripReport>> GetUnreadReportsForUserAsync(int userId)
    {
        // Get all trip reports that the user hasn't read yet
        var unreadReports = await _context.TripReports
            .Include(tr => tr.Equipment)
            .Include(tr => tr.Approver)
            .Where(tr => !_context.ReadStatuses.Any(rs => rs.UserId == userId && rs.TripReportId == tr.TripReportId && rs.IsRead))
            .OrderByDescending(tr => tr.CreatedAt)
            .ToListAsync();

        return unreadReports;
    }

    /// <summary>
    /// 承認待ち状態の出張報告書一覧を取得します。
    /// </summary>
    /// <returns>承認待ちレポートのリスト。</returns>
    public async Task<List<TripReport>> GetPendingApprovalReportsAsync()
    {
        return await _context.TripReports
            .Include(tr => tr.Equipment)
            .Where(tr => tr.ApprovalStatus == "pending")
            .OrderByDescending(tr => tr.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// すべての出張報告書を取得します。
    /// </summary>
    /// <returns>登録済みレポートのリスト。</returns>
    public async Task<List<TripReport>> GetAllReportsAsync()
    {
        return await _context.TripReports
            .Include(tr => tr.Equipment)
            .Include(tr => tr.Approver)
            .OrderByDescending(tr => tr.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// 指定ユーザーが提出した出張報告書一覧を取得します。
    /// </summary>
    /// <param name="submitterName">提出者名。</param>
    /// <returns>提出したレポートのリスト。</returns>
    public async Task<List<TripReport>> GetReportsBySubmitterAsync(string submitterName)
    {
        return await _context.TripReports
            .Include(tr => tr.Equipment)
            .Include(tr => tr.Approver)
            .Where(tr => tr.Submitter == submitterName)
            .OrderByDescending(tr => tr.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// 指定された ID の出張報告書を取得し、関連情報も読み込みます。
    /// </summary>
    /// <param name="tripReportId">取得したいレポート ID。</param>
    /// <returns>該当する <see cref="TripReport"/>。存在しない場合は null。</returns>
    public async Task<TripReport?> GetReportByIdAsync(int tripReportId)
    {
        return await _context.TripReports
            .Include(tr => tr.Equipment)
            .Include(tr => tr.Approver)
            .Include(tr => tr.Threads)
                .ThenInclude(t => t.Attachments)
            .Include(tr => tr.Threads)
                .ThenInclude(t => t.Comments)
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(tr => tr.TripReportId == tripReportId);
    }

    /// <summary>
    /// ユーザーがレポートを既読としてマークします。
    /// </summary>
    /// <param name="userId">既読にしたユーザー ID。</param>
    /// <param name="tripReportId">対象レポート ID。</param>
    /// <returns>処理成功時 true。</returns>
    public async Task<bool> MarkAsReadAsync(int userId, int tripReportId)
    {
        var existingStatus = await _context.ReadStatuses
            .FirstOrDefaultAsync(rs => rs.UserId == userId && rs.TripReportId == tripReportId);

        if (existingStatus != null)
        {
            existingStatus.IsRead = true;
            existingStatus.ReadAt = DateTime.UtcNow;
        }
        else
        {
            var readStatus = new ReadStatus
            {
                UserId = userId,
                TripReportId = tripReportId,
                IsRead = true,
                ReadAt = DateTime.UtcNow
            };
            _context.ReadStatuses.Add(readStatus);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 出張報告書を承認済みに更新します。
    /// </summary>
    /// <param name="tripReportId">承認対象のレポート ID。</param>
    /// <param name="approverId">承認したユーザー ID。</param>
    /// <returns>レポートが存在しない場合は false、それ以外は true。</returns>
    public async Task<bool> ApproveReportAsync(int tripReportId, int approverId)
    {
        var report = await _context.TripReports.FindAsync(tripReportId);
        if (report == null)
            return false;

        report.ApprovalStatus = "approved";
        report.ApprovedBy = approverId;
        report.ApprovedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 出張報告書を却下状態に更新します。
    /// </summary>
    /// <param name="tripReportId">却下対象のレポート ID。</param>
    /// <param name="approverId">却下を行ったユーザー ID。</param>
    /// <returns>レポートが存在しない場合は false、それ以外は true。</returns>
    public async Task<bool> RejectReportAsync(int tripReportId, int approverId)
    {
        var report = await _context.TripReports.FindAsync(tripReportId);
        if (report == null)
            return false;

        report.ApprovalStatus = "rejected";
        report.ApprovedBy = approverId;
        report.ApprovedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 新規の出張報告書を追加します。
    /// </summary>
    /// <param name="report">追加するレポート。</param>
    /// <returns>保存された <see cref="TripReport"/>。</returns>
    public async Task<TripReport> CreateReportAsync(TripReport report)
    {
        _context.TripReports.Add(report);
        await _context.SaveChangesAsync();
        return report;
    }

    /// <summary>
    /// 既存の出張報告書を更新します。
    /// </summary>
    /// <param name="report">更新内容を含むレポート。</param>
    /// <returns>常に true（例外発生時は上位へ伝播）。</returns>
    public async Task<bool> UpdateReportAsync(TripReport report)
    {
        _context.TripReports.Update(report);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 出張報告書を削除します。
    /// </summary>
    /// <param name="tripReportId">削除対象のレポート ID。</param>
    /// <returns>レポートが存在しない場合は false、それ以外は true。</returns>
    public async Task<bool> DeleteReportAsync(int tripReportId)
    {
        var report = await _context.TripReports.FindAsync(tripReportId);
        if (report == null)
            return false;

        _context.TripReports.Remove(report);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 出張報告書に添付ファイルを追加します。
    /// スレッドが存在しない場合は自動的に作成されます。
    /// </summary>
    /// <param name="tripReportId">対象のレポート ID。</param>
    /// <param name="attachments">添付ファイル情報のリスト（オブジェクト名、ファイル名、ファイルタイプ、ファイルサイズ）。</param>
    /// <returns>処理成功時 true。</returns>
    public async Task<bool> AddAttachmentsToReportAsync(int tripReportId, List<(string objectName, string fileName, string fileType, long fileSize)> attachments)
    {
        if (attachments.Count == 0)
            return true;

        // レポートに紐づくスレッドを取得、なければ作成
        var thread = await _context.Threads
            .FirstOrDefaultAsync(t => t.TripReportId == tripReportId);

        if (thread == null)
        {
            thread = new Models.Thread
            {
                TripReportId = tripReportId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Threads.Add(thread);
            await _context.SaveChangesAsync();
        }

        // 各ファイルの添付情報を作成
        foreach (var (objectName, fileName, fileType, fileSize) in attachments)
        {
            var attachment = new Attachment
            {
                ThreadId = thread.ThreadId,
                FileType = fileType,
                FilePath = objectName,
                FileName = fileName,
                FileSize = fileSize,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Attachments.Add(attachment);
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
