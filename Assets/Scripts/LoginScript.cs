using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class LoginScript : MonoBehaviour
{
	public delegate void LoginEvents();
	public event LoginEvents OnLoginSuccess;
	public event LoginEvents OnLoginFail;

	Task<WebResponse> task;

	public string authcode;
	public string email;

	public TextMeshProUGUI resultsText;

	// Start is called before the first frame update
	void Start()
	{
		//PlayerPrefs.DeleteAll();
	}

	// Update is called once per frame
	void Update()
	{
		// Every update, we check if the task is complete. If it is, we get the response and save it to the player prefs.
		if (task != null && task.IsCompleted) 
		{
			WebResponse response = null;
			// A 401 response throws an exception. This checks for that.
			try
			{
				response = task.Result;
			}
			catch (System.AggregateException e)
			{
				resultsText.text = "Error";
				Debug.Log("Error occured in getting web response. Most likely a 401 error. Error: " + e.InnerException.Message);
				task = null;
				OnLoginFail.Invoke();
				return;
			}

			// Gets the response stream and reads it into a string.
			Stream dataStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(dataStream);
			string responseFromServer = reader.ReadToEnd();
			PlayerPrefs.SetString("userID", responseFromServer);
			PlayerPrefs.Save();

			// Any way that this can exit must set task to null, or else we risk running this code over and over again.
			resultsText.text = "Logged in";
			task = null;
			OnLoginSuccess.Invoke();
		}
	}

	// This method is called by the login button in the scene.
	public void Login()
	{
		resultsText.text = "Logging in...";

		// Creates a new web request to the login endpoint.
		WebRequest request = WebRequest.Create("http://iml.cec.miamioh.edu:5000/CoMoVRAPI/LoginVR");
		request.Method = "POST";
		string jsonPostData = "{\"authcode\":\"" + authcode + "\",\"email\":\"" + email + "\"}";
		byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(jsonPostData);
		request.ContentType = "application/json";
		request.ContentLength = byteArray.Length;
		Stream dataStream = request.GetRequestStream();
		dataStream.Write(byteArray, 0, byteArray.Length);
		dataStream.Close();
		task = request.GetResponseAsync();
	}

	// This method is called by the logout button in the scene.
	public void Logout()
	{
		resultsText.text = "Logged out";
		PlayerPrefs.DeleteKey("userID");
		PlayerPrefs.Save();
	}
}
