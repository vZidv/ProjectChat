using ChatServer.Data;
using ChatServer.Models;
using ChatShared.DTO;
using ChatShared.DTO.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChatShared.DTO.Enums;
using ChatServer.Session;

namespace ChatServer.Handlers
{
    public class HandlerChatRoom
    {
        private readonly ProjectChatContext _context;

        public HandlerChatRoom(ProjectChatContext context)
        {
            _context = context;
        }

        public async Task<CreatChatRoomResultDTO> CreatRoomAsync(ChatRoomDTO roomDTO)
        {
            CreatChatRoomResultDTO result = new(true, null, roomDTO);
            if (roomDTO == null)
            {
                result.Success = false;
                result.ErrorMessage = "Room data is null.";
                return result;
            }

            int ChatRoomTypeId = _context.ChatRoomTypes.Where(t => t.Name == "Group").Select(t => t.Id).FirstOrDefault();
            var chatRoom = new ChatRoom()
            {
                Name = roomDTO.Name,
                IsPrivate = false,
                ChatRoomTypeId = ChatRoomTypeId,
                OwnerId = roomDTO.OwnerId
            };
            _context.ChatRooms.Add(chatRoom);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            result.ChatRoomDTO.Id = chatRoom.Id;

            return result;
        }

        public async Task<ChatRoomDTO[]> GetRoomsForClientAsync(int clientId)
        {
            ChatRoom[] rooms = await _context.ChatRooms.Where(x => x.IsPrivate == false || x.OwnerId == clientId).ToArrayAsync();
            var roomDTOs = new ChatRoomDTO[rooms.Length];
            for (int i = 0; i < rooms.Length; i++)
            {
                roomDTOs[i] = new ChatRoomDTO()
                {
                    Id = rooms[i].Id,
                    Name = rooms[i].Name,
                    //OwnerId = rooms[i].OwnerId
                };
            }
            return roomDTOs;
        }

        public async Task<RoomMessageDTO[]> GetHistoryChatRoomAsync(int chatRoomId, ChatType chatType, int senderId)
        {
            RoomMessageDTO[] result = null!;
            Message[] messages = null!;
            switch (chatType)
            {
                case ChatType.Private:
                    {


                    }
                    break;

                case ChatType.Group:
                    {
                        ChatRoom room = await _context.ChatRooms.Where(r => r.Id == chatRoomId).Include(r => r.Messages).ThenInclude(m => m.Client).FirstOrDefaultAsync();

                        messages = room.Messages.ToArray();
                        result = new RoomMessageDTO[messages.Length];

                    }
                    break;
            }

            if (messages.Length == 0)
                return result;

            for (int i = 0; i < messages.Length; i++)
            {
                result[i] = new RoomMessageDTO()
                {
                    Text = messages[i].Text,
                    Sender = messages[i].Client.Login,
                    SentAt = messages[i].SentAt,
                    isEdit = messages[i].IsEdited,

                    RoomId = chatRoomId
                };

            }

            return result;
        }

        public async Task<JoinInChatRoomResultDTO> AddNewMemberInChatRoomAsync(int clientId, int chatRoomId)
        {
            ChatRoomMember chatRoomMember = new()
            {
                ClientId = clientId,
                RoomId = chatRoomId,
                RoleId = 1 //<- - Default role, should be changed later
            };

            JoinInChatRoomResultDTO result = new()
            {
                IsSuccess = true,
            };

            try
            {
                _context.ChatRoomMembers.Add(chatRoomMember);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                throw new Exception($"Error adding member to chat room: {ex.Message}");
            }

            return result;
        }


    }
}
