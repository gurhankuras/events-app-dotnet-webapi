using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


public class RabbitMQBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IEmailSender _emailSender;
    private readonly IModel _channel;
    
    private readonly string transactionQueueName;
    private AsyncEventingBasicConsumer? transactionConsumer;
    private readonly JsonSerializerOptions _serializationSettings = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };


    public RabbitMQBusClient(IConfiguration configuration, IEmailSender emailSender)
    {
        _emailSender = emailSender;
        _configuration = configuration;
        var factory = new ConnectionFactory {
            HostName = "localhost",
            Port = 5672,
            DispatchConsumersAsync = true
        }; 

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("confirm-email", type: ExchangeType.Fanout, true);
            transactionQueueName = _channel.QueueDeclare("confirm-email-queue").QueueName;
            _channel.QueueBind(queue: transactionQueueName,
                              exchange: "confirm-email",
                              routingKey: "");
            
            _connection.ConnectionShutdown += RabbitMQConnectionShutdown;
             Console.WriteLine($"--- Connected to MesageBus");

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

    public void SubscribeToConfirmEmail()
    {
        transactionConsumer = new AsyncEventingBasicConsumer(_channel);
        transactionConsumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var request = JsonSerializer.Deserialize<EmailRequestDto>(message, _serializationSettings);
            if (request is null) {
                return;
            }
            //Console.WriteLine(emailConfirmationUrl);
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
        _channel.BasicConsume(queue: transactionQueueName,
                                autoAck: true,
                                consumer: transactionConsumer);
    }
}
