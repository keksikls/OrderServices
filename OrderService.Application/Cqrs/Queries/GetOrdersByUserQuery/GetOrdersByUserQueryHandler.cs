namespace OrderService.Application.Cqrs.Queries.GetOrdersByUserQuery;

public class GetOrdersByUserQueryHandler: IRequestHandler<GetOrdersByUserQuery, PagedList<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrdersByUserQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }
    
    public async Task<PagedList<OrderDto>> Handle(GetOrdersByUserQuery request, CancellationToken ct)
    {
        var (orders, totalCount) = await _orderRepository.GetByUserIdAsync(
            request.UserId,
            request.Page,
            request.PageSize,
            ct);

        return new PagedList<OrderDto>(
            _mapper.Map<List<OrderDto>>(orders),
            totalCount,
            request.Page,
            request.PageSize);
    }
}