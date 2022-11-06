using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;

using justbake.smartfoxhelper.interfaces;

namespace justbake.smartfoxhelper
{
	[RequireComponent(typeof(SFSLogin), typeof(SFSConnection))]
	public class SFSRoom : MonoBehaviour, IRoom
    {
        #region Singleton
    	
	    public static SFSRoom Instance { get; private set; }
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
		    login = GetComponent<SFSLogin>();
		    connection = GetComponent<SFSConnection>();
	    }
    	
    	#endregion
    	
    	#region public properties
    	public SFSLogin login{get;private set;}
    	public SFSConnection connection{get;private set;}
    	#endregion
    	
    	#region Editor
    	[SerializeField] protected UnityEvent<Room> _onLocalUserJoinRoom;
	    [SerializeField] protected UnityEvent<Room> _onLocalUserLeftRoom;
	    [SerializeField] protected UnityEvent<User, Room> _onRemoteUserJoinRoom;
	    [SerializeField] protected UnityEvent<User, Room> _onRemoteUserLeftRoom;
	    [SerializeField] protected UnityEvent<string> _onRoomJoinError;
	    [SerializeField] protected UnityEvent<string, object> _onRoomVariablesUpdate;
    	#endregion
    	
    	#region Monobehaviour
    	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
    	protected void Start()
    	{
		    connection.OnConnectionSuccess += AddSmartFoxListeners;
		    connection.OnDisconnect += RemoveSmartFoxListeners;
		    
		    OnLocalUserJoinRoom += (Room room) => {
		    	room.UserJoin?.Invoke(login.user);
		    };
		    
		    OnLocalUserLeftRoom += (Room room) => {
		    	room.UserLeft?.Invoke(login.user);
		    };
		    
		    OnRemoteUserJoinRoom += (User user, Room room) => {
		    	room.UserJoin?.Invoke(user);
		    };
		    
		    OnRemoteUserLeftRoom += (User user, Room room) => {
		    	room.UserLeft?.Invoke(user);
		    };
    	}
    	#endregion
    	
    	#region IRoom
    	
    	#region Actions
    	public Action<Room> OnLocalUserJoinRoom {get; set;}
	    public Action<Room> OnLocalUserLeftRoom {get; set;}
	    public Action<User, Room> OnRemoteUserJoinRoom {get; set;}
	    public Action<User, Room> OnRemoteUserLeftRoom {get; set;}
	    public Action<string> OnRoomJoinError {get;set;}
	    public Action<string, object> OnRoomVariablesUpdate {get;set;}
    	#endregion
    	
    	#region Methods
    	public void JoinRoom(int id)
    	{
    		if(connection.IsConnected && login.IsLoggedIn){
    			connection.GetSfsClient().Send(new JoinRoomRequest(id));
    		}
    	}
    	
	    public void LeaveRoom(int id)
	    {
	    	if(connection.IsConnected && login.IsLoggedIn){
	    		bool left = false;
    			foreach(Sfs2X.Entities.Room room in connection.GetSfsClient().RoomList)
    			{
    				if(room.Id == id)
    				{
    					connection.GetSfsClient().Send(new LeaveRoomRequest(room));
    					left = true;
    					break;
    				}
    			}
    			if(!left)
    			{
    				Debug.LogError("User not in room: " + id);
    			}
    		}
	    }
	    
	    public virtual Room InstanciateRoom(int id, int maxUsers, string group, Dictionary<string, object> variables, List<User> users) 
	    {
	    	return new Room(id, maxUsers, variables, users);
	    }
    	#endregion
    	
    	#endregion
    	
    	#region smartfox event listeners
    	private void OnLocalUserJoinedRoomEvent(BaseEvent evt)
    	{
    		Sfs2X.Entities.Room sfsRoom = (Sfs2X.Entities.Room)evt.Params["room"];
    		
    		Debug.Log("Local user Joined room: " + sfsRoom.Id + " " + sfsRoom.Name);
    		
	    	Room room = InstanciateRoom(sfsRoom.Id, sfsRoom.MaxUsers, sfsRoom.GroupId, sfsRoom.Variables(), sfsRoom.UsersList());
	    	OnLocalUserJoinRoom?.Invoke(room);
    	}
    	
