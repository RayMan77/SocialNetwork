﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTO;
using BL.DTO.Filters;
using BL.DTO.PostDTOs;

namespace BL.Services.Comment
{
	public interface ICommentService
	{
		int CreateComment(CommentDTO comment);

		int CreateComment(UserDTO user, CommentDTO comment, PostDTO post);

		void EditCommentMessage(CommentDTO comment);

		void DeleteComment(int id);

		CommentDTO GetCommentById(int id);

		CommentListQueryResultDTO ListComments(CommentFilter filter, int page=0);

	}
}
