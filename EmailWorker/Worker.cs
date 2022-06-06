namespace EmailWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMessageBusClient _busClient;
    //private readonly ISubscriber _confirmEmailListener;
    public Worker(ILogger<Worker> logger, IMessageBusClient busClient)
    {
        _logger = logger;
        _busClient = busClient;
        //_confirmEmailListener = confirmEmailListener;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EmailWorker started executing");
        _busClient.SubscribeToConfirmEmail();
    }
}
