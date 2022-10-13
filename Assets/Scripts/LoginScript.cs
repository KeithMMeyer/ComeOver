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
	public delegate void LoginEvent(string str);

	Task<WebResponse> task;

	string authcode;
	string email;
	string userID;

	UnityEngine.UI.Text resultsText;

	// Start is called before the first frame update
	void Start()
	{
		resultsText = GameObject.Find("ResultText").GetComponent<UnityEngine.UI.Text>();

		if (PlayerPrefs.HasKey("userID"))
		{
			Debug.Log("userID: " + PlayerPrefs.GetString("userID"));
		} else
		{
			Debug.Log("No userID");
			//PlayerPrefs.SetString("userID", "39356da5-4371-11ed-a142-fa163e1521a4");
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (PlayerPrefs.HasKey("userID"))
		{
			resultsText.text = PlayerPrefs.GetString("userID");
		}
	}

	public void Login()
	{
		resultsText.text = "Logging in...";
		
		// Makes a web request to POST to http://iml.cec.miamioh.edu:5000/CoMoVRAPI/LoginVR with the authcode and email 

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

		// When the task is complete, it will call the Login method
		task.ContinueWith(ProcessResponse);
	}

	public void ProcessResponse(Task<WebResponse> webresp)
	{
		try
		{
			WebResponse response = webresp.Result;
			Debug.Log("Processing response");
			Debug.Log(((HttpWebResponse)response).StatusCode);
			
			
			// If the response is a 401, set the results text to Error
			if (((HttpWebResponse)response).StatusCode != HttpStatusCode.OK)
			{
				resultsText.text = "Error";
			}
			else
			{
				Stream dataStream = response.GetResponseStream();
				StreamReader reader = new StreamReader(dataStream);
				string responseFromServer = reader.ReadToEnd();

				// Saves the response as a UserPref
				Debug.Log("Setting userID to: " + responseFromServer);
				PlayerPrefs.SetString("userID", responseFromServer);
				PlayerPrefs.Save();

				Debug.Log("Do we have the userID? " + PlayerPrefs.HasKey("userID"));
			}
		}
		catch (System.Exception e)
		{
			resultsText.text = "Error";
		}
	}

	public void OnEmailChanged(string str)
	{
		email = str;
	}

	public void OnAuthcodeChanged(string str)
	{
		authcode = str;
	}

	public void Logout()
	{
		PlayerPrefs.DeleteKey("userID");
		PlayerPrefs.Save();
	}
}
