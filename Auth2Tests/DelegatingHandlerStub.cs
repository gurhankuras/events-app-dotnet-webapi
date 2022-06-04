using Auth;
using Xunit;
using Xunit.Extensions;
using Xunit.Abstractions;
using Xunit.Sdk;
using Moq;
using System;
using System.Net.Http;
using Moq.Contrib.HttpClient;
using System.Net;
using Newtonsoft.Json;
using Auth.Linkedin;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Auth.Exceptions;
using Newtonsoft.Json.Serialization;
using Auth.Config;

namespace Auth2Tests;
public class LinkedinClientTests 
{

    public JsonSerializerSettings _jsonSerializerOptions { get; } = new JsonSerializerSettings 
            {
                 ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
            };

    private LinkedinClient makeSut(Action<Mock<HttpMessageHandler>> setup) {
        var handler = new Mock<HttpMessageHandler>();
        var client = handler.CreateClient();
        
        setup(handler);

        var mockFactory = new Mock<IHttpClientFactory>();


        mockFactory.Setup(f => f.CreateClient("")).Returns(client);

        var settings = new LinkedInSettings 
        {
            ClientId = "dsfdf",
            ClientSecret = "dfsdf"
        };

        var logger = Mock.Of<ILogger<LinkedinClient>>();
        var loader = new Mock<SettingsLoader<LinkedInSettings>>();
        loader.Setup(l => l.Load()).Returns(settings);
        return new LinkedinClient(mockFactory.Object, loader.Object, logger);
    }

    public LinkedInVerificationRequest dummyVerificationRequest { get; } = new LinkedInVerificationRequest 
        {
            ClientId = "dsfsd",
            ClientSecret = "asd",
            Code = "sad",
            GrantType = "sad",
            RedirectUri = "34"
        };
    
    [Fact]
    public async Task Test_GetsAccessTokenSuccessfully()
    {   
        var expectedResponse = new LinkedInAccessTokenResponse
        {
            AccessToken = "213123",
            ExpiresIn = 50000
        };
        
        var sut = makeSut((handler) => {
            handler.SetupAnyRequest()
            .ReturnsResponse(JsonConvert.SerializeObject(expectedResponse, _jsonSerializerOptions), "application/json");
        });

        var tok = await sut.GetAccessToken(dummyVerificationRequest);
        
        Assert.NotNull(tok);
        Assert.Equal(expectedResponse.AccessToken, tok.Token);
    }

    [Fact]
    public async Task Test_ThrowsLinkedinHTTPExceptionWith400_WhenLinkedinAPIReturns400()
    {        
        var sut = makeSut((handler) => {
            handler.SetupAnyRequest()
            .ReturnsResponse(HttpStatusCode.BadRequest);
        });
        
        var exception = await Assert.ThrowsAsync<LinkedinHTTPException>(() => sut.GetAccessToken(dummyVerificationRequest));

        Assert.Equal(400, exception.StatusCode);
    }

     [Fact]
    public async Task Test_ThrowsLinkedinHTTPExceptionWith500_WhenTheResponseFromAPICorrupted()
    {        
        var sut = makeSut((handler) => {
            handler.SetupAnyRequest()
            .ReturnsResponse(HttpStatusCode.OK, 
            JsonConvert.SerializeObject(new { }, _jsonSerializerOptions), "application/json");
        });
        
        var exception = await Assert.ThrowsAsync<LinkedinHTTPException>(() => sut.GetAccessToken(dummyVerificationRequest));

        Assert.Equal(500, exception.StatusCode);
    }
}