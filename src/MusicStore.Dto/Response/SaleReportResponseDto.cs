namespace MusicStore.Dto.Response;

public class SaleReportResponseDto
{
    public string ConcertName { get; set; } = default!;
    public decimal Total { get; set; }
}