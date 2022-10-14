using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
	string email;
	string authcode;

	GameObject loginScreen = null;
	GameObject emailScreen = null;
	TMP_InputField emailField = null;
	GameObject authcodeScreen = null;
	TMP_InputField authcodeField = null;
	GameObject loginStatusScreen = null;
	GameObject loginFailedScreen = null;
	GameObject welcomeScreen = null;
	TextMeshProUGUI nameText = null;
	Image image = null;
	TextMeshProUGUI modelsText = null;

	private void Start()
	{
		// Finds the login screen and sets it to active.
		loginScreen = GameObject.Find("Login Screen");
		loginScreen.SetActive(false);

		// Finds the email screen and sets it to inactive.
		emailScreen = GameObject.Find("Email Screen");

		// Finds the email text and sets it to empty.
		emailField = GameObject.Find("Email input field").GetComponent<TMP_InputField>();
		emailScreen.SetActive(false);

		// Finds the authcode screen and sets it to inactive.
		authcodeScreen = GameObject.Find("AuthCode Screen");

		// Finds the authcode text and sets it to empty.
		authcodeField = GameObject.Find("AuthCode input field").GetComponent<TMP_InputField>();
		authcodeScreen.SetActive(false);

		// Finds the login status screen and sets it to inactive.
		loginStatusScreen = GameObject.Find("Login Status Screen");
		loginStatusScreen.SetActive(false);

		// Finds the login failed screen and sets it to inactive.
		loginFailedScreen = GameObject.Find("Login Failed Screen");
		loginFailedScreen.SetActive(false);

		// Finds the welcome screen and sets it to inactive.
		welcomeScreen = GameObject.Find("Welcome Screen");

		// Finds the name text and sets it to inactive.
		nameText = GameObject.Find("NameText").GetComponent<TextMeshProUGUI>();
		
		// Finds the image
		image = GameObject.Find("User Image").GetComponent<Image>();

		modelsText = GameObject.Find("ModelsText").GetComponent<TextMeshProUGUI>();

		welcomeScreen.SetActive(false);

		if (PlayerPrefs.HasKey("userID"))
		{
			// If the player has already logged in, we don't need to show the login screen.
			// We can just show the welcome screen.
			StartCoroutine(DownloadPicture());
			nameText.text = "Hi, " + PlayerPrefs.GetString("firstName") + "!";
			GoToWelcomeScreen();
		}
		else
		{
			// If the player hasn't logged in, we need to show the login screen.
			loginScreen.SetActive(true);
		}
	}

	public void GoToEmailScreen()
	{
		loginScreen.SetActive(false);
		emailScreen.SetActive(true);
	}

	public void GoToAuthcodeScreen()
	{
		// Only continues if the email is valid.
		if (emailField.text.Contains("@"))
		{
			email = emailField.text;
			emailScreen.SetActive(false);
			authcodeScreen.SetActive(true);
		}
	}

	public void GoToLoginStatusScreen()
	{
		if (authcodeField.text.Trim() != "")
		{
			authcode = authcodeField.text;
			authcodeScreen.SetActive(false);
			loginStatusScreen.SetActive(true);
			Login();
		}
	}

	public void Login()
	{
		// Create a GameObject and attach the Login script to it.
		GameObject login = new GameObject("LoginScript Controller");
		LoginScript loginScript = login.AddComponent<LoginScript>();

		loginScript.email = email.Trim();
		loginScript.authcode = authcode.Trim();
		loginScript.resultsText = GameObject.Find("Login Status").GetComponent<TextMeshProUGUI>();

		loginScript.OnLoginSuccess += OnLoginSuccess;
		loginScript.OnLoginFail += OnLoginFail;
		loginScript.Login();
	}

	public void OnLoginSuccess()
	{
		Destroy(GameObject.Find("LoginScript Controller"));

		// Makes a call to the CoMoVR API to get the user's information.
		// This is done in a coroutine so that the game doesn't freeze while waiting for the API to respond.
		StartCoroutine(GetUser());

		// Waits 3 seconds before displaying the welcome screen
		StartCoroutine(WaitForWelcomeScreen());
	}

	public void OnLoginFail()
	{
		Destroy(GameObject.Find("LoginScript Controller"));

		loginFailedScreen.SetActive(true);
	}

	public void RetryLogin()
	{
		loginFailedScreen.SetActive(false);
		loginStatusScreen.SetActive(false);
		emailScreen.SetActive(true);
	}

	public void Logout()
	{
		PlayerPrefs.DeleteKey("userID");
		PlayerPrefs.DeleteKey("firstName");
		PlayerPrefs.DeleteKey("lastName");
		PlayerPrefs.DeleteKey("email");
		PlayerPrefs.DeleteKey("picture");
		PlayerPrefs.DeleteKey("metamodels");
		
		PlayerPrefs.Save();
		welcomeScreen.SetActive(false);
		loginScreen.SetActive(true);
	}

	public void GoToWelcomeScreen()
	{
		welcomeScreen.SetActive(true);
		loginStatusScreen.SetActive(false);
		modelsText.text = PlayerPrefs.GetString("metamodels");
	}

	IEnumerator WaitForWelcomeScreen()
	{
		yield return new WaitForSeconds(3);
		GoToWelcomeScreen();
	}

	IEnumerator GetUser()
	{
		// Creates a new WWW object to send the form to the API.
		WWW www = new WWW("http://iml.cec.miamioh.edu:5000/CoMoVRAPI/GetUser?userID=" + PlayerPrefs.GetString("userID"));

		// Waits for the API to respond.
		yield return www;

		// If the API responds with an error, it will be printed to the console.
		if (www.error != null)
		{
			Debug.Log(www.error);
		}
		else
		{
			// The response is a JSON string, so we need to parse it.
			// Parses the JSON string using the User class as a template.
			User user = JsonUtility.FromJson<User>(www.text);

			PlayerPrefs.SetString("firstName", user.first_name);
			PlayerPrefs.SetString("lastName", user.last_name);
			PlayerPrefs.SetString("email", user.email);
			PlayerPrefs.SetString("picture", user.picture);

			nameText.text = "Hi, " + user.first_name + "!";

			string models = "";

			foreach (string model in user.metamodels)
			{
				models += model + "\n";
			}

			PlayerPrefs.SetString("metamodels", models);

			PlayerPrefs.Save();

			StartCoroutine(DownloadPicture());
		}
	}

	IEnumerator DownloadPicture()
	{
		using (var www = UnityWebRequestTexture.GetTexture(PlayerPrefs.GetString("picture")))
		{
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log(www.error);
			}
			else
			{
				if (www.isDone)
				{
					// Creates a new default UI material to hold the downloaded texture.
					Material material = new Material(Shader.Find("UI/Default"));
					Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
					material.mainTexture = texture;
					// Sets the material of the picture image to the downloaded texture.
					image.material = material;
				}
			}
		}
	}
}
