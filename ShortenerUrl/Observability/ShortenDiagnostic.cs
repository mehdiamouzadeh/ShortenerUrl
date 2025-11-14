using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ShortenerUrl.Observability
{
    public sealed class ShortenDiagnostic
    {
        public const string MeterName = "Thisismehdi.Shorten";
        public const string GenerateCode = "Thisismehdi.Shorten.GenerateCode";
        public const string Redirect = "Thisismehdi.Shorten.Redirect";
        public const string FailedRedirect = "Thisismehdi.Shorten.Failed.Redirect";

        private readonly Counter<long> _redirectCounter;
        private readonly Counter<long> _failedRedirectCounter;
        private readonly Counter<long> _shortenCounter;

        public ShortenDiagnostic(IMeterFactory meterFactory)
        {
            var meter = meterFactory.Create(MeterName);
            _redirectCounter = meter.CreateCounter<long>(Redirect);
            _shortenCounter = meter.CreateCounter<long>(GenerateCode);
            _failedRedirectCounter = meter.CreateCounter<long>(FailedRedirect);
        }

        public void AddRedirection(string shortenCode) => _redirectCounter.Add(1, new TagList
        {
            new KeyValuePair<string, object?>("code",shortenCode)
        });
        public void AddFailedRefirection() => _failedRedirectCounter.Add(1);
        public void AddShorten() => _shortenCounter.Add(1);


    }

}
