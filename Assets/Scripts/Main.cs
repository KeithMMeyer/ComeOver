﻿using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class Main : MonoBehaviourPunCallbacks, IPunObservable
{
    public string inputfile;
    Iml iml;
    public string modelName;
    public string routingMode;
    public bool doImport = true;
	string masterClientID;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(iml.structuralModel.name);
            stream.SendNext(iml.structuralModel.routingMode);
            //Debug.Log("Writing!");
        }
        else
        {
            modelName = (string)stream.ReceiveNext();
            routingMode = (string)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
		// Caches the clientID of the headless server to check when they disconnect.
		if (PhotonNetwork.InRoom)
		{
			masterClientID = PhotonNetwork.MasterClient.UserId;
			if (PhotonNetwork.MasterClient.UserId == PhotonNetwork.LocalPlayer.UserId)
			{
				// The below code does not work in terms of muting output from the headless server
				
				// Gets all AudioListener components in the scene and deletes them
				AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();
				Debug.Log("Destroying " + audioListeners.Length + " audio listeners");
				foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
				{
					PhotonVoiceView photonVoiceView = photonView.GetComponent<PhotonVoiceView>();
					if (photonVoiceView != null && photonVoiceView.IsSpeaker)
					{
						Debug.Log("Muting headless output");
						photonVoiceView.SpeakerInUse.GetComponent<AudioSource>().mute = true;
					}
				}
				foreach (AudioListener audioListener in audioListeners)
				{
					Destroy(audioListener);
				}
			}
		}
		
		iml = Iml.GetSingleton();
        if (PhotonNetwork.IsMasterClient)
        {
            if (iml == null && doImport)
            {
                iml = Importer.ImportXml(inputfile);
            }
            else
            {
                if ((iml.structuralModel.classes.Count > 0 && iml.structuralModel.classes[0].gameObject != null))
                    return;
                if (iml == null && !doImport)
                {
                    iml = new Iml();
                    StructuralModel sm = new StructuralModel();
                    List<UserClass> classes = new List<UserClass>();
                    List<Relation> relations = new List<Relation>();
                    sm.classes = classes;
                    sm.relations = relations;
                    iml.structuralModel = sm;
                }

                Debug.LogWarning("Rerendering Iml.");
            }
        } else
        {
            return;
        }

        GenerateClasses(iml);

        foreach (Relation relation in iml.structuralModel.relations)
        {
            string source = relation.source;
            UserClass start = null;
            foreach (UserClass classXml in iml.structuralModel.classes)
            {
                if (classXml.id.Equals(source))
                {
                    start = classXml;
                    classXml.AddRelation(relation);
                    break;
                }
            }

            string destination = relation.destination;
            UserClass end = null;
            foreach (UserClass classXml in iml.structuralModel.classes)
            {
                if (classXml.id.Equals(destination))
                {
                    end = classXml;
                    classXml.AddRelation(relation);
                    break;
                }
            }
            relation.CreateGameObject();
            relation.AttachToClass(start, end);
        }
    }

    private void GenerateClasses(Iml iml)
    {

        foreach (UserClass classXml in iml.structuralModel.classes)
        {
            classXml.CreateGameObject();
        }
    }

	#region MonoBehaviourPunCallbacks Callbacks
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		Debug.Log($"Player {otherPlayer.NickName} left the room");
		if (otherPlayer.UserId == masterClientID)
		{
			Debug.Log("Master client left the room. Returning to lobby.");
			PhotonNetwork.LeaveRoom();
			SceneManager.LoadScene("Launcher");
		}
	}
	#endregion
}