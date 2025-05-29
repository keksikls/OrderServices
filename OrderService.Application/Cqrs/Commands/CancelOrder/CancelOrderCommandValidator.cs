using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace OrderService.Application.Cqrs.Commands.CancelOrder;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(x=>x.OrderId).NotEmpty();
        RuleFor(x=>x.Reason).NotEmpty();
    }
}