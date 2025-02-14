﻿using System.Net;
using System.Net.Sockets;

namespace WebServer;

/// <summary>
/// A lean and mean web server.
/// </summary>
public class Server
{
    private static HttpListener Listener;
    public static int MaxSimultaneousConnections = 20; //probably have more than one localhost IP
    private readonly static Semaphore _semaphore = new(MaxSimultaneousConnections, MaxSimultaneousConnections); //sets up a semaphore that waits for a specified number of simultaneously allowed connections. (https://stackoverflow.com/questions/70465029/understanding-semaphores-in-c-sharp)

    //We're going to make the initial assumption that we're connecting to the server on an intranet,
    //so we obtain the IP's of our local host:
    private static List<IPAddress> GetLocalHostIPs()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        return host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();
    }

    private static HttpListener InitializeListener(List<IPAddress> localHostIps)
    { 
        var listener = new HttpListener(); 
        listener.Prefixes.Add("http://localhost/");

        foreach (var ip in localHostIps)
        {
            Console.WriteLine("Listening on IP " + "http://" + ip.ToString() + "/");
            listener.Prefixes.Add("http://" + ip.ToString() + "/");
        }

        return listener;
    }

    public static void Start()
    {
        var ips = GetLocalHostIPs();
        Listener = InitializeListener(ips);
        Listener.Start();

        Task.Run(() =>
        {
            while (true)
            {
                _semaphore.WaitOne();
                StartConnectionListener(Listener);
            }
        });
    }

    /// <summary>
    /// Await connections.
    /// </summary>
    private static async void StartConnectionListener(HttpListener listener)
    {
        HttpListenerContext context = await listener.GetContextAsync();

        await Router.HandleAsync(context);

        // Release the semaphore so that another listener can be immediately started up.
        _semaphore.Release();
    }
}
