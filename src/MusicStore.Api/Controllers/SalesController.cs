using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Entities;
using MusicStore.Services.Interfaces;
using System.Security.Claims;

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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleAdmin)]
    public async Task<IActionResult> Get(int id)
    {
        var response = await service.GetAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Post(SaleRequestDto request)
    {
        var email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        var response = await service.AddAsync(email, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("ListSalesByDate")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.RoleAdmin)]
    public async Task<IActionResult> GetByDate([FromQuery] SaleByDateSearchDto? search, [FromQuery] PaginationDTO pagination)
    {
        var response = await service.GetAsync(search, pagination);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("ListSales")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Get(string email, [FromQuery] string? title, [FromQuery] PaginationDTO pagination)
    {
        var response = await service.GetAsync(email, title, pagination);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}