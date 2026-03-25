namespace RealWorld.Models.Entities;

public class Tag : BaseEntity
{
    public int Id { get; set; }
    required public string TagText { get; set; }

    // Relationships
    public Article Article { get; set; }
}