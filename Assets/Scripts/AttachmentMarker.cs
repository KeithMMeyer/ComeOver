using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AttachmentMarker : MonoBehaviour
{
    public GameObject visibilityIndicator;

    private XRSimpleInteractable interactable;
    private bool selected = false;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        interactable.hoverEntered.AddListener(delegate { visibilityIndicator.SetActive(true); });
        interactable.hoverExited.AddListener(delegate { if (!selected) { visibilityIndicator.SetActive(false); } });

        interactable.selectEntered.AddListener(delegate { selected = true; });
        interactable.selectExited.AddListener(delegate { selected = false; });

        visibilityIndicator.SetActive(false);
    }
}
