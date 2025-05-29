using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.Cqrs.Commands.CancelOrder;
using OrderService.Domain.Enum;

namespace OrderService.Application.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    public async Task Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = _orderRepository.GetByIdAsync(request.OrderId,ct)
                    ?? throw new ArgumentException("Order not found");

        OrderStatus status = OrderStatus.Cancelled;

        await _orderRepository.SaveChangesAsync(ct);
    }
}