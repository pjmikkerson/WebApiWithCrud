namespace WebApiWithCrud.DTOs
{
    public record CreateMovieDto(string Title, string Genre, DateTimeOffset ReleaseDate, double Rating);
}
