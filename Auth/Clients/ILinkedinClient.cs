using System.Web;
using Auth.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Auth.Linkedin;
using Auth.Config;

public interface ILinkedinClient {
    public Task<LinkedInAccessToken> GetAccessToken(LinkedInVerificationRequest request);
}



public class LinkedinClient: ILinkedinClient
{
    private readonly LinkedInSettings _linkedInSettings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ILinkedinClient> _logger;
    public JsonSerializerSettings _jsonSerializerSettings { get; } = new JsonSerializerSettings           {            
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        },
    };

    public LinkedinClient(IHttpClientFactory httpClientFactory, SettingsLoader<LinkedInSettings> settingsLoader, ILogger<LinkedinClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _linkedInSettings = settingsLoader.Load();
        _logger = logger;
    }

    public async Task<LinkedInAccessToken> GetAccessToken(LinkedInVerificationRequest request)
    {
        var queryString = buildQuery(request);
        var url = $"https://www.linkedin.com/oauth/v2/accessToken?{queryString}";

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        var httpClient = _httpClientFactory.CreateClient();
        
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        //httpResponseMessage.EnsureSuccessStatusCode();
        
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            try
            {
                var tokenResponse = JsonConvert.DeserializeObject<LinkedInAccessTokenResponse>(content, _jsonSerializerSettings);
                if (tokenResponse == null) 
                {
                    var message = "Couldn't decode data. There is some breaking changes in Linkedin API or there is bug in code.";
                    _logger.LogError(message);
                    throw new LinkedinHTTPException(500, message);
                }
                _logger.LogDebug(tokenResponse.AccessToken);
                _logger.LogDebug(tokenResponse.ExpiresIn.ToString());
                return new LinkedInAccessToken 
                {
                    Token = tokenResponse.AccessToken, 
                    ExpiresIn = tokenResponse.ExpiresIn
                };
            }
            catch (JsonSerializationException)
            {
                var message = "Couldn't decode data. There is some breaking changes in Linkedin API or there is bug in code.";
                _logger.LogError(message);
                throw new LinkedinHTTPException(500, message);
            }
            
            
        }
        else {
            var errorContent = await httpResponseMessage.Content.ReadAsStringAsync();
            _logger.LogTrace(errorContent);
            _logger.LogTrace(httpResponseMessage.StatusCode.ToString());
            throw new LinkedinHTTPException((int) httpResponseMessage.StatusCode);
        }
    }

    private String buildQuery(LinkedInVerificationRequest request) 
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        
        if (query is null) 
        {
            // TODO: create a appropriate exception
            throw new ArgumentNullException(); 
        }
        // TODO: remove grant_type
        query["grant_type"] = request.GrantType;
        query["code"] = request.Code;
        query["client_id"] = request.ClientId;
        query["client_secret"] = _linkedInSettings.ClientSecret;
        query["redirect_uri"] = request.RedirectUri;

        string queryString = query.ToString();
        return queryString;
    }
}