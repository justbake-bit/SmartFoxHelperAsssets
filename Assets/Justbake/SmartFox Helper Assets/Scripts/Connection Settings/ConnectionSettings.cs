using System;

using UnityEngine;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;
using Sfs2X.Util;

namespace justbake.smartfoxhelper
{
	[CreateAssetMenu(menuName="Multiplayer/ConnectionSettings")]
	[Serializable]
	public class ConnectionSettings : ScriptableObject
	{
		[Space(20)]
		[Header("SFS2X connection settings")]
	
		[Tooltip("IP address or domain name of the SmartFoxServer instance; if encryption is enabled, a domain name must be entered")]
		public string host = "127.0.0.1";
	
		[Tooltip("TCP listening port of the SmartFoxServer instance, used for TCP socket connection in all builds except WebGL")]
		public int tcpPort = 9933;
	
		[Tooltip("UDP listening port of the SmartFoxServer instance, used for UDP communication")]
		public int UdpPort = 9933;
	
		[Tooltip("HTTP listening port of the SmartFoxServer instance, used for WebSocket (WS) connections in WebGL build")]
		public int httpPort = 8080;
	
		[Tooltip("HTTPS listening port of the SmartFoxServer instance, used for WebSocket Secure (WSS) connections in WebGL build and connection encryption in all other builds")]
		public int httpsPort = 8443;
	
		[Tooltip("Use SmartFoxServer's HTTP tunneling (BlueBox) if TCP socket connection can't be established; not available in WebGL builds")]
		public bool useHttpTunnel = false;
	
		[Tooltip("Enable SmartFoxServer protocol encryption; 'host' must be a domain name and an SSL certificate must have been deployed")]
		public bool encrypt = false;
	
		[Tooltip("Wether or not the user needs a password to login")]
		public bool usePassword = false;
	
		[Tooltip("Name of the SmartFoxServer Zone to join")]
		public string zone = "BasicExamples";
	
		[Tooltip("Display SmartFoxServer client debug messages")]
		public bool debug = false;
	
		[Tooltip("Client-side SmartFoxServer logging level")]
		public LogLevel logLevel = LogLevel.INFO;
	}
}
