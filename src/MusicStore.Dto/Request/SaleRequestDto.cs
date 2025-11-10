namespace MusicStore.Dto.Request;

public record SaleRequestDto(int ConcertId, short TicketsQuantity, string Email, string FullName);