using UnityEditor;
using UnityEngine;

namespace justbake.smartfoxhelper
{
	[CustomEditor(typeof(SFSRoom))]
	public class SFSRoomEditor : Editor
	{
		SerializedProperty SFSRoomProperty;
		int id = 0;
		// This function is called when the object is loaded.
		protected void OnEnable()
		{
			SFSRoomProperty = serializedObject.FindProperty("SFSRoom");
		}
        
		public override void OnInspectorGUI()
		{
			
			if(Application.isPlaying) {
				SFSRoom _room = (target as SFSRoom);
				
				if(_room.connection.IsConnected && _room.login.IsLoggedIn)
				{
					
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
				}
				
			}
			
			DrawDefaultInspector();
		}
    }
}
