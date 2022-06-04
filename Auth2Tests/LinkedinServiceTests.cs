using Xunit;
using System;
using Auth.Linkedin;
using Moq;
using Microsoft.AspNetCore.Identity;
using mongoidentity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Auth2Tests;
public class LinkedinServiceTests 
{
    public static Mock<UserManager<TUser>> MockUserManager<TUser>(Action<Mock<UserManager<TUser>>> setup) where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var manager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        
        manager.Object.UserValidators.Add(new UserValidator<TUser>());
        manager.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
        
        setup(manager);
        return manager;
    }

    [Fact]
    public async Task ReturnsSuccess_WhenHappyPath() 
    {   
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "User1", Email = "user1@test.com" };

        var mockManager = MockUserManager<ApplicationUser>(manager => {
            manager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                   .ReturnsAsync(IdentityResult.Success);
            manager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                   .ReturnsAsync(user);
        });
        var sut = makeSUT(mockManager);
        
        // Act
        var success = await sut.VerifyUser(user.Id.ToString(), TestFixtures.AnyLinkedInVerificationRequest());

        // Assert
        Assert.True(success);
    }

    [Fact]
    public async Task ThrowsUserNotFoundException_IfUserNotExistsWithId() 
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        var mockManager = MockUserManager<ApplicationUser>(manager => {
            manager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                    .ReturnsAsync(It.IsAny<IdentityResult>());
            manager.Setup(m => m.FindByIdAsync(userId))
                   .ReturnsAsync(null as ApplicationUser);
        });
        var sut = makeSUT(mockManager);

        // Act - Assert
        await Assert.ThrowsAsync<UserNotFound>(() => sut.VerifyUser(userId, TestFixtures.AnyLinkedInVerificationRequest()));
    }

      [Fact]
    public async Task FailsSavingLinkedinToken() 
    {
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "User1", Email = "user1@test.com" };

        var mockManager = MockUserManager<ApplicationUser>(manager => {
            manager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                    .ReturnsAsync(IdentityResult.Failed(new IdentityError()));
            manager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                   .ReturnsAsync(user);
        });
        var sut = makeSUT(mockManager);

        var result = await sut.VerifyUser(user.Id.ToString(), TestFixtures.AnyLinkedInVerificationRequest());
        // Act - Assert
        Assert.Equal(result, false);
    }

     private LinkedinService makeSUT(Mock<UserManager<ApplicationUser>> manager) 
    {
        var mockClient = new Mock<ILinkedinClient>();
        mockClient.Setup(c => c.GetAccessToken(It.IsAny<LinkedInVerificationRequest>()))
                  .ReturnsAsync(TestFixtures.ValidLinkedinAccessToken()); 
        var logger = Mock.Of<ILogger<LinkedinService>>();
        var sut = new LinkedinService(mockClient.Object, manager.Object, logger);
        return sut;
    }


}

internal static class TestFixtures
{
    internal static LinkedInVerificationRequest AnyLinkedInVerificationRequest() {
        return new LinkedInVerificationRequest {
            ClientId = "",
            ClientSecret = "",
            Code = "",
            GrantType = "",
            RedirectUri = ""
        };
    }

    internal static LinkedInAccessToken ValidLinkedinAccessToken() 
    {
        return new LinkedInAccessToken {
            Token = "12345",
            ExpiresIn = 5000
        };
    }
}