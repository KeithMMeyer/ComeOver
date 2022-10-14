using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModelButtonScript : MonoBehaviour
{
	public delegate void ModelButtonClicked(string modelId);
	public static event ModelButtonClicked OnModelButtonClicked;

	public string modelId;

	private void Start()
	{
		modelId = gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
	}

	public void OnClick()
	{
		Debug.Log("Model button clicked: " + modelId);
		OnModelButtonClicked?.Invoke(modelId);
	}
}
