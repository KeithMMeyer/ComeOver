using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class Drag : MonoBehaviour
{
    public bool dragParent = false;
    private XRSimpleInteractable interactable;
    private Transform active;
    private Transform trash;
    private UserClass storage;
    private LockView lockView;

    Transform errorPanel;

    private bool grabbed = false;

    // Start is called before the first frame update
    void Start()
    {
        if (interactable == null)
        {
            init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbed)
        {
            Vector3 newPosition;
            bool found = LinePlaneIntersection(out newPosition, active.transform.GetChild(2).position, active.transform.GetChild(2).forward, new Vector3(0, 0, -1), new Vector3(0, 0, 3));
            //RaycastHit h;
            //bool found = active.GetComponent<XRRayInteractor>().GetCurrentRaycastHit(out h);
            if (found)
            {
                Transform movable = dragParent ? transform.parent : transform;
                Vector3 position = newPosition;
                position.z = movable.position.z;
                movable.position = position;
                if (gameObject.layer == 6) //classes
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        UpdateRelations(position);
                    }
                    else
                    {
                        PhotonView photonView = PhotonView.Get(this);
                        photonView.RPC("UpdateRelations", RpcTarget.MasterClient, position);
                    }
                }
                if (PhotonNetwork.IsMasterClient && gameObject.layer == 8) //relations
                {
                    if (transform.parent.name.Equals("Arrow"))
                    {
                        gameObject.GetComponentInParent<Identity>().relationReference.UpdatePoints(null, position);
                    }
                    else
                    {
                        gameObject.GetComponentInParent<Identity>().relationReference.UpdatePoints(position, null);
                    }

                }
            }
        }

    }

    private void init()
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
        Grabbed(null);
    }

    public void Grabbed(SelectEnterEventArgs args)
    {
        if (!interactable.isSelected && !Application.isEditor)
            return;
        if (interactable == null)
            init();
        if (!lockView.HasLock)
            return;
        //if (args != null)
        //    active = args.interactorObject.transform;
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
        if (PhotonNetwork.IsMasterClient && gameObject.layer == 8) //relations
        {
            Relation relation = gameObject.GetComponentInParent<Identity>().relationReference;
            if (relation.sourceClass != null && relation.destinationClass != null && relation.sourceClass.Equals(relation.destinationClass))
            {
                //Destroy(transform.parent.gameObject);
                relation.UpdatePoints(relation.sourceClass.gameObject.transform.position, null);
            }
            if (transform.parent.name.Equals("Arrow"))
            {
                storage = relation.destinationClass;
                relation.destinationClass = null;
            }
            else
            {
                storage = relation.sourceClass;
                relation.sourceClass = null;
            }
        }
    }

    public void Dropped(SelectExitEventArgs args)
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
        if (gameObject.layer == 6) //classes
        {
            List<Collider> collisionList = GetComponentInChildren<Collision>().collisionList;
            transform.GetChild(0).gameObject.SetActive(false); // turn off collider
            foreach (Collider c in collisionList)
            {
                if (c.transform == trash)
                {
                    TrashObject();
                    return;
                }
            }
            gameObject.GetComponentInParent<Identity>().classReference.SetPosition(transform.position);
        }
        if (gameObject.layer == 7) //attributes
        {
            List<Collider> collisionList = GetComponentInChildren<Collision>().collisionList;
            transform.GetChild(0).gameObject.SetActive(false); // turn off collider
            foreach (Collider c in collisionList)
            {
                if (c.transform == trash)
                {
                    TrashObject();
                    return;
                }
            }
            bool placed = PlaceAttribute(collisionList);

            if (!placed)
            {
                if (lockView.IsLocked && lockView.HasLock)
                {
                    errorPanel.gameObject.SetActive(true);
                    errorPanel.GetChild(1).GetComponent<Text>().text = "Attributes can only be added to IML Classes.";
                }
                else
                {
                    PhotonView photonView = PhotonView.Get(this);
                    photonView.RPC("PrintError", RpcTarget.Others, "Attributes can only be added to IML Classes.");
                }
                if (gameObject.GetComponentInParent<Identity>().attributeReference.parent != null)
                {
                    gameObject.GetComponentInParent<Identity>().attributeReference.parent.Resize();
                }
                else
                {
                    Destroy(gameObject.transform.parent.gameObject);
                }
            }

        }
        if (gameObject.layer == 8) //relations
        {
            List<Collider> collisionList = GetComponentInChildren<Collision>(true).collisionList;
            transform.GetChild(0).gameObject.SetActive(false); // turn off collider
            foreach (Collider c in collisionList)
            {
                if (c.transform == trash)
                {
                    TrashObject();
                    return;
                }
            }

            bool placed = placeRelation(collisionList);

            if (!placed)
            {
                Relation relation = gameObject.GetComponentInParent<Identity>().relationReference;
                //errorPanel.gameObject.SetActive(true);
                //errorPanel.GetChild(1).GetComponent<Text>().text = "Relations can only be added to IML Classes.";
                if (storage != null)
                {
                    if (transform.parent.name.Equals("Arrow"))
                    {
                        relation.AttachToClass(relation.sourceClass, storage);
                    }
                    else
                    {
                        relation.AttachToClass(storage, relation.destinationClass);
                    }
                    storage = null;
                }
                else
                {
                    Destroy(relation.gameObject);
                }
            }

        }
    }

    private bool PlaceAttribute(List<Collider> collisionList)
    {
        GameObject classObject = null;
        GameObject attributeObject = null;

        foreach (Collider c in collisionList)
        {
            if (classObject == null && c.gameObject.layer == 6) //classes
            {
                classObject = c.gameObject;
            }
            if (attributeObject == null && c.gameObject.layer == 7) //attributes
            {
                attributeObject = c.gameObject;
            }
        }

        if (classObject == null)
            return false;

        if (PhotonNetwork.IsMasterClient)
        {
            PlacingAttribute(classObject, attributeObject);
        }
        else
        {
            string id = classObject.GetComponentInParent<ClassView>().id;
            string text = null;
            if (attributeObject != null)
                text = attributeObject.transform.parent.GetChild(1).GetComponent<Text>().text;

            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("PlacingAttribute", RpcTarget.MasterClient, id, text);
            return true;
        }

        return true;
    }

    public void PlacingAttribute(string classId, string attributeText)
    {
        UserClass classRefence = null;
        GameObject attributeObject = null;

        foreach (UserClass c in Iml.GetSingleton().structuralModel.classes)
        {
            if (c.id.Equals(classId))
            {
                classRefence = c;
                break;
            }
        }
        if (classRefence == null)
            Debug.LogError("Tried to place attribute but no class match found! ID:'" + classId + "'");
        foreach (UserAttribute a in classRefence.attributes)
        {
            if (a.displayString.Equals(attributeText))
            {
                attributeObject = a.gameObject.transform.GetChild(0).gameObject;
                break;
            }
        }
        PlacingAttribute(classRefence.gameObject.transform.GetChild(0).gameObject, attributeObject);
    }

    private void PlacingAttribute(GameObject classObject, GameObject attributeObject)
    {
        UserAttribute attribute = gameObject.GetComponentInParent<Identity>().attributeReference;
        //UserAttribute other = attribute.parent.FindAttribute(name);
        //if (attribute.parent.FindRelation(name) != null || (other != null && other != attribute))
        //{
        //    errorPanel.gameObject.SetActive(true);
        //    errorPanel.GetChild(1).GetComponent<Text>().text = "Changing this attribute would result a duplicate name for attributes and/or relations; update aborted.";
        //    return false;
        //}

        if (attribute.parent != null)
        {
            UserClass oldClass = attribute.parent;
            oldClass.attributes.Remove(attribute);
            oldClass.Resize();
        }

        UserClass newClass = classObject.gameObject.GetComponentInParent<Identity>().classReference;

        int position = 0;

        if (classObject.transform.position.y > interactable.transform.position.y)
            position = newClass.attributes.Count;

        if (attributeObject != null)
            if (newClass.attributes.Contains(attributeObject.transform.parent.GetComponent<Identity>().attributeReference) && attributeObject.transform.position.y > interactable.transform.position.y)
                position = newClass.attributes.IndexOf(attributeObject.transform.parent.GetComponent<Identity>().attributeReference) + 1;

        newClass.attributes.Insert(position, attribute);
        newClass.Resize();
    }

    private bool placeRelation(List<Collider> collisionList)
    {
        foreach (Collider c in collisionList)
        {
            if (c.gameObject.layer == 6) //classes
            {
                Relation relation = gameObject.GetComponentInParent<Identity>().relationReference;
                UserClass newClass = c.gameObject.GetComponentInParent<Identity>().classReference;

                bool isArrow = transform.parent.name.Equals("Arrow");
                string message;
                if ((isArrow && !relation.CanAttach(relation.sourceClass, newClass, out message)) || (!isArrow && !relation.CanAttach(newClass, relation.destinationClass, out message)))
                {
                    if (lockView.IsLocked && lockView.HasLock)
                    {
                        errorPanel.gameObject.SetActive(true);
                        errorPanel.GetChild(1).GetComponent<Text>().text = message;
                    }
                    else
                    {
                        PhotonView photonView = PhotonView.Get(this);
                        photonView.RPC("PrintError", RpcTarget.Others, message);
                    }
                    return false;
                }

                if (storage != null)
                {
                    storage.relations.Remove(relation);
                }
                else
                {
                    Iml.GetSingleton().structuralModel.relations.Add(relation);
                }
                if (isArrow)
                {
                    relation.AttachToClass(relation.sourceClass, newClass);
                }
                else
                {
                    relation.AttachToClass(newClass, relation.destinationClass);
                }
                gameObject.GetComponent<EditRelation>().SetUpPositions();
                storage = null;
                return true;
            }
        }
        return false;
    }

    private void UpdateRelations(Vector3 position)
    {
        UserClass classReference = transform.parent.GetComponent<Identity>().classReference;
        //TODO update stored position
        classReference.UpdateRelations();

    }

    public void TrashObject()
    {
        GameObject.Find("ToolBox").GetComponent<ToolBox>().closeAll();
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("TrashObject", RpcTarget.MasterClient);
            return;
        }
        if (gameObject.layer == 6) //classes
        {
            UserClass classReference = gameObject.GetComponentInParent<Identity>().classReference;
            TrashClass(classReference);
        }
        if (gameObject.layer == 7) //attributes
        {
            UserAttribute attribute = gameObject.GetComponentInParent<Identity>().attributeReference;
            //Destroy(attribute.gameObject);
            PhotonNetwork.Destroy(attribute.gameObject);
            List<UserClass> imlClasses = Iml.GetSingleton().structuralModel.classes;
            foreach (UserClass uc in imlClasses)
            {
                if (uc.attributes.Contains(attribute))
                {
                    uc.attributes.Remove(attribute);
                    uc.Resize();
                    break;
                }
            }
        }
        if (gameObject.layer == 8) //relations
        {
            Relation relation = gameObject.GetComponentInParent<Identity>().relationReference;
            TrashRelation(relation);
        }
    }

    private void TrashClass(UserClass classReference)
    {
        //Destroy(classReference.gameObject);
        PhotonNetwork.Destroy(classReference.gameObject);
        foreach (UserAttribute ua in classReference.attributes)
        {
            //Destroy(ua.gameObject);
            PhotonNetwork.Destroy(ua.gameObject);
        }
        Iml.GetSingleton().structuralModel.classes.Remove(classReference);
        List<Relation> imlRelations = Iml.GetSingleton().structuralModel.relations;
        for (int i = 0; i < imlRelations.Count; i++)
        {
            if (imlRelations[i].source.Equals(classReference.id) || imlRelations[i].destination.Equals(classReference.id))
            {
                TrashRelation(imlRelations[i]);
                i--;
            }
        }
    }

    private void TrashRelation(Relation relation)
    {
        //Destroy(relation.gameObject);
        PhotonNetwork.Destroy(relation.gameObject);
        Iml.GetSingleton().structuralModel.relations.Remove(relation);
        relation.sourceClass.relations.Remove(relation);
        relation.destinationClass.relations.Remove(relation);
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
