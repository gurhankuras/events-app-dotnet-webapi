
using System.Text;
using System.Text.Json;
using Auth.AsyncServices;
using RabbitMQ.Client;

class RabbitMQMessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly JsonSerializerOptions _serializationSettings = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

    public RabbitMQMessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;
        var factory = new ConnectionFactory {
            HostName = "localhost",
            Port = 5672
        }; 

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("trigger", type: ExchangeType.Fanout, true);
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

    public void PublishUser(UserPublishedDto userPublishedDto)
    {
        var message = JsonSerializer.Serialize(userPublishedDto, _serializationSettings);

        if (_connection.IsOpen) 
        {
            Console.WriteLine(" ---- Rabbitmq connection is open, sending the message....");
            SendMessage(message);
        }
    }

    public void PublishConfirmationEmail()
    {
        
    }

    private void SendMessage(string message) 
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "trigger", 
                            routingKey: "", 
                            basicProperties: null,
                            body: body );
    }

    public void Dispose() {
        if(_connection.IsOpen) 
        {
            _channel.Close();
            _connection.Close();
        }
    }
}