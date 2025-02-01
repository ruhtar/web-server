using System.Net;
using System.Text;

namespace WebServer;

public class Router
{
    public static async Task HandleAsync(HttpListenerContext context)
    {

        var request = context.Request;
        var response = context.Response;

        Log.Print(request);
        if (request.Url?.AbsolutePath == "/favicon.ico")
        {
            var faviconPath = Path.Combine(Environment.CurrentDirectory, "icons8-favicon-94.png");

            faviconPath = faviconPath.Replace("bin\\Debug\\net8.0\\", "");

            if (File.Exists(faviconPath))
            {
                byte[] faviconBytes = File.ReadAllBytes(faviconPath);

                response.ContentType = "image/x-icon";
                response.ContentLength64 = faviconBytes.Length;

                await response.OutputStream.WriteAsync(faviconBytes);
                response.OutputStream.Close();
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            return;
        }

        var method = request.HttpMethod;

        // We have a connection, do something...

        string responseString = "Hello World! " + DateTime.UtcNow.AddHours(-3).ToString();
        byte[] encoded = Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = encoded.Length;
        context.Response.OutputStream.Write(encoded, 0, encoded.Length);
        context.Response.OutputStream.Close();
        return;
    }
}
