namespace MusicStore.Dto.Request;

public class ConcertRequestDto
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Place { get; set; } = default!;
    public double UnitPrice { get; set; }
    public int GenreId { get; set; }
    public string DateEvent { get; set; } = default!;
    public string TimeEvent { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public int TicketsQuantity { get; set; }
}