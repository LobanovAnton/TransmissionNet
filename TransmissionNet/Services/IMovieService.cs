namespace TransmissionNet.Services;

public interface IMovieService
{
    Task<MovieInfo?> SearchAsync(string torrentName, CancellationToken cancellationToken = default);
}
