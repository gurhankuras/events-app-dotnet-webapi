public static class FileStorageUtils
{
    // TODO: save id image but keep this to provide base url
    public static string getProfileImageURL(string id)
    {
        var baseURL = "https://gkevents-app.s3.eu-central-1.amazonaws.com/profile";
        return $"{baseURL}/{id}";
    }
}