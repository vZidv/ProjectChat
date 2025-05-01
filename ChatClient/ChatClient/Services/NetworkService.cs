using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

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

        //..

        public NetworkService(string host = "127.0.0.1", int port = 8888)
        {
            _host = host;
            _port = port;
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
        public async Task SendAsync<T>(T data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var bytes = Encoding.UTF8.GetBytes(json + "\n");
                await _stream.WriteAsync(bytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось отправить сообщение на сервер.", ex);
            }
        }

        public async Task<T> ResponseAsync<T>()
        {
            try
            {
                var bytesRead = 10;
                var response = new List<byte>();

                while ((bytesRead = _stream.ReadByte()) != '\n')
                    response.Add(Convert.ToByte(bytesRead));

                return Deserialize<T>(response.ToArray());
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
