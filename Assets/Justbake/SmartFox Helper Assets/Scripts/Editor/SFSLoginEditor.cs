using UnityEngine;
using UnityEditor;

namespace justbake.smartfoxhelper
{
	[CustomEditor(typeof(SFSLogin))]
	public class SFSLoginEditor : Editor
	{
		SerializedProperty SFSLoginProperty;
		string name;
		string password;
	    
		// This function is called when the object is loaded.
		protected void OnEnable()
		{
			SFSLoginProperty = serializedObject.FindProperty("SFSLogin");
		}
	    
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
	    	
			if(Application.isPlaying) {
				SFSLogin _login = (target as SFSLogin);
				if(_login.connection.IsConnected) {
					if(!_login.IsLoggedIn) {
						name = EditorGUILayout.TextField("username:", name);
						if(_login.UsePassword)
							password = EditorGUILayout.TextField("password:", password);
						
						if(GUILayout.Button("Login")) {
							if(_login.UsePassword)
								_login.Login(name, password);
							else
								_login.Login(name);
						}
					}else
					{
						
						if(GUILayout.Button("Logout")) {
							_login.Logout();
						}
					}
			    	
				}
			}
		}
	}
}
