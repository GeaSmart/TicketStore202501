using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Services.Interfaces;

namespace MusicStore.Api.Controllers;

[ApiController]
[Route("api/genres")]
public class GenresController : ControllerBase
{
    private readonly IGenreService service;

    public GenresController(IGenreService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var response = await service.GetAsync();
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var response = await service.GetAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post(GenreRequestDto genreRequestDto)
    {
        var response = await service.AddAsync(genreRequestDto);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, GenreRequestDto genreRequestDto)
    {
        var response = await service.UpdateAsync(id, genreRequestDto);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await service.DeleteAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}