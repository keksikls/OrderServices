namespace OrderService.Domain.Entities;

public class CartItemEntity : BaseEntity
{
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
    public int Price { get; set; }
    
    public CartEntity? Cart { get; set; }
    public Guid CartId { get; set; }
}