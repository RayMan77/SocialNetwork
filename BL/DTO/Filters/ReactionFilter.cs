﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTO.UserDTOs;

namespace BL.DTO.Filters
{
	public class ReactionFilter
	{
		public PostDTO Post { get; set; }

		public AccountDTO Account { get; set; }

	}
}
