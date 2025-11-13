using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;

namespace ShortenerUrl.Models;



[Collection("UrlTags")]
public class UrlTag
{
    public ObjectId Id { get; set; }
    public required string ShortenCode { get; set; }
    public required string DestinationUrl { get; set; }
    public DateTime CreatedOn { get; set; }

}

