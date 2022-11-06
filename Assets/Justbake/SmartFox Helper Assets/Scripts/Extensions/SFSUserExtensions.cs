using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace justbake.smartfoxhelper
{
	public static class SFSUserExtensions
    {
	    public static Dictionary<string, object> Variables(this Sfs2X.Entities.User user)
	    {
	    	Dictionary<string, object> variables = new Dictionary<string, object>();
	    	foreach ( Sfs2X.Entities.Variables.UserVariable item in user.GetVariables())
	    	{
	    		variables.Add(item.Name, item.Value);
	    	}
	    	return variables;
	    }
    }
}
