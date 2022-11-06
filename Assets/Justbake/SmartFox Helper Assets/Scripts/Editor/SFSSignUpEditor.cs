using UnityEngine;
using UnityEditor;

namespace justbake.smartfoxhelper
{
	[CustomEditor(typeof(SFSSignUp))]
	public class SFSSignUpEditor : Editor
    {
	    SerializedProperty SFSLoginProperty;
	    string name = "";
	    string email = "";
	    string password = "";
	    
	    // This function is called when the object is loaded.
	    protected void OnEnable()
	    {
		    SFSLoginProperty = serializedObject.FindProperty("SFSSignUp");
	    }
	    
	    public override void OnInspectorGUI()
	    {
		    DrawDefaultInspector();
		    
		    if(Application.isPlaying) {
			    SFSSignUp _signUp = (target as SFSSignUp);
			    if(_signUp.connection.IsConnected && _signUp.login.IsLoggedIn) {
			    	
			    	name = EditorGUILayout.TextField("username:", name);
			    	email = EditorGUILayout.TextField("email:", email);
			    	password = EditorGUILayout.TextField("password:", password);
			    	
			    	if(GUILayout.Button("SignUp")) {
				    	_signUp.SignUp(name, password, email);
			    	}
			    	
			    }
		    }
	    }
    }
}
