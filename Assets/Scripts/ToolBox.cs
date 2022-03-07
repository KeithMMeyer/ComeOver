using System.Collections;
using UnityEngine;

public class ToolBox : MonoBehaviour
{
    public Camera viewCamera;
    private Vector3 velocity = Vector3.zero;

    private PrimaryButtonWatcher watcher;
    public bool IsPressed = false; // used to display button state in the Unity Inspector window

    public StructuralModel modelReference;

    public UserClass Target;


    void Start()
    {
        watcher = GetComponent<PrimaryButtonWatcher>();
        watcher.primaryButtonPress.AddListener(onPrimaryButtonEvent);
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

        modelReference = GameObject.Find("IML").GetComponent<Identity>().modelReference;
    }

    public void onPrimaryButtonEvent(bool pressed)
    {
        IsPressed = pressed;

        if (pressed)
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        } else
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }

    }

    private void Update()
    {
        Vector3 lookAtPos = new Vector3(viewCamera.transform.position.x, viewCamera.transform.position.y, viewCamera.transform.position.z);
        transform.LookAt(lookAtPos);
    }

    public void UpdateSettings()
    {
        Target.setAbstract(true);
    }
}