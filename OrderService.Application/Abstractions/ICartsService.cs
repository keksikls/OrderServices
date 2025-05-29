using OrderService.Application.Models.Carts;

namespace OrderService.Application.Abstractions;

public interface ICartsService
{
    Task<CartDto> Create(CartDto cart, CancellationToken ct);
}