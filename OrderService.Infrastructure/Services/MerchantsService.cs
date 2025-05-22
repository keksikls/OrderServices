using OrderService.Application.Abstractions;
using OrderService.Application.Models.Merchant;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Data.DbContext;

namespace OrderService.Infrastructure.Services;

public class MerchantsService(OrderDbContext context) : IMerchantsService
{
    public async Task<MerchantDto> Create(MerchantDto merchant)
    {
        var entity = new MerchantEntity
        {
            Name = merchant.Name,
            Phone = merchant.Phone,
            Website = merchant.Website
        };

        var result = await context.Merchants.AddAsync(entity);
        var resultentity = result.Entity;

        await context.SaveChangesAsync();

        return new MerchantDto
        {
            Name = resultentity.Name,
            Phone = resultentity.Phone,
            Website = resultentity.Website  
        };
    }
}