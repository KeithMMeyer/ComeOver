using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Com.Mustang.CoMoVR {
    public class LobbyGameManager : MonoBehaviourPunCallbacks
    {
        // set up for the unity side of making buttons work
        [SerializeField]
        private Text lobbyPageNumber;
        [SerializeField]
        public Button[] buttons;
        [SerializeField]
        private Button incrementer;
        [SerializeField]
        private Button decrementer;

        // the current page of rooms currently on display in the lobby
        private int currentLobbyPage = 0;

        // an array of the roomnames to allow for easy connection
        public string[] roomyNames;

        // gets the names of 6 or less rooms to attach to buttons on any page
        public string[] getLobbyNames()
        {
            int j = currentLobbyPage * 6;
            string[] currentPageNames = new string[6];
            for (int i = 0; i < 6 && (j + i) < roomyNames.Length; i++)
            {
                currentPageNames[i] = roomyNames[j + i];
            }
            Debug.Log(roomyNames.Length + "");
            return currentPageNames;
        }

        // attaches the appropriate methods to each button
        private void initializeButtons()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                Button button = buttons[i];

                int buttonIndex = i;
                button.onClick.AddListener(() => testButton(button.GetComponentInChildren<Text>().text));
            }
            incrementer.onClick.AddListener(() => changePage(1));
            decrementer.onClick.AddListener(() => changePage(-1));
        }

        // used to setup the text for each gameobject on the page that needs updated
        // Called whenever the page is changed
        private void setupCurrentLobbypage()
        {
            //gets the correct info for each button that will lead to a lobby to present and puts it into the ui
            double numPages = roomyNames.Length / 6.0;
            lobbyPageNumber.text = (currentLobbyPage + 1) + " of " + Math.Ceiling(numPages);
            string[] roomsOnPage = getLobbyNames();
            Debug.Log(roomsOnPage[0] + "");
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponentInChildren<Text>().text = roomsOnPage[i]; //will change text to lobby name;
            }
        }

        // Changes the page if viable by changing the currentLobbyPage number
        private void changePage(int changer)
        {
            double numPages = roomyNames.Length / 6.0;
            if ((changer == -1 && currentLobbyPage == 0) || (changer == 1 && (currentLobbyPage + 1) == Math.Ceiling(numPages)))
            {
                Debug.Log("Nothing Changed");
            }
            else
            {
                currentLobbyPage += changer;
                Debug.Log("Something Changed");
                setupCurrentLobbypage();
            }

        }

        // Start is called before the first frame update
        // Starts up and initializes everything
        void Start()
        {
            Connect();
            initializeButtons();
            //getRoomNames(roomyNames);
            //setupCurrentLobbypage();
        }

        // Update is called once per frame
        void Update()
        {

        }

        // A method to see if buttons are using the right values
        public void testButton(string name)
        {
            Debug.Log("" + name);
            PhotonNetwork.JoinRoom(name);
            //PhotonNetwork.JoinRandomRoom();
        }

        // a list of names of rooms to be used in other methods and bridge between methods
        private List<string> roomNames = new List<string>();
        string gameVersion = GlobalSettings.gameVersion;

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("Hi");
            int i = 0;
            foreach (RoomInfo room in roomList)
            {
                i++;
                //add names to a list to use later
                if (!room.RemovedFromList)
                {
                    roomNames.Add(room.Name);
                    Debug.Log($"Room name: {room.Name}");
                }
            }
            Debug.Log(i + "");
            roomyNames = getRoomNames();
            setupCurrentLobbypage();
        }

        // puts roomnames into an array instead of a list and allows for this method to be called and give data to other classes
        public string[] getRoomNames()
        {
            string[] rooms = new string[roomNames.Count];
            Debug.Log("Current num" + roomNames.Count);
            int i = 0;
            foreach (string s in roomNames)
            {
                rooms[i] = s;
                i++;
            }
            return rooms;
        }
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
            }
            else
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Joined lobby");
            //PhotonNetwork.JoinRandomRoom();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to master server");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Error: The server is not running or no rooms are available! Please contact your administrator.");
        }


        public override void OnLeftLobby()
        {
            Debug.Log("Left lobby");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined a Room");
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }
}
