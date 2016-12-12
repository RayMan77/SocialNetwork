﻿using System.Web.Mvc;
using Antlr.Runtime.Tree;
using BL.Facades;
using Microsoft.AspNet.Identity;
using PL.Models;
using Utils.Enums;

namespace PL.Controllers
{
	[Authorize]
	public class PostController : Controller
	{
		[HttpPost]
		public ActionResult Create(DefaultPageModel model)
		{
			var account = userFacade.GetUserById(int.Parse(User.Identity.GetUserId()));
			if (model.BackPage == null) model.BackPage = "FrontPage";
			if (model.Group != null)
			{
				var group = groupFacade.GetGroupById(model.Group.ID);
				model.NewPost.PrivacyLevel = account.DefaultPostPrivacy;
				postFacade.SendPost(model.NewPost, account, group);

				return RedirectToAction(model.BackPage, "Page", new {groupId = model.Group.ID});
			}

			postFacade.SendPost(model.NewPost, account);

			return RedirectToAction(model.BackPage, "Page");
		}

		public ActionResult Edit(int id, string viewName = "FrontPage")
		{
			var post = postFacade.GetPostById(id);
			if (post.Sender.ID != int.Parse(User.Identity.GetUserId()))
				return RedirectToAction("AccessDenied", "Page");
			return View(new PostEditModel {Post = post, BackView = viewName});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(PostEditModel model)
		{
			postFacade.UpdatePostMessage(model.Post);
			return ResolveBackView(model);
		}

		public ActionResult Delete(int id, string viewName = "FrontPage")
		{
			var post = postFacade.GetPostById(id);
			if (post.Sender.ID != int.Parse(User.Identity.GetUserId()))
				return RedirectToAction("AccessDenied", "Page");
			return View(new PostEditModel {Post = post, BackView = viewName});
		}

		[HttpPost]
		public ActionResult Delete(PostEditModel model)
		{
			postFacade.DeletePost(model.Post.ID);
			return ResolveBackView(model);
		}


		public ActionResult Reactions(int id, string viewName = "FrontPage", int page = 1)
		{
			var post = postFacade.GetPostById(id);
			var user = userFacade.GetUserById(int.Parse(User.Identity.GetUserId()));

			if (post.Sender.ID != user.ID)
				if ((post.PrivacyLevel == PostPrivacyLevel.OnlyYou) ||
					((post.PrivacyLevel == PostPrivacyLevel.OnlyFriends) && !userFacade.AreUsersFriends(user.ID, post.Sender.ID))
					|| ((post.Group != null) && !userFacade.ListGroupsWithUser(user).Contains(post.Group))
					|| ((post.Group != null) && (post.Group.GroupPrivacyLevel != GroupPrivacyLevel.Public)))
					return RedirectToAction("AccessDenied", "Page");

			var comments = postFacade.GetCommentsOnPost(post, page);
			var reactions = postFacade.GetReactionsOnPost(post);
			var userReaction = postFacade
				.GetReactionOfUserOnPost(post, userFacade.GetUserById(int.Parse(User.Identity.GetUserId())))?
				.UserReaction;

			return View(new OpenPostModel
			{
				Post = post,
				Comments = comments,
				Reactions = reactions,
				PostId = post.ID,
				BackView = viewName
				,
				UserReaction = userReaction
				,
				Page = page
			});
		}

		[HttpPost]
		public ActionResult CreateComment(OpenPostModel model)
		{
			var user = userFacade.GetUserById(int.Parse(User.Identity.GetUserId()));
			var post = postFacade.GetPostById(model.PostId);

			postFacade.CommentPost(post, model.NewComment, user);
			return RedirectToAction("Reactions", new {id = model.PostId, viewName = model.BackView});
		}



		public ActionResult DeleteComment(int id, string backView = "FrontPage")
		{
			var user = userFacade.GetUserById(int.Parse(User.Identity.GetUserId()));
			var comment = postFacade.GetCommentById(id);
			if (comment.Sender.ID != user.ID) return RedirectToAction("AccessDenied", "Page");
			return View(new CommentEditModel {BackView = backView, Comment = comment, PostId = comment.Post.ID});
		}

		[HttpPost]
		public ActionResult DeleteComment(CommentEditModel model)
		{
			postFacade.DeleteComment(model.Comment);
			return RedirectToAction("Reactions", new {id =model.PostId, viewName = model.BackView});

		}

		public ActionResult EditComment(int id, string backView = "FrontPage")
		{
			var user = userFacade.GetUserById(int.Parse(User.Identity.GetUserId()));
			var comment = postFacade.GetCommentById(id);
			if (comment.Sender.ID != user.ID) return RedirectToAction("AccessDenied", "Page");

			return View(new CommentEditModel {BackView = backView, Comment = comment, PostId = comment.Post.ID});

		}

		[HttpPost]
		public ActionResult EditComment(CommentEditModel model)
		{
			postFacade.UpdateComment(model.Comment);
			return RedirectToAction("Reactions", "Post", new {id = model.PostId, backView = model.BackView});

		}

		[HttpPost]
		public ActionResult CreateReaction(OpenPostModel model)
		{
			var user = userFacade.GetUserById(int.Parse(User.Identity.GetUserId()));
			var post = postFacade.GetPostById(model.PostId);

			if (model.UserReaction != null)
				postFacade.ReactOnPost(post, model.UserReaction.Value, user);
			else
				postFacade.DeleteReactionFromUserOnPost(user, post);
			return RedirectToAction("Reactions", new {id = model.PostId, viewName = model.BackView});
		}

		#region Utils

		public ActionResult ResolveBackView(PostEditModel model)
		{
			switch (model.BackView)
			{
				case "GroupPage":
					return RedirectToAction("GroupPage", "Page", new {groupId = model.Post.Group.ID});
				case "UserPage":
					return RedirectToAction("UserPage", "Page", new {userId = model.Post.Sender.ID});
				default:
					return RedirectToAction(model.BackView, "Page");
			}
		}

		#endregion

		#region Dependency

		private readonly PostFacade postFacade;
		private readonly UserFacade userFacade;
		private readonly GroupFacade groupFacade;

		public PostController(PostFacade postFacade, UserFacade userFacade, GroupFacade groupFacade, GroupFacade groupFacade1)
		{
			this.postFacade = postFacade;
			this.userFacade = userFacade;
			this.groupFacade = groupFacade1;
		}

		#endregion
	}
}