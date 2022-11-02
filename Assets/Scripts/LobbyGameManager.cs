using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Com.Mustang.CoMoVR { 
public class LobbyGameManager : MonoBehaviour
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
    public string[] roomNames;

    // gets the names of 6 or less rooms to attach to buttons on any page
    public string[] getLobbyNames()
    {
        int j = currentLobbyPage * 6;
        string[] currentPageNames = new string[6];
        for (int i = 0; i < 6 && (j + i) < roomNames.Length; i++)
        {
            currentPageNames[i] = roomNames[j + i];
        }
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
        double numPages = roomNames.Length / 6;
        lobbyPageNumber.text = (currentLobbyPage + 1) + " of " + Math.Ceiling(numPages);
        string[] roomsOnPage = getLobbyNames();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponentInChildren<Text>().text = roomsOnPage[i]; //will change text to lobby name;
        }
    }

    // Changes the page if viable by changing the currentLobbyPage number
    private void changePage(int changer)
    {
        double numPages = roomNames.Length / 6;
        if ((changer == -1 && currentLobbyPage == 0) || (changer == 1 && currentLobbyPage == Math.Ceiling(numPages)))
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
            Debug.Log("1");
        LobbyConnection con = new LobbyConnection();
        con.Connect();
        initializeButtons();
        setupCurrentLobbypage();
        con.getRoomNames(roomNames);
            Debug.Log("2");
    }

    // Update is called once per frame
    void Update()
    {

    }

    // A method to see if buttons are using the right values
    public void testButton(string name)
    {
        Debug.Log("" + name);
    }
}

public class LobbyConnection : MonoBehaviourPunCallbacks
{
    // a list of names of rooms to be used in other methods and bridge between methods
    private List<string> roomNames = new List<string>();
    string gameVersion = GlobalSettings.gameVersion;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            //add names to a list to use later
            if (!room.RemovedFromList)
            {
                roomNames.Add(room.Name);
                Debug.Log(room.Name);
            }
            Debug.Log(room.Name);
        }
    }

    // puts roomnames into an array instead of a list and allows for this method to be called and give data to other classes
    public void getRoomNames(string[] rooms)
    {
        rooms = new string[roomNames.Count];
        int i = 0;
            Debug.Log("3");
        foreach (string s in roomNames) {
            rooms[i] = s;
            i++;
        }
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
}
}
