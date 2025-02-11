﻿using WebApiWithCrud.DTOs;
using WebApiWithCrud.Services;

namespace WebApiWithCrud.Endpoints
{
    public static class MovieEndpoints
    {
        public static void MapMovieEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var movieApi = endpoints.MapGroup("/api/movies").WithTags("Movies");

            movieApi.MapPost("/", async (IMovieService service, CreateMovieDto command) =>
            {
                var movie = await service.CreateMovieAsync(command);
                return TypedResults.Created($"/api/movies/{movie.Id}", movie);
            });

            movieApi.MapGet("/", async (IMovieService service) =>
            {
                var movies = await service.GetAllMoviesAsync();
                return TypedResults.Ok(movies);
            });

            movieApi.MapGet("/{id}", async (IMovieService service, Guid id) =>
            {
                var movie = await service.GetMovieByIdAsync(id);

                return movie is null
                    ? (IResult)TypedResults.NotFound(new { Message = $"Movie with ID {id} not found." })
                    : TypedResults.Ok(movie);
            });

            movieApi.MapPut("/{id}", async (IMovieService service, Guid id, UpdateMovieDto command) =>
            {
                await service.UpdateMovieAsync(id, command);
                return TypedResults.NoContent();
            });

            movieApi.MapDelete("/{id}", async (IMovieService service, Guid id) =>
            {
                await service.DeleteMovieAsync(id);
                return TypedResults.NoContent();
            });
        }
    }
}
