using MediatR;
using OrderService.Application.Abstractions;

namespace OrderService.Application.Cqrs.Commands.UpdateOrder;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    public async Task Handle(UpdateOrderCommand request, CancellationToken ct)
    {
        await _orderRepository.UpdateAsync(request.orderDto, ct);
        
        await _orderRepository.SaveChangesAsync(ct);
    }
}