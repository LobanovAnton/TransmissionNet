using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace TransmissionNet.Services;

public partial class KinoPoiskService(string apiKey) : IMovieService
{
    private static readonly HttpClient Client = new() { BaseAddress = new Uri("https://api.poiskkino.dev/") };

    public async Task<MovieInfo?> SearchAsync(string torrentName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(apiKey))
            return null;

        string query = CleanTorrentName(torrentName);
        if (string.IsNullOrEmpty(query))
            return null;

        try
        {
            // Search by name to get movie ID
            KpSearchResponse? search = await SendAsync<KpSearchResponse>(
                $"v1.4/movie/search?query={Uri.EscapeDataString(query)}&limit=1", cancellationToken);

            int? movieId = search?.Docs?.FirstOrDefault()?.Id;
            if (movieId == null)
                return null;

            // Get full movie details with persons
            KpMovie? movie = await SendAsync<KpMovie>($"v1.4/movie/{movieId}", cancellationToken);
            if (movie == null)
                return null;

            return new MovieInfo
            {
                Title = movie.Name ?? movie.AlternativeName ?? "",
                Year = movie.Year?.ToString() ?? "",
                Genres = movie.Genres?.Select(g => g.Name ?? "").Where(n => n.Length > 0).ToArray() ?? [],
                RatingKp = movie.Rating?.Kp?.ToString("0.0") ?? "",
                RatingImdb = movie.Rating?.Imdb?.ToString("0.0") ?? "",
                Description = movie.Description ?? movie.ShortDescription ?? "",
                Director = movie.Persons?.FirstOrDefault(p => p.EnProfession == "director")?.Name ?? "",
                DirectorPhotoUrl = movie.Persons?.FirstOrDefault(p => p.EnProfession == "director")?.Photo,
                PosterUrl = movie.Poster?.PreviewUrl ?? movie.Poster?.Url,
                Actors = movie.Persons?
                    .Where(p => p.EnProfession == "actor")
                    .Take(10)
                    .Select(p => new ActorInfo
                    {
                        Name = p.Name ?? p.EnName ?? "",
                        Role = p.Description ?? "",
                        PhotoUrl = p.Photo
                    }).ToArray() ?? []
            };
        }
        catch (OperationCanceledException) { throw; }
        catch { return null; }
    }

    private async Task<T?> SendAsync<T>(string url, CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, url);
        request.Headers.Add("X-API-KEY", apiKey);

        using HttpResponseMessage response = await Client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return default;

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    private static string CleanTorrentName(string name)
    {
        string clean = name;
        clean = ExtensionRegex().Replace(clean, "");
        clean = TagsRegex().Replace(clean, " ");
        clean = SeparatorRegex().Replace(clean, " ");
        Match yearMatch = YearRegex().Match(clean);
        if (yearMatch.Success)
            clean = clean[..yearMatch.Index];
        clean = SpacesRegex().Replace(clean, " ");
        return clean.Trim();
    }

    [GeneratedRegex(@"\.(torrent|mkv|avi|mp4)$", RegexOptions.IgnoreCase)]
    private static partial Regex ExtensionRegex();

    [GeneratedRegex(@"\b(720p|1080p|2160p|4K|HDRip|BDRip|WEB-?DL|BluRay|x264|x265|HEVC|AAC|DTS|Atmos|HDR|10bit|Rip|Remux)\b", RegexOptions.IgnoreCase)]
    private static partial Regex TagsRegex();

    [GeneratedRegex(@"[._]")]
    private static partial Regex SeparatorRegex();

    [GeneratedRegex(@"\b(19|20)\d{2}\b")]
    private static partial Regex YearRegex();

    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex SpacesRegex();
}

file class KpSearchResponse
{
    [JsonPropertyName("docs")]
    public KpMovie[]? Docs { get; set; }
}

file class KpMovie
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("alternativeName")]
    public string? AlternativeName { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("shortDescription")]
    public string? ShortDescription { get; set; }

    [JsonPropertyName("rating")]
    public KpRating? Rating { get; set; }

    [JsonPropertyName("poster")]
    public KpImage? Poster { get; set; }

    [JsonPropertyName("genres")]
    public KpNameItem[]? Genres { get; set; }

    [JsonPropertyName("persons")]
    public KpPerson[]? Persons { get; set; }
}

file class KpRating
{
    [JsonPropertyName("kp")]
    public double? Kp { get; set; }

    [JsonPropertyName("imdb")]
    public double? Imdb { get; set; }
}

file class KpImage
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("previewUrl")]
    public string? PreviewUrl { get; set; }
}

file class KpNameItem
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

file class KpPerson
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("enName")]
    public string? EnName { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("enProfession")]
    public string? EnProfession { get; set; }

    [JsonPropertyName("photo")]
    public string? Photo { get; set; }
}
