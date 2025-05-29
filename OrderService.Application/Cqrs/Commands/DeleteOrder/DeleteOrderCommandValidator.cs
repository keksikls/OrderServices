using FluentValidation;

namespace OrderService.Application.Cqrs.Commands.DeleteOrder;

public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(x=>x.id).NotEmpty();
        RuleFor(x=>x.ct).NotEmpty();
    }
}