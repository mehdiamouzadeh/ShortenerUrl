using Microsoft.AspNetCore.Mvc;
using ShortenerUrl.Services;

namespace ShortenerUrl.Endpoints;

public static class RedirectEndpoint
{

    public static void MapRedirectEndPoint(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("/{shorten_code}", async (ShortenService service,
            CancellationToken cancellationToken,
            [FromRoute(Name = "shorten_code")] string shortencode) =>
        {
            //validation : xss attack
            var longUrl = await service.GetLongUrlAsync(shortencode, cancellationToken);
            return Results.Redirect(longUrl);

        });
    }

}

