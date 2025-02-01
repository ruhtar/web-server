using System.Net;

namespace WebServer;

internal class Log
{
    /// <summary>
    /// Log requests.
    /// </summary>
    public static void Print(HttpListenerRequest request)
    {
        // Extrai o caminho da URL (tudo após o domínio)
        string path = request.Url.AbsolutePath;

        // Imprime o endereço IP remoto, o método HTTP e o caminho da URL
        Console.WriteLine(request.RemoteEndPoint + " " + request.HttpMethod + " " + path);
    }
}
