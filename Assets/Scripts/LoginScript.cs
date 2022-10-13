using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class LoginScript : MonoBehaviour
{
	Task<WebResponse> task;

	public string authcode;
	public string email;

	UnityEngine.UI.Text resultsText;

	// Start is called before the first frame update
	void Start()
	{
		// Finds the results text to display the results of the login. Won't be needed in the final version.
		resultsText = GameObject.Find("ResultText").GetComponent<UnityEngine.UI.Text>();

		if (PlayerPrefs.HasKey("userID"))
		{
			resultsText.text = "Logged in";
		} else
		{
			resultsText.text = "Not logged in";
		}
		
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
			catch (System.Exception e)
			{
				resultsText.text = "Error";
				Debug.Log("Error occured in getting web response. Most likely a 401 error. Error: " + e.Message);
				task = null;
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

	// These two methods exist to be utilized by the text inputs in the scene.
	public void OnEmailChanged(string str)
	{
		email = str;
	}

	public void OnAuthcodeChanged(string str)
	{
		authcode = str;
	}

	// This method is called by the logout button in the scene.
	public void Logout()
	{
		resultsText.text = "Logged out";
		PlayerPrefs.DeleteKey("userID");
		PlayerPrefs.Save();
	}
}
