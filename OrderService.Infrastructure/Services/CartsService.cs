using Microsoft.EntityFrameworkCore;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Application.Models.Carts;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Data.DbContext;

namespace OrderService.Infrastructure.Services;

public class CartsService(OrderDbContext context) : ICartsService
{
    public async Task<CartDto> Create(CartDto cart, CancellationToken ct)
    {
        var cartEntity = new CartEntity();
        var cartSaveResult = await context.Carts.AddAsync(cartEntity);
        await context.SaveChangesAsync();


        var cartItems = cart.CartItems
            .Select(item => new CartItemEntity
            {
                Name = item.Name,
                Quantity = item.Quantity,
                Price = item.Price,
                CartId = cartSaveResult.Entity.Id
            });

        await context.CartItems.AddRangeAsync(cartItems);
        await context.SaveChangesAsync();

        var result = await context.Carts
            .Include(x => x.CartItems)
            .FirstAsync(x => x.Id == cartSaveResult.Entity.Id);

        return result.ToDto();
    }
}