namespace OrderService.Domain.Exceptions;

public class DomainException : Exception
{
    public string Code { get; }
    public int InvalidQuantity { get; }
    public Guid ProductId { get; }

    private DomainException(string code,string message) : base(message)
    {
        Code = code;
    }

    public DomainException(string code,string message,Exception innerException, int invalidQuantity,Guid productId) 
        : base(message, innerException)
    {
        ProductId = productId;
        InvalidQuantity = invalidQuantity;
        Code = code;
    }

    // Невозможно изменить неожидающий заказ
    public class OrderNotPendingException : DomainException
    {
        public OrderNotPendingException(Guid orderId, OrderStatus currentStatus) 
            : base("ORDER_NOT_PENDING", 
                $"Order {orderId} is in {currentStatus} status. Only pending orders can be modified.")
        {
        }
    }
    
    //невозможно оплатить заказ
    public class CannotPayOrderException : DomainException
    {
        public CannotPayOrderException(Guid orderId,string reason) 
            :base("Cannot_Pay_Order",
                $"Order {orderId} cannot be paid: {reason} .")
        {
            
        }
    }
    
    // Неверная цена товара
    public class InvalidItemPriceException : DomainException
    {
        public InvalidItemPriceException(decimal price)
            :base("Invalid_Item_Price",
                $"Price {price} is invalid.")
        {
            
        }
    }
    
    // Нельзя отменить заказ
    public class CannotCancelOrderException : DomainException
    {
        public CannotCancelOrderException(Guid orderId,string reason)
            : base("Cannot_Cancel_Order",
                $"Order {orderId} cannot be canceled: {reason} .")
        {
            
        }
    }
    
    public class InvalidItemQuantityException : DomainException
    {
        public InvalidItemQuantityException(int invalidQuantity)
            : base(
                code:"Invalid_Item_Quantity",
                message:$"Quantity {invalidQuantity} is invalid.")
        {
            
        }
    }
    
    public class OrderItemNotFoundException : DomainException
    {
        public OrderItemNotFoundException(Guid orderId,Guid productId ) 
            : base(
                code:"Order_Item_NotFound",
                message:$"Order {orderId} does not exist: {productId}")
        {
            
        }
    }
    
    public class OrderShippingException : DomainException
    {
        public OrderShippingException(Guid orderId,OrderStatus status)
            :base("Order_Shipping_Error",
                $"Imposible to ship order {orderId} , status {status}")
        {
            
        }
    }
    
    public class EmptyOrderShippingException  : DomainException
    {
        public EmptyOrderShippingException (Guid orderId)
            :base(code:"Empty_Order_Shipping",
                $"Order {orderId} is empty.")
        {
            
        }
    }
    
    public class InvalidShippingDataException   : DomainException
    {
        public InvalidShippingDataException (string message)
            :base(code:"Invalid_Shipping_Data",
                message)
        {
            
        }
    }

    public class CancelledOrderException : DomainException
    {
        public CancelledOrderException(Guid orderId)
            :base("Order has Paid",
                $"Order {orderId} already cancelled.")
        {
            
        }
    }

    public class InvalidProductIdException : DomainException
    {
        public InvalidProductIdException(Guid ProductId)
            :base("Invalid_Product",
                $"Product {ProductId} is invalid.")
        {
            
        }
    }

    public class InvalidQuantityException : DomainException
    {
        public InvalidQuantityException(int newQuantity)
            :base("Invalid_Quantity",
                $"Quantity {newQuantity} is invalid.")
        {
            
        }
    }
    
    public class InvalidPriceException : DomainException
    {
        public InvalidPriceException(decimal unitPrice)
            :base("Invalid_Price",
                $"Unit price {unitPrice} is invalid.")
        {
            
        }
    }

    public class InvalidProductName : DomainException
    {
        public InvalidProductName(string productName)
            : base("Invalid_Product_Name",
                $"Product name {productName} is invalid.")
        {
            
        }
    }
}