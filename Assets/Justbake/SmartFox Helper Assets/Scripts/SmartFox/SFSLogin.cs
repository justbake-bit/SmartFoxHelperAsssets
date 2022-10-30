using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Entities;
using Sfs2X.Requests;
using Sfs2X.Util;

using justbake.smartfoxhelper.interfaces;

namespace justbake.smartfoxhelper 
{
	[RequireComponent(typeof(SFSConnection))]
	public class SFSLogin : MonoBehaviour, ILogin
	{
		#region Singleton
    	
		public static SFSLogin Instance { get; private set; }
		private void Awake() 
		{ 
			// If there is an instance, and it's not me, delete myself.
    
			if (Instance != null && Instance != this) 
			{ 
				Destroy(this); 
			} 
			else 
			{ 
				Instance = this; 
			}
			
			DontDestroyOnLoad(this);
			
			Application.runInBackground = true;
			connection = GetComponent<SFSConnection>();
		}
    	
    	#endregion
    	
    	#region public properties
		public SFSConnection connection{get; private set;}
    	#endregion
    	
    	#region Editor variables
		[SerializeField] protected UnityEvent _onLogin;
		[SerializeField] protected UnityEvent _onLogout;
		[SerializeField] protected UnityEvent<string> _onLoginError;
    	#endregion
    	
    	#region Monobehaviour
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			
			connection.OnConnectionSuccess += AddSmartFoxListeners;
			connection.OnDisconnect += RemoveSmartFoxListeners;
		}
    	#endregion
    	
    	#region ILogin
    	
    	#region private properties
		private User _user;
		private bool _shouldWarn = true;
    	#endregion
    	
    	#region public properties
    	
		public User user {
			get
			{
				return _user;
			}
		}
		
		private bool _loggedIn = false;
		public bool IsLoggedIn
		{
			get
			{
				return _loggedIn;
			}
		}
    	#endregion
    	
    	#region Actions
		public Action OnLogin {get; set;}
		public Action OnLogout {get; set;}
		public Action<string> OnLoginError {get;set;}
    	#endregion
    	
    	#region Methods
		public bool UsePassword {
			get{
				return connection.connectionSettings.usePassword;
			}
		}
    	
		public void Login(string name) {
			_shouldWarn = false;
			Login(name, "");
		}
    	
		public void Login(string name, string password) {
			if(connection.IsConnected) {
				if(connection.connectionSettings.usePassword) {
					connection.GetSfsClient().Send(new LoginRequest(name, password));
				}else
				{
					if(_shouldWarn)
						Debug.LogWarning("Ignoring passwrod because connection settings are set to not use passord");
					connection.GetSfsClient().Send(new LoginRequest(name));
				}
			} else {
				Debug.LogError("You are not connected to the server");
			}
			_shouldWarn = true;
		}
		
		public void Logout(){
			connection.GetSfsClient().Send(new LogoutRequest());
		}
    	#endregion
    	
    	#endregion
    	
    	#region smartfox event listeners
		private void OnLoginEvent(BaseEvent evt)
		{
			Sfs2X.Entities.User user = (Sfs2X.Entities.User)evt.Params["user"];
			Debug.Log("Loged in as " + user.Name);
			_loggedIn = true;
			_user = new User(user);
			OnLogin?.Invoke();
			_onLogin?.Invoke();
		}
		
		private void OnLogoutEvent(BaseEvent evt)
		{
			Debug.Log("Loged out");
			_loggedIn = false;
			_user = null;
			OnLogout?.Invoke();
			_onLogout?.Invoke();
		}

		private void OnLoginErrorEvent(BaseEvent evt)
		{
			// Disconnect
			// NOTE: this causes a CONNECTION_LOST event with reason "manual", which in turn removes all SFS listeners
			connection.GetSfsClient().Disconnect();
			
			Debug.Log("Login failed due to the following error: " + (string)evt.Params["errorMessage"]);
			
			OnLoginError?.Invoke((string)evt.Params["errorMessage"]);
			_onLoginError?.Invoke((string)evt.Params["errorMessage"]);
		}
    	#endregion
    	
    	#region SFSConnection Override
		protected void AddSmartFoxListeners()
		{
			
			connection.GetSfsClient().AddEventListener(SFSEvent.LOGIN, OnLoginEvent);			
			connection.GetSfsClient().AddEventListener(SFSEvent.LOGOUT, OnLogoutEvent);
			connection.GetSfsClient().AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginErrorEvent);
		}
		
		protected void RemoveSmartFoxListeners()
		{
			
			if(connection.GetSfsClient() != null)
			{
				connection.GetSfsClient().RemoveEventListener(SFSEvent.LOGIN, OnLoginEvent);			
				connection.GetSfsClient().AddEventListener(SFSEvent.LOGOUT, OnLogoutEvent);
				connection.GetSfsClient().RemoveEventListener(SFSEvent.LOGIN_ERROR, OnLoginErrorEvent);
			}
		}
    	#endregion
    }
}
