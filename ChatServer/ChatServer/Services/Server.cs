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
using System.Collections.Concurrent;
using ChatServer.Session;

namespace ChatServer.Services
{
    public class Server
    {
        private readonly TcpListener _listener;
        private HandlerClient _handlerClient = new();

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
                _ = Task.Run(() => HandleSessionAsync(client));
            }
        }

        private async Task HandleSessionAsync(TcpClient client)
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
                                    _handlerClient.AddClient(result, stream);

                                await SendResponseAsync(stream, result, ResponseType.LoginResult);

                                Console.WriteLine($"Результат авторизации отправлен пользователю: {client.Client.RemoteEndPoint}");
                            }

                            break;

                        case RequestType.Register:
                            {
                                var registerDTO = JsonConvert.DeserializeObject<RequestDTO<ClientSignUpDTO>>(request).Data;

                                var handleSingUp = new HandleSignUp(new Data.ProjectChatContext());
                                bool result = await handleSingUp.HandleSignUpAsync(registerDTO);

                                await SendResponseAsync(stream, result, ResponseType.SignUpResult);

                                Console.WriteLine($"Результат авторизации отправлен пользователю: {client.Client.RemoteEndPoint}");
                            }
                            break;
                        case RequestType.CreatRoom:
                            {
                                var requestDTO = JsonConvert.DeserializeObject<RequestDTO<ChatRoomDTO>>(request);
                                if (_handlerClient.TryGetSession(requestDTO.Token) == null)
                                    break;

                                var createRoomDTO = requestDTO.Data;
                                var handleCreateRoom = new HandlerRoom(new Data.ProjectChatContext());
                                CreatChatRoomResultDTO result = await handleCreateRoom.CreatRoomAsync(createRoomDTO);

                                await SendResponseAsync(stream, result, ResponseType.CreatRoomResult);

                                Console.WriteLine($"Результат создания комнаты отправлен пользователю: {client.Client.RemoteEndPoint}");
                            }
                            break;
                        case RequestType.GetChatRooms:
                            {
                                var requestDTO = JsonConvert.DeserializeObject<RequestDTO<GetChatRoomsDTO>>(request);
                                if (_handlerClient.TryGetSession(requestDTO.Token) == null)
                                    break;

                                var getRoomsDTO = requestDTO.Data;
                                var handleGetRooms = new HandlerRoom(new Data.ProjectChatContext());
                                ChatRoomDTO[] roomsDTO = await handleGetRooms.GetRoomsForClientAsync(getRoomsDTO.ClientId);

                                await SendResponseAsync(stream, roomsDTO, ResponseType.GetChatRooms);

                                Console.WriteLine($"Результат на список доступны комнат отправлен пользователю: {client.Client.RemoteEndPoint}");
                            }
                            break;
                        case RequestType.UpdateCurrentRoom:
                            {
                                var requestDTO = JsonConvert.DeserializeObject<RequestDTO<ChatRoomDTO>>(request);
                                if (_handlerClient.TryGetSession(requestDTO.Token) == null)
                                    break;

                                ClientSession session = _handlerClient.TryGetSession(requestDTO.Token);
                                _handlerClient.ClientInRoom(session, requestDTO.Data.Id);

                                Console.WriteLine($"Пользователь: {client.Client.RemoteEndPoint} зашел в комнату: Name:{requestDTO.Data.Name}; Id:{requestDTO.Data.Id}");
                            }
                            break;
                        case RequestType.SendMessage:
                            {
                                var jsonRequest = JObject.Parse(request);
                                string token = jsonRequest.GetValue("Token")?.ToString();
                                if (_handlerClient.TryGetSession(token) == null)
                                    break;

                                MessageType messageType = (jsonRequest.GetValue("Data") as JObject).GetValue("MessageType").ToObject<MessageType>();

                                ClientSession session = _handlerClient.TryGetSession(token);
                                var handlerMessage = new HandlerMessage(new Data.ProjectChatContext(), _handlerClient);

                                switch (messageType)
                                {
                                    case MessageType.RoomMessage:
                                        {
                                            var messageDTO = JsonConvert.DeserializeObject<RequestDTO<RoomMessageDTO>>(request).Data;

                                            MessageDTO newMessageDTO = await handlerMessage.WritingMessageAsync(messageDTO, session.Client.Id);

                                            var message = newMessageDTO as RoomMessageDTO;
                                            ClientSession[] clients = _handlerClient.GetClientsFromRoom(message.RoomId);

                                            foreach (var clientSession in clients)
                                            {
                                                if (session.Client.Id == clientSession.Client.Id)
                                                    continue;
                                                await SendResponseAsync(clientSession.Stream, newMessageDTO, ResponseType.Message);
                                                Console.WriteLine($"Отправил сообщение {newMessageDTO.Text} пользователю: {session.Client.Id}");
                                            }
                                            Console.WriteLine($"Сообщение {message.Text} в комнате {message.RoomId} записано");
                                        }
                                        break;
                                    case MessageType.PrivateMessage:
                                        {
                                            var messageDTO = JsonConvert.DeserializeObject<RequestDTO<PrivateMessageDTO>>(request).Data;

                                            MessageDTO newMessageDTO = await handlerMessage.WritingMessageAsync(messageDTO, session.Client.Id);

                                            var message = newMessageDTO as PrivateMessageDTO;

                                            await SendResponseAsync(stream, newMessageDTO, ResponseType.Message);
                                            Console.WriteLine($"Сообщение {message.Text} отправлено пользователю {message.ClientId} записано");
                                        }
                                        break;
                                }
                            }
                            break;
                        case RequestType.GetHistoryRoom:
                            {
                                var requestDTO = JsonConvert.DeserializeObject<RequestDTO<GetRoomHistoryDTO>>(request);
                                if (_handlerClient.TryGetSession(requestDTO.Token) == null)
                                    break;

                                var roomHandler = new HandlerRoom(new Data.ProjectChatContext());
                                RoomMessageDTO[] messageDTO = await roomHandler.GetHistoryRoomAsync(requestDTO.Data.RoomId);

                                var result = new RoomHistoryDTO()
                                {
                                    MessageDTOs = messageDTO,
                                    RoomId = requestDTO.Data.RoomId
                                };

                                await SendResponseAsync(stream, result, ResponseType.GetHistoryRoom);
                                Console.WriteLine($"Пользователю: {client.Client.RemoteEndPoint} отправлена история комнаты: {requestDTO.Data.RoomId}");
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
                    _handlerClient.TryRemoveSession(currentToken);
            }
        }
        private async Task SendResponseAsync<T>(Stream stream, T response, ResponseType type, string? message = null)
        {
            try
            {
                var responseDTO = new ResponseDTO<T>()
                {
                    Data = response,
                    Message = message,
                    Type = type
                };

                var json = JsonConvert.SerializeObject(responseDTO);
                var bytes = Encoding.UTF8.GetBytes(json + "\n");
                await stream.WriteAsync(bytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке ответа: {ex.Message}");
            }
        }
    }
}
