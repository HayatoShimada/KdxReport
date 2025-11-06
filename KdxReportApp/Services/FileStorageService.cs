using Minio;
using Minio.DataModel.Args;

namespace KdxReport.Services;

/// <summary>
/// MinIO を利用したファイル保存まわりのユーティリティサービス。
/// アップロード前のバケット存在確認や、ダウンロード・削除・署名付き URL 生成などをまとめています。
/// </summary>
public class FileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileStorageService> _logger;
    private readonly string _bucketName;

    public FileStorageService(
        IMinioClient minioClient,
        IConfiguration configuration,
        ILogger<FileStorageService> logger)
    {
        _minioClient = minioClient;
        _configuration = configuration;
        _logger = logger;
        _bucketName = configuration["MinIO:BucketName"] ?? "kdxreport";
    }

    /// <summary>
    /// 対象バケットが存在しない場合は作成します。
    /// シンプルな初期化処理なのでアプリ起動時やアップロード直前に呼び出します。
    /// </summary>
    /// <exception cref="Exception">MinIO 側でエラーが発生した場合。</exception>
    public async Task EnsureBucketExistsAsync()
    {
        try
        {
            var beArgs = new BucketExistsArgs().WithBucket(_bucketName);
            bool found = await _minioClient.BucketExistsAsync(beArgs);

            if (!found)
            {
                var mbArgs = new MakeBucketArgs().WithBucket(_bucketName);
                await _minioClient.MakeBucketAsync(mbArgs);
                _logger.LogInformation("Bucket {BucketName} created successfully.", _bucketName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ensuring bucket exists.");
            throw;
        }
    }

    /// <summary>
    /// ファイルを MinIO にアップロードし、生成されたオブジェクトキーを返します。
    /// </summary>
    /// <param name="fileStream">アップロード対象のストリーム。</param>
    /// <param name="fileName">原本のファイル名。オブジェクト名生成の一部に使用します。</param>
    /// <param name="contentType">MIME タイプ。</param>
    /// <returns>保存先のオブジェクトキー。</returns>
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            await EnsureBucketExistsAsync();

            var objectName = $"{Guid.NewGuid()}/{fileName}";

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs);

            _logger.LogInformation("File {FileName} uploaded successfully as {ObjectName}.", fileName, objectName);

            return objectName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}.", fileName);
            throw;
        }
    }

    /// <summary>
    /// 指定されたオブジェクトをダウンロードし、メモリストリームとして返します。
    /// </summary>
    /// <param name="objectName">MinIO 上のオブジェクトキー。</param>
    /// <returns>呼び出し側で読み取れる <see cref="Stream"/>。</returns>
    public async Task<Stream> DownloadFileAsync(string objectName)
    {
        try
        {
            var memoryStream = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithCallbackStream((stream) =>
                {
                    stream.CopyTo(memoryStream);
                });

            await _minioClient.GetObjectAsync(getObjectArgs);

            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file {ObjectName}.", objectName);
            throw;
        }
    }

    /// <summary>
    /// オブジェクトを削除します。成功時は true を返します。
    /// </summary>
    /// <param name="objectName">削除対象のオブジェクトキー。</param>
    /// <returns>削除成功時 true、失敗時 false。</returns>
    public async Task<bool> DeleteFileAsync(string objectName)
    {
        try
        {
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName);

            await _minioClient.RemoveObjectAsync(removeObjectArgs);

            _logger.LogInformation("File {ObjectName} deleted successfully.", objectName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {ObjectName}.", objectName);
            return false;
        }
    }

    /// <summary>
    /// 署名付き URL を生成し、期限付きで直接アクセスできるようにします。
    /// </summary>
    /// <param name="objectName">対象オブジェクトキー。</param>
    /// <param name="expiryInSeconds">URL の有効期限（秒）。</param>
    /// <returns>署名付き URL。</returns>
    public async Task<string> GetPresignedUrlAsync(string objectName, int expiryInSeconds = 3600)
    {
        try
        {
            var presignedGetObjectArgs = new PresignedGetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithExpiry(expiryInSeconds);

            string url = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting presigned URL for {ObjectName}.", objectName);
            throw;
        }
    }

    /// <summary>
    /// ファイル名から画像拡張子かどうかを判定します。
    /// </summary>
    /// <param name="fileName">判定対象のファイル名。</param>
    /// <returns>画像拡張子の場合 true。</returns>
    public bool IsImageFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension is ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp";
    }

    /// <summary>
    /// ファイル名から PDF かどうかを判定します。
    /// </summary>
    /// <param name="fileName">判定対象のファイル名。</param>
    /// <returns>PDF の場合 true。</returns>
    public bool IsPdfFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension == ".pdf";
    }

    /// <summary>
    /// アップロード許可されている拡張子かどうかをまとめて判定します。
    /// </summary>
    /// <param name="fileName">判定対象のファイル名。</param>
    /// <returns>許可された拡張子であれば true。</returns>
    public bool IsAllowedFileType(string fileName)
    {
        return IsImageFile(fileName) || IsPdfFile(fileName);
    }
}
