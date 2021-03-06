﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BL.DTO.ChatDTOs;
using BL.DTO.Filters;
using BL.DTO.UserDTOs;
using BL.Queries;
using BL.Repositories;
using Castle.Core.Internal;
using Riganti.Utils.Infrastructure.Core;

namespace BL.Services.Chat
{
	public class ChatService : AppService, IChatService
	{
		public int ChatPageSize => 20;

		#region Dependency

		private readonly ChatRepository chatRepository;

		private readonly ChatListQuery chatListQuery;

		private readonly UserRepository userRepository;


		public ChatService(ChatRepository chatRepository, ChatListQuery chatListQuery
			, UserRepository userRepository)
		{
			this.chatRepository = chatRepository;
			this.chatListQuery = chatListQuery;
			this.userRepository = userRepository;
		}

		#endregion

		#region CreateDelete

		public int CreateChat(ChatDTO chatDto)
		{
			int id;
			using (var uow = UnitOfWorkProvider.Create())
			{
				if (chatDto.ChatUsers.Count == 2)
				{
					var tmp = CheckIfPrivateChatExists(chatDto);
					if (tmp != -1) return tmp;
				}
				var list = chatDto.ChatUsers
					.Select(chatUser => userRepository.GetById(chatUser.ID))
					.ToList();

				var chatName = new StringBuilder();
				if (chatDto.Name.IsNullOrEmpty())
				{
					list.ForEach(u => chatName.Append(u.Name + ", "));
					chatDto.Name = chatName.ToString();
				}
				var chatEnt = new DAL.Entities.Chat
				{
					Name = chatDto.Name
				};
				chatEnt.ChatUsers.AddRange(list);

				chatRepository.Insert(chatEnt);
				uow.Commit();
				id = chatEnt.ID;
			}
			return id;
		}

		public void DeleteChat(int chatId)
		{
			using (var uow = UnitOfWorkProvider.Create())
			{
				chatRepository.Delete(chatId);
				uow.Commit();
			}
		}

		#endregion

		#region Update

		public void RemoveUserFromChat(ChatDTO chat, AccountDTO account)
		{
			using (var uow = UnitOfWorkProvider.Create())
			{
				var chatEnt = chatRepository.GetById(chat.ID, c => c.ChatUsers, c => c.Messages);
				var userEnt = userRepository.GetById(account.ID);

				chatEnt.ChatUsers.Remove(userEnt);

				if (chatEnt.ChatUsers.Count == 0)
				{
					chatRepository.Delete(chatEnt);
					return;
				}
				chatRepository.Update(chatEnt);
				uow.Commit();
			}
		}

		public void AddUserToChat(ChatDTO chat, AccountDTO account)
		{
			using (var uow = UnitOfWorkProvider.Create())
			{
				var chatEnt = chatRepository.GetById(chat.ID);
				var userEnt = userRepository.GetById(account.ID);
				chatEnt.ChatUsers.Add(userEnt);
				chatRepository.Update(chatEnt);
				uow.Commit();
			}
		}

		public void AddUsersToChat(ChatDTO chat, List<AccountDTO> accounts)
		{
			accounts.ForEach(a => AddUserToChat(chat, a));
		}

		public void EditChatName(ChatDTO chatDto)
		{
			using (var uow = UnitOfWorkProvider.Create())
			{
				var chat = chatRepository.GetById(chatDto.ID, c => c.ChatUsers, c => c.Messages);
				chat.Name = chatDto.Name;
				chatRepository.Update(chat);
				uow.Commit();
			}
		}

		#endregion

		#region Get

		public ChatDTO GetChatById(int chatId)
		{
			using (UnitOfWorkProvider.Create())
			{
				var chat = chatRepository.GetById(chatId, c => c.ChatUsers, c => c.Messages);
				return chat != null ? Mapper.Map<ChatDTO>(chat) : null;
			}
		}

		public ChatListQueryResultDTO ListChats(ChatFilter filter, int page = 0)
		{
			using (UnitOfWorkProvider.Create())
			{
				using (UnitOfWorkProvider.Create())
				{
					var query = GetChatQuery(filter);
					query.Skip = (page > 0 ? page - 1 : 0)*ChatPageSize;
					query.Take = ChatPageSize;

					query.AddSortCriteria(s => s.Name, SortDirection.Descending);
					return new ChatListQueryResultDTO
					{
						RequestedPage = page,
						ResultCount = query.GetTotalRowCount(),
						ResultChats = query.Execute(),
						Filter = filter
					};
				}
			}
		}

		public List<AccountDTO> GetUsersInChat(ChatDTO chat)
		{
			using (UnitOfWorkProvider.Create())
			{
				var chatEnt = chatRepository.GetById(chat.ID, c => c.ChatUsers);
				return Mapper.Map<List<AccountDTO>>(chatEnt.ChatUsers);
			}
		}

		#endregion

		#region addiotional

		private IQuery<ChatDTO> GetChatQuery(ChatFilter filter)
		{
			var query = chatListQuery;
			query.ClearSortCriterias();
			query.Filter = filter;
			return query;
		}

		private int CheckIfPrivateChatExists(ChatDTO privateChat)
		{
			var tmpChatList = ListChats(new ChatFilter {Account = privateChat.ChatUsers.FirstOrDefault()});
			foreach (var chatTmp in tmpChatList.ResultChats)
			{
				if (chatTmp.ChatUsers.Count != 2) continue;
				if (chatTmp.ChatUsers.Contains(privateChat.ChatUsers[0])
					&& chatTmp.ChatUsers.Contains(privateChat.ChatUsers[1]))
					return chatTmp.ID;
			}

			return -1;
		}

		#endregion
	}
}