using AutoMapper;
using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.Models.Orders;

namespace OrderService.Application.Cqrs.Queries.GetOrdersByStatus;

public class GetOrdersByStatusQueryHandler : IRequestHandler<GetOrdersByStatusQuery, List<OrderDto>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetOrdersByStatusQueryHandler(
        IOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(
        GetOrdersByStatusQuery request, 
        CancellationToken ct)
    {
        var orders = await _repository.GetByStatusAsync(request.Status, ct);
        return _mapper.Map<List<OrderDto>>(orders);
    }
}