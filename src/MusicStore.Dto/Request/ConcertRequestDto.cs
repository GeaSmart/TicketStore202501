using Microsoft.AspNetCore.Http;
using MusicStore.Dto.Validations;

namespace MusicStore.Dto.Request;

public class ConcertRequestDto
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ExtendedDescription { get; set; } = default!;
    public string Place { get; set; } = default!;
    public double UnitPrice { get; set; }
    public int GenreId { get; set; }
    public string DateEvent { get; set; } = default!;
    public string TimeEvent { get; set; } = default!;
    [FileSizeValidation(1)]
    [FileTypeValidation(fileTypeGroup: FileTypeGroup.Image)]
    public IFormFile? Image { get; set; }
    public int TicketsQuantity { get; set; }
}