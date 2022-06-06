using Amazon.Runtime;
using Amazon.SimpleEmail;
using EmailWorker;

var hostBuilder = Host.CreateDefaultBuilder(args);
IHost host = hostBuilder
    .ConfigureServices((hostContext, services) =>
    {
        var emailConfig = hostContext.Configuration.GetSection("EmailSendingSettings").Get<AwsEmailConfiguration>();
        services.AddSingleton<AwsEmailConfiguration>(emailConfig);
        services.AddSingleton<IAmazonSimpleEmailService, AmazonSimpleEmailServiceClient>(builder => {
            var settings = builder.GetRequiredService<AwsEmailConfiguration>();
            var credentials = new BasicAWSCredentials(settings.SmtpUser, settings.SmtpPassword);
            var client = new AmazonSimpleEmailServiceClient(credentials, Amazon.RegionEndpoint.USEast1);
            return client;
        });
       
        services.AddSingleton<IEmailSender, AWSEmailSender>();
        services.AddSingleton<IMessageBusClient, RabbitMQBusClient>();
        //services.AddSingleton<IS, RabbitMQBusClient>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
