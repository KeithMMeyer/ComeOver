using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class DragObject : MonoBehaviour
{
    public bool dragParent = false;
    protected XRSimpleInteractable interactable;
    private Transform active;
    protected Transform trash;
    protected UserClass storage;
    protected LockView lockView;

    protected Transform errorPanel;

    protected bool grabbed = false;

    // Start is called before the first frame update
    void Start()
    {
        if (interactable == null)
        {
            Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbed)
            TryUpdatePosition(out _);
    }

    protected bool TryUpdatePosition(out Vector3 newPosition)
    {
        bool found = LinePlaneIntersection(out newPosition, active.transform.GetChild(2).position, active.transform.GetChild(2).forward, new Vector3(0, 0, -1), new Vector3(0, 0, 3));

        if (found)
        {
            Transform movable = dragParent ? transform.parent : transform;
            Vector3 position = newPosition;
            position.z = movable.position.z;
            movable.position = position;
        }
        return found;
    }

    private void Init()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        interactable.selectEntered.AddListener(delegate (SelectEnterEventArgs args) { if (!grabbed) { active = args.interactorObject.transform; Invoke(nameof(Grabbed), 0.25f); } else { Grabbed(args); } });
        interactable.selectExited.AddListener(Dropped);

        trash = GameObject.Find("Trash").transform;
        trash.GetComponent<MeshRenderer>().forceRenderingOff = true;
        errorPanel = GameObject.Find("Main Canvas").transform.GetChild(1);
        lockView = GetComponentInParent<LockView>();
    }

    public void Grabbed()
    {
        //SelectEnterEventArgs args = new SelectEnterEventArgs();
        //args.interactorObject = GameObject.Find("RightHand Controller").GetComponent<IXRSelectInteractor>();
        Grabbed(null);
    }

    public virtual void Grabbed(SelectEnterEventArgs args)
    {
        if (interactable == null)
            Init();
        if (!interactable.isSelected && !Application.isEditor && args == null)
            return;
        if (!lockView.HasLock)
            return;
        if (args != null)
            active = args.interactorObject.transform;
        if (dragParent)
        {
            transform.GetComponentInParent<PhotonView>().RequestOwnership();
        }
        else
        {
            transform.GetComponent<PhotonView>().RequestOwnership();
        }
        grabbed = true;
        trash.GetComponent<MeshRenderer>().forceRenderingOff = false;
        transform.GetChild(0).gameObject.SetActive(true); // enable collider
    }

    public virtual void Dropped(SelectExitEventArgs args)
    {
        if (dragParent)
        {
            transform.GetComponentInParent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
        }
        else
        {
            transform.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
        }

        if (!grabbed)
            return;
        grabbed = false;
        trash.GetComponent<MeshRenderer>().forceRenderingOff = true;
    }

    public virtual void Trash()
    {
        GameObject.Find("ToolBox").GetComponent<ToolBox>().closeAll();
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("Trash", RpcTarget.MasterClient);
            return;
        }
    }

    private static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
    {
        float length;
        float dotNumerator;
        float dotDenominator;
        Vector3 vector;
        intersection = Vector3.zero;

        //calculate the distance between the linePoint and the line-plane intersection point
        dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
        dotDenominator = Vector3.Dot(lineVec, planeNormal);

        if (dotDenominator != 0.0f)
        {
            length = dotNumerator / dotDenominator;

            vector = Vector3.Normalize(lineVec) * length;

            intersection = linePoint + vector;

            return true;
        }

        else
            return false;
    }

    public void Dropped()
    {
        Dropped(null);
    }
}
