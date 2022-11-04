using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TestAPI.Models;

namespace TestAPI
{
	public class DB
	{
		public MySqlConnection connection { get; set; }
		public MySqlConnection testDBConnection { get; set; }
		public Config config { get; set; }

		public DB()
		{
			// Gets the config file
			config = JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText("Config" + Path.DirectorySeparatorChar + "config.json"));

			if (Path.DirectorySeparatorChar != '\\')
			{
				// Linux
				config.iml_root = config.iml_root.Replace("\\", Path.DirectorySeparatorChar.ToString());
			}

			// Creates the connection string
			string connectionString = $"server={config.host};user={config.username};database={config.database};port=3306;password={config.password}";

			// Connects to the MySQL database with the username and password
			connection = new MySqlConnection(connectionString);

			// Opens the connection to the database
			connection.Open();
		}
		
		public List<List<string>> ExecuteQuery(string query)
		{
			List<List<string>> results = new List<List<string>>();

			// Creates a new command that will be executed on the database
			var command = connection.CreateCommand();

			// Sets the command to be executed
			command.CommandText = query;

			// Executes the command and stores each row in a list of strings
			using (var reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					List<string> row = new List<string>();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						row.Add(reader[i].ToString());
					}
					results.Add(row);
				}
			}

			return results;
		}

		public bool TryGetUserFromID(string userID, out User user)
		{
			user = null;
			List<List<string>> results = ExecuteQuery($"SELECT * FROM user WHERE userID = '{userID}';");
			string[] metamodels = ExecuteQuery($"SELECT metamodelId FROM metamodels WHERE userID = '{userID}';").Select(x => x[0]).ToArray();
			if (results.Count > 0)
			{
				user = new User
				{
					userID = results[0][0],
					first_name = results[0][1],
					last_name = results[0][2],
					email = results[0][3],
					picture = results[0][4],
					metamodels = metamodels
				};
				return true;
			}
			return false;
		}

		public bool TryGetIMLDiagrams(string userID, out List<IMLDiagram> diagrams)
		{
			diagrams = new List<IMLDiagram>();
			
			List<List<string>> queryResults = ExecuteQuery($"SELECT * FROM metamodels WHERE userID = '{userID}';");

			foreach (List<string> row in queryResults)
			{
				string filePath = row[2];
				//string diagramData = System.IO.File.ReadAllText(config.iml_root + Path.DirectorySeparatorChar + userID + Path.DirectorySeparatorChar + "metamodels" + Path.DirectorySeparatorChar + filename);
				string diagramData = System.IO.File.ReadAllText(filePath);
				string diagramName = Path.GetFileName(filePath);
				
				diagrams.Add(new IMLDiagram
				{
					metamodelId = row[0],
					userId = row[1],
					diagramName = diagramName,
					diagramData = diagramData
				});
			}

			return diagrams.Count > 0;
		}

		public bool TryGetUserID(AuthToken token, out string userID)
		{
			userID = null;
			
			List<List<string>> results = ExecuteQuery($"SELECT * FROM iml.token WHERE token = '{token.authcode}';");
			if (results.Count > 0)
			{
				// Input authcode exists
				userID = results[0][1];

				// Check if token is expired
				DateTime tokenExpiration = DateTime.Parse(results[0][2]);

				// Uses the current time to check if the token has expired
				if (tokenExpiration < DateTime.Now - TimeSpan.FromSeconds(int.Parse(config.access_code_expiry)))
				{
					// Token is expired
					return false;
				}
				
				if (TryGetUserFromID(userID, out User user))
				{
					//Console.WriteLine("User from DB is " + user.email + " and user from token is " + token.email);
					// User matching email exists
					if (user.email == token.email)
					{
						// Email matches
						return true;
					}
				}
			}
			return false;
		}

		// ONLY RUN WHEN TOKEN IS VALIDATED
		public void DeleteToken(AuthToken token)
		{
			ExecuteQuery($"DELETE FROM iml.token WHERE token = '{token.authcode}';");
		}

		public void Close()
		{
			connection.Close();
		}
	}
}