    	private void OnUserLeftRoomEvent(BaseEvent evt)
    	{ 
    		Sfs2X.Entities.Room sfsRoom = (Sfs2X.Entities.Room)evt.Params["room"];
	    	Sfs2X.Entities.User sfsUser = (Sfs2X.Entities.User)evt.Params["user"];
	    	
	    	Room room = InstanciateRoom(sfsRoom.Id, sfsRoom.MaxUsers, sfsRoom.GroupId, sfsRoom.Variables(), sfsRoom.UsersList());
	    	User user = login.InstanciateUser(sfsUser.Id, sfsUser.Name, sfsUser.Variables());
	    	
	    	if(login.user.Equals(user)){
	    		Debug.Log("Local user left room: " + sfsRoom.Id + " " + sfsRoom.Name);
	    		OnLocalUserLeftRoom?.Invoke(room);
	    	}else{
	    		Debug.Log("Remote user: " + sfsUser.Id + " "  + sfsUser.Name + " left room: " + sfsRoom.Id + " " + sfsRoom.Name);
	    		OnRemoteUserLeftRoom?.Invoke(user, room);
	    	}
    	}
    	
    	private void OnRemoteUserJoinedRoomEvent(BaseEvent evt)
    	{
    		
    		Sfs2X.Entities.Room sfsRoom = (Sfs2X.Entities.Room)evt.Params["room"];
	    	Sfs2X.Entities.User sfsUser = (Sfs2X.Entities.User)evt.Params["user"];
	    	
	    	Debug.Log("Remote user: " + sfsUser.Id + " "  + sfsUser.Name + " joined room: " + sfsRoom.Id + " " + sfsRoom.Name);
	    	
	    	Room room = null;
	    	User user = null;// = new User(sfsUser);
	    	OnRemoteUserLeftRoom?.Invoke(user, room);
    	}
    	
    	private void OnJoinRoomError(BaseEvent evt)
    	{
    		String errorMessage = (String)evt.Params["errorMessage"];
    		OnRoomJoinError?.Invoke(errorMessage);
    	}
    	
    	private void JoinDefaultRoomOnLogin()
    	{
    		JoinRoom(0);
    	}
    		
    	private void OnRoomVariablesUpdateEvent(BaseEvent evt)
    	{
    		Sfs2X.Entities.Room sfsRoom = (Sfs2X.Entities.Room) evt.Params["room"];
    		List<string> variableNames = (List<string>) evt.Params["changedVars"];
    		
    		foreach(Room room in login.user.joinedRooms)
    		{
    			if(room.id == sfsRoom.Id)
    			{
	    			foreach(string name in variableNames)
	    			{
    					room.VariablesUpdate?.Invoke(name, sfsRoom.GetVariable(name));
	    			}
    			}
    		}
    	}
    	
    	#endregion
    	
    	#region SFSConnection Override
	    protected void AddSmartFoxListeners()
	    {
		    connection.GetSfsClient().AddEventListener(SFSEvent.ROOM_JOIN, OnLocalUserJoinedRoomEvent);
		    connection.GetSfsClient().AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserLeftRoomEvent);
			connection.GetSfsClient().AddEventListener(SFSEvent.USER_ENTER_ROOM, OnRemoteUserJoinedRoomEvent);
		    connection.GetSfsClient().AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnJoinRoomError);
		    connection.GetSfsClient().AddEventListener(SFSEvent.ROOM_VARIABLES_UPDATE, OnRoomVariablesUpdateEvent);
	    }
		
	    protected void RemoveSmartFoxListeners()
	    {
			
		    if(connection.GetSfsClient() != null)
		    {
			    connection.GetSfsClient().RemoveEventListener(SFSEvent.ROOM_JOIN, OnLocalUserJoinedRoomEvent);
			    connection.GetSfsClient().RemoveEventListener(SFSEvent.USER_EXIT_ROOM, OnUserLeftRoomEvent);
			    connection.GetSfsClient().RemoveEventListener(SFSEvent.USER_ENTER_ROOM, OnRemoteUserJoinedRoomEvent);
			    connection.GetSfsClient().RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR, OnJoinRoomError);
			    connection.GetSfsClient().RemoveEventListener(SFSEvent.ROOM_VARIABLES_UPDATE, OnRoomVariablesUpdateEvent);
		    }
	    }
    	#endregion
    }
}
