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
using ChatShared.Events;
using Newtonsoft.Json;
using ChatClient.CustomControls;

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

        public NetworkService(string host = "127.0.0.1", int port = 8888)
        {
            _host = host;
            _port = port;
        }
        public NetworkService(IEventAggregator eventAggregator) :base()
        {
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

        public async Task ListenMessageAsync()
        {
            while (true)
            {
                try
                {
                    var bytesRead = 10;
                    var response = new List<byte>();

                    while ((bytesRead = _stream.ReadByte()) != '\n')
                        response.Add(Convert.ToByte(bytesRead));

                    var type = Deserialize<ResponseDTO<Object>>(response.ToArray()).Type;

                    switch (type)
                    {
                        case ResponseType.Message:
                            var messageDTO = Deserialize<ResponseDTO<ChatMessageDTO>>(response.ToArray()).Data;
                            _eventAggregator.Publish(new ChatMessageEvent(messageDTO));
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сети {ex.Message}", "Ошибка сети",MessageBoxButton.OK,MessageBoxType.Error);
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
