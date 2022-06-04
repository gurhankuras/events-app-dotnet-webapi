using Xunit;
using Auth;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System;
using Moq;

namespace AuthTests;

/*
interface LinkRequest {}

interface ILinkGenerator<T> where T: LinkRequest {
    Uri Generate(T request);
}

public class EmailVerificationLinkRequest: LinkRequest {
    public string Code { get; set; }
    public string UserId { get; set; }
}


public class EmailVerificationLinkGenerator : ILinkGenerator<EmailVerificationLinkRequest>
{
    public Uri Generate(EmailVerificationLinkRequest request)
    {
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(request.Code));
        var encodedUserId = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(request.UserId));

        var urlBuilder = new UriBuilder();
        urlBuilder.Host = "localhost";
        urlBuilder.Path = "account/email/verify";
        urlBuilder.Scheme = "http";
        urlBuilder.Port = 5000;
        urlBuilder.Query = $"userId={encodedUserId}&token={encodedToken}";

        return urlBuilder.Uri;
    }
}



public class LinkGeneratorTests
{
    public string ExpectedUrl { get; } = "http://localhost:5000/account/email/verify?userId=NzdkZDU0YWItNWE3OS00MzU4LTgzYTQtZmU1M2MwNTUzZGI2&token=Q2ZESjhHdGZIUTE5RXR4RHF1eUJkZTNScWxlanRXSnBydnJZQlkxbWdkWFNGZkc5UUkrLzdYVFdxQ0VNdU5nTVJORWhDejlxZkd6d2doc2ZvOFRXcnEzSVppM2dUUU9mOGg2U213SFBPS3RpU0FWUkY3SHdQUmNwaVNjcE9vSXcwVDQ2TVdkSEQ0NlBWMXl0aHNtallHVlFCWG5yRUhxSnQ2VGxDVGNUaGZZbVFkcEUwVGpsa3hIRnVRdFdYRTMvS1k2dExuTjhrT1RBRUtvZ290ZXZndnZaTzBJUGFwd2ZxdWREeHFnYjl3VXVjd1Vwb3dnQm9NMHdmS2pvZGZVM3BOTVhmUT09";

    public string ExpectedCode { get; } = "Q2ZESjhHdGZIUTE5RXR4RHF1eUJkZTNScWxlanRXSnBydnJZQlkxbWdkWFNGZkc5UUkrLzdYVFdxQ0VNdU5nTVJORWhDejlxZkd6d2doc2ZvOFRXcnEzSVppM2dUUU9mOGg2U213SFBPS3RpU0FWUkY3SHdQUmNwaVNjcE9vSXcwVDQ2TVdkSEQ0NlBWMXl0aHNtallHVlFCWG5yRUhxSnQ2VGxDVGNUaGZZbVFkcEUwVGpsa3hIRnVRdFdYRTMvS1k2dExuTjhrT1RBRUtvZ290ZXZndnZaTzBJUGFwd2ZxdWREeHFnYjl3VXVjd1Vwb3dnQm9NMHdmS2pvZGZVM3BOTVhmUT09";

    [Fact]
    public void Test1()
    {
        var sut = new EmailVerificationLinkGenerator();
        var id = Guid.NewGuid().ToString();
        var request = new EmailVerificationLinkRequest {
            Code = ExpectedCode,
            UserId = id
        };

        var uri = sut.Generate(request);
        var generatedQueryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
        var receivedQueryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(new Uri(ExpectedUrl).Query);

        var generatedToken = generatedQueryDictionary["token"].ToString().DecodeQueryFromURL(Encoding.UTF8);
        var receivedToken = receivedQueryDictionary["token"].ToString().DecodeQueryFromURL(Encoding.UTF8);

        Assert.Equal(generatedToken, receivedToken);
    }
}
*/