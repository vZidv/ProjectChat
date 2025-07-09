using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChatShared.DTO;
using ChatShared.DTO.Enums;
using ChatShared.DTO.Messages;
using ChatShared.Events;
using Newtonsoft.Json;
using ChatClient.CustomControls;
using Newtonsoft.Json.Linq;

namespace ChatClient.Services
{
    public class NetworkService
    {
        private readonly string _host;
        private readonly int _port;

        private TcpClient _client;

        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;

        private readonly IEventAggregator _eventAggregator;

        //..

        public NetworkService(IEventAggregator eventAggregator, string host = "192.168.0.100", int port = 8888)
        {
            _host = host;
            _port = port;
            _eventAggregator = eventAggregator;
        }

        //Подключение и инициализация потока
        public async Task ConnectAsync()
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(_host, _port);

                _stream = _client.GetStream();
                _reader = new StreamReader(_stream);
                _writer = new StreamWriter(_stream);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось подключиться к серверу.", "Ошибка подключения", MessageBoxButton.OK, MessageBoxType.Error);
                Dispose();
                throw new Exception("Не удалось подключиться к серверу.", ex);
            }

        }

        //Отправка сообщения
        public async Task SendAsync<T>(T data, RequestType type)
        {
            try
            {
                var request = new RequestDTO<T>
                {
                    Type = type,
                    Data = data,
                    Token = NetworkSession.Token
                };
                var json = JsonConvert.SerializeObject(request);
                var bytes = Encoding.UTF8.GetBytes(json + "\n");
                await _stream.WriteAsync(bytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось отправить сообщение на сервер.", ex);
            }
        }

        public async Task ListenAsync()
        {
            while (true)
            {
                try
                {
                    var json = await _reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(json)) continue;

                    var type = JsonConvert.DeserializeObject<ResponseDTO<object>>(json).Type;

                    switch (type)
                    {
                        case ResponseType.Message:
                            {
                                var messageDTO = JsonConvert.DeserializeObject<ResponseDTO<MessageDTO>>(json).Data;
                                _eventAggregator.Publish(new ChatMessageEvent(messageDTO));
                            }
                            break;
                        case ResponseType.GetHistoryChatRoom:
                            {
                                var roomHistoryDTO = JsonConvert.DeserializeObject<ResponseDTO<ChatHistoryDTO>>(json).Data;
                                _eventAggregator.Publish(new ChatHistoryEvent(roomHistoryDTO));
                            }
                            break;
                        case ResponseType.CreatRoomResult:
                            {
                                var resultCreatRoom = JsonConvert.DeserializeObject<ResponseDTO<CreatChatRoomResultDTO>>(json).Data;
                                _eventAggregator.Publish(new CreatRoomEvent(resultCreatRoom));
                            }
                            break;
                        case ResponseType.SearchChatsResult:
                            {
                                var resultSearchChats = JsonConvert.DeserializeObject<ResponseDTO<SeachChatResultDTO>>(json).Data;
                                _eventAggregator.Publish(new SearchChatsEvent(resultSearchChats));
                            }
                            break;                        
                        case ResponseType.JoinInChatRoomResult:
                            {
                                var resultJoinInChatRoom = JsonConvert.DeserializeObject<ResponseDTO<JoinInChatRoomResultDTO>>(json).Data;
                                _eventAggregator.Publish(new AddMemberInChatEvent(resultJoinInChatRoom));
                            }
                            break;
                        case ResponseType.AddContactResult:
                            {
                                var resultAddContact = JsonConvert.DeserializeObject<ResponseDTO<AddContactResultDTO>>(json).Data;
                                _eventAggregator.Publish(new AddContactEvent(resultAddContact));
                            }
                            break;

                    }
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show($"Ошибка сети {ex.Message}", "Ошибка сети", MessageBoxButton.OK, MessageBoxType.Error))
                    {
                        NetworkSession.Dispose();
                        App.Current.Shutdown();
                    }
                }
            }
        }

        public async Task<T> ResponseAsync<T>()
        {
            ResponseDTO<T> responseDTO;
            try
            {
                var bytesRead = 10;
                var response = new List<byte>();

                while ((bytesRead = _stream.ReadByte()) != '\n')
                    response.Add(Convert.ToByte(bytesRead));

                responseDTO = Deserialize<ResponseDTO<T>>(response.ToArray());
                return responseDTO.Data;
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось получить ответ от сервера.", ex);
            }
        }

        private T Deserialize<T>(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(json);
        }

        //Закрытие соединения
        public void Dispose()
        {
            _stream?.Dispose();
            _client?.Dispose();
        }
       
    }
}
