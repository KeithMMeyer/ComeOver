using System;

[Serializable]
public class User
{
	public string userID;

	public string first_name;

	public string last_name;

	public string email;

	public string picture;

	public string[] metamodels;

	public override string ToString()
	{
		return $"User: {userID} {first_name} {last_name} {email} {picture}";
	}
}