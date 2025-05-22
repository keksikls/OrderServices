namespace OrderService.Application.Models.Carts;

public class CartDto
{
    public Guid Id { get; set; }
    public List<CartItemDto> CartItems { get; set; } = null!;
}