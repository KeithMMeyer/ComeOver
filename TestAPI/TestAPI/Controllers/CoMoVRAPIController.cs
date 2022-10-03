using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [ApiController]
	[Route("[controller]")]
	public class CoMoVRAPIController : ControllerBase
	{
		private readonly ILogger<CoMoVRAPIController> _logger;

		// Database singleton
		DB _database;
		
		DB database
		{
			get 
			{
				if (_database == null || _database.connection.State == System.Data.ConnectionState.Closed)
				{
					_database = new DB();
				}
				return _database;
			}
		}

		// A constructor that 
		public CoMoVRAPIController(ILogger<CoMoVRAPIController> logger)
		{
			_logger = logger;
		}
		// Gets the email of the user given their userID
		[HttpGet("GetEmail")]
		public string GetEmail(string userID)
		{
			Console.WriteLine(database);
			if (database.TryGetUserFromID(userID, out User user))
			{
				return user.email;
			}

			// If the user is not found, throw a 404 error
			Response.Clear();
			Response.StatusCode = 404;
			return "User not found";
		}

		[HttpGet("GetModel")]
		public string GetModel(string userID, string diagramID)
		{
			if (database.TryGetIMLDiagrams(userID, out List<IMLDiagram> diagrams))
			{
				diagrams = diagrams.Where(x => x.diagramID == diagramID).ToList();
				if (diagrams.Count > 0)
				{
					// Serializes the result
					return JsonConvert.SerializeObject(diagrams[0]);
				}
			}

			// If the diagram is not found, throw a 404 error
			Response.Clear();
			Response.StatusCode = 404;
			return "Diagram not found";
		}
	}
}