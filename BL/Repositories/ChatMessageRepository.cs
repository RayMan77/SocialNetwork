﻿using DAL.Entities;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.EntityFramework;

namespace BL.Repositories
{
	public class ChatMessageRepository : EntityFrameworkRepository<ChatMessage, int>
	{
		public ChatMessageRepository(IUnitOfWorkProvider provider) : base(provider)
		{
		}
	}
}