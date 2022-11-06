using System;
using UnityEngine;

namespace justbake.smartfoxhelper
{
	public interface ISignUp
	{
		Action SignUpSucess {get; set;}
		Action<string> SignUpError {get;set;}
		void SignUp(string name, string password, string email);
	}
}
