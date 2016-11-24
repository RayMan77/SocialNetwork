﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BL.DTO;
using BL.Facades;
using Microsoft.AspNet.Identity;
using PL.Models;

namespace PL.Controllers
{
	public class ChatController : Controller
	{
		public ChatFacade ChatFacade { get; set; }
		public UserFacade UserFacade { get; set; }


		public ActionResult Index()
		{
			//neprihlaseny
			if (string.IsNullOrEmpty(User.Identity.GetUserId()))
			{
				var chats = ChatFacade.ListAllUsersChats(UserFacade.GetUserById(1));
				return View(new ChatListModel { Chats = chats });
			}
			var usersChats = ChatFacade.ListAllUsersChats(UserFacade.GetUserById(int.Parse(User.Identity.GetUserId())));
			
			return View(new ChatListModel {Chats = usersChats});
		}


		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Create(FormCollection collection)
		{
			try
			{
				// TODO: Add insert logic here

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}


		public ActionResult Delete(int id)
		{
			var chat = ChatFacade.GetChatById(id);
			return View(chat);


		}

		[HttpPost]
		public ActionResult Delete(ChatDTO chat)
		{
			ChatFacade.DeleteChat(chat);
			return RedirectToAction("Index");


		}

		public ActionResult OpenChat(int id )
		{
			var chat = ChatFacade.GetChatById(id);
			return View(new OpenChatModel { Chat = chat , ChatMessages = ChatFacade.GetChatMessagesFromChat(chat),ChatId = chat.ID});
		}

		[HttpPost]
		public ActionResult OpenChat(OpenChatModel model)
		{
			var user = UserFacade.GetUserById(1);
			var chat = ChatFacade.GetChatById(model.ChatId);
			ChatFacade.SendChatMessageToChat(chat, user, model.NewChatMessage);
			return RedirectToAction("OpenChat",new {id=chat.ID});
		}


		public ActionResult DeleteMessage(int id)
		{
			var msg = ChatFacade.GetChatMessageById(id);
			return View(new ChatMessageModel {ChatMessage = msg,ChatId = msg.Chat.ID});
		}

		[HttpPost]
		public ActionResult DeleteMessage(ChatMessageModel model)
		{
			ChatFacade.DeleteChatMessage(model.ChatMessage);
			return RedirectToAction("OpenChat", new {id = model.ChatId});
		}

		public ActionResult EditMessage(int id)
		{
			var msg = ChatFacade.GetChatMessageById(id);
			return View(new ChatMessageModel { ChatMessage = msg, ChatId = msg.Chat.ID });

		}
		[HttpPost]
		public ActionResult EditMessage(ChatMessageModel model)
		{
			ChatFacade.EditChatMessage(model.ChatMessage,model.ChatMessage.Message);
			return RedirectToAction("OpenChat", new { id = model.ChatId });

		}
	}
}
