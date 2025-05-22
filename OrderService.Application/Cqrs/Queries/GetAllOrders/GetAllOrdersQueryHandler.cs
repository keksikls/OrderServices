namespace OrderService.Application.Cqrs.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, PagedList<OrderDto>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetAllOrdersQueryHandler(IOrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedList<OrderDto>> Handle(
        GetAllOrdersQuery request, 
        CancellationToken ct)
    {
        var (orders, totalCount) = await _repository.GetAllAsync(
            request.Page,
            request.PageSize,
            request.Status,
            request.FromDate,
            request.ToDate,
            ct);

        return new PagedList<OrderDto>(
            _mapper.Map<List<OrderDto>>(orders),
            totalCount,
            request.Page,
            request.PageSize);
    }
}