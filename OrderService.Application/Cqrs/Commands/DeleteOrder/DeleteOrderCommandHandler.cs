using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.Models.Orders;
using OrderService.Domain.Enum;

namespace OrderService.Application.Cqrs.Commands.DeleteOrder;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
{
    private readonly IOrderRepository _orderRepository;

    public DeleteOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    public async Task Handle(DeleteOrderCommand request, CancellationToken ct)
    {
         await _orderRepository.DeleteAsync(request.id, ct);
         OrderStatus status = OrderStatus.Delete;
         
         await _orderRepository.SaveChangesAsync(ct);
    }
}