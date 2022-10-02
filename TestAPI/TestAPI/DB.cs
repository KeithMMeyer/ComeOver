using MySql.Data.MySqlClient;
using TestAPI.Models;

namespace TestAPI
{
	public class DB
	{
		public MySqlConnection connection { get; set; }

		public DB()
		{
			// Connects to the MySQL database with the username and password
			connection = new MySqlConnection("server=localhost;user=root;password=root;database=mydb");

			// Opens the connection to the database
			connection.Open();
		}
		
		public List<List<String>> ExecuteQuery(string query)
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
			List<List<string>> results = ExecuteQuery($"SELECT * FROM users WHERE userID = '{userID}';");
			if (results.Count > 0)
			{
				user = new User
				{
					userID = results[0][0],
					email = results[0][1]
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
				string filepath = row[1];
				string diagramData = System.IO.File.ReadAllText(filepath);
				string diagramName = Path.GetFileName(filepath);
				
				diagrams.Add(new IMLDiagram
				{
					diagramID = row[0],
					userID = row[2],
					diagramName = diagramName,
					diagramData = diagramData
				});
			}

			if (diagrams.Count > 0)
			{
				return true;
			}

			return false;
		}

		public void Close()
		{
			connection.Close();
		}
	}
}
