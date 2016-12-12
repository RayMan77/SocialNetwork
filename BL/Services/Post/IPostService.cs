﻿using System.Collections.Generic;
using BL.DTO.Filters;
using BL.DTO.PostDTOs;

namespace BL.Services.Post
{
	public interface IPostService
	{
		int CreatePost(PostDTO post);

		void EditPostMessage(PostDTO post);

		void DeletePost(int id);

		void DeletePost(PostDTO post);

		PostDTO GetPostById(int id);

		PostListQueryResultDTO ListPosts(PostFilter filter, int page = 0);
		List<ReactionDTO> GetReactionsOnPost(PostDTO post);
	}
}