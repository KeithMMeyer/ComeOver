using UnityEngine;
using System;
using Photon.Pun;
using System.Linq;

namespace Com.Mustang.CoMoVR
{
	public class HeadlessLauncher : MonoBehaviourPunCallbacks
	{
		#region Private Serializable Fields
		/// <summary>
		/// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
		/// The max number is the desired amount of players + 1 for the headless
		/// </summary>
		[Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
		[SerializeField]
		private byte maxPlayersPerRoom = 5;
		#endregion

		#region Private Fields
		/// <summary>
		/// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
		/// </summary>
		string gameVersion = GlobalSettings.gameVersion;

		/// <summary>
		/// This is the name of the room determined by the command line argument "roomNumber"
		/// </summary>
		string roomName;
		#endregion

		#region MonoBehaviour CallBacks
		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during initialization phase.
		/// </summary>
		void Start()
		{
			// This will change to better detect the launch flags
			
			// Reads the command line argument "roomNumber" and uses it as the room name
			roomName = Environment.GetCommandLineArgs().Where(x => x.StartsWith("-roomname")).FirstOrDefault();
			roomName = roomName.Replace("-roomname=", "");
			Debug.LogError("Room Name: " + roomName);
			
			if (roomName == null)
			{
				Debug.LogError("<Color=Red><a>Missing</a></Color> roomNumber in command line arguments. Please set it when you launch the client.");
				Application.Quit();
			}
			else
			{
				Connect();
			}
		}
		#endregion
		
		#region Public Methods
		/// <summary>
		/// Start the connection process.
		/// - if not yet connected, Connect this application instance to Photon Cloud Network
		/// </summary>
		public void Connect()
		{
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = gameVersion;
		}

		public void StartServer()
		{
			// Creates a room with a userlimit that was defined in the maxPlayersPerRoom variable
			// and a name that was defined in the roomName variable
			// The room will also allow userIDs to be broadcast so that clients can determine
			// if the headless server is in the room.
			PhotonNetwork.CreateRoom(roomName, new Photon.Realtime.RoomOptions { MaxPlayers = maxPlayersPerRoom, PublishUserId = true });
		}
		#endregion

		#region GlobalEventListener Callbacks
		public override void OnConnectedToMaster()
		{
			Debug.LogError("Connected to Photon Network");
			Debug.LogError("Starting Server...");
			StartServer();
		}

		public override void OnCreatedRoom()
		{
			// This is called when the headless server has created a room
			// The master client will always be the headless server, and if it leaves,
			// all connected clients will return to the lobby.
			Debug.LogError("Server Started");
			PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
			// All clients synchronize the scene that the master client is running,
			// so launch the scene on the master client. All clients will then
			// automatically load the scene.
			PhotonNetwork.LoadLevel("MainScene");
		}
		#endregion
	}
}