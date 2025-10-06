namespace MusicStore.Entities;

public class EntityBase
{
    public int Id { get; set; }
    public bool Status { get; set; } = true; //Soft delete
}