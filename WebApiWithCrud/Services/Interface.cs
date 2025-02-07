using WebApiWithCrud.DTOs;

namespace WebApiWithCrud.Services
{
    public interface IMovieService
    {
        Task<MovieDto> GetMovieAsync(CreateMovieDto command);
        Task<MovieDto?> GetMovieByIdAsync(Guid id);
        Task<IEnumerable<MovieDto>> GetAllMoviesAsync();
        Task UpdateMocieAsync(Guid id, UpdateMovieDto command);
        Task DeleteMocieAsync(Guid id);
    }
}
