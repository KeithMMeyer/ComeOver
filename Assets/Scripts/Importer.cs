using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Networking;

public class Importer
{
    public static Iml ImportXmlFromFile(string fileName)
    {
        try
        {
            var path = Application.streamingAssetsPath + "/" + fileName;
            string contents = File.ReadAllText(path);

            XmlSerializer serializer = new XmlSerializer(typeof(Iml));
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(contents)))
            {
                return (Iml)serializer.Deserialize(stream);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception importing xml file: " + e);
            return default;
        }
    }

	public static MetaModel ImportXmlFromAPI(string diagramID)
	{
		// Makes a request to the API to get the diagram using the userID and diagramID.

		UnityWebRequest request = new UnityWebRequest(diagramID);
		DownloadHandlerBuffer dH = new DownloadHandlerBuffer();
		request.downloadHandler = dH;


		request.SendWebRequest();

		while (!request.isDone)
		{
			// Wait for the request to finish.
		}

		if (request.responseCode != 200)
		{
			return default;
		}
		
		string body = request.downloadHandler.text;

		// Parses the body as a JSON object using the MetaModel class.
		MetaModel metaModel = JsonUtility.FromJson<MetaModel>(body);
		
		try
		{
			// Parses the XML string using the Iml class as a template.
			XmlSerializer serializer = new XmlSerializer(typeof(Iml));
			using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(metaModel.diagramData)))
			{
				metaModel.metaModel = (Iml)serializer.Deserialize(stream);
				return metaModel;
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Exception importing xml file: " + e);
			return default;
		}
	}
}