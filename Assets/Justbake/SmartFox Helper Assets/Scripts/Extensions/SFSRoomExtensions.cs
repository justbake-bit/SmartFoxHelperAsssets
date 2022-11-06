using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace justbake.smartfoxhelper
{
	public static class SFSRoomExtensions
    {
	    public static List<User> UsersList(this Sfs2X.Entities.Room room) {
	    	List<User> users = new List<User>();
	    	
	    	foreach(Sfs2X.Entities.User user in room.UserList)
	    	{
	    		users.Add(new User(user.Id, user.Name, user.Variables()));
	    	}
	    	
	    	return users;
	    }
	    
	    public static Dictionary<string, object> Variables(this Sfs2X.Entities.Room room)
	    {
	    	Dictionary<string, object> variables = new Dictionary<string, object>();
	    	foreach ( Sfs2X.Entities.Variables.RoomVariable item in room.GetVariables())
	    	{
	    		variables.Add(item.Name, item.Value);
	    	}
	    	return variables;
	    }
    }
}
