using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ChatServer.Handlers;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using ChatShared.DTO;
using ChatServer.Models;
using System.Collections.Concurrent;

namespace ChatServer.Services
{
    public class Server
    {
        private readonly TcpListener _listener;
        private static readonly ConcurrentDictionary<string, ClientSession> _sessions = new();

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
            string? currentToken = null;
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
                    requestString.Clear();

                    RequestType type = JObject.Parse(request).GetValue("Type").ToObject<RequestType>();
                    Console.WriteLine($"Получен запрос: {type}");

                    switch (type)
                    {
                        //Обработка логина
                        case RequestType.Login:
                            {
                                var loginDTO = JsonConvert.DeserializeObject<RequestDTO<ClientLoginDTO>>(request).Data;

                                HandleLogin handleLogin = new HandleLogin(new Data.ProjectChatContext());
                                LoginResultDTO result = await handleLogin.HandleLoginAsync(loginDTO);

                                if (result.Success)
                                {
                                    var session = new ClientSession()
                                    {
                                        ClientId = result.ClientId,
                                        Token = result.Token,
                                        CurrentRoomId = -1
                                    };
                                    _sessions.TryAdd(result.Token, session);
                                }
                                await SendResponseAsync(stream, result);

                                Console.WriteLine($"Результат авторизации отправлен пользователю: {client.Client.RemoteEndPoint}");
                            }

                            break;

                        case RequestType.Register:
                            {
                                var registerDTO = JsonConvert.DeserializeObject<RequestDTO<ClientSignUpDTO>>(request).Data;

                                var handleSingUp = new HandleSignUp(new Data.ProjectChatContext());
                                bool result = await handleSingUp.HandleSignUpAsync(registerDTO);

                                await SendResponseAsync(stream, result);

                                Console.WriteLine($"Результат авторизации отправлен пользователю: {client.Client.RemoteEndPoint}");
                            }
                            break;
                        case RequestType.CreatRoom:
                            {
                                var requestDTO = JsonConvert.DeserializeObject<RequestDTO<ChatRoomDTO>>(request);
                                if (!TokenCheck(requestDTO.Token))
                                    break;

                                var createRoomDTO = requestDTO.Data;
                                var handleCreateRoom = new HandlerRoom(new Data.ProjectChatContext());
                                bool result = await handleCreateRoom.CreatRoomAsync(createRoomDTO);

                                await SendResponseAsync(stream, result);

                                Console.WriteLine($"Результат создания комнаты отправлен пользователю: {client.Client.RemoteEndPoint}");
                            }
                            break;
                        case RequestType.GetChatRooms:
                            {
                                var requestDTO = JsonConvert.DeserializeObject<RequestDTO<GetChatRoomsDTO>>(request);
                                if (!TokenCheck(requestDTO.Token))
                                    break;

                                var getRoomsDTO = requestDTO.Data;
                                var handleGetRooms = new HandlerRoom(new Data.ProjectChatContext());
                                ChatRoomDTO[] roomsDTO = await handleGetRooms.GetRoomsForClientAsync(getRoomsDTO.ClientId);

                                await SendResponseAsync(stream, roomsDTO);

                                Console.WriteLine($"Результат на список доступны комнат отправлен пользователю: {client.Client.RemoteEndPoint}");
                            }
                            break;

                        case RequestType.SendMessage:
                            {
                                //var sendMessageDTO = JsonConvert.DeserializeObject<SendMessageDTO>(request);
                                //var json = JsonConvert.SerializeObject("Сообщение отправлено");
                                //var bytes = Encoding.UTF8.GetBytes(json + "\n");
                                //await stream.WriteAsync(bytes);
                                //Console.WriteLine($"Результат отправки сообщения пользователю: {client.Client.RemoteEndPoint}");
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
            finally
            {
                if (!string.IsNullOrEmpty(currentToken))
                    _sessions.TryRemove(currentToken, out _);
            }
        }
        private async Task SendResponseAsync<T>(Stream stream, T response)
        {
            try
            {
                var json = JsonConvert.SerializeObject(response);
                var bytes = Encoding.UTF8.GetBytes(json + "\n");
                await stream.WriteAsync(bytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке ответа: {ex.Message}");
            }
        }
        /// <summary>
        ///  True - token is valid, false - token is not valid
        /// </summary>
        /// <param name="token">Client token</param>
        /// <returns></returns>
        private bool TokenCheck(string token)
        {
            if (!_sessions.TryGetValue(token, out var session))
            {
                Console.WriteLine($"Не удалось найти сессию для токена: {token}");
                return false;
            }
            return true;
        }
    }
}
