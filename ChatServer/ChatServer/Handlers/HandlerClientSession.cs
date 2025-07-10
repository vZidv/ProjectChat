using ChatServer.Models;
using ChatServer.Session;
using ChatShared.DTO;
using ChatShared.DTO.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Handlers
{
    public class HandlerClientSession
    {
        private static readonly ConcurrentDictionary<string, ClientSession> _sessions = new();
        private static readonly ConcurrentDictionary<int, List<ClientSession>> _roomClients = new();


        public void AddClient(LoginResultDTO loginResult, NetworkStream stream)
        {
            var session = new ClientSession()
            {
                Client = loginResult.ClientProfileDTO,
                Token = loginResult.Token,
                Stream = stream,
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

        public void ClientInChatRoom(ClientSession client, int chatRoomId)
        {
            if (!_roomClients.ContainsKey(chatRoomId))
                _roomClients[chatRoomId] = new List<ClientSession>();

            if (!_roomClients[chatRoomId].Contains(client))
                _roomClients[chatRoomId].Add(client);
        }

        public ClientSession[]? GetClientsFromChatRoom(int chatRoomId)
        {
            if (!_roomClients.ContainsKey(chatRoomId))
                return null;
            return _roomClients[chatRoomId].ToArray();
        }

    }
}
