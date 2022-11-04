namespace TestAPI.Models
{
    public class User
    {
        public string userID { get; set; }

		public string first_name { get; set; }

		public string last_name { get; set; }

        public string email { get; set; }

		public string picture { get; set; }

		public string[] metamodels { get; set; }

		public override string ToString()
		{
			return $"User: {userID} {first_name} {last_name} {email} {picture}";
		}
	}
}