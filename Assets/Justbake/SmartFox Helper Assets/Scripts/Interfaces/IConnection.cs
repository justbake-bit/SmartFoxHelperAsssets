using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace justbake.smartfoxhelper.interfaces
{
	public interface IConnection
	{
		ConnectionSettings connectionSettings{get;}
		bool IsConnected {get;}
		Action OnConnectionSuccess {get;set;}
		Action OnDisconnect {get;set;}
		Action<string> OnConnectionFail {get;set;}
		Action<string> OnConnectionLost {get;set;}
		void Connect();
		void Disconnect();
	}
}
