namespace OrderService.Domain.Entities;

public class OrderItem : BaseEntity
{
    public ProductId ProductId { get; private set; }
    public Quantity Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    
    // Внешний ключ
    public Guid OrderId { get; private set; }
    // Навигационное свойство
    public Order Order { get; private set; }

    public Money TotalPrice => Money.Create(UnitPrice.Amount * Quantity.Value);

    public OrderItem(ProductId productId, Quantity quantity, Money unitPrice)
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity));
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
    }

    private OrderItem() { }

    public void IncreaseQuantity(int value)
    {
        Quantity = Quantity.Increase(value);
    }

    public void UpdateQuantity(int newValue)
    {
        if (newValue <= 0)
        {
            throw new DomainException.InvalidQuantityException(newValue);
        }
        Quantity = Quantity.Update(newValue);
    }
}