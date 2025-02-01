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
            await HandleFavicon(response);

            return;
        }

        if (request.RawUrl == "/page")
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "index.html");

            filePath = FixAssetsPath(filePath);

            if (File.Exists(filePath))
            {
                string htmlContent = await File.ReadAllTextAsync(filePath);
                byte[] buffer = Encoding.UTF8.GetBytes(htmlContent);

                response.ContentType = "text/html";
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                byte[] error = Encoding.UTF8.GetBytes("<h1>404 - Página não encontrada</h1>");
                response.OutputStream.Write(error);
            }

            return;
        }

        string responseString = "Hello World! " + DateTime.UtcNow.AddHours(-3).ToString();
        byte[] encoded = Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = encoded.Length;
        context.Response.OutputStream.Write(encoded, 0, encoded.Length);
        context.Response.OutputStream.Close();
        return;
    }

    private static async Task HandleFavicon(HttpListenerResponse response)
    {
        var faviconPath = Path.Combine(Environment.CurrentDirectory, "favicon.png");

        faviconPath = FixAssetsPath(faviconPath);

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
    }

    private static string FixAssetsPath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return filePath;

        filePath = filePath.Replace("WebServer\\bin\\Debug\\net8.0", "wwwroot");
        return filePath;
    }
}
