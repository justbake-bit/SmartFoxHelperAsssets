using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace justbake.smartfoxhelper.interfaces
{
	public interface IRoom
    {
	    Action<Room> OnLocalUserJoinRoom {get; set;}
	    Action<Room> OnLocalUserLeftRoom {get; set;}
	    Action<User, Room> OnRemoteUserJoinRoom {get; set;}
	    Action<User, Room> OnRemoteUserLeftRoom {get; set;}
	    Action<string> OnRoomJoinError {get;set;}
	    void JoinRoom(int id);
	    void LeaveRoom(int id);
    }
}
