using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Application.Models.Orders;

namespace OrderService.Application.Cqrs.Queries.GetOrderById;
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken ct)
        {
            var order = await _orderRepository.GetByIdWithCartAsync(request.OrderId, ct)
                        ?? throw new ArgumentException("Order not found");

            return order.ToDto(order.Cart);
        }
    }
