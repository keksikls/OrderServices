using FluentValidation;

namespace OrderService.Application.Cqrs.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x=>x.OrderDto.CustomerId).NotEmpty();
        RuleFor(x=>x.OrderDto.MerchantId).NotEmpty();
        RuleFor(x=>x.OrderDto.Cart).NotEmpty();
        RuleFor(x=>x.OrderDto.OrderName).NotEmpty();
    }
}