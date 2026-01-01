using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Entities;
using MusicStore.Services.Interfaces;

namespace MusicStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleAdmin)]
    public class ConcertsController : ControllerBase
    {
        private readonly IConcertService concertService;

        public ConcertsController(IConcertService concertService)
        {
            this.concertService = concertService;
        }

        [HttpGet("title")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string? title, [FromQuery] PaginationDTO pagination)
        {
            var response = await concertService.GetAsync(title, pagination);
            return response.Success ? Ok(response.Data) : BadRequest(response);
        }
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var response = await concertService.GetAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] ConcertRequestDto request)
        {
            var response = await concertService.AddAsync(request);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromForm] ConcertRequestDto request)
        {
            var response = await concertService.UpdateAsync(id, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await concertService.DeleteAsync(id);
            return Ok(response);
        }
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id)
        {
            return Ok(await concertService.FinalizeAsync(id));
        }
    }
}
