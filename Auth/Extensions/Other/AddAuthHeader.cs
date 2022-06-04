using System.Text;
using Microsoft.AspNetCore.WebUtilities;

public static class AddAuthHeaderExtension {
    public static void AddAuthHeader(this HttpResponse response, string token) {
        response.Headers.Add("Authorization", token);
    }
}

public static class MyTestExtensions {
    public static string DecodeQueryFromURL(this string str, Encoding encoding) {
        return encoding.GetString(WebEncoders.Base64UrlDecode(str));
    }
}