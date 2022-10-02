namespace TestAPI.Models
{
    public class IMLDiagram
    {
        public string diagramID { get; set; }

        public string userID { get; set; }

        public string diagramName { get; set; }

        public string diagramData { get; set; }

		public IMLDiagram() : this("", "", "", "") { }

		public IMLDiagram(string diagramID, string userID, string diagramName, string diagramData)
		{
			this.diagramID = diagramID;
			this.userID = userID;
			this.diagramName = diagramName;
			this.diagramData = diagramData;
		}

	}
}