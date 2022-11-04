namespace TestAPI.Models
{
    public class IMLDiagram
    {
        public string metamodelId { get; set; }

        public string userId { get; set; }

        public string diagramName { get; set; }

        public string diagramData { get; set; }

		public IMLDiagram() : this("", "", "", "") { }

		public IMLDiagram(string diagramID, string userID, string diagramName, string diagramData)
		{
			this.metamodelId = diagramID;
			this.userId = userID;
			this.diagramName = diagramName;
			this.diagramData = diagramData;
		}

	}
}