using FluentValidation;

namespace OrderService.Application.Cqrs.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x=>x.orderDto.CustomerId).NotEmpty();
        RuleFor(x=>x.orderDto.MerchantId).NotEmpty();
        RuleFor(x=>x.orderDto.Cart).NotEmpty();
        RuleFor(x=>x.orderDto.OrderName).NotEmpty();
    }
}