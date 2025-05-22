namespace OrderService.Domain.Entities;

public class CartEntity : BaseEntity
{
    public List<CartItemEntity>? CartItems  { get; set; }
}