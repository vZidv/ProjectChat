using System.Net.Sockets;
using System.Net;

namespace ChatServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine($"Новое подключение: {client.Client.RemoteEndPoint}");
                _ = Task.Run(() => HandleClientAsync(client));

            }

            async Task HandleClientAsync(TcpClient client)
            {
                try
                {
                    using var stream = client.GetStream();
                    using var reader = new StreamReader(stream);
                    using var writer = new StreamWriter(stream);

                    while (client.Connected)
                    {
                        var json = await reader.ReadLineAsync();
                        if (string.IsNullOrEmpty(json)) continue;

                        Console.WriteLine($"Получено: {json}");
                        await writer.WriteLineAsync("Сообщение получено!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }
    }
}
