using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace justbake.smartfoxhelper
{
	[Serializable]
	public class User: IEquatable<User>
	{
		#region private properties
		#endregion
		#region public properties
		/// <summary>
		/// The id of the user
		/// </summary>
		public int id {get; private set;}
		/// <summary>
		/// The name of the user
		/// </summary>
		public string name {get; private set;}
		/// <summary>
		/// the private array to store the id of the rooms the user is joined in
		/// </summary>
		private List<Room> _joinedRooms;
		public List<Room> joinedRooms {
			get {
				return _joinedRooms;
			}
		}
		#endregion
		
		#region Events
		/// <summary>
		/// Event fired when this user joins a room
		/// </summary>
		/// <params>the id of the room the user joined</params>
		public Action<Room> JoinedRoom {get; set;}
		/// <summary>
		/// Event fired when this user leaves a room
		/// </summary>
		/// <params>the id of the room the user left</params>
		public Action<Room> LeftRoom {get; set;}
		#endregion
		
		#region constructors
		/// <summary>
		/// The basic constructor of a room needs an id.
		/// </summary>
		/// <param name="id">the id of the room</param>
		/// <param name="MaxUsers">maximum amout of users the room can have joined</param>
		/// <returns></returns>
		public User(int id){
			this.id = id;
			_joinedRooms = new List<Room>();
			
			AddEventListeners();
		}
    	
		/// <summary>
		/// Constructs a room from a smartfox room
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		public User(Sfs2X.Entities.User user): this(user.Id) {
			this.name = user.Name;
		}
		#endregion
		
		#region helper
		private void AddEventListeners()
		{
			JoinedRoom += OnUserJoinedRoom;
			LeftRoom += OnUserLeftRoom;
		}
		
		public Room IsInRoom(int id)
		{
			foreach(Room joinedRoom in _joinedRooms)
			{
				if(joinedRoom.id == id) return joinedRoom;
			}
			return null;
		}
		
		public bool IsInRoom(Room room){
			foreach(Room joinedRoom in _joinedRooms)
			{
				if(joinedRoom.Equals(room)) return true;
			}
			return false;
		}
		#endregion
		
		#region EventListeners
		private void OnUserJoinedRoom(Room room) 
		{
			_joinedRooms.Add(room);
		}
		
		private void OnUserLeftRoom(Room room)
		{
			_joinedRooms.Remove(room);
		}
		#endregion
		
		#region IEquatable
		public bool Equals(User user)
		{
			return this.id == user.id;
		}
		#endregion
    }
}
