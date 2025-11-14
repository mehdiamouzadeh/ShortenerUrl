using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ShortenerUrl.Infrastructure;
using ShortenerUrl.Models;
using ShortenerUrl.Observability;
using System.Security.Cryptography;
using System.Text;

namespace ShortenerUrl.Services;

public class MetricsDo
{
    public int ShortenCode = 0;
    public int Redirect = 0;

}

public class ShortenService(
    ShortenUrlDbContext dbContext,
    MetricsDo metricsDo,
    ShortenDiagnostic shortenDiagnostic,
    IConfiguration configuration)
{
    private readonly ShortenUrlDbContext _dbContext = dbContext;
    private readonly IConfiguration _configuration = configuration;
    private readonly ShortenDiagnostic _shortenDiagnostic = shortenDiagnostic;
    public (int ShortCount, int RedirectCount) GetMetrics => new(metricsDo.ShortenCode, metricsDo.Redirect);
    public async Task<string> ShortenUrAsyncl(string longUrl, CancellationToken cancellationToken)
    {
        _shortenDiagnostic.AddShorten();
        var url = await _dbContext.UrlTags.FirstOrDefaultAsync(x => x.DestinationUrl == longUrl, cancellationToken);
        if (url is not null)
            return GetServiceUrl(url.ShortenCode);
        var urlTag = new UrlTag
        {
            CreatedOn = DateTime.Now,
            DestinationUrl = longUrl,
            ShortenCode = await GenerateShortenCodeAsync(longUrl),
        };
        _dbContext.UrlTags.Add(urlTag);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return GetServiceUrl(urlTag.ShortenCode);
    }
    private string GetServiceUrl(string shortenCode)
        => $"{_configuration["BaseUrl"]}/{shortenCode}";
    private async Task<string> GenerateShortenCodeAsync(string longUrl)
    {
        using MD5 md5 = MD5.Create();

        // 1️⃣ تولید هش از URL
        byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(longUrl));

        // 2️⃣ تبدیل هش به رشته هگزادسیمال
        string hashCode = BitConverter.ToString(hashBytes)
            .Replace("-", "")
            .ToLower();

        // 3️⃣ بررسی چند بخش مختلف از هش تا تکراری نباشد
        for (int i = 0; i <= 7; i++)
        {
            // از موقعیت i به بعد 7 کاراکتر جدا می‌کنیم
            string candidateCode = hashCode.Substring(i, 7);

            // بررسی در دیتابیس: آیا این کد قبلاً استفاده شده؟
            if (await _dbContext.UrlTags.AnyAsync(x => x.ShortenCode == candidateCode))
                continue; // اگر تکراری بود، برو سراغ بعدی

            // اگر تکراری نبود، همون رو برگردون
            return candidateCode;
        }

        // اگر هیچ کد یکتایی پیدا نشد
        throw new Exception("Invalid shorten code generating.");
    }

    public async Task<string> GetLongUrlAsync(string shortencode, CancellationToken cancellationToken)
    {
        var url = await _dbContext.UrlTags.FirstOrDefaultAsync(x => x.ShortenCode == shortencode, cancellationToken);
        if (url is not null)
        {
            _shortenDiagnostic.AddRedirection(shortencode);
            return url.DestinationUrl;
        }
        _shortenDiagnostic.AddFailedRefirection();
        throw new Exception("Invalid shorten code.");

    }
}


