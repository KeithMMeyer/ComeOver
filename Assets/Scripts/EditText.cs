using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshCollider))]

public class EditText : MonoBehaviour
{
    [SerializeField] InputField inputField;

    private void Start()
    {
        inputField = GameObject.Find("InputField").GetComponent<InputField>();
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("detected click");
            inputField.gameObject.SetActive(true);
            inputField.placeholder.GetComponent<Text>().text = transform.parent.GetChild(1).GetComponent<TextMesh>().text;
            inputField.text = transform.parent.GetChild(1).GetComponent<TextMesh>().text;
            inputField.GetComponent<SaveText>().targetText = transform.parent.GetChild(1).GetComponent<TextMesh>();
            inputField.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        }
    }
}
