using OrderService.Application.Models.Merchant;

namespace OrderService.Application.Abstractions;

public interface IMerchantsService
{
    Task<MerchantDto> Create (MerchantDto merchant);
}