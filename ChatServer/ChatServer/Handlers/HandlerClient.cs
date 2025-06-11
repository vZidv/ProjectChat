using ChatServer.Models;
using ChatShared.DTO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Handlers
{
    public class HandlerClient
    {
        private static readonly ConcurrentDictionary<string, ClientSession> _sessions = new();
        private static readonly ConcurrentDictionary<int, List<ClientSession>> _roomClients = new();


        public void AddClient(LoginResultDTO loginResult, NetworkStream stream)
        {
            var session = new ClientSession()
            {
                ClientId = loginResult.ClientId,
                Token = loginResult.Token,
                Stream = stream,
                CurrentRoomId = -1
            };
            _sessions.TryAdd(loginResult.Token, session);
        }
        
        public ClientSession? TryGetSession(string token)
        {
            ClientSession? session = null;
            _sessions.TryGetValue(token, out session);
            return session;
        }
        public ClientSession? TryRemoveSession(string token)
        {
            ClientSession? session = null;
            _sessions.TryRemove(token, out session);
            return session;
        }

        public void ClientInRoom(ClientSession client, int IdRoom)
        {
            if (!_roomClients.ContainsKey(IdRoom))
                _roomClients[IdRoom] = new List<ClientSession>();

            if(client.CurrentRoomId > 0)
                _roomClients[IdRoom].Remove(client);

            _roomClients[IdRoom].Add(client);
            client.CurrentRoomId = IdRoom;
        }

        public ClientSession[]? GetClientsFromRoom(int IdRoom)
        {
            if (!_roomClients.ContainsKey(IdRoom))
                return null;
            return _roomClients[IdRoom].ToArray();
        }
    }
}
