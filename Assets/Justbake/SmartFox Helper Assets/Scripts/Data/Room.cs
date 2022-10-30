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
    	#endregion
    	
    	#region constructors
		/// <summary>
		/// The basic constructor of a room needs an id.
		/// </summary>
		/// <param name="id">the id of the room</param>
		/// <param name="MaxUsers">maximum amout of users the room can have joined</param>
		/// <returns></returns>
		public Room(int id, int MaxUsers){
			this.id = id;
			this.MaxUsers = MaxUsers;
			
			_users = new List<User>(MaxUsers);
			AddEventListeners();
		}
    	
		/// <summary>
		/// Constructs a room from a smartfox room
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		public Room(Sfs2X.Entities.Room room) : this(room.Id, room.MaxUsers){
	    	this.name = room.Name;
	    }
	    #endregion
	    
		private void AddEventListeners()
		{
			CapacityChaged += OnCapacityChanged;
			UserJoin += OnUserJoinedRoom;
			UserLeft += OnUserLeftRoom;
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
		
		#region Equatable
		public bool Equals(Room room)
		{
			return this.id == room.id;
		}
		#endregion
    }
}
