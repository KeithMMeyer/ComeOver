using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class Importer
{
    public static Iml ImportXml(string file)
    {
        try
        {
            var path = Application.streamingAssetsPath + "/" + file;
            string contents;
            if (Application.isEditor)
            {
                contents = File.ReadAllText(path);
            }
            else
            {
               
                UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(path);
                www.SendWebRequest();
                while (!www.isDone)
                {
                }
                contents = www.downloadHandler.text;
            }

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
}