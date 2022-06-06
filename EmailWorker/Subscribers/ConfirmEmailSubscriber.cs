using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
/*
public interface ISubscriber 
{
    void Listen();
}

public class ConfirmEmailSubscriber : ISubscriber
{
    private readonly string exchangeName = "confirm-email";
    private readonly string queueName = "confirm-email-queue";
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly IEmailSender _emailSender;
    private AsyncEventingBasicConsumer? consumer;
    private readonly JsonSerializerOptions _serializationSettings = new JsonSerializerOptions {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

     public void Listen()
    {
        consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var request = JsonSerializer.Deserialize<EmailRequestDto>(message, _serializationSettings);
            if (request is null) {
                return;
            }
            try
            {
                await _emailSender.Send(request.EmailReceiver, "Confirm your email", EmailHtmlTemplates.ConfirmEmail(request.ConfirmationUrl));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            await Task.Yield();
        };
        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

     public ConfirmEmailSubscriber(IConfiguration configuration, IEmailSender emailSender)
    {
        _emailSender = emailSender;
        _configuration = configuration;
        
    }

    private void Connect() 
    {
        var factory = new ConnectionFactory {
            HostName = "localhost",
            Port = 5672,
            DispatchConsumersAsync = true
        }; 

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchangeName, type: ExchangeType.Fanout, true);
            _channel.QueueDeclare(queueName);
            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");
            _connection.ConnectionShutdown += RabbitMQConnectionShutdown;
             Console.WriteLine($"--- Connected to MessageBus");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"----< {ex.Message}");    
        }
    }

    private void RabbitMQConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine($"RabbitMQ connection shutdown");
    }
}
*/