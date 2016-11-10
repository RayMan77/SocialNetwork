﻿using System;
using AutoMapper;
using BL.DTO;
using BL.DTO.ChatDTOs;
using BL.DTO.Filters;
using BL.Queries;
using BL.Repositories;
using Riganti.Utils.Infrastructure.Core;

namespace BL.Services.ChatMessage
{
	public class ChatMessageService : AppService, IChatMessageService
	{

		public int ChatMessagePageSize => 30;
		#region Dependency

		private readonly ChatMessageRepository chatMessageRepository;

		private readonly ChatMessageListQuery chatMessageListQuery;


		private readonly UserRepository userRepository;

		private readonly ChatRepository chatRepository;


		public ChatMessageService(ChatMessageRepository chatMessageRepository, UserRepository userRepository, ChatRepository chatRepository, ChatMessageListQuery chatMessageListQuery)
		{
			this.chatMessageRepository = chatMessageRepository;
			this.userRepository = userRepository;
			this.chatRepository = chatRepository;
			this.chatMessageListQuery = chatMessageListQuery;
		}

		#endregion

		#region CreateDelete
		public int PostMessageToChat(ChatDTO chat, UserDTO user, ChatMessageDTO message)
		{
			using (var uow = UnitOfWorkProvider.Create())
			{
				var chatEnt = chatRepository.GetById(chat.ID);
				var userEnt = userRepository.GetById(user.ID);

				var newChatMessage = Mapper.Map<ChatMessageDTO, DAL.Entities.ChatMessage>(message);
				newChatMessage.Time = DateTime.Now;
				newChatMessage.Sender = userEnt;
				newChatMessage.Chat = chatEnt;
				chatMessageRepository.Insert(newChatMessage);
				uow.Commit();
				return newChatMessage.ID;

			}
		}

		public void DeleteChatMessage(int messageId)
		{
			using (var uow = UnitOfWorkProvider.Create())
			{
				chatMessageRepository.Delete(messageId);
				uow.Commit();
			}
		}

		public void DeleteChatMessage(ChatMessageDTO chatMessage)
		{
			DeleteChatMessage(chatMessage.ID);
		}

		#endregion

		#region Update

		public void EditChatMessage(ChatMessageDTO messageDto)
		{
			using (var uow = UnitOfWorkProvider.Create())
			{
				var message = chatMessageRepository.GetById(messageDto.ID,c=>c.Chat,c=>c.Sender);
				message.Message = messageDto.Message;
				chatMessageRepository.Update(message);
				uow.Commit();
			}
		}

		#endregion

		#region Get

		public ChatMessageDTO GetChatMessageById(int id)
		{
			using (UnitOfWorkProvider.Create())
			{
				var message = chatMessageRepository.GetById(id);
				return message != null ? Mapper.Map<ChatMessageDTO>(message) : null;
			}
		}

		public ChatMessageListQueryResultDTO ListChatMessages(ChatMessageFilter filter, int page = 0)
		{
			using (UnitOfWorkProvider.Create())
			{
				

				var query = GetChatMessageQuery(filter);

				query.Skip = (page > 0 ? page - 1 : 0) * ChatMessagePageSize;
				query.Take = ChatMessagePageSize;

				query.AddSortCriteria(s => s.Time, SortDirection.Descending);

				return new ChatMessageListQueryResultDTO
				{
					RequestedPage = page,
					ResultCount = query.GetTotalRowCount(),
					ResultMessages = query.Execute(),
					Filter = filter
				};

			}
		}


		#endregion

		#region addiotional
		private IQuery<ChatMessageDTO> GetChatMessageQuery(ChatMessageFilter filter = null)
		{
			var query = chatMessageListQuery;
			query.ClearSortCriterias();
			query.Filter = filter;
			return query;
		}
		#endregion


	}
}
