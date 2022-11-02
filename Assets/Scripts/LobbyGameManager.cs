using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGameManager : MonoBehaviour
{
    [SerializeField]
    private Text lobbyPageNumber;
    [SerializeField]
    public Button[] buttons;

    private int curretLobbyPage = 0;

    public string[] getLobbyNames()
    {
        return null;
        //Gets the name of 5 or so lobbies to put with each button
    }

    private void initializeButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];

            int buttonIndex = i;
            button.onClick.AddListener(() => testButton(button.GetComponentInChildren<Text>().text));
        }
    }

    private void setupCurrentLobbypage()
    {
        //gets the correct info for each button that will lead to a lobby to present and puts it into the ui
        for (int i =0; i < buttons.Length; i++)
        {
            buttons[i].GetComponentInChildren<Text>().text = "LobbyName"; //will change text to lobby name;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        initializeButtons();
        setupCurrentLobbypage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void testButton(string name)
    {
        Debug.Log("" + name);
    }
}
