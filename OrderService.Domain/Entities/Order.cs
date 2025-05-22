namespace OrderService.Domain.Entities;

public class Order : BaseEntity
{
    public OrderName OrderName { get; set; } 
    private readonly List<OrderItem> _orderItemsInternal  = new();

    public IReadOnlyList<OrderItem> OrderItems => _orderItemsInternal .AsReadOnly();
    
    public List<OrderItem> OrderItemsInternal
    {
        get => _orderItemsInternal ;
        private set
        {
            _orderItemsInternal .Clear();
            _orderItemsInternal .AddRange(value);
        }
    }
    public Address ShippingAddress  { get; private set; }
    public OrderStatus Status { get; set; }
    
    //Customer
    public CustomerEntity? Customer { get; set; }
    public Guid? CustomerId { get; set; }

    //Cart
    public CartEntity? Cart { get; set; }
    public Guid? CartId { get; set; }

    //merchant
    public MerchantEntity? Merchant { get; set; }
    public Guid MerchantId { get; set; }
    
    public DateTime? PaidDate  { get; private set; }
    public DateTime? ShippedDate  { get; private set; }
    public DateTime? CancelledDate   { get; private set; }
    
    public Money TotalAmount { get; private set; }

    //добавить товар
    public void AddItem(Guid productId, int quantity, decimal price)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException.OrderNotPendingException(Id, Status);

        if (price <= 0)
            throw new DomainException.InvalidItemPriceException(price);

        if (quantity <= 0)
            throw new DomainException.InvalidItemQuantityException(quantity);

        if (productId == Guid.Empty)
            throw new DomainException.InvalidProductIdException(productId);

        var existing = _orderItemsInternal.FirstOrDefault(x => x.ProductId.Value == productId);

        if (existing != null)
        {
            existing.IncreaseQuantity(quantity);
        }
        else
        {
            var newItem = new OrderItem(
                ProductId.Create(productId), 
                Quantity.Create(quantity), 
                Money.Create(price));
            
            _orderItemsInternal.Add(newItem);
        }

        UpdateTotalAmount();

        //добовляется ток после нового продукта 
        if (existing == null)
        {
            AddDomainEvent(new OrderItemAddEvent(
                OrderId: Id,
                ProductId: productId,
                Quantity: quantity,
                Price: price,
                OccurredOn: DateTime.UtcNow
            ));
        }
    }
    
    //убрать товар
    public void RemoveItem(Guid productId)
    {   
        var item = _orderItemsInternal.FirstOrDefault(u=>u.ProductId.Value==productId);
        
        if (item == null)
        {
            throw new DomainException.OrderItemNotFoundException(
                orderId:Id,
                productId:productId);
        }
        
        _orderItemsInternal.Remove(item);
        UpdateTotalAmount();
        
        AddDomainEvent(new OrderRemoveItemEvent(Id,productId,DateTime.UtcNow));
    }
    
    // Подтверждение оплаты
    public void MarkAsPaid()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new DomainException.CannotCancelOrderException(
                Id, $"Current status is {Status}");
        }

        if (!_orderItemsInternal.Any())
        {
            throw new DomainException.CannotCancelOrderException(
                Id, "Order is empty");
        }

        Status = OrderStatus.Paid;
        PaidDate = DateTime.UtcNow;

        AddDomainEvent(new OrderPaidEvent(Id, PaidDate.Value, DateTime.UtcNow));
    }

    //заказ отменён
    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
        {
            throw new DomainException.CancelledOrderException(Id);
        }
        
        Status = OrderStatus.Cancelled;
        CancelledDate = DateTime.UtcNow;
        
        AddDomainEvent(new OrderCancelledEvent(Id, CancelledDate.Value, DateTime.UtcNow));
    }
    
    // заказ отгружен
    public void Ship()
    {
        if (Status != OrderStatus.Paid)
        {
            throw new DomainException.OrderShippingException(
                orderId: Id,
                status: Status);
        }

        if (!_orderItemsInternal.Any())
        {
            throw new DomainException.EmptyOrderShippingException(
                Id);
        }
        
        Status = OrderStatus.Shipped;
        ShippedDate = DateTime.UtcNow;
        
        AddDomainEvent(new OrderShippedEvent(Id, ShippedDate.Value, DateTime.UtcNow));
    }

    public void SetShippingAdress(Address address)
    {
        ShippingAddress = address ?? throw new ArgumentNullException(nameof(address));
    }

    private void UpdateTotalAmount()
    {
        var total = _orderItemsInternal
            .Select(i=>i.TotalPrice)
            .Aggregate(Money.Create(0), (sum, next) => sum.Add(next));
        TotalAmount = total;
    }
}