using Polly;
using Polly.Retry;


var retryStrategy = new RetryStrategyOptions
{
    MaxRetryAttempts = 5,
    Delay = TimeSpan.FromSeconds(5)
};
var pipeline = new ResiliencePipelineBuilder().AddRetry(retryStrategy).Build();

var act = () =>
{
    var httpClint = new HttpClient();
    httpClint.BaseAddress = new Uri("http://localhost:5053");
    var data = httpClint.GetStreamAsync("shorten?long_url=https://virgool.io").GetAwaiter().GetResult();
    Console.WriteLine(data);
};
try{
    pipeline.Execute(act);

}
catch (Exception rx)
{
    throw;
}



