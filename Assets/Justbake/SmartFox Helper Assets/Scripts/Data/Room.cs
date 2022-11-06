using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sfs2X;

namespace justbake.smartfoxhelper
{
	[Serializable]
	public class Room: IEquatable<Room>
	{
		#region private properties
		/// <summary>
		/// the private array to store user information
		/// </summary>
		private List<User> _users;
		
		/// <summary>
		/// A dictionary to store the rooms variables
		/// </summary>
		protected Dictionary<string, object> _variables {get; private set;}
		#endregion
		
		#region public properties
		/// <summary>
		/// the id of the room on the server
		/// </summary>
		public int id { get; private set;}
		/// <summary>
		/// the name of the room
		/// </summary>
		public string name { get; private set;}
		/// <summary>
		/// the amount of users the room can have
		/// </summary>
		public int MaxUsers{ get; private set;}
		/// <summary>
		/// is the local user joined in the room
		/// </summary>
		public bool isJoined {get; private set;}
		
		/// <summary>
		/// the public getter of the user array
		/// </summary>
		public List<User> users {
			get {
				return _users;
			}
		}
		#endregion
		
		#region Events
		/// <summary>
		/// This event is fired when the room's capacaty changes
		/// </summary>
		/// <param>the the new capacity of the room</param>
		/// <param>the old capacity of the room</param>
		public Action<int, int> CapacityChaged {get; set;}
		/// <summary>
		/// Event fired when a remote user joins the room
		/// </summary>
		/// <param>the id of the user that joined</param>
		public Action<User> UserJoin {get; set;}
		/// <summary>
		/// Event fired when a remote user leaves the room
		/// </summary>
		/// <param>the id of the user that left</param>
		public Action<User> UserLeft {get; set;}
		
		/// <summary>
		/// Event Fired When a room variable is updated
		/// </summary>
		public Action<string, object> VariablesUpdate {get; set;}
    	#endregion
    	
    	#region constructors
		/// <summary>
		/// The basic constructor of a room needs an id.
		/// </summary>
		/// <param name="id">the id of the room</param>
		/// <param name="MaxUsers">maximum amout of users the room can have joined</param>
		/// <returns></returns>
		public Room(int id, int MaxUsers, Dictionary<string, object> variables = null, List<User> users=null)
		{
			this.id = id;
			this.MaxUsers = MaxUsers;
			if(variables == null) variables = new Dictionary<string, object>();
			if(users == null) users = new List<User>(MaxUsers);
			_variables = variables;
			_users = users;
			AddEventListeners();
		}
	    #endregion
	    
		private void AddEventListeners()
		{
			CapacityChaged += OnCapacityChanged;
			UserJoin += OnUserJoinedRoom;
			UserLeft += OnUserLeftRoom;
			VariablesUpdate += OnVariablesUpdate;
		}
	    
		private void OnCapacityChanged(int newCapacity, int oldCapacity) 
		{
			_users.Capacity = newCapacity;
			this.MaxUsers = newCapacity;
		}
		
		private void OnUserJoinedRoom(User user)
		{
			_users.Add(user);
			user.JoinedRoom?.Invoke(this);
		}
		
		private void OnUserLeftRoom(User user)
		{
			_users.Remove(user);
			user.LeftRoom?.Invoke(this);
		}
		
		private void OnVariablesUpdate(string name, object value)
		{
			if(_variables.ContainsKey(name))
			{
				_variables[name] = value;
			}else
			{
				_variables.Add(name, value);
			}
		}
		
		#region Equatable
		public bool Equals(Room room)
		{
			return this.id == room.id;
		}
		#endregion
    }
}
