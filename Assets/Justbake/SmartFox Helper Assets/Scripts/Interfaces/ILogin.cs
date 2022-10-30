using System;
using UnityEngine;
using UnityEngine.UI;

using Sfs2X;
using Sfs2X.Entities;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;
using Sfs2X.Util;

namespace justbake.smartfoxhelper.interfaces
{
	public interface ILogin
	{
		User user {get;}
		bool IsLoggedIn {get;}
		Action OnLogin {get; set;}
		Action OnLogout {get; set;}
		Action<string> OnLoginError {get;set;}
		void Login(string name);
		void Login(string name, string password);
		void Logout();
	}
}
