using Auth.JwtExtensions;
using Auth.Jwt;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using MongoDB.Driver;
using Auth.Services;
using FluentValidation.AspNetCore;
using Amazon.S3;
using Amazon;
using Nest;
using Auth.Models;
using mongoidentity;
using Microsoft.AspNetCore.Identity;
using Auth.AsyncServices;
using Auth.Linkedin;
using System.Reflection;
using Auth.Filters;
using Auth.Config;

var builder = WebApplication.CreateBuilder(args);
AWSConfigsS3.UseSignatureVersion4 = true;

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserProvider, UserProvider>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.WebHost.ConfigureKestrel(options => {   options.ListenAnyIP(5000); });
builder.Services.AddSingleton<IElasticClient, ElasticClient>(opt => {
    var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
    .DefaultMappingFor<Event>(x => x.IndexName("events"));
    
    var client = new ElasticClient(settings);
    return client;
});

builder.Services.AddSingleton<ILinkedinClient, LinkedinClient>();
builder.Services.AddSingleton<SettingsLoader<LinkedInSettings>, LinkedinSettingsLoader>();
builder.Services.AddScoped<ILinkedInService, LinkedinService>();
builder.Services.AddSingleton<IMessageBusClient, RabbitMQMessageBusClient>();
builder.Services.AddSingleton<IAmazonS3, AmazonS3Client>(options => {
    var s3Secrets = builder.Configuration.GetSection(nameof(AWSS3Config)).Get<AWSS3Config>() 
                        ?? throw new ArgumentNullException(nameof(AWSS3Config));
    var config = new AmazonS3Config();
    config.RegionEndpoint = Amazon.RegionEndpoint.EUCentral1;
    return new AmazonS3Client(s3Secrets.AccessKey, s3Secrets.SecretKey, config);
});

builder.Services.AddSingleton<AWSFileUploader>();
builder.Services.AddSingleton<AWSEmailSettings>(
    builder.Configuration.GetSection("EmailSendingSettings").Get<AWSEmailSettings>());
var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbConfig)).Get<MongoDbConfig>();

builder.Services.AddSingleton<IMongoClient, MongoClient>(b => new MongoClient(mongoDbSettings.ConnectionString));
builder.Services.AddSingleton<IEmailSender, AWSEmailSender>();
builder.Services.AddSingleton<IAmazonSimpleEmailService, AmazonSimpleEmailServiceClient>(builder => {
    var settings = builder.GetRequiredService<AWSEmailSettings>();
    var credentials = new BasicAWSCredentials(settings.SmtpUser, settings.SmtpPassword);
    var client = new AmazonSimpleEmailServiceClient(credentials, Amazon.RegionEndpoint.USEast1);
    return client;
});

builder.Services.AddSingleton<IUserRepository, MongoUserRepository>();
builder.Services.AddSingleton<IEventRepository, MongoEventRepository>();
builder.Services.AddSingleton<IEventService, EventService>();
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
    options.Password.RequiredLength = 9;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddRoles<ApplicationRole>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(mongoDbSettings.ConnectionString, mongoDbSettings.Name)
    .AddDefaultTokenProviders();
builder.Services.AddJwtAuthService(builder.Configuration);




// Add services to the container.
builder.Services.AddControllers(options => {
    if (builder.Environment.IsDevelopment()) {
        options.Filters.Add<LogRouteFilter>();
    }
    options.Filters.Add<ValidationFilter>();
})
.ConfigureApiBehaviorOptions(options => {
    options.SuppressModelStateInvalidFilter = true; 
})
.AddFluentValidation(options => {
    options.ImplicitlyValidateChildProperties = true;
    options.ImplicitlyValidateRootCollectionElements = true;
    options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization(options => {
     options.AddPolicy("premium",
         policy => policy.RequireRole("premium"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
