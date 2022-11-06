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
		/// <summary>
		/// A dictionary to store the users variables
		/// </summary>
		protected Dictionary<string, object> _variables {get; private set;}
		
		/// <summary>
		/// A list to store the rooms the user is joined in
		/// </summary>
		private List<Room> _joinedRooms;
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
		
		/// <summary>
		/// Event fired when the user's variable is updated or created on the server.
		/// </summary>
		/// <params>the name of the variable</params>
		/// <params>the value of the variable</params>
		public Action<string, object> VariablesUpdate;
		#endregion
		
		#region constructors
		/// <summary>
		/// The basic constructor of a user needs an id.
		/// </summary>
		/// <param name="id">the id of the user</param>
		/// <param name="name">the name of the user</param>
		/// <param name="variables">the variables of the user</param>
		/// <returns></returns>
		public User(int id, string name = "", Dictionary<string, object> variables=null)
		{
			this.id = id;
			this.name = name;
			if(variables == null) variables = new Dictionary<string, object>();
			this._variables = variables;
			_joinedRooms = new List<Room>();
			AddEventListeners();
		}
		#endregion
		
		#region helper
		private void AddEventListeners()
		{
			JoinedRoom += OnUserJoinedRoom;
			LeftRoom += OnUserLeftRoom;
			VariablesUpdate += OnVariablesUpdate;
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
		#endregion
		
		#region IEquatable
		public bool Equals(User user)
		{
			return this.id == user.id;
		}
		#endregion
    }
}
