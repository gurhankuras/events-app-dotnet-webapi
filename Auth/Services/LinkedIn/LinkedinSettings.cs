using Auth.Config;

namespace Auth.Linkedin;

public interface SettingsLoader<T> {
    T Load();
}
public class LinkedinSettingsLoader : SettingsLoader<LinkedInSettings>
{
    private readonly IConfiguration _configuration;

    public LinkedinSettingsLoader(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public LinkedInSettings Load()
    {
        return _configuration.GetSection("LinkedInSettings").Get<LinkedInSettings>();
    }
}