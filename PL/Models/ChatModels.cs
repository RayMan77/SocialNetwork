﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BL.DTO;
using BL.DTO.UserDTOs;

namespace PL.Models
{
	public class OpenChatModel
	{
		public ChatDTO Chat { get; set; }

		public List<ChatMessageDTO> ChatMessages { get; set; }

		public ChatMessageDTO NewChatMessage { get; set; }

		public int ChatId { get; set; }

		public int Page { get; set; }

		public List<AccountDTO> Accounts { get; set; }
	}

	public class ChatListModel
	{
		public List<ChatDTO> Chats { get; set; }
	}

	public class ChatMessageModel
	{
		public ChatMessageDTO ChatMessage { get; set; }
		public int ChatId { get; set; }
	}
}