using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ChatServer.DTO;
using ChatServer.Handlers;

namespace ChatServer.Services
{
    public class Server
    {
        private readonly TcpListener _listener;

        public Server(int port)
        {
            _listener = new(IPAddress.Any, port);
        }

        public async Task Start()
        {
            _listener.Start();
            Console.WriteLine($"Сервер запущен. Ожидание подключений...");
            while (true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine($"Новое подключение: {client.Client.RemoteEndPoint}");
                _ = Task.Run(() => HandleClientAsync(client));
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using var stream = client.GetStream();
                using var reader = new StreamReader(stream);
                using var writer = new StreamWriter(stream);


                var bytesRead = 10;
                var requestString = new List<byte>();

                while (client.Connected)
                {
                    while ((bytesRead = stream.ReadByte()) != '\n')
                        requestString.Add(Convert.ToByte(bytesRead));

                    var request = Encoding.UTF8.GetString(requestString.ToArray());
                    RequestDTO requestDTO = JsonConvert.DeserializeObject<RequestDTO>(request);

                    Console.WriteLine($"Получен запрос: {requestDTO.Type}");
                    switch (requestDTO.Type)
                    {
                        //Обработка логина
                        case RequestType.Login:
                            {
                                var loginDTO = JsonConvert.DeserializeObject<ClientLoginDTO>(request);

                                HandleLogin handleLogin = new HandleLogin(new Data.ProjectChatContext());
                                bool result = await handleLogin.HandleLoginAsync(loginDTO);

                                var response = JsonConvert.SerializeObject(result);
                                var bytes = Encoding.UTF8.GetBytes(response + "\n");
                                await stream.WriteAsync(bytes);

                                Console.WriteLine($"Результат авторизации отправлен пользователю: {client.Client.RemoteEndPoint}");
                            }

                            break;

                        case RequestType.Register:
                            {
                                var registerDTO = JsonConvert.DeserializeObject<ClientSignUpDTO>(request);

                                var handleSingUp = new HandleSignUp(new Data.ProjectChatContext());
                                bool result = await handleSingUp.HandleSignUpAsync(registerDTO);

                                var bytes = Encoding.UTF8.GetBytes(result.ToString() + "\n");
                                await stream.WriteAsync(bytes);

                                Console.WriteLine($"Результат авторизации отправлен пользователю: {client.Client.RemoteEndPoint}");
                            }
                            break;
                        default:
                            Console.WriteLine("Неизвестный тип запроса");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

    }
}
