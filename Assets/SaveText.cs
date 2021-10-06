using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveText : MonoBehaviour
{

    public TextMesh targetText;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<InputField>().onEndEdit.AddListener(saveText);
    }


    private void saveText(string textInField)
    {
        if (targetText != null)
        {
            targetText.text = textInField;
        }
        targetText = null;
        gameObject.SetActive(false);
    }
}
