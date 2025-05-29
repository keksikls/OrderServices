using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderService.Application.Abstractions;
using OrderService.Application.Models.Orders;
using OrderService.Domain.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderService.Infrastructure.BackgroundService;

public class CreateOrderConsumer : Microsoft.Extensions.Hosting.BackgroundService, IDisposable
{
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly IChannel _channel;
    private readonly IConnection _connection;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CreateOrderConsumer> _logger;

    public CreateOrderConsumer(
        IOptions<RabbitMqOptions> options,
        IServiceProvider serviceProvider,
        ILogger<CreateOrderConsumer> logger)
    {
        _rabbitMqOptions = options.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;

        ValidateRabbitMqOptions();

        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.HostName ?? "localhost",
            Port = _rabbitMqOptions.Port,
            UserName = _rabbitMqOptions.UserName ?? "guest",
            Password = _rabbitMqOptions.Password ?? "guest",
            VirtualHost = _rabbitMqOptions.VirtualHost ?? "/"
        };

        try
        {
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            _logger.LogInformation("Successfully connected to RabbitMQ with host: {Host}, port: {Port}, virtualHost: {VirtualHost}", 
                factory.HostName, factory.Port, factory.VirtualHost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to establish RabbitMQ connection");
            throw;
        }
    }

    private void ValidateRabbitMqOptions()
    {
        if (_rabbitMqOptions == null)
        {
            throw new ArgumentNullException(nameof(_rabbitMqOptions), "RabbitMQ options are not configured");
        }

        if (string.IsNullOrEmpty(_rabbitMqOptions.CreateOrderQueueName))
        {
            throw new ArgumentException("CreateOrderQueueName is required in RabbitMQ configuration", 
                nameof(_rabbitMqOptions.CreateOrderQueueName));
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            try
            {
                _logger.LogInformation("Received message: {Message}", message);
                
                var createOrderDto = JsonSerializer.Deserialize<CreateOrderDto>(message, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                })!;

                using var scope = _serviceProvider.CreateScope();
                var ordersService = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                await ordersService.Create(createOrderDto, stoppingToken);

                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, stoppingToken);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize message: {Message}", message);
                // В случае неверного формата сообщения, подтверждаем его получение
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message: {Message}", message);
                // В случае ошибки обработки, отправляем NACK для повторной обработки
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            _rabbitMqOptions.CreateOrderQueueName,
            autoAck: false,
            consumer,
            cancellationToken: stoppingToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
        
        _logger.LogInformation("CreateOrderConsumer disposed");
    }
}