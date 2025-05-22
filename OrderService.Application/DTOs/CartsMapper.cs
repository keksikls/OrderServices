namespace OrderService.Application.DTOs;

public static class CartsMapper
{
    public static CartDto ToDto(this CartEntity entity)
    {
        return new CartDto
        {
            Id = entity.Id,
            CartItems = entity.CartItems!.Select(item => new CartItemDto
            {
                Id = item.Id,
                Quantity = item.Quantity,
                Name = item.Name,
                Price = item.Price
            }).ToList()
        };
    }

    public static CartEntity ToEntity(this CartDto cartDto)
    {
        return new CartEntity
        {
            CartItems = cartDto.CartItems.Select(cart=>cart.ToEntity()).ToList()
        };
    }

    public static CartItemEntity ToEntity(this CartItemDto cartItemDto)
    {
        return new CartItemEntity
        {
            Name = cartItemDto.Name,
            Quantity = cartItemDto.Quantity,
            Price = cartItemDto.Price
        };
    }
}