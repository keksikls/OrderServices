namespace OrderService.Application.DTOs;

public static class OrdersMapper
{
    public static OrderDto ToDto(this Order entity, CartEntity? cart = null)
    {
        return new OrderDto
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId!.Value,
            MerchantId = entity.MerchantId,
            OrderName = entity.OrderName,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            Cart = cart?.ToDto() ?? entity.Cart?.ToDto()
        };
    }

    public static Order ToEntity(this CreateOrderDto dto)
    {
        return new Order
        {
            CustomerId = dto.CustomerId,
            MerchantId = dto.MerchantId,
            OrderName = dto.OrderName,
            Cart = dto.Cart?.ToEntity()
        };
    }
}