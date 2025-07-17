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
                Console.WriteLine($"Не удалось создать комнату : {result.ErrorMessage}");

                return result;
            }

            int ChatRoomTypeId = _context.ChatRoomTypes.Where(t => t.Name == "Group").Select(t => t.Id).FirstOrDefault();




            ChatRoom chatRoom = new()
            {
                Name = roomDTO.Name,
                ChatRoomTypeId = ChatRoomTypeId,
                OwnerId = roomDTO.OwnerId,
                IsPrivate = roomDTO.IsPrivate,
            };
            if (roomDTO.AvotarBase64 != null)
                chatRoom.AvatarPath = await new HandlerAvatar(_context).SaveAvatarAsync($"{chatRoom.Name}{roomDTO.AvatarExtension}", roomDTO.AvotarBase64);


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

        public async Task<bool> UpdateProfileRoomAsync(ChatMiniProfileDTO chatMiniProfileDTO)
        {
            ChatRoom chatRoom = await _context.ChatRooms.Where(c => c.Id == chatMiniProfileDTO.Id).FirstOrDefaultAsync();

            chatRoom.Name = chatMiniProfileDTO.Name;

            if (chatMiniProfileDTO.AvatarBase64 != null)
                chatRoom.AvatarPath = await new HandlerAvatar(_context).SaveAvatarAsync($"{chatMiniProfileDTO.Name}{chatMiniProfileDTO.AvatarExtension}",chatMiniProfileDTO.AvatarBase64);
            try
            {
                _context.ChatRooms.Update(chatRoom);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось обновить профиль комнаты id: {chatMiniProfileDTO.Id} Error:{ex.Message}");
                return false;
            }
        }

        public async Task<CreatChatRoomResultDTO> CreatPrivateRoomAsync(int[] membersId)
        {
            CreatChatRoomResultDTO result = new(true, null, null);

            int ChatRoomTypeId = await _context.ChatRoomTypes.Where(t => t.Name == "Private").Select(t => t.Id).FirstOrDefaultAsync();

            ChatRoom chatRoom = new()
            {
                Name = null,
                ChatRoomTypeId = ChatRoomTypeId,
                OwnerId = null,
                IsPrivate = true,
            };
            _context.ChatRooms.Add(chatRoom);

            try
            {
                await _context.SaveChangesAsync();

                foreach (var client in membersId)
                    await AddNewMemberInChatRoomAsync(client, chatRoom.Id);

                result.ChatRoomDTO = new ChatRoomDTO()
                {
                    Id = chatRoom.Id,
                    ChatType = ChatType.Private,
                    IsPrivate = true
                };
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;

                Console.WriteLine($"Не удалось создать комнату : {result.ErrorMessage}");
                return result;
            }
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

        public async Task<MessageDTO[]> GetHistoryChatRoomAsync(int chatRoomId, ChatType chatType, int senderId)
        {
            ChatRoom room = await _context.ChatRooms.Where(r => r.Id == chatRoomId).Include(r => r.Messages).ThenInclude(m => m.Client).FirstOrDefaultAsync();

            Message[] messages = room.Messages.ToArray();
            var result = new MessageDTO[messages.Length];

            if (messages.Length == 0)
                return result;

            for (int i = 0; i < messages.Length; i++)
            {
                result[i] = new MessageDTO()
                {
                    Text = messages[i].Text,
                    Sender = messages[i].Client.Name,
                    SentAt = messages[i].SentAt,
                    isEdit = messages[i].IsEdited,

                    RoomId = chatRoomId,
                    IsOwner = messages[i].Client.Id == senderId,
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

            ChatMiniProfileDTO room =  await _context.ChatRooms.Where(r => r.Id == chatRoomId).
                Select(r => new ChatMiniProfileDTO()
                {
                    Id = r.Id,
                    Name = r.Name,
                    IsMember = true,
                    ChatType = ChatType.Group,
                    LastMessaget = string.Empty, // <- - Replace with actual last message
                    LastActivity = DateTime.Now // <- - Replace with actual last activity
                }).FirstOrDefaultAsync();

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
            result.ChatMiniProfileDTO = room;
            return result;
        }


    }
}
