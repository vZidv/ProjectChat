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
using ChatShared.DTO.Enums;
using ChatShared.DTO.Messages;
using System.Collections.Concurrent;
using ChatServer.Session;
using static System.Collections.Specialized.BitVector32;

namespace ChatServer.Services
{
    public class Server
    {
        private readonly TcpListener _listener;
        private HandlerClientSession _handlerClient = new();

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

                                var handleLogin = new HandleClient(new Data.ProjectChatContext());
                                LoginResultDTO result = await handleLogin.ClientLoginAsync(loginDTO);

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
                                var handleCreateRoom = new HandlerChatRoom(new Data.ProjectChatContext());
                                CreatChatRoomResultDTO result = await handleCreateRoom.CreatRoomAsync(createRoomDTO);

                                await SendResponseAsync(stream, result, ResponseType.CreatRoomResult);

                                Console.WriteLine($"Результат создания комнаты отправлен пользователю: {client.Client.RemoteEndPoint}");
                            }
                            break;
                        case RequestType.GetChats:
                            {
                                var requestDTO = JsonConvert.DeserializeObject<RequestDTO<GetChatsDTO>>(request);
                                if (_handlerClient.TryGetSession(requestDTO.Token) == null)
                                    break;

                                var getRoomsDTO = requestDTO.Data;
                                var handlerChat = new HandlerChatList(new Data.ProjectChatContext());
                                List<ChatMiniProfileDTO> chatMiniProfileDTOs = await handlerChat.GetChatListForClientAsync(getRoomsDTO.ClientId);

                                await SendResponseAsync(stream, chatMiniProfileDTOs, ResponseType.GetChats);

                                Console.WriteLine($"Результат на список доступных чатов отправлен пользователю: {client.Client.RemoteEndPoint}");
                            }
                            break;
                        case RequestType.UpdateCurrentChatRoom:
                            {
                                var requestDTO = JsonConvert.DeserializeObject<RequestDTO<ChatMiniProfileDTO>>(request);
                                if (_handlerClient.TryGetSession(requestDTO.Token) == null)
                                    break;

                                ClientSession session = _handlerClient.TryGetSession(requestDTO.Token);
                                _handlerClient.ClientInChatRoom(session, requestDTO.Data.Id);

                                Console.WriteLine($"Пользователь: {client.Client.RemoteEndPoint} зашел в комнату: Name:{requestDTO.Data.Name}; Id:{requestDTO.Data.Id}");
                            }
                            break;
                        case RequestType.SendMessage:
                            {
                                var requestDTO = JsonConvert.DeserializeObject<RequestDTO<MessageDTO>>(request);
                                if (_handlerClient.TryGetSession(requestDTO.Token) == null)
                                    break;

                                MessageDTO messageDTO = requestDTO.Data;
                                ClientSession session = _handlerClient.TryGetSession(requestDTO.Token);

                                var handlerMessage = new HandlerMessage(new Data.ProjectChatContext());
                                MessageDTO newMessageDTO = await handlerMessage.WritingMessageAsync(messageDTO, session.Client.Id);

                                ClientSession[] clients = _handlerClient.GetClientsFromChatRoom(messageDTO.RoomId);

                                foreach (var clientSession in clients)
                                {
                                    if (session.Client.Id == clientSession.Client.Id)
                                        continue;
                                    await SendResponseAsync(clientSession.Stream, newMessageDTO, ResponseType.Message);
                                    Console.WriteLine($"Отправил сообщение {newMessageDTO.Text} пользователю: {session.Client.Id}");
                                }
                                Console.WriteLine($"Сообщение {messageDTO.Text} в комнате {messageDTO.RoomId} записано");

                            }
                            break;
                        case RequestType.GetHistoryChat:
                            {
                                var requestDTO = JsonConvert.DeserializeObject<RequestDTO<GetChatHistoryDTO>>(request);
                                if (_handlerClient.TryGetSession(requestDTO.Token) == null)
                                    break;

                                GetChatHistoryDTO chatHistoryDTO = requestDTO.Data;
                                ClientSession session = _handlerClient.TryGetSession(requestDTO.Token);

                                var roomHandler = new HandlerChatRoom(new Data.ProjectChatContext());
                                MessageDTO[] messageDTO = await roomHandler.GetHistoryChatRoomAsync(chatHistoryDTO.ChatId, chatHistoryDTO.ChatType, session.Client.Id);

                                var result = new ChatHistoryDTO()
                                {
                                    ChatId = chatHistoryDTO.ChatId,
                                    ChatType = chatHistoryDTO.ChatType,
                                    MessageDTOs = messageDTO
                                };

                                await SendResponseAsync(stream, result, ResponseType.GetHistoryChatRoom);
                                Console.WriteLine($"Пользователю: {client.Client.RemoteEndPoint} отправлена история комнаты: {requestDTO.Data.ChatId}");
                            }
                            break;
                        case RequestType.SearchChats:
                            {
                                var requestDTO = JsonConvert.DeserializeObject<RequestDTO<SeachChatDTO>>(request);
                                if (_handlerClient.TryGetSession(requestDTO.Token) == null)
                                    break;

                                SeachChatDTO seachChatDTO = requestDTO.Data;
                                ClientSession session = _handlerClient.TryGetSession(requestDTO.Token);

                                var handlerChat = new HandlerChatList(new Data.ProjectChatContext());
                                List<ChatMiniProfileDTO> chatMiniProfileDTOs = await handlerChat.SearchNewChatForClientByName(seachChatDTO.SearchText, session.Client.Id);

                                var result = new SeachChatResultDTO()
                                {
                                    Chats = chatMiniProfileDTOs.ToArray(),
                                    TotalCount = chatMiniProfileDTOs.Count,
                                    SearchText = seachChatDTO.SearchText
                                };

                                await SendResponseAsync(stream, result, ResponseType.SearchChatsResult);
                                Console.WriteLine($"Пользователю: {client.Client.RemoteEndPoint} отправлено {result.TotalCount} чатов, результат по поиска: \"{result.SearchText}\"");
                            }
                            break;
                        case RequestType.JoimInChatGroup:
                            {
                                var requestDTO = JsonConvert.DeserializeObject<RequestDTO<JoinInChatRoomDTO>>(request);
                                if (_handlerClient.TryGetSession(requestDTO.Token) == null)
                                    break;

                                JoinInChatRoomDTO joinInChatRoomDTO = requestDTO.Data;

                                var handlerChatRoom = new HandlerChatRoom(new Data.ProjectChatContext());
                                JoinInChatRoomResultDTO result = await handlerChatRoom.AddNewMemberInChatRoomAsync(joinInChatRoomDTO.ClientId,joinInChatRoomDTO.ChatRoomId);

                                await SendResponseAsync(stream, result, ResponseType.JoinInChatRoomResult);
                                Console.WriteLine($"Пользователю: {client.Client.RemoteEndPoint} был добавлен в комнтау {joinInChatRoomDTO.ChatRoomId}");
                            }
                            break;
                        case RequestType.AddContact:
                            {
                                var requestDTO = JsonConvert.DeserializeObject<RequestDTO<AddContactDTO>>(request);
                                if (_handlerClient.TryGetSession(requestDTO.Token) == null)
                                    break;

                                AddContactDTO addContactDTO = requestDTO.Data;

                                var context = new Data.ProjectChatContext();

                                var handelerContact = new HandlerContact(context);
                                AddContactResultDTO result = await handelerContact.AddContactToClientAsync(addContactDTO.SenderClientId, addContactDTO.ReceiverClientId);

                                var handelerChatRoom = new HandlerChatRoom(context);
                                var chatRoomResult = await handelerChatRoom.CreatPrivateRoomAsync(new int[] { addContactDTO.SenderClientId, addContactDTO.ReceiverClientId });

                                await SendResponseAsync(stream, result, ResponseType.AddContactResult);
                                Console.WriteLine($"Пользователю: {client.Client.RemoteEndPoint} был добавлен контакт: {addContactDTO.ReceiverClientId}");
                            }
                            break;
                        case RequestType.ClientProfileUpdate:
                            {
                                var requestDTO = JsonConvert.DeserializeObject<RequestDTO<ClientProfileDTO>>(request);
                                if (_handlerClient.TryGetSession(requestDTO.Token) == null)
                                    break;

                                ClientProfileDTO clientProfileDTO = requestDTO.Data;

                                var handelerContact = new HandleClient(new Data.ProjectChatContext());
                                bool result = await handelerContact.ClientProfileUpdateAsync(clientProfileDTO);

                                Console.WriteLine($"Данные пользователя id: {clientProfileDTO.Id} обновлены, результат {result}");
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
