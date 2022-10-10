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

		public CoMoVRAPIController(ILogger<CoMoVRAPIController> logger)
		{
			_logger = logger;
		}

		[HttpGet("GetUser")]
		public User GetUser(string userID)
		{
			if (database.TryGetUserFromID(userID, out User user))
			{
				return user;
			}

			// If the user is not found, throw a 404 error
			Response.Clear();
			Response.StatusCode = 404;
			return null;
		}

		[HttpGet("GetModel")]
		public string GetModel(string userID, string diagramID)
		{
			if (database.TryGetIMLDiagrams(userID, out List<IMLDiagram> diagrams))
			{
				diagrams = diagrams.Where(x => x.metamodelId == diagramID).ToList();
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
		
		[HttpPost("LoginVR")]
		public string LoginVR([FromBody] AuthToken authToken)
		{
			if (database.TryGetUserID(authToken, out string userID))
			{
				database.DeleteToken(authToken);
				return userID;
			}

			// If the token is not valid or incorrect, throw a 401 error
			Response.Clear();
			Response.StatusCode = 401;
			return "Invalid token";
		}
	}
}