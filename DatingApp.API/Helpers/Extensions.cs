using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    // static means we dont need to create an instance when using its methods
    public static class Extensions
    {
        // general purpose class 

        public static void AddApplicationError(this HttpResponse response, string message)
        {
            // used to add headers to the output stream
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}