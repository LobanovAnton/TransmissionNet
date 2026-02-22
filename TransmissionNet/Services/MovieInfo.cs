namespace TransmissionNet.Services;

public class MovieInfo
{
    public string Title { get; set; } = "";
    public string Year { get; set; } = "";
    public string[] Genres { get; set; } = [];
    public string RatingKp { get; set; } = "";
    public string RatingImdb { get; set; } = "";
    public string Description { get; set; } = "";
    public string Director { get; set; } = "";
    public string? DirectorPhotoUrl { get; set; }
    public string? PosterUrl { get; set; }
    public ActorInfo[] Actors { get; set; } = [];
}

public class ActorInfo
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string? PhotoUrl { get; set; }
}
