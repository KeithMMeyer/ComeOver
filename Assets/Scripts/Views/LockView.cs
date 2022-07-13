using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LockView : MonoBehaviour, IPunObservable
{
    public UnityEvent OnUnlocked;
    //Photon.Realtime.Player owner;
    [SerializeField]
    private int owner = -1;
    public bool IsLocked = false;

    //public bool hasLock { get { return isLocked && PhotonNetwork.LocalPlayer.ActorNumber == owner; } }
    public bool HasLock { get { return IsLocked && PhotonNetwork.LocalPlayer.ActorNumber == owner; } }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(owner);
            stream.SendNext(IsLocked);
        }
        else
        {
            owner = (int)stream.ReceiveNext();
            IsLocked = (bool)stream.ReceiveNext();

            if (IsLocked)
                if (HasLock)
                {
                    gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/EditColor");
                }
                else
                {
                    gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/LockedColor");
                }
        }
    }

    public bool RequestLock()
    {
        if (!IsLocked || PhotonNetwork.LocalPlayer.ActorNumber == owner)
        {
            IsLocked = true;
            owner = PhotonNetwork.LocalPlayer.ActorNumber;
            return true;
        }
        return false;
    }

    [PunRPC]
    public void RemoteLock(int actorNumber)
    {
        if (!IsLocked || actorNumber == owner)
        {
            IsLocked = true;
            owner = actorNumber;
        }
    }

    public bool Unlock()
    {
        if (!IsLocked || PhotonNetwork.LocalPlayer.ActorNumber == owner)
        {
            IsLocked = false;
            owner = -1;
            OnUnlocked.Invoke();
            return true;
        }
        return false;
    }

    [PunRPC]
    public void RemoteUnlock(int actorNumber)
    {
        if (!IsLocked || actorNumber == owner)
        {
            IsLocked = false;
            owner = -1;
            OnUnlocked.Invoke();
        }
    }

    public bool ForceUnlock() //TODO update to automatically check if owner left room
    {
        IsLocked = false;
        owner = -1;
        OnUnlocked.Invoke();
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
