using UnityEngine;
using UnityEditor;
namespace justbake.smartfoxhelper
{
	[CustomEditor(typeof(SFSConnection))]
	public class SFSConnectionEditor : Editor
    {
	    SerializedProperty SFSConnectionProperty;
	    
	    // This function is called when the object is loaded.
	    protected void OnEnable()
	    {
	    	SFSConnectionProperty = serializedObject.FindProperty("SFSConnection");
	    }
	    
	    public override void OnInspectorGUI()
	    {
	    	DrawDefaultInspector();
	    	
	    	if(Application.isPlaying) {
	    		SFSConnection _connection = (target as SFSConnection);

		    	if(_connection.IsConnected) {
			    	if(GUILayout.Button("Disonnect")) {
			    		_connection.Disconnect();
			    	}
		    	}else {
		    		if(GUILayout.Button("Connect")) {
			    		_connection.Connect();
		    		}
		    	}
	    	}
	    }
    }
}
