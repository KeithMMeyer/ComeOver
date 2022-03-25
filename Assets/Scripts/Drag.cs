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

    Transform errorPanel;

    private bool grabbed = false;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        interactable.selectEntered.AddListener(Grabbed);
        interactable.selectExited.AddListener(Dropped);

        errorPanel = GameObject.Find("Main Canvas").transform.GetChild(1);
        trash = GameObject.Find("Trash").transform;
        trash.GetComponent<MeshRenderer>().forceRenderingOff = true;

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
                    UpdateRelations(position);
                }
                if (gameObject.layer == 8) //relations
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

    public void Grabbed(SelectEnterEventArgs args)
    {
        grabbed = true;
        trash.GetComponent<MeshRenderer>().forceRenderingOff = false;
        active = args.interactorObject.transform;
        transform.GetChild(0).gameObject.SetActive(true);
        if (gameObject.layer == 8) //relations
        {
            if (transform.parent.name.Equals("Arrow"))
            {
                storage = gameObject.GetComponentInParent<Identity>().relationReference.destinationClass;
                gameObject.GetComponentInParent<Identity>().relationReference.destinationClass = null;
            } else
            {
                storage = gameObject.GetComponentInParent<Identity>().relationReference.sourceClass;
                gameObject.GetComponentInParent<Identity>().relationReference.sourceClass = null;
            }
        }
    }

    public void Dropped(SelectExitEventArgs args)
    {
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
            bool placed = placeAttribute(collisionList);

            if (!placed)
            {
                errorPanel.gameObject.SetActive(true);
                errorPanel.GetChild(1).GetComponent<Text>().text = "Attributes can only be added to IML Classes.";
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

            bool placed = placeRelation(collisionList);

            if (!placed)
            {
                Relation relation = gameObject.GetComponentInParent<Identity>().relationReference;
                //errorPanel.gameObject.SetActive(true);
                //errorPanel.GetChild(1).GetComponent<Text>().text = "Relations can only be added to IML Classes.";
                if (relation.sourceClass != null)
                {
                    if (transform.parent.name.Equals("Arrow"))
                    {
                        relation.AttachToClass(relation.sourceClass, storage);
                    } else
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

    private bool placeAttribute(List<Collider> collisionList)
    {
        foreach (Collider c in collisionList)
        {
            if (c.gameObject.layer == 6) //classes
            {
                UserAttribute attribute = gameObject.GetComponentInParent<Identity>().attributeReference;
                if (attribute.parent != null)
                {
                    UserClass oldClass = attribute.parent;
                    oldClass.attributes.Remove(attribute);
                    oldClass.Resize();
                }

                UserClass newClass = c.gameObject.GetComponentInParent<Identity>().classReference;

                int position = 0;

                if (c.transform.position.y > interactable.transform.position.y)
                    position = newClass.attributes.Count;
                foreach (Collider c2 in collisionList)
                {
                    if (c2.gameObject.layer == 7) //attributes
                    {
                        if (newClass.attributes.Contains(c2.transform.parent.GetComponent<Identity>().attributeReference) && c2.transform.position.y > interactable.transform.position.y)
                        {
                            position = newClass.attributes.IndexOf(c2.transform.parent.GetComponent<Identity>().attributeReference) + 1;
                            break;
                        }
                    }
                }
                newClass.attributes.Insert(position, attribute);
                newClass.Resize();

                return true;
            }
        }
        return false;
    }

    private bool placeRelation(List<Collider> collisionList)
    {
        foreach (Collider c in collisionList)
        {
            if (c.gameObject.layer == 6) //classes
            {
                Relation relation = gameObject.GetComponentInParent<Identity>().relationReference;
                UserClass newClass = c.gameObject.GetComponentInParent<Identity>().classReference;
                storage.relations.Remove(relation);
                if (transform.parent.name.Equals("Arrow"))
                {
                    relation.AttachToClass(relation.sourceClass, newClass);
                } else
                {
                    relation.AttachToClass(newClass, relation.destinationClass);
                }
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
        if (gameObject.layer == 6) //classes
        {
            UserClass classReference = gameObject.GetComponentInParent<Identity>().classReference;
            TrashClass(classReference);
        }
        if (gameObject.layer == 7) //attributes
        {
            UserAttribute attribute = gameObject.GetComponentInParent<Identity>().attributeReference;
            Destroy(attribute.gameObject);
            List<UserClass> imlClasses = Iml.GetSingleton().structuralModel.classes;
            foreach (UserClass uc in imlClasses)
            {
                if (uc.attributes.Contains(attribute))
                {
                    uc.attributes.Remove(attribute);
                    uc.Resize();
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
        Destroy(classReference.gameObject);
        foreach (UserAttribute ua in classReference.attributes)
        {
            Destroy(ua.gameObject);
        }
        List<Relation> imlRelations = Iml.GetSingleton().structuralModel.relations;
        for (int i = 0; i < imlRelations.Count; i++)
        {
            if (imlRelations[i].source.Equals(classReference.id) || imlRelations[i].destination.Equals(classReference.id))
            {
                TrashRelation(imlRelations[i]);
                i--;
            }
        }
        Iml.GetSingleton().structuralModel.classes.Remove(classReference);
    }

    private void TrashRelation(Relation relation)
    {
        Destroy(relation.gameObject);
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
}
