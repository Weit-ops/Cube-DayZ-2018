using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public class VkSettings : MonoBehaviour {

	public  int VkAppId;
	public  string auth_url;
	public  List<String> scope=new List<string>();
	public  bool forceOAtuh;
	public  bool revoke;
	public  string apiVersion;

	public bool notify;
	public bool friends;
	public bool photos;			// 	Доступ к фотографиям.
	public bool audio;			// 	Доступ к аудиозаписям.
	public bool	video;			// 	Доступ к видеозаписям.
	public bool	docs;			// 	Доступ к документам.
	public bool	notes;			// 	Доступ заметкам пользователя.
	public bool pages;			//	Доступ к wiki-страницам.
	public bool status;			// 	Доступ к статусу пользователя.
	public bool offers;			// 	Доступ к предложениям (устаревшие методы).
	public bool	questions;		// 	Доступ к вопросам (устаревшие методы).
	public bool	wall;			// 	Доступ к обычным и расширенным методам работы со стеной.
	//	Внимание, данное право доступа недоступно для сайтов (игнорируется при попытке авторизации).
	public bool groups;			// 	Доступ к группам пользователя.
	public bool messages;		// 	(для Standalone-приложений) Доступ к расширенным методам работы с сообщениями.
	public bool notifications;	// 	Доступ к оповещениям об ответах пользователю.
	public bool stats;			// 	Доступ к статистике групп и приложений пользователя, администратором которых он является.
	public bool	ads;			// 	Доступ к расширенным методам работы с рекламным API.
	public bool offline;		// 	Доступ к API в любое время со стороннего сервера.
	public bool nohttps;		// 	Возможность осуществлять запросы к API без HTTPS.
	//	Внимание, данная возможность находится на этапе тестирования и может быть изменена.
	void Awake(){
		generateScope();
	}
	public void generateScope()
	{
		if(scope!=null)
		   scope.Clear();
		if(notify)scope.Add ("notify");
		if(friends)scope.Add ("friends");
		if(photos)scope.Add("photos");
		if(audio)scope.Add ("audio");
		if (video)scope.Add ("video");
		if (docs)scope.Add ("docs");
		if (notes)scope.Add ("notes");
		if (pages)scope.Add ("pages");
		if (status)scope.Add ("status");
		if (offers)scope.Add ("offers");
		if (questions)scope.Add ("questions");
		if (wall)scope.Add ("wall");
		if (groups)scope.Add ("groups");
		if (messages)scope.Add ("messages");
		if (notifications)scope.Add ("notifications");
		if (stats)scope.Add ("stats");
		if (ads)scope.Add ("ads");
		if (offline)scope.Add ("offline");
		if (nohttps)scope.Add ("nohttps");
	}
}
