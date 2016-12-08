﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTO;
using BL.DTO.Filters;
using BL.DTO.GroupDTOs;
using BL.DTO.UserDTOs;
using BL.Services.Group;

namespace BL.Facades
{
	public class GroupFacade
	{
		#region Dependency
		private readonly IGroupService groupService;

		public GroupFacade(IGroupService groupService)
		{
			this.groupService = groupService;
		}
		#endregion

		public int CreateNewGroup(GroupDTO group)
		{
			return groupService.CreateGroup(group);
		}

		public void DeleteGroup(GroupDTO group)
		{
			groupService.DeleteGroup(group);
		}

		public void EditGroupName(GroupDTO group)
		{
			groupService.EditGroupName(group);
		}

		public GroupDTO GetGroupById(int id)
		{
			return groupService.GetGroupById(id);
		}

		public void AddUserToGroup(GroupDTO group, AccountDTO account)
		{
			groupService.AddUserToGroup(group,account);
		}

		public void RemoveUserFromGroup(GroupDTO group, AccountDTO account)
		{
			groupService.RemoveUserFromGroup(group,account);
		}

		public List<GroupDTO> ListGroupsWithName(string name , int page=0)
		{
			return groupService.ListGroups(new GroupFilter {Name = name}, page).ResultGroups.ToList();
		}

	

		public List<AccountDTO> ListUsersInGroup(GroupDTO group, int page = 0)
		{
			return groupService.ListUsersInGroup(group);

		}

		public List<GroupDTO> ListAllGroups()
		{
			return groupService.ListGroups(new GroupFilter()).ResultGroups.ToList();

		}

	}
}
