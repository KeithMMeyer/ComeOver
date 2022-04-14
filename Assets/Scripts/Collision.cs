using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{

    public List<Collider> collisionList = new List<Collider>();

    void Start()
    {
        Physics.IgnoreCollision(transform.parent.GetComponent<Collider>(), GetComponent<Collider>());
        Collider[] colliders = transform.parent.parent.GetComponentsInChildren<Collider>();

        foreach (Collider c in colliders)
        {
            Physics.IgnoreCollision(c, GetComponent<Collider>());
        }

    }

    private void OnEnable()
    {
        collisionList = new List<Collider>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!collisionList.Contains(other))
            collisionList.Add(other);
        //Debug.Log("Collision!");

    }

    public void OnTriggerExit(Collider other)
    {
        collisionList.Remove(other);
    }
}
