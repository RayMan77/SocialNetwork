﻿using System.Collections.Generic;
using BL.DTO.GroupDTOs;
using BL.DTO.UserDTOs;
using Utils.Enums;

namespace BL.DTO.Filters
{
	public class PostFilter
	{
		public string Message { get; set; }

		public AccountDTO Sender { get; set; }

		public FrontPageFilter FrontPageFilter { get; set; }

		public bool AreFriends { get; set; }

		public bool CurrentUser { get; set; }

		public PostPrivacyLevel? PrivacyLevel { get; set; }

		public GroupDTO Group { get; set; }
	}

	public class FrontPageFilter
	{
		public AccountDTO Account { get; set; }

		public List<GroupDTO> Groups { get; set; }

		public List<AccountDTO> Friends { get; set; }
	}
}