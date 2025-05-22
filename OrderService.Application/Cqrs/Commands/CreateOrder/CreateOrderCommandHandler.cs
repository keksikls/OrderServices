namespace OrderService.Application.Cqrs.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var order = request.OrderDto.ToEntity();
        await _orderRepository.AddAsync(order, ct);
        await _orderRepository.SaveChangesAsync(ct);
    
        return order.ToDto();
    }
}