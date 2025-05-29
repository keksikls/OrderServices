using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.CancelOrder;
using OrderService.Application.Cqrs.Commands.CancelOrder;
using OrderService.Application.Cqrs.Commands.CreateOrder;
using OrderService.Application.Cqrs.Commands.DeleteOrder;
using OrderService.Application.Cqrs.Commands.UpdateOrder;
using OrderService.Application.Cqrs.Queries.GetAllOrders;
using OrderService.Application.Cqrs.Queries.GetOrderById;
using OrderService.Application.Cqrs.Queries.GetOrdersByStatus;
using OrderService.Application.Cqrs.Queries.GetOrdersByUserQuery;
using OrderService.Application.Models.Orders;
using OrderService.Domain.Entities;
using OrderService.Domain.Enum;
using OrderService.Domain.Models;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IMediator mediator,
        ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    /// Создание нового заказа
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        _logger.LogInformation("Creating new order : {OrderName}");
        _logger.LogDebug("Order details: {@OrderDto}", dto);

        try
        {
            var command = new CreateOrderCommand(dto);
            var result = await _mediator.Send(command);
            
            _logger.LogInformation("Order created with ID: {OrderId}", result.Id);
            return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for order creation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order");
            return StatusCode(500);
        }
    }
    
    /// Получение заказа по ID
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
    {
        _logger.LogDebug("Fetching order with ID: {OrderId}", id);

        try
        {
            var query = new GetOrderByIdQuery(id);
            var order = await _mediator.Send(query);
            
            if (order == null)
            {
                _logger.LogWarning("Order not found: {OrderId}", id);
                return NotFound();
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching order {OrderId}", id);
            return StatusCode(500);
        }
    }
    
    /// Отмена заказа
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelOrder(
        [FromRoute] Guid id,
        [FromBody] CancelOrderCommand request)
    {
        _logger.LogInformation("Cancelling order {OrderId}. Reason: {Reason}", id, request.Reason);

        try
        {
            var command = new CancelOrderCommand(id, request.Reason);
            await _mediator.Send(command);
            
            _logger.LogInformation("Order {OrderId} cancelled successfully", id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Order not found during cancellation: {OrderId}", id);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel order {OrderId}", id);
            return StatusCode(500);
        }
    }
    
    /// Получение заказов пользователя
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderDto>>> GetUserOrders(Guid userId)
    {
        _logger.LogDebug("Fetching orders for user {UserId}", userId);

        try
        {
            var query = new GetOrdersByUserQuery(userId);
            var orders = await _mediator.Send(query);
            
            _logger.LogInformation("Found {Count} orders for user {UserId}", userId);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching orders for user {UserId}", userId);
            return StatusCode(500);
        }
    }
    
    /// Получение заказов по статусу
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderDto>>> GetOrdersByStatus(OrderStatus status)
    {
        _logger.LogDebug("Fetching orders with status {Status}", status);

        try
        {
            var query = new GetOrdersByStatusQuery(status);
            var orders = await _mediator.Send(query);
            
            _logger.LogInformation("Found {Count} orders with status {Status}", orders.Count, status);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching orders with status {Status}", status);
            return StatusCode(500);
        }
    }
    
    /// Получение всех заказов (с пагинацией)
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedList<OrderDto>>> GetAllOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogDebug("Fetching all orders. Page: {Page}, PageSize: {PageSize}", page, pageSize);

        try
        {
            var query = new GetAllOrdersQuery(page, pageSize);
            var orders = await _mediator.Send(query);
            
            _logger.LogInformation("Fetched {Count} orders (page {Page})", orders.Items.Count, page);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching orders list");
            return StatusCode(500);
        }
    }

    //Удаление заказа
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteOrder([FromRoute]Guid id,CancellationToken ct)
    {
        if (id == Guid.Empty || id == null)
        {
            return NotFound();
        }
        
        _logger.LogInformation($"Method DeleteGet / get{id} for category proccess ");

        try
        {
            await _mediator.Send(new DeleteOrderCommand(id, ct));
            return NoContent();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
    
    //обновление
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateOrder([FromRoute]Guid id,[FromBody] OrderDto orderDto,CancellationToken ct)
    {
        try
        {
            if (id != orderDto.Id)
            {
                return BadRequest("Id in route doesn not match");
            }

            await _mediator.Send(new UpdateOrderCommand(orderDto, ct));
            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating order with id {OrderId}",id);
            return StatusCode(500);
        }
    }
}