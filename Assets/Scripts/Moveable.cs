using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]

public class Moveable : MonoBehaviour
{

    public UserClass classReference;
    public bool snapable;

    private Vector3 screenPoint;
    private Vector3 offset;
    private bool isColliding;

    /*
    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(transform.parent.position);

        offset = transform.parent.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        offset.y += 2;

        transform.GetComponent<MeshCollider>().isTrigger = true;

    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.parent.position = curPosition;

        if (classReference != null)
            classReference.updateRelations();

    }

    void OnMouseUp()
    {
        Vector3 curPosition = transform.parent.position;
        curPosition.y -= 2;
        transform.parent.position = curPosition;

        transform.GetComponent<MeshCollider>().isTrigger = false;
        Debug.Log("Collision: " + isColliding);

        if (classReference != null)
            classReference.updateRelations();
    }

    private void OnTriggerEnter()
    {
        isColliding = true;
    }

    private void OnTriggerExit()
    {
        isColliding = false;
    }
    */

}