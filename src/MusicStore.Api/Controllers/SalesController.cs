using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Services.Interfaces;

namespace MusicStore.Api.Controllers;

[ApiController]
[Route("api/sales")]
public class SalesController : ControllerBase
{
    private readonly ISaleService service;

    public SalesController(ISaleService service)
    {
        this.service = service;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var response = await service.GetAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }
    [HttpPost]
    public async Task<IActionResult> Post(SaleRequestDto request)
    {
        var response = await service.AddAsync(request.Email, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("ListSalesByDate")]
    public async Task<IActionResult> GetByDate([FromQuery] SaleByDateSearchDto search, [FromQuery] PaginationDTO pagination)
    {
        var response = await service.GetAsync(search, pagination);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("ListSales")]
    public async Task<IActionResult> Get(string email, [FromQuery] string? title, [FromQuery] PaginationDTO pagination)
    {
        var response = await service.GetAsync(email, title, pagination);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}