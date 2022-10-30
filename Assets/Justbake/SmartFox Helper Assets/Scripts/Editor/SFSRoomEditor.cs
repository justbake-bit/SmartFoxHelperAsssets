using UnityEditor;
using UnityEngine;

namespace justbake.smartfoxhelper
{
	[CustomEditor(typeof(SFSRoom))]
	public class SFSRoomEditor : Editor
	{
		SerializedProperty SFSRoomProperty;
		string error;
		int id;
		// This function is called when the object is loaded.
		protected void OnEnable()
		{
			SFSRoomProperty = serializedObject.FindProperty("SFSRoom");
			
			SFSRoom _room = (target as SFSRoom);
			_room.OnRoomJoinError += OnRoomJoinError;
		}
        
		public override void OnInspectorGUI()
		{
			
			if(Application.isPlaying) {
				SFSRoom _room = (target as SFSRoom);
				
				if(_room.connection.IsConnected && _room.login.IsLoggedIn)
				{
					Debug.Log( _room.login.user.joinedRooms.Count);
					
					foreach(Room room in _room.login.user.joinedRooms) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.IntField(room.id);
						EditorGUILayout.TextArea(room.name);
						if(GUILayout.Button("Leave")) {
							_room.LeaveRoom(room.id);
						}
						EditorGUILayout.EndHorizontal();
					}
					
					EditorGUILayout.BeginHorizontal();
					id = EditorGUILayout.IntField(id);
					
					if(GUILayout.Button("Join")) {
						_room.JoinRoom(id);
					}
					EditorGUILayout.EndHorizontal();
					
					if(this.error != null) {
						EditorGUILayout.TextArea(error);
					}
				}
				
			}
			
			DrawDefaultInspector();
		}
		
		public void OnRoomJoinError(string error)
		{
			this.error = error;
		}
    }
}
