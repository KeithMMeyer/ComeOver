using UnityEngine;
using System;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace Com.Mustang.CoMoVR
{
    public class Launcher : MonoBehaviourPunCallbacks
	{
        #region Private Serializable Fields

        #endregion

        #region Private Fields
        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = GlobalSettings.gameVersion;
        #endregion

        #region MonoBehaviour CallBacks
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
			// #Critical
			// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
			PhotonNetwork.AutomaticallySyncScene = true;
		}

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
			// Determines if the application is running in server mode from the server flag
			if (Environment.GetCommandLineArgs().Contains("-server"))
			{
				Debug.LogError("Loading server");
				SceneManager.LoadScene("HeadlessScene");
			} else
			{
				Debug.Log("Loading client");
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
			if (!PhotonNetwork.IsConnected)
			{
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
			} else
			{
				PhotonNetwork.JoinLobby();
			}
        }
        #endregion

        #region MonoBehaviourPunCallbacks Callbacks
        public override void OnConnectedToMaster()
		{
			Debug.Log("Connected to master server");
			PhotonNetwork.JoinLobby();
        }

		public override void OnJoinedLobby()
		{
			Debug.Log("Joined lobby");
			PhotonNetwork.JoinRandomRoom();
		}

		public override void OnLeftLobby()
		{
			Debug.Log("Left lobby");
		}

		public override void OnRoomListUpdate(List<RoomInfo> roomList)
		{
			int i = 0;
			foreach (RoomInfo room in roomList)
			{
				// Rooms that no longer exist can still show up in roomList.
				// The RemovedFromList parameter will determine if the room
				// is still available.
				if (!room.RemovedFromList)
				{
					//Debug.Log($"Room name: {room.Name}");
					i++;
				}
			}

			//Debug.Log($"There are {i} rooms.");
		}

		public override void OnDisconnected(DisconnectCause cause)
		{
			Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
		}

		public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Error: The server is not running or no rooms are available! Please contact your administrator.");
        }
		#endregion
	}
}