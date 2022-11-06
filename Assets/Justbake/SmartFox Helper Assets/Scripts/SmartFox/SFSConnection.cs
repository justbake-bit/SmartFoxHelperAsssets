using System;

using UnityEngine;
using UnityEngine.Events;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;
using Sfs2X.Util;

using justbake.smartfoxhelper.interfaces;

namespace justbake.smartfoxhelper
{
	public class SFSConnection : MonoBehaviour, IConnection
	{
    	#region Singleton
    	
		public static SFSConnection Instance { get; private set; }
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
		}
    	
    	#endregion
    	
    	#region private variables
		protected SmartFox _smartFox;
    	#endregion
    	
    	#region Editor variables
		[SerializeField] protected bool _connectOnStart;
		[SerializeField] protected ConnectionSettings _connectionSettings;
		
		[SerializeField] protected UnityEvent _onConnectionSuccess;
		[SerializeField] protected UnityEvent<string> _onConnectionLost;
		[SerializeField] protected UnityEvent<string> _onConnectionFail;
		[SerializeField] protected UnityEvent _onDisconnect;
    	#endregion
    	
    	#region MonoBehaviour
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			if(_connectOnStart) Connect();
		}
		// Update is called every frame, if the MonoBehaviour is enabled.
		protected void Update()
		{
			if(_smartFox != null)
				_smartFox.ProcessEvents();
		}
		
		// Sent to all game objects before the application is quit.
		protected void OnApplicationQuit()
		{
			if(_smartFox != null && _smartFox.IsConnected)
			{
				_smartFox.Disconnect();
			}
		}
    	#endregion
    	
        #region IConnection
        
        #region properties
		public bool IsConnected 
		{
			get
			{
				return _smartFox != null && _smartFox.IsConnected;
			}
		}
		
		public ConnectionSettings connectionSettings{
			get{
				return _connectionSettings;
			}
		}
        #endregion
        
        #region Actions
		public Action OnConnectionSuccess {get;set;}
		public Action OnDisconnect {get; set;}
		public Action<string> OnConnectionFail {get;set;}
		public Action<string> OnConnectionLost {get;set;}
        #endregion
        
        #region Methods
		public void Connect()
		{
			// Set connection parameters
			ConfigData cfg = new ConfigData();
			cfg.Host = _connectionSettings.host;
			cfg.Port = _connectionSettings.tcpPort;
			cfg.UdpHost = _connectionSettings.host;
			cfg.UdpPort = _connectionSettings.UdpPort;
			cfg.Zone = _connectionSettings.zone;
			cfg.Debug = _connectionSettings.debug;
			
			// Initialize SmartFox client
			// The singleton class SFSConnection holds a reference to the SmartFox class instance,
			// so that it can be shared among all the scenes
			_smartFox = new SmartFox();
			
			// Configure SmartFox internal logger
			_smartFox.Logger.EnableConsoleTrace = _connectionSettings.debug;

			// Add event listeners
			AddSmartFoxListeners();

			// Connect to SmartFoxServer
			_smartFox.Connect(cfg);
		
			Debug.Log("Connecting to Smartfox server...");
		}

		public void Disconnect()
		{
			if(_smartFox != null && _smartFox.IsConnected)
			{
				_smartFox.Disconnect();
				Debug.Log("Disconnecting from Smartfox server...");
			}
		}
		#endregion
        #endregion

		#region smartfox event listeners
		public void OnConnectionEvent(BaseEvent evt)
		{
			// Check if the conenction was established or not
			if ((bool)evt.Params["success"])
			{
				Debug.Log("Connected to smartfox");
				Debug.Log("SFS2X API version: " + _smartFox.Version);
				Debug.Log("Connection mode is: " + _smartFox.ConnectionMode);
				OnConnectionSuccess?.Invoke();
				_onConnectionSuccess?.Invoke();
			}
			else
			{
				Debug.Log("Connection failed; is the server running at all?");
				OnConnectionFail?.Invoke("Connection failed; is the server running at all?");
				_onConnectionFail?.Invoke("Connection failed; is the server running at all?");
			}
		}

		public void OnConnectionLostEvent(BaseEvent evt)
		{
			// Remove SFS listeners
			RemoveSmartFoxListeners();

			// Show error message
			string reason = (string)evt.Params["reason"];
			
			Debug.Log("Connection lost; reason is: " + reason);
			OnDisconnect?.Invoke();
			_onDisconnect?.Invoke();
			OnConnectionLost?.Invoke("Connection lost; reason is: " + reason);
			_onConnectionLost?.Invoke("Connection lost; reason is: " + reason);
			_smartFox = null;
		}
		#endregion
        
        #region helper
        
		protected virtual void AddSmartFoxListeners() {
			_smartFox.AddEventListener(SFSEvent.CONNECTION, OnConnectionEvent);
			_smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLostEvent);
		}
        
		protected virtual void RemoveSmartFoxListeners() {
			if(_smartFox != null) 
			{
				_smartFox.RemoveEventListener(SFSEvent.CONNECTION, OnConnectionEvent);
				_smartFox.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLostEvent);
			}
		}
        
		/// <summary>
		/// Gets the existing SmartFox class instance.
		/// </summary>
		/// <returns>the smart fox instance</returns>
		public SmartFox GetSfsClient()
		{
			return _smartFox;
		}
    	
        #endregion
	}
}
