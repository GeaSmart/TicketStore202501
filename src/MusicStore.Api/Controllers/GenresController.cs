using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories;
using System.Net;

namespace MusicStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly IGenreRepository repository;
        private readonly ILogger<GenresController> logger;

        public GenresController(IGenreRepository repository, ILogger<GenresController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = new BaseResponseGeneric<ICollection<GenreResponseDto>>();
            try
            {
                //Mapping
                var genresDb = await repository.GetAsync();

                var genres = genresDb.Select(x => new GenreResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status
                }).ToList();

                response.Data = genres;
                response.Success = true;
                logger.LogInformation($"Obteniendo todos los géneros musicales.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "Ocurrió un error al obtener la información.";
                logger.LogError(ex, $"{response.ErrorMessage} {ex.Message}");
                return BadRequest(response);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = new BaseResponseGeneric<GenreResponseDto>();
            try
            {
                var genreDb = await repository.GetAsync(id);

                if (genreDb is null)
                {
                    logger.LogWarning($"Género musical con id {id} no se encontró.");
                    return NotFound(response);
                }
                else
                {
                    var genre = new GenreResponseDto
                    {
                        Id = genreDb.Id,
                        Name = genreDb.Name,
                        Status = genreDb.Status
                    };
                    response.Data = genre;
                    response.Success = true;
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = "Ocurrió un error al obtener la información.";
                logger.LogError(ex, $"{response.ErrorMessage} {ex.Message}");
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(GenreRequestDto genreRequestDto)
        {
            var response = new BaseResponseGeneric<int>();
            try
            {
                var genreDb = new Genre()
                {
                    Name = genreRequestDto.Name,
                    Status = genreRequestDto.Status
                };
                var genreId = await repository.AddAsync(genreDb);
                response.Data = genreId;
                response.Success = true;
                logger.LogInformation($"Género musical insertado con id: {genreId}.");
                return StatusCode((int)HttpStatusCode.Created, response);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "Ocurrió un error al insertar.";
                logger.LogError(ex, $"{response.ErrorMessage} {ex.Message}");
                return BadRequest(response);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, GenreRequestDto genreRequestDto)
        {
            var response = new BaseResponse();
            try
            {
                var genreDb = await repository.GetAsync(id);

                if (genreDb is null)
                {
                    response.ErrorMessage = "No se encontró el registro";
                    return NotFound(response);
                }
                genreDb.Name = genreRequestDto.Name;
                genreDb.Status = genreRequestDto.Status;

                await repository.UpdateAsync();
                response.Success = true;
                logger.LogInformation($"Género musical con id {id} actualizado.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "Ocurrió un error al actualizar.";
                logger.LogError($"{response.ErrorMessage} {ex.Message}");
                return BadRequest(response);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new BaseResponse();
            try
            {
                var genreDb = await repository.GetAsync(id);
                if (genreDb is null)
                {
                    response.ErrorMessage = "No se encontró el registro";
                    return NotFound(response);
                }
                await repository.DeleteAsync(id);
                response.Success = true;
                logger.LogInformation($"Género musical con id {id} eliminado.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "Ocurrió un error al eliminar.";
                logger.LogError($"{response.ErrorMessage} {ex.Message}");
                return BadRequest(response);
            }
        }
    }
}
