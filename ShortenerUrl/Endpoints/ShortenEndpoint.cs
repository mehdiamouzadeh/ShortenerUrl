using Microsoft.AspNetCore.Mvc;
using ShortenerUrl.Services;

namespace ShortenerUrl.Endpoints;

public static class ShortenEndpoint
{

    public static void MapShortenEndPoint(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("/Shorten", async (ShortenService service,
            CancellationToken cancellationToken,
            [FromQuery(Name = "long_url")] string Url) =>
        {
            //validation : xss attack
            var shortenUrl = await service.ShortenUrAsyncl(Url, cancellationToken);
            return Results.Ok(shortenUrl);

        });
    }

}

