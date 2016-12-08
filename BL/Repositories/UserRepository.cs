﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DAL.Entities;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.EntityFramework;

namespace BL.Repositories
{
	public class UserRepository : EntityFrameworkRepository<Account, int>
	{
		public UserRepository(IUnitOfWorkProvider provider) : base(provider)
		{
		}
	}
}