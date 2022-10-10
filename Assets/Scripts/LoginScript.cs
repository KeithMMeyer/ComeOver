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

	public string authcode;
	public string email;
	string userID = "Not logged in...";

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		// Updates the Unity UI with the userID

		GameObject gameObject = GameObject.Find("ResultText");
		UnityEngine.UI.Text text = gameObject.GetComponent<UnityEngine.UI.Text>();
		text.text = userID;
	}

	public void Login()
	{
		userID = "Logging in...";
		Debug.Log("Logging in with email: " + email + " and authcode: " + authcode);
		
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

	public void ProcessResponse(Task<WebResponse> str)
	{
		try
		{
			// Prints the response from the server
			WebResponse response = str.Result;
			Stream dataStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(dataStream);
			string responseFromServer = reader.ReadToEnd();
			Debug.Log(responseFromServer);
			userID = "Logged in!";
		}
		catch (System.Exception e)
		{
			userID = "Error";
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
}
