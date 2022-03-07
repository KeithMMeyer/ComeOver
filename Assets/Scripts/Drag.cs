using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class Drag : MonoBehaviour
{
    public bool dragParent = false;
    private XRSimpleInteractable interactable;
    private Transform left;
    private Transform right;
    private Transform active;
    private Vector3 startingPos;

    Transform errorPanel;

    private bool grabbed = false;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        startingPos = interactable.transform.position;

        left = GameObject.Find("XR Rig").transform.GetChild(0).GetChild(1);
        right = GameObject.Find("XR Rig").transform.GetChild(0).GetChild(2);

        interactable.onSelectEntered.AddListener(Grabbed);
        interactable.onSelectExited.AddListener(Dropped);
        errorPanel = GameObject.Find("Main Canvas").transform.GetChild(1);

    }

    public void Grabbed(XRBaseInteractor i)
    {
        grabbed = true;
        RaycastHit h;
        bool hit = left.GetComponent<XRRayInteractor>().GetCurrentRaycastHit(out h);
        if (hit && h.transform.Equals(transform))
        {
            active = left;
        }
        else
        {
            active = right;
        }
        active = right;
        if (gameObject.layer == 7) //attributes
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void Dropped(XRBaseInteractor i)
    {
        if (gameObject.layer == 7) //attributes
        {
            List<Collider> collisionList = GetComponentInChildren<Collision>().collisionList;
            transform.GetChild(0).gameObject.SetActive(false);
            bool placed = false;
            foreach (Collider c in collisionList)
            {
                if (c.gameObject.layer == 6) //classes
                {
                    UserAttribute attribute = gameObject.GetComponentInParent<Identity>().attributeReference;
                    if (gameObject.GetComponentInParent<Identity>().attributeReference.parent != null)
                    {
                        UserClass oldClass = attribute.parent;
                        oldClass.attributes.Remove(attribute);
                        oldClass.generateAttributes();
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
                                position = newClass.attributes.IndexOf(c2.transform.parent.GetComponent<Identity>().attributeReference)+1;
                                break;
                            }
                        }
                    }
                    newClass.attributes.Insert(position, attribute);
                    newClass.generateAttributes();
                    placed = true;
                    break;
                }
            }
            if (!placed)
            {
                errorPanel.gameObject.SetActive(true);
                errorPanel.GetChild(1).GetComponent<Text>().text = "Attributes can only be added to IML Classes.";
                if (gameObject.GetComponentInParent<Identity>().attributeReference.parent != null)
                {
                    gameObject.GetComponentInParent<Identity>().attributeReference.parent.generateAttributes();
                } else
                {
                    Destroy(gameObject.transform.parent.gameObject);
                }
            }

        }
        grabbed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbed)
        {
            RaycastHit h;
            bool found = active.GetComponent<XRRayInteractor>().GetCurrentRaycastHit(out h);
            if (found)
            {
                Transform movable = dragParent ? transform.parent : transform;
                Vector3 position = h.point;
                position.z = movable.position.z;
                movable.position = position;
                if (gameObject.layer == 6) //classes
                {
                    UpdateRelations(position);
                }
            }
        }

    }

    void UpdateRelations(Vector3 position)
    {
        UserClass classReference = transform.parent.GetComponent<Identity>().classReference;
        //TODO update stored position
        classReference.updateRelations();

    }
}
