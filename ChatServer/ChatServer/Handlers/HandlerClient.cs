using ChatServer.Models;
using ChatShared.DTO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Handlers
{
    public class HandlerClient
    {
        private static readonly ConcurrentDictionary<string, ClientSession> _sessions = new();
        private static readonly ConcurrentDictionary<ChatRoomDTO, List<ClientSession>> _roomClients = new();


        public void AddClient(LoginResultDTO loginResult)
        {
            var session = new ClientSession()
            {
                ClientId = loginResult.ClientId,
                Token = loginResult.Token,
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

        public void ClientInRoom(ClientSession client, ChatRoomDTO room)
        {
            if (!_roomClients.ContainsKey(room))
                _roomClients[room] = new List<ClientSession>();

            _roomClients[room].Add(client);
            client.CurrentRoomId = room.Id;
        }

        public ClientSession[]? GetSessionsFromRoom(ChatRoomDTO room)
        {
            if (!_roomClients.ContainsKey(room))
                return null;
            return _roomClients[room].ToArray();
        }
    }
}
