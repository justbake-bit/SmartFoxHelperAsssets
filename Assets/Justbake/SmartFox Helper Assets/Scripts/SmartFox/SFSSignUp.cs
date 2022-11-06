using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Util;

namespace justbake.smartfoxhelper
{
	[RequireComponent(typeof(SFSConnection))]
	public class SFSSignUp : MonoBehaviour, ISignUp
	{
        #region Singleton
    	
		public static SFSSignUp Instance { get; private set; }
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
			login = GetComponent<SFSLogin>();
		}
    	
    	#endregion
    	
    	#region private properties
		// Define SignUp extension command
		private string CMD_SUBMIT = "$SignUp.Submit";
    	#endregion
    	
    	#region public properties
		public SFSConnection connection{get; private set;}
		public SFSLogin login{get; private set;}
    	#endregion
    	
    	#region Editor
		[SerializeField] protected UnityEvent _onSignUpSuccess;
		[SerializeField] protected UnityEvent<string> _onSignUpError;
    	#endregion
    	
    	#region Monobehaviour
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			connection.OnConnectionSuccess += AddSmartFoxListeners;
			connection.OnDisconnect += RemoveSmartFoxListeners;
		}
    	#endregion
        
        #region ISignUp
        
        #region Actions
		public Action SignUpSucess {get; set;}
		public Action<string> SignUpError {get;set;}
        #endregion
        
        #region methods
		public void SignUp(string name, string password, string email)
		{
			ISFSObject sfso = SFSObject.NewInstance();
			sfso.PutUtfString("username", name);
			sfso.PutUtfString("password", password);
			sfso.PutUtfString("email", email);
			
			Debug.Log($"trying to sign up user {name}");
     
			connection.GetSfsClient().Send(new Sfs2X.Requests.ExtensionRequest(CMD_SUBMIT, sfso));
		}
	    #endregion
	    
	    #region smartfox event listeners
		private void OnExtensionResponse(BaseEvent evt)
		{
			string cmd = (string)evt.Params["cmd"];
			SFSObject sfso = (SFSObject)evt.Params["params"];
		    
			if (cmd == CMD_SUBMIT)
			{
				if (sfso.GetBool("success")){
					Debug.Log("Success, thanks for registering");
					SignUpSucess?.Invoke();
					_onSignUpSuccess?.Invoke();
				}
				else
				{
					String error = (string)evt.Params["errorMessage"];
					Debug.LogError("SignUp error:" + error);
					SignUpError?.Invoke(error);
					_onSignUpError?.Invoke(error);
				}
			}
		}
	    #endregion
	    #endregion
	    
	    #region Event listener methods
		protected void AddSmartFoxListeners()
		{
			connection.GetSfsClient().AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
		}
		
		protected void RemoveSmartFoxListeners()
		{
			if(connection.GetSfsClient() != null)
			{
				connection.GetSfsClient().RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
			}
		}
    	#endregion
	}
}
