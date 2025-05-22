namespace OrderService.Application.Models.Carts;

public class CartItemDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
    public int Price { get; set; }
}