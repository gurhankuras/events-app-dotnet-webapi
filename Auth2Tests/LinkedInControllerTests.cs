using Xunit;
using System;
using Auth.Linkedin;
using Moq;
using Microsoft.AspNetCore.Identity;
using mongoidentity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;


namespace Auth2Tests;
public class LinkedinControllerTests 
{
    Mock<ILinkedInService> makeService(Action<Mock<ILinkedInService>> setup) 
    {
        var service = new Mock<ILinkedInService>();
        setup(service);
        return service;
    }
    
    [Fact]
    public async Task ReturnsSuccess_WhenHappyPath() 
    {
        // Arrange
        var userId = "12345";
        var service = makeService(srv => {
            srv.Setup(s => s.VerifyUser(userId, It.IsAny<LinkedInVerificationRequest>()))
                .ReturnsAsync(true);
        });

        var logger = Mock.Of<ILogger<LinkedInController>>();
        var provider = new Mock<IUserProvider>();
        provider.Setup(p => p.Id).Returns(userId);
        var sut = new LinkedInController(service.Object, logger, provider.Object);
        
        var expected = new OkObjectResult(new { Message = "Basarili" });

        // Act
        var actionResult = await sut.VerifyAsLinkedInUser(It.IsAny<LinkedInVerificationRequest>());   
        var result = actionResult.Result as OkObjectResult;

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}