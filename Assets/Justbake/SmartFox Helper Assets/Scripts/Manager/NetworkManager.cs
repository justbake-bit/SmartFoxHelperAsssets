using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using justbake.smartfoxhelper.interfaces;

namespace justbake.smartfoxhelper
{
	[RequireComponent(typeof(IConnection))]
	[RequireComponent(typeof(ILogin))]
	[RequireComponent(typeof(IRoom))]
	[RequireComponent(typeof(ISignUp))]
    public class NetworkManager : MonoBehaviour
    {
        #region Singleton
	    private static NetworkManager _instance;
	    
	    public static NetworkManager Instance{
	    	get{
	    		return _instance;
	    	}
	    }
	    
	    private void Awake() 
	    {
	    	if(_instance == null) 
	    	{
	    		_instance = this;
	    	}else
	    	{
	    		Destroy(this);
	    	}
		    DontDestroyOnLoad(this);
			
		    Application.runInBackground = true;
		    
		    connection = GetComponent<IConnection>();
		    login = GetComponent<ILogin>();
		    room = GetComponent<IRoom>();
		    signup = GetComponent<ISignUp>();
	    }
	    #endregion
	    
	    [SerializeField] private GameObject NetworkErrorPanel;
	    [SerializeField] private TMP_Text errorText;
	    
	    public IConnection connection;
	    public ILogin login;
	    public IRoom room;
	    public ISignUp signup;
	    
	    public void SetNetworkError(string message){
	    	NetworkErrorPanel.SetActive(true);
	    	errorText.text = message;
	    }
	    
	    public void ClearNetworkError()
	    {
	    	NetworkErrorPanel.SetActive(false);
	    }
    }
}
